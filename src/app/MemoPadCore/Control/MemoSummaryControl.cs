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
using MemoPadCore.Model;
using TapfishCore.Resources;
using TapfishCore.Ui;

namespace MemoPadCore.Control
{
  public class MemoClickedEventArgs : EventArgs
  {
    public TextDocument Document { get; set; }
  }

  public class MemoSummaryControl : Grid
  {
    public const int DESIRED_WIDTH = 200;
    public const int DESIRED_HEIGHT = 200;

    const int TITLE_WIDTH = DESIRED_WIDTH;
    const int TITLE_HEIGHT = 44;
    const double TITLE_FONT_SIZE = 26;

    const int TEXT_LEFT_MARGIN = 26;
    const int TEXT_TOP_MARGIN = 6;
    const int SUMMARY_TOP_MARGIN = 6;
    const int SUMMARY_WIDTH = 128;
    const int SUMMARY_BOTTOM_SHADOW = 14;
    const int SUMMARY_HEIGHT = DESIRED_HEIGHT - TITLE_HEIGHT - TEXT_TOP_MARGIN - SUMMARY_TOP_MARGIN - SUMMARY_BOTTOM_SHADOW;

    const int MORE_COMMANDS_WIDTH = 48;
    const int MORE_COMMANDS_HEIGHT = 48;
    const int MORE_COMMANDS_RIGHT_MARGIN = 12;
    const int MORE_COMMANDS_BOTTOM_MARGIN = 15;

    const int BUTTON_IMAGE_WIDTH = 90;
    const int BUTTON_IMAGE_HEIGHT = 90;

    const int BUTTON_CANCEL_X = 100;
    const int BUTTON_CANCEL_Y = 91;

    const int BUTTON_CLIPBOARD_X = 100;
    const int BUTTON_CLIPBOARD_Y = 1;

    const int BUTTON_TRASH_X = 10;
    const int BUTTON_TRASH_Y = 1;

    const double SUMMARY_FONT_SIZE = 18;

    public event EventHandler<MemoClickedEventArgs> Click;

    #region Static components

    static BitmapImage _background;
    static FontFamily _titlefont;
    static Thickness _titlemargin;

    static SolidColorBrush _titlefontcolor;
    static SolidColorBrush _titleengravingcolor;

    static FontFamily _summaryfont;
    static SolidColorBrush _summaryfontcolor;
    static Thickness _summarymargin;

    static BitmapImage _morecommandbmp;

    static BitmapImage _flipbackground;
    static BitmapImage _fliptrash;
    static BitmapImage _flipclipboard;
    static BitmapImage _fliparrow;

    /// <summary>
    /// Static initializer
    /// </summary>
    static MemoSummaryControl()
    {
      _background = new BitmapImage
      {
        CreateOptions = BitmapCreateOptions.None
      };
      _background.UriSource =
        new Uri("/Images/memo-list/memo-summary-flip-front.png", UriKind.Relative);

      _titlefont = new FontFamily("Segoe WP SemiLight");
      _titlemargin = new Thickness(TEXT_LEFT_MARGIN, TEXT_TOP_MARGIN, 0, 0);
      _titlefontcolor = new SolidColorBrush(Color.FromArgb(255, 110, 110, 110));
      _titleengravingcolor =
        new SolidColorBrush(Colors.White);

      _summaryfont = new FontFamily("Segoe WP Light");
      _summaryfontcolor = new SolidColorBrush(Color.FromArgb(255, 136, 136, 136));
      _summarymargin = new Thickness(
                              TEXT_LEFT_MARGIN,
                              TEXT_TOP_MARGIN + TITLE_HEIGHT + SUMMARY_TOP_MARGIN, 0, 0);

      _morecommandbmp = BitmapUtils.CreateBitmapImmediately("/Images/memo-list/more-commands.png");

      _flipbackground = BitmapUtils.CreateBitmapImmediately("/Images/memo-list/memo-summary-flip-back.png");
      _fliptrash = BitmapUtils.CreateBitmapImmediately("/Images/memo-list/memo-summary-trash.png");
      _flipclipboard = BitmapUtils.CreateBitmapImmediately("/Images/memo-list/memo-summary-clipboard.png");
      _fliparrow = BitmapUtils.CreateBitmapImmediately("/Images/memo-list/memo-summary-arrow-selected.png");

    }

    #endregion Static components

    ImageBrush _backgroundbrush;
    TextBlock _title;
    TextBlock _titleengraving;
    Image _morecommands;

    PlaneProjection _planeprojection;
    TextBlock _summary;
    public TextDocument Doc { get; private set; }

    Storyboard _sbrotate;
    DoubleAnimation _anirotate;

    ImageButton _cancelbutton;
    ImageButton _trashbutton;
    ImageButton _clipboardbutton;

