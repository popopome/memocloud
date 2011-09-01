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
    public const int DESIRED_WIDTH = 360;
    public const int DESIRED_HEIGHT = 488;

    const int TITLE_WIDTH = 240;
    const int TITLE_HEIGHT = 44;
    const double TITLE_FONT_SIZE = 30;

    const int SUMMARY_WIDTH = 250;
    const int SUMMARY_HEIGHT = 300;
    const double SUMMARY_FONT_SIZE = 20;

    public event EventHandler<MemoClickedEventArgs> Click;

    #region Static components

    static BitmapImage _background;
    static ImageBrush _backgroundbrush;
    static FontFamily _titlefont;
    static Thickness _titlemargin;

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
      _titlemargin = new Thickness(0, 25, 0, 0);

      _summaryfont = new FontFamily("Segoe WP Light");
      _summaryfontcolor = new SolidColorBrush(Color.FromArgb(255, 136, 136, 136));
      _summarymargin = new Thickness(0, 85, 0, 0);
    }

    #endregion Static components

    TextBlock _title;
    TextBlock _summary;
    public TextDocument Doc { get; private set; }

    /// <summary>
    /// CTOR
    /// </summary>
    public MemoSummaryControl()
    {
      this.Background = _backgroundbrush;

      _title = new TextBlock
      {
        HorizontalAlignment = HorizontalAlignment.Center,
        VerticalAlignment = VerticalAlignment.Top,
        Margin = _titlemargin,
        FontFamily = _titlefont,
        FontSize = TITLE_FONT_SIZE,
        MaxWidth = TITLE_WIDTH,
        TextWrapping = TextWrapping.NoWrap,
        Height = TITLE_HEIGHT,
        Foreground = new SolidColorBrush(Colors.Black)
      };
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

    /// <summary>
    /// Open text document
    /// </summary>
    /// <param name="doc"></param>
    public void Open(TextDocument doc)
    {
      Doc = doc;
      doc.LoadSummary();

      _title.Text = doc.Title;
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