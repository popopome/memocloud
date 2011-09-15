using System;
using System.Collections.Generic;
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
using Microsoft.Phone.Controls;

namespace TapfishCore.Ui
{
  public class BackKeyHandler
  {
    private class BackKeyCallback
    {
      public Func<bool> Callback { get; set; }

      public void OnBackKeyPressed(
            object sender,
            System.ComponentModel.CancelEventArgs e)
      {
        e.Cancel = Callback();
      }
    }

    static Dictionary<int, BackKeyCallback> _callbacktable
      = new Dictionary<int, BackKeyCallback>();

    public static void InstallBackKeyHandler(int id, Func<bool> action)
    {
      var frame =
        (Application.Current.RootVisual as PhoneApplicationFrame).Content
        as PhoneApplicationPage;
      if (null == frame)
        return;

      var backkeycallback = new BackKeyCallback
      {
        Callback = action
      };

      Debug.Assert(false == _callbacktable.ContainsKey(id));
      _callbacktable.Add(id, backkeycallback);

      frame.BackKeyPress +=
        new EventHandler<System.ComponentModel.CancelEventArgs>(backkeycallback.OnBackKeyPressed);
    }

    public static void UninstallBackKeyHandler(int id)
    {
      if (_callbacktable.ContainsKey(id) == false)
        return;

      var backkeycallback = _callbacktable[id];
      var frame =
        (Application.Current.RootVisual as PhoneApplicationFrame).Content
        as PhoneApplicationPage;
      if (null == frame)
        return;

      frame.BackKeyPress -=
        new EventHandler<System.ComponentModel.CancelEventArgs>(backkeycallback.OnBackKeyPressed);
      _callbacktable.Remove(id);
    }

  }
}