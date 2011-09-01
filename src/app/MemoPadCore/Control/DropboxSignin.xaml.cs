using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using DropNet;
using MemoPadCore.Common;
using Newtonsoft.Json;
using TapfishCore.Ui;

namespace MemoPadCore.Control
{
  public partial class DropboxSignin : UserControl
  {
    const string VS_NORMAL = "Normal";
    const string VS_SIGNING = "Signing";

    DropNetClient _dropboxclient;

    public event EventHandler<DropboxSigninEventArgs> SigninSucceeded;
    public event EventHandler<DropboxSigninEventArgs> SigninFailed;

    /// <summary>
    /// CTOR
    /// </summary>
    public DropboxSignin()
    {
      InitializeComponent();
    }

    public class DropBoxErrorJsonModel
    {
      public string error { get; set; }
    }

    /// <summary>
    /// Set sync folder name
    /// </summary>
    public void SetSyncFolderName(string folderpath)
    {
      _folder.Text = folderpath;
    }

    /// <summary>
    /// Sign-in button clicked
    /// </summary>
    /// <param name="sender">Event sender</param>
    /// <param name="e">Event parameter</param>
    void OnSigninClicked(
          object sender,
          ManipulationCompletedEventArgs e)
    {
      if (false == UiUtils.IsTapped(e))
        return;

      var email = _email.Text.Trim();
      var password = _password.Password.Trim();
      var folder = _folder.Text.Trim();

      if (string.IsNullOrEmpty(email))
      {
        MessageBox.Show("Please enter e-mail");
        _email.Focus();
        return;
      }

      if (string.IsNullOrEmpty(password))
      {
        MessageBox.Show("Please enter your password");
        _password.Focus();
        return;
      }

      if (string.IsNullOrEmpty(folder))
      {
        MessageBox.Show("Please enter your sync folder name");
        _folder.Focus();
        return;
      }

      VisualStateManager.GoToState(this, VS_SIGNING, true);

      _dropboxclient = new DropNetClient(AppSetting.DROPBOX_API_KEY,
                                         AppSetting.DROPBOX_API_SECRET);
      _dropboxclient.LoginAsync(email, password,
          (userlogin) =>
          {
            var usertoken = userlogin.Token;
            var secret = userlogin.Secret;

            if (SigninSucceeded != null)
              SigninSucceeded(this,
                              new DropboxSigninEventArgs
                              {
                                UserToken = usertoken,
                                UserSecret = secret,
                                RemoteFolderName = folder
                              });
          },
          (error) =>
          {
            var errormsg = ExtractErrorMessage(error);
            VisualStateManager.GoToState(this, VS_NORMAL, true);
            if (SigninFailed != null)
              SigninFailed(this,
                           new DropboxSigninEventArgs
                           {
                             ErrorMessage = errormsg
                           });
          });

    }

    /// <summary>
    /// Extract error message from given exception
    /// </summary>
    /// <param name="error">Error object</param>
    /// <returns>Error message</returns>
    string ExtractErrorMessage(DropNet.Exceptions.DropboxException error)
    {
      string errormessage = error.Message ?? "";
      var resp = error.Response;
      if (resp.ContentType == "application/json")
      {
        try
        {
          var errormodel =
          JsonConvert.DeserializeObject<DropBoxErrorJsonModel>(resp.Content);
          if (errormodel != null)
            errormessage = errormodel.error;
        }
        catch (System.Exception e)
        {
        }
      }

      return errormessage;
    }

    /// <summary>
    /// Create account field is pressed
    /// </summary>
    /// <param name="sender">Event sender</param>
    /// <param name="e">Event parameter</param>
    void OnCreateAccountClicked(
              object sender,
              ManipulationCompletedEventArgs e)
    {
    }
  }

  public class DropboxSigninEventArgs : EventArgs
  {
    public string UserToken { get; set; }
    public string UserSecret { get; set; }
    public string ErrorMessage { get; set; }
    public string RemoteFolderName { get; set; }
  }
}