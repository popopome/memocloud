using System;
using System.Collections.Generic;
using System.Net;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace MemoPadCore
{
  public class TapAndHoldRepeater
  {
    UIElement _el;
    IDisposable _repeatconn;
    Action<bool> _callback;

    public void Attach(UIElement el,
                       Action<bool> callback)
    {
      _el = el;
      InstallTouchHandlers(el);
      _callback = callback;
    }

    public void Detach()
    {
      if (_el != null)
      {
        UninstallTouchHandlers(_el);
        _el = null;
      }

      if (_repeatconn != null)
      {
        _repeatconn.Dispose();
        _repeatconn = null;
      }
      _callback = null;
    }

    void InstallTouchHandlers(UIElement el)
    {
      UninstallTouchHandlers(el);
      el.ManipulationCompleted += new EventHandler<ManipulationCompletedEventArgs>(OnManipulationCompleted);
      el.ManipulationStarted += new EventHandler<ManipulationStartedEventArgs>(OnManipulationStarted);
    }

    void UninstallTouchHandlers(UIElement el)
    {
      el.ManipulationCompleted -= new EventHandler<ManipulationCompletedEventArgs>(OnManipulationCompleted);
      el.ManipulationStarted -= new EventHandler<ManipulationStartedEventArgs>(OnManipulationStarted);
    }

    void OnManipulationStarted(object sender, ManipulationStartedEventArgs e)
    {

      _repeatconn = Observable.Timer(TimeSpan.FromMilliseconds(700),
                                     TimeSpan.FromMilliseconds(100))
                                 .ObserveOnDispatcher()
                                 .Subscribe(
                                  val =>
                                  {
                                    if (_callback != null)
                                      _callback(false);
                                  }
                                 );
    }

    void OnManipulationCompleted(object sender, ManipulationCompletedEventArgs e)
    {
      if (_repeatconn != null)
      {
        _repeatconn.Dispose();
        _repeatconn = null;
      }

      if (_callback != null)
        _callback(true);
    }
  }
}