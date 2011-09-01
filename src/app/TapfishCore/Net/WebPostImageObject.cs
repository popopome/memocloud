using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using TapfishCore.Resources;

namespace TapfishCore.Net
{
  public class WebPostImageObject
  {
    public string FieldName { get; set; }
    public string FileName { get; set; }
    public string ContentType { get; set; }
    public byte[] FileData { get; set; }

    public static WebPostImageObject CreateJpeg(
                      string fieldName,
                      string fn,
                      BitmapSource bmp)
    {
      return new WebPostImageObject
      {
        FieldName = fieldName,
        FileName = fn,
        ContentType = WebMimeType.IMAGE_JPEG,
        FileData = BitmapUtils.JpegBytesFromBitmapImage(bmp, 100)
      };
    }

  }
}