using System;
using System.Diagnostics;
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
using MemoPadCore.Common;
using MemoPadCore.Model;
using Microsoft.Phone.Tasks;
using TapfishCore.Platform;
using TapfishCore.Resources;
using TapfishCore.Ui;

namespace MemoPadCore.Control
{
  public class MemoClickedEventArgs : EventArgs
  {
    public Memo Document { get; set; }
  }

  public class MemoSummaryControl : Grid
  {
    #region Constants

    public const int DESIRED_WIDTH = 200;
    public const int DESIRED_HEIGHT = 200;

    #endregion Constants

    #region Events

    public event EventHandler<MemoClickedEventArgs> Click;
    public event EventHandler<MemoClickedEventArgs> DeleteClicked;
    public event EventHandler<MemoClickedEventArgs> FrontToBackFlipped;

    #endregion Events

    #region Fields

    PlaneProjection _planeprojection;
    public Memo Memo { get; private set; }

    Storyboard _sbrotate;
    DoubleAnimation _anirotate;

    MiniTextMemoControl _textmemo;

    #endregion Fields

    #region Properties

    public bool IsFlipped
    {
      get
      {
        if (_textmemo != null)
          return _textmemo.IsFlipped;

        return false;
      }
    }

    #endregion Properties

    /// <summary>
    /// CTOR
    /// </summary>
    public MemoSummaryControl()
    {
      _planeprojection = new PlaneProjection
      {
        CenterOfRotationX = 0.5,
        CenterOfRotationY = 0.5
      };
      this.Projection = _planeprojection;

      this.ManipulationStarted += new EventHandler<ManipulationStartedEventArgs>(OnManipStarted);
      this.ManipulationDelta += new EventHandler<ManipulationDeltaEventArgs>(OnManipDelta);
      this.ManipulationCompleted += new EventHandler<ManipulationCompletedEventArgs>(OnManipulationCompleted);
    }

    /// <summary>
    /// Create text memo controls
    /// </summary>
    void CreateTextMemoControls(Memo memo)
    {
      memo.LoadSummary();

      _textmemo = new MiniTextMemoControl();
      this.Children.Add(_textmemo);

      _textmemo.EmailClicked += new EventHandler(OnEmailClicked);
      _textmemo.CopyToClipboardClicked += new EventHandler(OnCopyToClipboardClicked);
      _textmemo.TrashClicked += new EventHandler(OnTrashButtonClicked);
      _textmemo.CancelClicked += new EventHandler(OnCancelButtonClicked);
      _textmemo.MoreCommandsClicked += new EventHandler(OnMoreCommandsClicked);

      _textmemo.Open(memo);
    }

    /// <summary>
    /// More commands button clicked
    /// </summary>
    /// <param name="sender">Event sender</param>
    /// <param name="e">Event parameter</param>
    void OnMoreCommandsClicked(object sender, EventArgs e)
    {
      this.FlipFrontToBack();
    }

    /// <summary>
    /// Trash button is clicked
    /// </summary>
    /// <param name="sender">Event sender</param>
    /// <param name="e">Event parameter</param>
    void OnTrashButtonClicked(object sender, EventArgs e)
    {
      var msg = string.Format("Are you sure to delete the memo('{0}')?",
                              Memo.Title);
      var result =
        MessageBox.Show(msg, "Confirmation", MessageBoxButton.OKCancel);
      if (result == MessageBoxResult.Cancel)
        return;

      if (DeleteClicked != null)
        DeleteClicked(this,
                      new MemoClickedEventArgs
                      {
                        Document = this.Memo
                      });
    }

    /// <summary>
    /// Open text document
    /// </summary>
    /// <param name="memo"></param>
    public void Open(Memo memo)
    {
      Memo = memo;
      if (memo.IsTextMemo)
        CreateTextMemoControls(memo);
    }

    void OnManipStarted(object sender, ManipulationStartedEventArgs e)
    {
      if (false == IsFrontVisible())
        e.Handled = true;
    }

    void OnManipDelta(object sender, ManipulationDeltaEventArgs e)
    {
      if (false == IsFrontVisible())
        e.Handled = true;
    }