    /// <summary>
    /// CTOR
    /// </summary>
    public MemoSummaryControl()
    {
      _backgroundbrush = new ImageBrush
      {
        ImageSource = _background
      };
      this.Background = _backgroundbrush;

      _planeprojection = new PlaneProjection
      {
        CenterOfRotationX = 0.5,
        CenterOfRotationY = 0.5
      };
      this.Projection = _planeprojection;

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

      _morecommands = new Image
      {
        Width = MORE_COMMANDS_WIDTH,
        Height = MORE_COMMANDS_HEIGHT,
        HorizontalAlignment = HorizontalAlignment.Right,
        VerticalAlignment = VerticalAlignment.Bottom,
        Source = _morecommandbmp,
        Margin = new Thickness(0, 0, MORE_COMMANDS_RIGHT_MARGIN, MORE_COMMANDS_BOTTOM_MARGIN)
      };
      _morecommands.ManipulationCompleted += new EventHandler<ManipulationCompletedEventArgs>(OnMoreCommands_ManipCompleted);
      this.Children.Add(_morecommands);

      _cancelbutton = CreateImageButton(
                           "/Images/memo-list/memo-summary-arrow.png",
                           "/Images/memo-list/memo-summary-arrow-selected.png",
                           100,
                           94);
      _cancelbutton.Clicked += new EventHandler(OnCancelButtonClicked);
      _trashbutton = CreateImageButton(
                           "/Images/memo-list/memo-summary-trash.png",
                           "/Images/memo-list/memo-summary-trash-selected.png",
                           12,
                           1);
      _clipboardbutton = CreateImageButton(
                           "/Images/memo-list/memo-summary-clipboard.png",
                           "/Images/memo-list/memo-summary-clipboard-selected.png",
                           100,
                           1);

      this.ManipulationCompleted += new EventHandler<ManipulationCompletedEventArgs>(OnManipulationCompleted);
    }

    private ImageButton CreateImageButton(
      string normalimgpath,
      string focusimgpath,
      double x,
      double y)
    {
      var btn = new ImageButton();
      btn.Create(BUTTON_IMAGE_WIDTH,
                           BUTTON_IMAGE_HEIGHT,
                           normalimgpath,
                           focusimgpath);
      btn.SetXYWithMargin(x, y);
      this.Children.Add(btn);
      btn.Hide();
      return btn;
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

    /// <summary>
    /// Open text document
    /// </summary>
    /// <param name="doc"></param>
    public void Open(TextDocument doc)
    {
      Doc = doc;
      doc.LoadSummary();

      _title.Text = doc.Title;
      _titleengraving.Text = _title.Text;
      _summary.Text = doc.Summary;
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
              Document = Doc
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
      if (Doc.IsRevised)
      {
        Open(Doc);
        Doc.IsRevised = false;
      }
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
      PrepareAnimation();

      _anirotate.To = -90;
      _sbrotate.Completed -= new EventHandler(OnFlipCompleted_FrontToBack);
      _sbrotate.Completed += new EventHandler(OnFlipCompleted_FrontToBack);
      _sbrotate.Begin();

      e.Handled = true;
    }

    void OnFlipCompleted_FrontToBack(object sender, EventArgs e)
    {
      _sbrotate.Completed -= new EventHandler(OnFlipCompleted_FrontToBack);
      _backgroundbrush.ImageSource = _flipbackground;

      HideFrontContent();
      ShowBackContent();

      _anirotate.To = -180;
      _sbrotate.Begin();

    }

    void ShowBackContent()
    {
      _cancelbutton.Show();
      _trashbutton.Show();
      _clipboardbutton.Show();
    }

    void HideBackContent()
    {
      _cancelbutton.Hide();
      _trashbutton.Hide();
      _clipboardbutton.Hide();
    }

    /// <summary>
    /// Hide front content
    /// </summary>
    void HideFrontContent()
    {
      _title.Hide();
      _titleengraving.Hide();
      _summary.Hide();
      _morecommands.Hide();
    }

    void ShowFrontContent()
    {
      _title.Show();
      _titleengraving.Show();
      _summary.Show();
      _morecommands.Show();
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

    void OnCancelButtonClicked(object sender, EventArgs e)
    {
      PrepareAnimation();

      _anirotate.To = -90;
      _anirotate.Completed -= new EventHandler(OnFlipCompleted_BackToFront);
      _anirotate.Completed += new EventHandler(OnFlipCompleted_BackToFront);
      _sbrotate.Begin();
    }

    void OnFlipCompleted_BackToFront(object sender, EventArgs e)
    {
      _cancelbutton.Hide();

      HideBackContent();
      ShowFrontContent();
      _backgroundbrush.ImageSource = _background;

      _anirotate.Completed -= new EventHandler(OnFlipCompleted_BackToFront);
      _anirotate.To = 0;
      _sbrotate.Begin();
    }
  }
}