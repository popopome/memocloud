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
    public struct Vector3
    {
        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }

        public Vector3(double x, double y, double z)
            : this()
        {
            X = x;
            Y = y;
            Z = z;
        }

        /// <summary>
        /// Cross product
        /// </summary>
        /// <param name="a">Vector a</param>
        /// <param name="b">Vector b</param>
        /// <returns>Cross product value</returns>
        public static Vector3 Cross(Vector3 a, Vector3 b)
        {
            return Cross(a.X, a.Y, a.Z,
                         b.X, b.Y, b.Z);
        }

        /// <summary>
        /// Cross product
        /// </summary>
        /// <param name="a0"></param>
        /// <param name="a1"></param>
        /// <param name="a2"></param>
        /// <param name="b0"></param>
        /// <param name="b1"></param>
        /// <param name="b2"></param>
        /// <returns></returns>
        public static Vector3 Cross(double a0, double a1, double a2,
                                    double b0, double b1, double b2)
        {
            return new Vector3(
                        a1 * b2 - a2 * b1,
                        a2 * b0 - a0 * b2,
                        a0 * b1 - a1 * b0);
        }
    }
}