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

    const double SUMMARY_FONT_SIZE = 18;

    public event EventHandler<MemoClickedEventArgs> Click;

    #region Static components

    static BitmapImage _background;
    static ImageBrush _backgroundbrush;
    static FontFamily _titlefont;
    static Thickness _titlemargin;

    static SolidColorBrush _titlefontcolor;
    static SolidColorBrush _titleengravingcolor;

    static FontFamily _summaryfont;
    static SolidColorBrush _summaryfontcolor;
    static Thickness _summarymargin;

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
        new Uri("/Images/memo-list/memo-summary-back.png", UriKind.Relative);
      _backgroundbrush = new ImageBrush
      {
        ImageSource = _background
      };

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
    }

    #endregion Static components

    TextBlock _title;
    TextBlock _titleengraving;

    TextBlock _summary;
    public TextDocument Doc { get; private set; }

    /// <summary>
    /// CTOR
    /// </summary>
    public MemoSummaryControl()
    {
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
        HorizontalAlignment = HorizontalAlignment.Center,
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

      this.ManipulationCompleted += new EventHandler<ManipulationCompletedEventArgs>(OnManipulationCompleted);
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
      if (UiUtils.IsTapped(e))
      {
        if (Click != null)
          Click(this, new MemoClickedEventArgs
            {
              Document = Doc
            });
      }
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
  }
}