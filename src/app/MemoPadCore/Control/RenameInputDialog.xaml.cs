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
using TapfishCore.Platform;
using TapfishCore.Resources;

namespace MemoPadCore.Control
{
  public partial class RenameInputDialog : UserControl
  {
    const int MAX_PATH_LEN = 128;
    static char[] INVALID_FILE_NAME_CHARACTERS =
      new char[]
      {
        '\\', '/', ':', '*', '?', '"', '<', '>', '|'
      };

    public event EventHandler<RenameInputDialogEventArgs> Changed;
    public event EventHandler Canceled;

    string _basepath;
    string _originfilename;
    string _fileext;

    public RenameInputDialog()
    {
      InitializeComponent();
    }

    /// <summary>
    /// Set file name
    /// </summary>
    /// <param name="fn"></param>
    public void SetFileName(string fullpath)
    {
      _basepath = PathUtil.AddBackslash(PathUtil.PathOnly(fullpath));
      _fileext = PathUtil.Extension(fullpath);
      _originfilename = PathUtil.NameOnlyNoExt(fullpath);

      var fn = PathUtil.NameOnlyNoExt(fullpath);
      _newname.Text = fn;
      _newname.MaxLength = MAX_PATH_LEN;
    }

    /// <summary>
    /// OK button clicked
    /// </summary>
    /// <param name="sender">Event sender</param>
    /// <param name="e">Event parameter</param>
    void OnOKClicked(
          object sender,
          RoutedEventArgs e)
    {
      var fn = _newname.Text.Trim();
      if (string.IsNullOrEmpty(fn))
      {
        MessageBox.Show("Empty filename is not allowed. Please enter file name again.");
        _newname.Focus();
        return;
      }

      if (fn.ToLower() == _originfilename.ToLower())
      {
        FireCancelEvent();
        return;
      }

      if (fn.Length > MAX_PATH_LEN)
      {
        var msg =
          string.Format(
                  "File name could not be longer than {0} characters. Please enter shorter filename.",
                  MAX_PATH_LEN);
        MessageBox.Show(msg);
        _newname.Focus();
        return;
      }

      if (-1 != fn.IndexOfAny(INVALID_FILE_NAME_CHARACTERS))
      {
        var msg =
          string.Format(
          "Filename cannot contain any of the following characters:\r\n{0}",
          new string(INVALID_FILE_NAME_CHARACTERS));
        MessageBox.Show(msg);
        _newname.Focus();
        return;
      }

      if (-1 != fn.IndexOf(".."))
      {
        MessageBox.Show("Filename cannot contain dot sequences(..)");
        _newname.Focus();
        return;
      }

      var fullpath =
        string.Concat(_basepath, fn, _fileext);
      if (StorageIo.Exists(fullpath))
      {
        var msg =
          string.Format("Filename '{0}' already exists. Please use other name.",
                        fn);
        MessageBox.Show(msg);
        _newname.Focus();
        return;
      }

      if (Changed != null)
      {
        Changed(this,
          new RenameInputDialogEventArgs
          {
            FullPath = fullpath,
            NameOnlyNoExt = fn
          });
      }
    }

    void OnCancelClicked(
          object sender,
          RoutedEventArgs e)
    {
      FireCancelEvent();
    }

    void FireCancelEvent()
    {
      if (Canceled != null)
        Canceled(this, null);
    }

    void OnGotFocus(object sender, System.Windows.RoutedEventArgs e)
    {
      _newname.Focus();
    }

    void OnNewNameGotFocus(object sender, System.Windows.RoutedEventArgs e)
    {
      _newname.SelectAll();
    }
  }

  public class RenameInputDialogEventArgs : EventArgs
  {
    public string FullPath { get; set; }
    public string NameOnlyNoExt { get; set; }
  }
}