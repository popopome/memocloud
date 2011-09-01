using System;
using System.Collections.Generic;
using System.ComponentModel;
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
  public class BackKeyHandlerManager
  {
    public static List<Action<CancelEventArgs>> BackKeyHandlers { get; set; }

    static BackKeyHandlerManager()
    {
      BackKeyHandlers = new List<Action<CancelEventArgs>>();
    }

    public static void Register(Action<CancelEventArgs> handler)
    {
      int index = BackKeyHandlers.IndexOf(handler);
      if (-1 != index)
        return;

      BackKeyHandlers.Add(handler);
    }

    public static void Unregister(Action<CancelEventArgs> handler)
    {
      int index = BackKeyHandlers.IndexOf(handler);
      if (-1 != index)
        BackKeyHandlers.RemoveAt(index);
    }

    public static void Process(CancelEventArgs args)
    {
      int cnt = BackKeyHandlers.Count - 1;
      for (int i = cnt; i >= 0; --i)
      {
        BackKeyHandlers[i].Invoke(args);
        if (args.Cancel == true)
          break;
      }
    }
  }
}