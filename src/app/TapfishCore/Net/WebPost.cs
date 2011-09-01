using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace TapfishCore.Net
{
  public class WebPostEventArgs : EventArgs
  {
    public bool Succeeded { get; set; }
    public int ErrorCode { get; set; }
    public string ErrorDesc { get; set; }
    public string ResponseText { get; set; }
  }

  public class WebPost
  {
    static readonly string PREFIX = "--";
    static readonly string NEWLINE = "\r\n";
    static readonly string BOUNDARY = DateTime.Now.Ticks.ToString("x", CultureInfo.InvariantCulture);

    public event EventHandler<WebPostEventArgs> PostFailed;
    public event EventHandler<WebPostEventArgs> PostFinished;

    HttpWebRequest Request { get; set; }

    /// <summary>
    /// Post image
    /// </summary>
    /// <param name="url"></param>
    /// <param name="data"></param>
    /// <param name="imgRaw"></param>
    public void PostImage(
                  string url,
                  Dictionary<string, object> data,
                  WebPostImageObject media)
    {
      byte[] postData = BuildImagePostData(data, media);

      try
      {
        Request = HttpWebRequest.Create(url) as HttpWebRequest;
        Request.ContentType =
                  string.Concat("multipart/form-data; boundary=",
                                BOUNDARY);
        Request.Method = "POST";
        Request.BeginGetRequestStream(
            (asyncResult) => OnGetRequestStreamCallback(asyncResult, postData),
           Request);
      }
      catch (System.Exception e)
      {
        FirePostFailedEvent(e);
        CleanUp();
      }
    }

    /// <summary>
    /// Clean up this object.
    /// </summary>
    void CleanUp()
    {
      Request = null;
    }

    /// <summary>
    /// Fire post failed event.
    /// </summary>
    /// <param name="e"></param>
    private void FirePostFailedEvent(Exception e)
    {
      if (PostFailed != null)
        PostFailed(this,
                   new WebPostEventArgs
                   {
                     Succeeded = false,
                     ErrorDesc = e.ToString()
                   });
    }

    /// <summary>
    /// Callback from get request stream.
    /// Actually send post data
    /// </summary>
    /// <param name="asyncResult"></param>
    /// <param name="postData"></param>
    void OnGetRequestStreamCallback(IAsyncResult asyncResult,
                                    byte[] postData)
    {
      try
      {
        var req = asyncResult.AsyncState as HttpWebRequest;
        var stm = req.EndGetRequestStream(asyncResult);
        stm.Write(postData, 0, postData.Length);
        stm.Close();

        req.BeginGetResponse(new AsyncCallback(OnGetResponseCallback), req);
      }
      catch (System.Exception e)
      {
        FirePostFailedEvent(e);
        CleanUp();
      }
    }

    /// <summary>
    /// Get Response callback.
    /// </summary>
    /// <param name="asyncResult"></param>
    void OnGetResponseCallback(IAsyncResult asyncResult)
    {
      try
      {
        var responseText = "";
        var req = asyncResult.AsyncState as HttpWebRequest;
        using (var response = req.EndGetResponse(asyncResult))
        {
          using (var stm = response.GetResponseStream())
          {
            using (var reader = new StreamReader(stm))
            {
              responseText = reader.ReadToEnd();
              FirePostFinishedEvent(responseText);
            }
          }
        }
      }
      catch (System.Exception e)
      {
        FirePostFailedEvent(e);
      }

      CleanUp();
    }

    /// <summary>
    /// Fire post finished event.
    /// </summary>
    /// <param name="responseText"></param>
    void FirePostFinishedEvent(string responseText)
    {
      if (PostFinished != null)
        PostFinished(this,
                     new WebPostEventArgs
                     {
                       Succeeded = true,
                       ResponseText = responseText
                     });
    }

    /// <summary>
    /// Build Post data
    /// </summary>
    /// <param name="data"></param>
    /// <param name="imgRaw"></param>
    /// <returns></returns>
    internal byte[] BuildImagePostData(
                  Dictionary<string, object> data,
                  WebPostImageObject media)
    {
      var sb = new StringBuilder();

      foreach (var kv in data)
      {
        sb.Append(PREFIX).Append(BOUNDARY).Append(NEWLINE);
        sb.Append("Content-Disposition: form-data; name=\"").Append(kv.Key).Append("\"").Append(NEWLINE);
        sb.Append(NEWLINE);
        sb.Append(kv.Value);
        sb.Append(NEWLINE);
      }

      sb.Append(PREFIX).Append(BOUNDARY).Append(NEWLINE);
      if (string.IsNullOrEmpty(media.FieldName))
        sb.Append("Content-Disposition: form-data; filename=\"").Append(media.FileName).Append("\"").Append(NEWLINE);
      else
        sb.Append("Content-Disposition: form-data; name=\"").Append(media.FieldName).Append("\";filename=\"").Append(media.FileName).Append("\"").Append(NEWLINE);

      sb.Append("Content-Type: ").Append(media.ContentType).Append(NEWLINE);
      sb.Append(NEWLINE);

      var lastPart = string.Concat(NEWLINE, PREFIX, BOUNDARY, PREFIX, NEWLINE);

      byte[] headerBytes = Encoding.UTF8.GetBytes(sb.ToString());
      byte[] fileData = media.FileData;
      byte[] lastBytes = Encoding.UTF8.GetBytes(lastPart.ToString());

      var postData =
        new byte[headerBytes.Length + fileData.Length + lastBytes.Length];

      Buffer.BlockCopy(headerBytes, 0, postData, 0, headerBytes.Length);
      Buffer.BlockCopy(fileData, 0, postData, headerBytes.Length, fileData.Length);
      Buffer.BlockCopy(lastBytes, 0, postData, headerBytes.Length + fileData.Length, lastBytes.Length);

      return postData;
    }
  }
}