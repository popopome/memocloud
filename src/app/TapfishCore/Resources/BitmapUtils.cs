using System;
using System.Diagnostics;
using System.IO;
using System.IO.IsolatedStorage;
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
using Microsoft.Xna.Framework.Media;
using TapfishCore.Math;

namespace TapfishCore.Resources
{
  public class BitmapUtils
  {

    /// <summary>
    /// Convert bitmap image to jpeg bytes
    /// </summary>
    /// <param name="img"></param>
    /// <returns></returns>
    public static byte[] JpegBytesFromBitmapImage(BitmapSource img)
    {
      const int DEFAULT_QUALITY = 70;
      return JpegBytesFromBitmapImage(img, DEFAULT_QUALITY);
    }

    /// <summary>
    /// Bitmap image to jpeg bytes
    /// </summary>
    /// <param name="img"></param>
    /// <param name="qualityLevel"></param>
    /// <returns></returns>
    public static byte[] JpegBytesFromBitmapImage(
                    BitmapSource img,
                    int qualityLevel)
    {
      var wbmp = new WriteableBitmap(img);
      using (var stm = new MemoryStream())
      {
        wbmp.SaveJpeg(
                stm,
                wbmp.PixelWidth,
                wbmp.PixelHeight,
                0,
                qualityLevel);

        return stm.ToArray();
      }
    }

    /// <summary>
    /// Save bitmap to media library
    /// </summary>
    /// <param name="bmp"></param>
    public static Picture SaveBitmapToMediaLibrary(BitmapSource bmp)
    {
      const string FILENAME_PREFIX = "TapfishPhoto_";
      using (var stm = new MemoryStream())
      {
        var wbmp = new WriteableBitmap(bmp);
        Extensions.SaveJpeg(wbmp, stm, wbmp.PixelWidth, wbmp.PixelHeight, 0, 85);
        stm.Seek(0, SeekOrigin.Begin);

        var outputName = string.Concat(FILENAME_PREFIX, DateTime.Now.Ticks);
        var library = new MediaLibrary();
        var pic = library.SavePicture(outputName, stm);
        return pic;
      }
    }

    public static void SaveBitmapToIso(string path,
                                       BitmapSource src)
    {
      byte[] raw = JpegBytesFromBitmapImage(src);
      StorageIo.WriteBinaryFile(path, raw);
    }

    public static BitmapImage LoadBitmapFromIso(string path)
    {
      return LoadBitmapFromIso(path,
        BitmapCreateOptions.BackgroundCreation
            | BitmapCreateOptions.DelayCreation);
    }

    public static BitmapImage LoadBitmapFromIso(string path,
      BitmapCreateOptions options)
    {
      byte[] raw = StorageIo.ReadBinaryFile(path);
      if (null == raw)
        return null;

      using (var stm = new MemoryStream())
      {
        stm.Write(raw, 0, raw.Length);
        stm.Seek(0, SeekOrigin.Begin);

        var img = new BitmapImage
        {
          CreateOptions = options
        };

        img.SetSource(stm);
        return img;
      }
    }

    public static BitmapSource RotateBitmap(BitmapSource bmp, double angle)
    {
      Image img = new Image();
      img.Source = bmp;

      Canvas c = new Canvas();
      c.Width = bmp.PixelWidth;
      c.Height = bmp.PixelHeight;
      c.Children.Add(img);

      TransformGroup grp = new TransformGroup();
      grp.Children.Add(new TranslateTransform { X = -c.Width / 2, Y = -c.Height / 2 });
      grp.Children.Add(new RotateTransform { Angle = angle });
      grp.Children.Add(new TranslateTransform { X = c.Height / 2, Y = c.Width / 2 });

      var wbmp = new WriteableBitmap(c, grp);
      wbmp.Invalidate();
      return wbmp;
    }

    /// <summary>
    /// Capture element
    /// </summary>
    /// <param name="el">Element</param>
    /// <returns>Bitmap image</returns>
    public static BitmapSource CaptureElement(UIElement el)
    {
      var bmp = new WriteableBitmap(el, new TranslateTransform());
      bmp.Invalidate();
      bmp.Render(el, new TranslateTransform());
      return bmp;
    }

    public static BitmapImage CreateBitmapImmediately(string path)
    {
      var stminfo = Application.GetResourceStream(new Uri(path, UriKind.Relative));
      if (null == stminfo)
        return null;

      var bmp = new BitmapImage
      {
        CreateOptions = BitmapCreateOptions.None
      };

      bmp.SetSource(stminfo.Stream);
      return bmp;
    }

    /// <summary>
    /// Resize bitmap
    /// </summary>
    /// <param name="src">Source bitmap</param>
    /// <param name="maxw">Maximum width</param>
    /// <param name="maxh">Maximum height</param>
    /// <returns></returns>
    public static BitmapSource ResizeBitmap(
                    BitmapSource src,
                    int maxw,
                    int maxh)
    {
      Debug.Assert(src.PixelWidth > 0);
      Debug.Assert(src.PixelHeight > 0);

      var m = Matrix33
            .Create()
            .SetRectToRect(
              new Rect(0, 0, src.PixelWidth, src.PixelHeight),
              new Rect(0, 0, maxw, maxh),
              Stretch.Uniform);

      var lt = m.Transform(0, 0);
      var rb = m.Transform(src.PixelWidth, src.PixelHeight);

      var img = new Image
      {
        Stretch = Stretch.Uniform,
        Source = src,
        Width = rb.X - lt.X,
        Height = rb.Y - lt.Y
      };
      var wbmp = new WriteableBitmap(img, null);
      wbmp.Invalidate();
      return wbmp;
    }

  }
}