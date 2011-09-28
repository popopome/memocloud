using System;
using System.Diagnostics;

using System.Diagnostics;

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
  /// <summary>
  /// Matrix 3x3
  /// </summary>
  public struct Matrix33
  {
    public double n00;
    public double n01;
    public double n02;

    public double n10;
    public double n11;
    public double n12;

    public double n20;
    public double n21;
    public double n22;

    public double Tx { get { return n02; } }
    public double Ty { get { return n12; } }
    public double Sx { get { return n00; } }
    public double Sy { get { return n11; } }

    public static Matrix33 Create()
    {
      var m = default(Matrix33);
      m.Identity();
      return m;
    }

    public Matrix33(Matrix33 m)
      : this()
    {
      this = m;
    }

    /// <summary>
    /// Identity matrix
    /// </summary>
    public Matrix33 Identity()
    {
      n00 = 1;
      n01 = 0;
      n02 = 0;

      n10 = 0;
      n11 = 1;
      n12 = 0;

      n20 = 0;
      n21 = 0;
      n22 = 1;

      return this;
    }

    public Matrix33 SetRectToRect(Rect src, Rect dst, Stretch stretch)
    {
      double tx = 0;
      double ty = 0;
      double sx = dst.Width / src.Width;
      double sy = dst.Height / src.Height;

      bool xlarger = false;

      if (stretch != Stretch.Fill)
      {
        if (stretch == Stretch.Uniform)
        {
          if (sx > sy)
          {
            xlarger = true;
            sx = sy;
          }
          else
            sy = sx;
        }
        else
        {
          if (sx > sy)
          {
            xlarger = true;
            sy = sx;
          }
          else
            sx = sy;
        }
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
      else if (stretch == Stretch.UniformToFill)
      {
        double diff = 0;
        if (xlarger)
          diff = dst.Height - src.Height * sy;
        else
          diff = dst.Width - src.Width * sx;

        diff /= 2;

        if (xlarger)
          ty += diff;
        else
          tx += diff;
      }

      this.Identity();
      this.n00 = sx;
      this.n11 = sy;
      this.n02 = tx;
      this.n12 = ty;

      return this;
    }

    public Matrix33 FromMatrix(Matrix m)
    {
      n00 = m.M11;
      n01 = m.M21;
      n02 = m.OffsetX;

      n10 = m.M12;
      n11 = m.M22;
      n12 = m.OffsetY;

      n20 = n21 = 0;
      n22 = 1;
      return this;
    }

    public Matrix ToMatrix()
    {
      //
      // Silverlight Matrix is column first one.
      // Hence I map each field by row first one.
      //
      return new Matrix
      {
        M11 = n00,
        M21 = n01,
        OffsetX = n02,
        M12 = n10,
        M22 = n11,
        OffsetY = n12
      };
    }

    public Matrix33 Mul(Matrix33 m)
    {
      this = Mul(this, m);
      return this;
    }

    public static Matrix33 Mul(Matrix33 m1, Matrix33 m2)
    {
      return new Matrix33
      {
        n00 = m1.n00 * m2.n00 + m1.n01 * m2.n10 + m1.n02 * m2.n20,
        n01 = m1.n00 * m2.n01 + m1.n01 * m2.n11 + m1.n02 * m2.n21,
        n02 = m1.n00 * m2.n02 + m1.n01 * m2.n12 + m1.n02 * m2.n22,

        n10 = m1.n10 * m2.n00 + m1.n11 * m2.n10 + m1.n12 * m2.n20,
        n11 = m1.n10 * m2.n01 + m1.n11 * m2.n11 + m1.n12 * m2.n21,
        n12 = m1.n10 * m2.n02 + m1.n11 * m2.n12 + m1.n12 * m2.n22,

        n20 = m1.n20 * m2.n00 + m1.n21 * m2.n10 + m1.n22 * m2.n20,
        n21 = m1.n20 * m2.n01 + m1.n21 * m2.n11 + m1.n22 * m2.n21,
        n22 = m1.n20 * m2.n02 + m1.n21 * m2.n12 + m1.n22 * m2.n22
      };
    }

    public Matrix33 PreTranslate(double tx, double ty)
    {
      n02 = n01 * tx + n02 * ty + n02;
      n12 = n11 * tx + n12 * ty + n12;
      return this;
    }

    public Matrix33 PostTranslate(double tx, double ty)
    {
      n02 += tx;
      n12 += ty;

      return this;
    }

    public Matrix33 PreRotate(double angleDegree)
    {
      double angleRadian = MathUtils.DEG2RAD(angleDegree);
      double sinval = System.Math.Sin(angleRadian);
      double cosval = System.Math.Cos(angleRadian);

      double new00 = n00 * cosval + n01 * sinval;
      double new01 = -n00 * sinval + n01 * cosval;

      double new10 = n10 * cosval + n11 * sinval;
      double new11 = -n10 * sinval + n11 * cosval;

      n00 = new00;
      n01 = new01;
      n10 = new10;
      n11 = new11;

      return this;
    }

    public Matrix33 PostRotate(double angleDegree)
    {
      double angleRadian = MathUtils.DEG2RAD(angleDegree);

      double sinval = System.Math.Sin(angleRadian);
      double cosval = System.Math.Cos(angleRadian);

      double new00 = cosval * n00 - sinval * n10;
      double new01 = cosval * n01 - sinval * n11;
      double new02 = cosval * n02 - sinval * n12;

      double new10 = sinval * n00 + cosval * n10;
      double new11 = sinval * n01 + cosval * n11;
      double new12 = sinval * n02 + cosval * n12;

      n00 = new00;
      n01 = new01;
      n02 = new02;

      n10 = new10;
      n11 = new11;
      n12 = new12;

      return this;
    }

    public Matrix33 PreScale(double sx, double sy)
    {
      n00 *= sx;
      n01 *= sy;
      n10 *= sx;
      n11 *= sy;
      return this;
    }

    public Matrix33 PostScale(double sx, double sy)
    {
      n00 *= sx;
      n01 *= sx;
      n02 *= sx;

      n10 *= sy;
      n11 *= sy;
      n12 *= sy;

      return this;
    }

    /// <summary>
    /// This guy returns the inverse of matrix.
    /// The code is from Ogre3d.
    /// </summary>
    /// <returns></returns>
    public Matrix33 Inverse()
    {
      Matrix33 inv = new Matrix33
      {
        n00 = n11 * n22 - n12 * n21,
        n01 = n02 * n21 - n01 * n22,
        n02 = n01 * n12 - n02 * n11,

        n10 = n12 * n20 - n10 * n22,
        n11 = n00 * n22 - n02 * n20,
        n12 = n02 * n10 - n00 * n12,

        n20 = n10 * n21 - n11 * n20,
        n21 = n01 * n20 - n00 * n21,
        n22 = n00 * n11 - n01 * n10
      };

      double det =
          n00 * inv.n00 +
          n01 * inv.n10 +
          n02 * inv.n20;
      Debug.Assert(det >= 0.00001);
      return inv.MulScala(1 / det);
    }

    /// <summary>
    /// Multiply scala value
    /// </summary>
    /// <param name="val">Double value</param>
    /// <returns>Return updated matrix</returns>
    public Matrix33 MulScala(double val)
    {
      n00 *= val;
      n01 *= val;
      n02 *= val;

      n10 *= val;
      n11 *= val;
      n12 *= val;

      n20 *= val;
      n21 *= val;
      n22 *= val;

      return this;
    }

    public Point Transform(Point pt)
    {
      return Transform(pt.X, pt.Y);
    }

    public Point Transform(double x, double y)
    {
      return new Point(n00 * x + n01 * y + n02,
                       n10 * x + n11 * y + n12);
    }

    public Rect Transform(double x1, double y1, double x2, double y2)
    {
      var lt = Transform(x1, y1);
      var rb = Transform(x2, y2);
      return new Rect(lt, rb);
    }

    public static Matrix33 Interpolate(
            Matrix33 a,
            Matrix33 b,
            double t)
    {
      double invt = 1.0 - t;

      var m = Matrix33.Create();
      m.n00 = (a.n00 * invt + b.n00 * t);
      m.n01 = (a.n01 * invt + b.n01 * t);
      m.n02 = (a.n02 * invt + b.n02 * t);

      m.n10 = (a.n10 * invt + b.n10 * t);
      m.n11 = (a.n11 * invt + b.n11 * t);
      m.n12 = (a.n12 * invt + b.n12 * t);

      m.n20 = (a.n20 * invt + b.n20 * t);
      m.n21 = (a.n21 * invt + b.n21 * t);
      m.n22 = (a.n22 * invt + b.n22 * t);

      return m;
    }

    public bool IsSame(Matrix33 rhs)
    {
      return n00 == rhs.n00
        && n01 == rhs.n01
        && n02 == rhs.n02
        && n10 == rhs.n10
        && n11 == rhs.n11
        && n12 == rhs.n12
        && n20 == rhs.n20
        && n21 == rhs.n21
        && n22 == rhs.n22;
    }
  }
}