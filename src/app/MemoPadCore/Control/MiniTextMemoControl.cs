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
using MemoPadCore.Common;
using MemoPadCore.Model;
using TapfishCore.Resources;
using TapfishCore.Ui;

namespace MemoPadCore.Control
{
  public class MiniTextMemoControl : Grid
  {
    #region Constants

    const int TITLE_WIDTH = MiniMemoControlUtil.DESIRED_WIDTH;
    const int TITLE_HEIGHT = 44;
    const double TITLE_FONT_SIZE = 26;

    const int TEXT_LEFT_MARGIN = 26;
    const int TEXT_TOP_MARGIN = 6;
    const int SUMMARY_TOP_MARGIN = 6;
    const int SUMMARY_WIDTH = 128;
    const int SUMMARY_BOTTOM_SHADOW = 14;
    const int SUMMARY_HEIGHT = MiniMemoControlUtil.DESIRED_HEIGHT - TITLE_HEIGHT - TEXT_TOP_MARGIN - SUMMARY_TOP_MARGIN - SUMMARY_BOTTOM_SHADOW;

    const int BUTTON_CLIPBOARD_X = 100;
    const int BUTTON_CLIPBOARD_Y = 1;

    const double SUMMARY_FONT_SIZE = 18;

    #endregion Constants

    #region Events

    public event EventHandler CancelClicked;
    public event EventHandler TrashClicked;
    public event EventHandler CopyToClipboardClicked;
    public event EventHandler EmailClicked;
    public event EventHandler MoreCommandsClicked;

    #endregion Events

    #region Static Fields

    static BitmapImage _background;

    static FontFamily _titlefont;
    static Thickness _titlemargin;

    static SolidColorBrush _titlefontcolor;
    static SolidColorBrush _titleengravingcolor;

    static FontFamily _summaryfont;
    static SolidColorBrush _summaryfontcolor;
    static Thickness _summarymargin;

    static BitmapImage _flipclipboard;

    #endregion Static Fields

    #region Fields

    ImageBrush _backgroundbrush;
    TextBlock _title;
    TextBlock _titleengraving;
    TextBlock _summary;

    ImageButton _cancelbutton;
    ImageButton _trashbutton;
    ImageButton _clipboardbutton;
    ImageButton _emailbutton;

    Image _morecommands;

    Memo _memo;

    #endregion Fields

    #region Properties

    public bool IsFlipped
    {
      get
      {
        return _cancelbutton.IsVisible();
      }
    }

    #endregion Properties

    #region Static initializer

    static MiniTextMemoControl()
    {
      _background = new BitmapImage
      {
        CreateOptions = BitmapCreateOptions.None
      };
      _background.UriSource =
        new Uri("/Images/memo-list/memo-summary-flip-front.png", UriKind.Relative);

      _titlefont = AppSetting.FONT_SEGOE_WP_SEMILIGHT;
      _titlemargin = new Thickness(TEXT_LEFT_MARGIN, TEXT_TOP_MARGIN, 0, 0);
      _titlefontcolor = AppSetting.COLOR_SUMMARY_TITLE;
      _titleengravingcolor =
        new SolidColorBrush(Colors.White);

      _summaryfont = AppSetting.FONT_SEGOE_WP_LIGHT;
      _summaryfontcolor = AppSetting.COLOR_SUMMARY_BODY;
      _summarymargin = new Thickness(
                              TEXT_LEFT_MARGIN,
                              TEXT_TOP_MARGIN + TITLE_HEIGHT + SUMMARY_TOP_MARGIN, 0, 0);

      _flipclipboard = BitmapUtils.CreateBitmapImmediately("Images/memo-list/memo-summary-clipboard.png");

    }

    #endregion Static initializer

    #region CTOR