    /// <summary>
    /// Touch up
    /// </summary>
    /// <param name="sender">Event sender</param>
    /// <param name="e">Event parameter</param>
    void OnManipulationCompleted(object sender, ManipulationCompletedEventArgs e)
    {
      bool isfrontvisible = IsFrontVisible();

      if (isfrontvisible)
      {
        if (UiUtils.IsTapped(e))
        {
          if (Click != null)
            Click(this, new MemoClickedEventArgs
            {
              Document = Memo
            });
        }
        return;
      }

    }

    /// <summary>
    /// Check which side is now visible.
    /// </summary>
    /// <returns></returns>
    bool IsFrontVisible()
    {
      return (_planeprojection.RotationY == 0);
    }

    /// <summary>
    /// Reload revised document
    /// </summary>
    public void ReloadRevisedDocument()
    {
      if (Memo.IsRevised)
      {
        Open(Memo);
        Memo.IsRevised = false;
      }
    }

    /// <summary>
    /// Start flipping from front to back
    /// </summary>
    private void FlipFrontToBack()
    {
      PrepareAnimation();

      _anirotate.To = -90;
      _sbrotate.Completed -= new EventHandler(OnFlipCompleted_FrontToBack);
      _sbrotate.Completed += new EventHandler(OnFlipCompleted_FrontToBack);
      _sbrotate.Begin();
    }

    /// <summary>
    /// Flip completed
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    void OnFlipCompleted_FrontToBack(object sender, EventArgs e)
    {
      _sbrotate.Completed -= new EventHandler(OnFlipCompleted_FrontToBack);
      _textmemo.ShowBackSide();

      _anirotate.To = -180;

      _sbrotate.Begin();

      ThreadUtil.UiCall(() =>
        {
          if (FrontToBackFlipped != null)
            this.FrontToBackFlipped(
                    this,
                    new MemoClickedEventArgs
                    {
                      Document = this.Memo
                    });
        });
    }

    /// <summary>
    /// Prepare animation
    /// </summary>
    void PrepareAnimation()
    {
      if (_sbrotate == null)
      {
        _anirotate = new DoubleAnimation
        {
          Duration = TimeSpan.FromMilliseconds(150)
        };
        Storyboard.SetTarget(_anirotate, _planeprojection);
        Storyboard.SetTargetProperty(_anirotate, new PropertyPath("RotationY"));

        _sbrotate = new Storyboard();
        _sbrotate.Children.Add(_anirotate);
      }
    }

    /// <summary>
    /// Cancel button clicked
    /// </summary>
    /// <param name="sender">Cancel button</param>
    /// <param name="e">Event sender</param>
    void OnCancelButtonClicked(object sender, EventArgs e)
    {
      FlipBackToFront();
    }

    /// <summary>
    /// Flip back to front
    /// </summary>
    public void FlipBackToFront()
    {
      PrepareAnimation();

      _anirotate.To = -90;
      _anirotate.Completed -= new EventHandler(OnFlipCompleted_BackToFront);
      _anirotate.Completed += new EventHandler(OnFlipCompleted_BackToFront);
      _sbrotate.Begin();
    }

    /// <summary>
    /// Storyboard completed
    /// BACK ---> FRONT
    /// </summary>
    /// <param name="sender">Event sender</param>
    /// <param name="e">Event parameter</param>
    void OnFlipCompleted_BackToFront(object sender, EventArgs e)
    {
      _textmemo.ShowFrontSide();
      _anirotate.Completed -= new EventHandler(OnFlipCompleted_BackToFront);
      _anirotate.To = 0;
      _sbrotate.Begin();
    }

    /// <summary>
    /// Copy to clipboard command is clicked
    /// </summary>
    /// <param name="sender">Event sender</param>
    /// <param name="e">Event parameter</param>
    void OnCopyToClipboardClicked(object sender, EventArgs e)
    {
      Memo.Open();
      System.Windows.Clipboard.SetText(
        this.Memo.Text);

      FlipBackToFront();
    }

    /// <summary>
    /// Email is clicked
    /// </summary>
    /// <param name="sender">Event sender</param>
    /// <param name="e">Event parameter</param>
    void OnEmailClicked(object sender, EventArgs e)
    {
      try
      {
        var task = new EmailComposeTask();
        this.Memo.Open();
        task.Body = this.Memo.Text;
        task.Subject = this.Memo.Title;
        task.Show();

        FlipBackToFront();
      }
      catch (System.Exception err)
      {
        Debug.WriteLine("EmailComposeTask failed:" + err);
      }
    }
  }
}