using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Net;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using MemoPadCore.Model;
using TapfishCore.Resources;
using TapfishCore.Ui;

namespace MemoPadCore.Control
{

  public class MiniPhotoMemoControl : Grid
  {

    #region Events

    public event EventHandler CancelClicked;
    public event EventHandler TrashClicked;
    public event EventHandler MoreCommandsClicked;

    #endregion Events

    #region Static Fields

    #endregion Static Fields

    #region Fields

    Image _morecommands;
    ImageBrush _backgroundbrush;
    ImageButton _trashbutton;
    ImageButton _cancelbutton;
    Image _image;

    #endregion Fields

    #region Static initializer

    static MiniPhotoMemoControl()
    {
    }

    #endregion Static initializer

    #region CTOR

    /// <summary>
    /// CTOR
    /// </summary>
    public MiniPhotoMemoControl()
    {
      _backgroundbrush = new ImageBrush();
      this.Background = _backgroundbrush;

      _image = new Image
      {
        Stretch = Stretch.UniformToFill,
        CacheMode = new BitmapCache()
      };

      this.Children.Add(_image);

      CreateBackSideButton();
      CreateMoreButton();
    }

    #endregion CTOR

    /// <summary>
    /// Create more buttons
    /// </summary>
    void CreateMoreButton()
    {
      _morecommands = MiniMemoControlUtil.CreateMoreCommandsImage();
      _morecommands.ManipulationCompleted += new EventHandler<ManipulationCompletedEventArgs>(OnMoreCommands_ManipCompleted);
      this.Children.Add(_morecommands);
    }

    /// <summary>
    /// Create backside button
    /// </summary>
    void CreateBackSideButton()
    {
      _cancelbutton = MiniMemoControlUtil.CreateCancelImageButton(this);
      _cancelbutton.Clicked += new EventHandler(OnCancelButtonClicked);

      _trashbutton = MiniMemoControlUtil.CreateTrashImageButton(this);
      _trashbutton.Clicked += new EventHandler(OnTrashButtonClicked);
    }

    void OnCancelButtonClicked(object sender, EventArgs e)
    {
      if (CancelClicked != null)
        CancelClicked(this, null);
    }

    void OnTrashButtonClicked(object sender, EventArgs e)
    {
      if (TrashClicked != null)
        TrashClicked(this, null);
    }

    /// <summary>
    /// Open text document
    /// </summary>
    /// <param name="memo"></param>
    public void Open(Memo memo)
    {
      Debug.Assert(memo.IsPhotoMemo);
      if (false == memo.IsPhotoMemo)
        return;

      memo.Open();

      if (memo.Thumb != null)
      {
        _image.Source = memo.Thumb;
      }
      else
      {
        Observable
        .FromEvent<PropertyChangedEventHandler, PropertyChangedEventArgs>(
            a => new PropertyChangedEventHandler((x, xe) => a(xe)),
            h => memo.PropertyChanged += h,
            h => memo.PropertyChanged -= h)
        .Where(args => args.PropertyName == Memo.ThumbPropertyName)
        .Select(x => memo.Thumb)
        .Where(thumb => thumb != null)
        .Subscribe(thumb =>
        {
          _image.Source = thumb;
        });
      }
    }

    /// <summary>
    /// Show front size
    /// </summary>
    public void ShowFrontSide()
    {
      _backgroundbrush.ImageSource = null;

      _morecommands.Show();
      _image.Show();

      _cancelbutton.Hide();
      _trashbutton.Hide();
    }

    /// <summary>
    /// Hide backside
    /// </summary>
    public void ShowBackSide()
    {
      _backgroundbrush.ImageSource = MiniMemoControlUtil.FlipBackground;
      _cancelbutton.Show();
      _trashbutton.Show();

      _morecommands.Hide();
      _image.Hide();
    }

    /// <summary>
    /// Manipulation completed
    /// </summary>
    /// <param name="sender">Event sender</param>
    /// <param name="e">Event parameter</param>
    void OnMoreCommands_ManipCompleted(
            object sender,
            ManipulationCompletedEventArgs e)
    {
      if (UiUtils.IsTapped(e) == false)
        return;

      //
      // Should be true
      // to block being clicked on memo itself.
      //
      e.Handled = true;

      if (MoreCommandsClicked != null)
        MoreCommandsClicked(this, null);
    }
  }
}