using System;
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
using Microsoft.Phone.Controls;

namespace TapfishCore.Ui
{
  public class BackKeyHook
  {
    Action<CancelEventArgs> Callback { get; set; }

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="callback"></param>
    public BackKeyHook(Action<CancelEventArgs> callback)
    {
      Callback = callback;
    }

    public void Hook()
    {
      Unhook();
      BackKeyHandlerManager.Register(OnBackKeyPressed);
    }

    public void Unhook()
    {
      BackKeyHandlerManager.Unregister(OnBackKeyPressed);
    }

    void OnBackKeyPressed(CancelEventArgs e)
    {
      if (Callback != null)
        Callback(e);
    }
  }
}