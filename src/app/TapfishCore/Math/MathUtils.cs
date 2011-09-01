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
    public class MathUtils
    {
        /// <summary>
        /// Convert radian to degree
        /// </summary>
        /// <param name="radian"></param>
        /// <returns></returns>
        public static double RAD2DEG(double radian)
        {
            return 180 * radian / System.Math.PI;
        }

        /// <summary>
        /// Convert degree to radian
        /// </summary>
        /// <param name="degree"></param>
        /// <returns></returns>
        public static double DEG2RAD(double degree)
        {
            return System.Math.PI * degree / 180;
        }
    }
}