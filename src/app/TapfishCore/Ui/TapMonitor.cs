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
    public class TapMonitor
    {
        Action _callback;

        public TapMonitor(UIElement el, Action callback)
        {
            el.ManipulationCompleted += new EventHandler<ManipulationCompletedEventArgs>(OnManipulationCompleted);
            _callback = callback;
        }

        void OnManipulationCompleted(object sender, ManipulationCompletedEventArgs e)
        {
            if (UiUtils.IsTapped(e))
                if (_callback != null)
                    _callback();
        }
    }
}