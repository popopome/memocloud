using System;
using System.Net;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace TapfishCore.Platform
{
  public class ThreadUtil
  {
    public static void SafeCall(Action a)
    {
      Deployment.Current.Dispatcher.BeginInvoke(a);
    }

    public static void Execute(Action a)
    {
      Thread t = new Thread(new ThreadStart(() => a()));
      t.Start();
    }

    public static void Execute<T>(Action<T> a, T param)
      where T : class
    {
      Thread t = new Thread(new ParameterizedThreadStart((p) => a(p as T)));
      t.Start(param);
    }

    public static void UiCall(Action a)
    {
      Deployment.Current.Dispatcher.BeginInvoke(a);
    }

  }
}