    public MiniTextMemoControl()
    {
      _backgroundbrush = new ImageBrush
      {
        ImageSource = _background
      };
      this.Background = _backgroundbrush;

      _title = CreateTitleBlock();
      _titleengraving = CreateTitleBlock();

      Thickness m = _titleengraving.Margin;
      m.Top = m.Top + 1.5;
      _titleengraving.Margin = m;
      _titleengraving.Foreground = _titleengravingcolor;

      this.Children.Add(_titleengraving);
      this.Children.Add(_title);

      _summary = new TextBlock
      {
        HorizontalAlignment = HorizontalAlignment.Left,
        VerticalAlignment = VerticalAlignment.Top,
        FontFamily = _summaryfont,
        FontSize = SUMMARY_FONT_SIZE,
        MaxWidth = SUMMARY_WIDTH,
        Margin = _summarymargin,
        TextWrapping = TextWrapping.Wrap,
        Height = SUMMARY_HEIGHT,
        MaxHeight = SUMMARY_HEIGHT,
        Foreground = _summaryfontcolor
      };
      this.Children.Add(_summary);

      CreateMoreButton();
      CreateBackSideButtons();

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
    /// Create back-side buttons
    /// </summary>
    void CreateBackSideButtons()
    {
      _cancelbutton = MiniMemoControlUtil.CreateCancelImageButton(this);
      _cancelbutton.Clicked += new EventHandler(OnCancelButtonClicked);

      _trashbutton = MiniMemoControlUtil.CreateTrashImageButton(this);
      _trashbutton.Clicked += new EventHandler(OnTrashButtonClicked);

      _clipboardbutton =
        MiniMemoControlUtil.CreateImageButton(
                           this,
                           "Images/memo-list/memo-summary-clipboard.png",
                           "Images/memo-list/memo-summary-clipboard-selected.png",
                           100,
                           1);
      _clipboardbutton.Clicked += new EventHandler(OnCopyToClipboardClicked);

      _emailbutton =
        MiniMemoControlUtil.CreateImageButton(
                           this,
                          "Images/memo-list/memo-summary-email.png",
                          "Images/memo-list/memo-summary-email-selected.png",
                          100,
                          94);
      _emailbutton.Clicked += new EventHandler(OnEmailClicked);
    }

    TextBlock CreateTitleBlock()
    {
      return new TextBlock
      {
        HorizontalAlignment = HorizontalAlignment.Left,
        VerticalAlignment = VerticalAlignment.Top,
        Margin = _titlemargin,
        FontFamily = _titlefont,
        FontSize = TITLE_FONT_SIZE,
        MaxWidth = TITLE_WIDTH,
        TextWrapping = TextWrapping.NoWrap,
        Height = TITLE_HEIGHT,
        Foreground = new SolidColorBrush(Colors.Black)
      };
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

    void OnCopyToClipboardClicked(object sender, EventArgs e)
    {
      if (CopyToClipboardClicked != null)
        CopyToClipboardClicked(this, null);
    }

    void OnEmailClicked(object sender, EventArgs e)
    {
      if (EmailClicked != null)
        EmailClicked(this, null);
    }

    /// <summary>
    /// Open text document
    /// </summary>
    /// <param name="memo"></param>
    public void Open(Memo memo)
    {
      _memo = memo;

      _title.Text = _memo.Title;
      _titleengraving.Text = _title.Text;
      _summary.Text = _memo.Summary;
    }

    /// <summary>
    /// Show front size
    /// </summary>
    public void ShowFrontSide()
    {
      _backgroundbrush.ImageSource = _background;
      _title.Show();
      _titleengraving.Show();
      _summary.Show();
      _morecommands.Show();

      _cancelbutton.Hide();
      _trashbutton.Hide();
      _clipboardbutton.Hide();
      _emailbutton.Hide();
    }

    /// <summary>
    /// Hide backside
    /// </summary>
    public void ShowBackSide()
    {
      _backgroundbrush.ImageSource = MiniMemoControlUtil.FlipBackground;
      _cancelbutton.Show();
      _trashbutton.Show();
      _clipboardbutton.Show();
      _emailbutton.Show();

      _title.Hide();
      _titleengraving.Hide();
      _summary.Hide();
      _morecommands.Hide();
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