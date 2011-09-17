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
using TapfishCore.Ui;

namespace MemoPadCore.Control
{
  public partial class SyncBox : UserControl
  {
    #region Constants

    const int PAGE_WIDTH = 480;
    const string VS_HIDDEN = "Hidden";
    const string VS_ACTIVE = "Active";

    #endregion Constants

    public event EventHandler DoneButtonClicked;

    #region Fields

    int _curuploading;
    int _curdownloading;

    int _numuploadings;
    int _numdownloadings;

    #endregion Fields

    #region CTOR

    public SyncBox()
    {
      InitializeComponent();

      _donebutton.Click += (x, xe) =>
        {
          if (DoneButtonClicked != null)
            DoneButtonClicked(this, null);
        };

    }

    #endregion CTOR

    /// <summary>
    /// Set sync information
    /// </summary>
    /// <param name="numuploadings">Number of uploadings</param>
    /// <param name="numdownloadings">Number of downloadings</param>
    public void SetSyncInfo(int numuploadings,
                            int numdownloadings)
    {
      _numuploadings = numuploadings;
      _numdownloadings = numdownloadings;

      _curuploading = 0;
      _curdownloading = 0;

      UpdateDescription("");
      UpdateProgress();

      _donebutton.Hide();
    }

    /// <summary>
    /// Update progress
    /// </summary>
    /// <param name="totaluploadings">Total uploading count</param>
    /// <param name="curuploadings">Current uploading count</param>
    /// <param name="totaldownloadings">Total downloading count</param>
    /// <param name="curdownloadings">Current downloading count</param>
    public void UpdateProgress(
                  int totaluploadings,
                  int curuploadings,
                  int totaldownloadings,
                  int curdownloadings)
    {
      _numuploadings = totaluploadings;
      _numdownloadings = totaldownloadings;

      _curuploading = curuploadings;
      _curdownloading = curdownloadings;

      UpdateProgress();
    }

    /// <summary>
    /// Update progress
    /// </summary>
    void UpdateProgress()
    {
      _uploadingstatus.Text =
        _numuploadings == 0
        ? "0/0"
        : string.Format("{0}/{1}", _curuploading, _numuploadings);

      _downloadingstatus.Text =
        _numdownloadings == 0
        ? "0/0"
        : string.Format("{0}/{1}", _curdownloading, _numdownloadings);

      int cnt = _numuploadings + _numdownloadings;
      if (cnt == 0)
      {
        _curprogress.Width = 0;
        return;
      }

      int total = (_numuploadings + _numdownloadings);
      int curuploading = (_numuploadings == 0) ? 0 : _curuploading;
      int curdownloading = (_numdownloadings == 0) ? 0 : _curdownloading;

      if (total == 0)
        _curprogress.Width = 0;
      else
      {
        double progress =
        (double)(curuploading + curdownloading) / total;

        _curprogress.Width = PAGE_WIDTH * progress;
      }

    }

    /// <summary>
    /// Update description
    /// </summary>
    /// <param name="msg">Message</param>
    public void UpdateDescription(string msg)
    {
      _desc.Text = msg;
    }

    /// <summary>
    /// Hide sync box
    /// </summary>
    public void HideSyncBox()
    {
      VisualStateManager.GoToState(this, VS_HIDDEN, false);
    }

    /// <summary>
    /// Show syncbox
    /// </summary>
    public void ShowSyncBox()
    {
      VisualStateManager.GoToState(this, VS_ACTIVE, true);
    }

    /// <summary>
    /// Sync is finished
    /// </summary>
    public void SyncFinished()
    {
      UpdateDescription("Finished");
      _donebutton.Show();
    }

  }
}