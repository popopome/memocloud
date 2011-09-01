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
    public static class PointExtension
    {
        public static double Length(this Point p)
        {
            return System.Math.Sqrt(p.X * p.X + p.Y * p.Y);
        }
    }
}