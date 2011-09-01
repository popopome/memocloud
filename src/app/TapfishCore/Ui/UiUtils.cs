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

namespace TapfishCore.Ui
{
    public class UiUtils
    {
        public static readonly double TAP_THRESHOLD = 32;

        /// <summary>
        /// Is Tapped.
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        public static bool IsTapped(ManipulationCompletedEventArgs e)
        {
            if (e.IsInertial)
                return false;

            Point t = e.TotalManipulation.Translation;
            return System.Math.Abs(t.X) <= TAP_THRESHOLD
                && System.Math.Abs(t.Y) <= TAP_THRESHOLD;
        }
    }
}