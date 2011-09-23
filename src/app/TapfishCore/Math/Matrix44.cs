using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace TapfishCore.Math
{

  public class Matrix44
  {
    public static CompositeTransform SetRectToRect(Rect src,
            Rect dst,
            Stretch stretch)
    {
      double tx = 0;
      double ty = 0;
      double sx = dst.Width / src.Width;
      double sy = dst.Height / src.Height;

      bool xlarger = false;

      if (stretch != Stretch.Fill)
      {
        if (sx > sy)
        {
          xlarger = true;
          sx = sy;
        }
        else
          sy = sx;
      }

      tx = dst.Left - src.Left * sx;
      ty = dst.Top - src.Top * sy;

      if (stretch == Stretch.Uniform)
      {
        double diff = 0;
        if (xlarger)
          diff = dst.Width - src.Width * sx;
        else
          diff = dst.Height - src.Height * sy;

        diff /= 2;

        if (xlarger)
          tx += diff;
        else
          ty += diff;
      }

      return
        new CompositeTransform
        {
          ScaleX = sx,
          ScaleY = sy,
          TranslateX = tx,
          TranslateY = ty
        };
    }
  }
}