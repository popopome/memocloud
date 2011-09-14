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
using TapfishCore.Resources;

namespace TapfishCore.Ui
{
  public class ImageButton : Canvas
  {
    BitmapImage _normalimg;
    BitmapImage _focusimg;
    ImageBrush _imgbrush;

    public event EventHandler Clicked;

    public ImageButton()
    {
      this.ManipulationStarted += new EventHandler<ManipulationStartedEventArgs>(OnManipStarted);
      this.ManipulationCompleted += new EventHandler<ManipulationCompletedEventArgs>(OnManipCompleted);
      this.ManipulationDelta += new EventHandler<ManipulationDeltaEventArgs>(OnManipDelta);
    }

    public void Create(BitmapImage normalbmp,
                       BitmapImage focusbmp)
    {
      Debug.Assert(normalbmp.PixelWidth > 0);
      Debug.Assert(normalbmp.PixelHeight > 0);

      this.Width = normalbmp.PixelWidth;
      this.Height = normalbmp.PixelHeight;

      _normalimg = normalbmp;
      _focusimg = focusbmp;

      _imgbrush = new ImageBrush
      {
        ImageSource = _normalimg
      };
      this.Background = _imgbrush;
    }

    public void Create(double w,
                       double h,
                       string normalimgpath,
                       string focusimgpath)
    {
      this.Width = w;
      this.Height = h;

      _normalimg = BitmapUtils.CreateBitmapImmediately(normalimgpath);
      if (string.IsNullOrEmpty(focusimgpath))
        _focusimg = _normalimg;
      else
        _focusimg = BitmapUtils.CreateBitmapImmediately(focusimgpath);

      _imgbrush = new ImageBrush
      {
        ImageSource = _normalimg
      };
      this.Background = _imgbrush;
    }

    public void SetXYWithMargin(double x, double y)
    {
      this.HorizontalAlignment = HorizontalAlignment.Left;
      this.VerticalAlignment = VerticalAlignment.Top;
      this.Margin = new Thickness(x, y, 0, 0);
    }

    void OnManipStarted(object sender, ManipulationStartedEventArgs e)
    {
      _imgbrush.ImageSource = _focusimg;
      e.Handled = true;
    }

    void OnManipDelta(object sender, ManipulationDeltaEventArgs e)
    {
      var pt = e.ManipulationOrigin;
      if (pt.X < 0
        || pt.X > this.Width
        || pt.Y < 0
        || pt.Y > this.Height)
        _imgbrush.ImageSource = _normalimg;
      else
        _imgbrush.ImageSource = _focusimg;

      e.Handled = true;
    }

    void OnManipCompleted(object sender, ManipulationCompletedEventArgs e)
    {
      e.Handled = true;

      _imgbrush.ImageSource = _normalimg;
      if (UiUtils.IsTapped(e))
      {
        if (Clicked != null)
          Clicked(this, null);
      }
    }

  }
}