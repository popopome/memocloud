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
using GalaSoft.MvvmLight.Messaging;
using MemoPadCore.Model;

namespace MemoPadCore.Common.Messages
{
  public class OpenMemoMessage : NotificationMessage
  {
    public Memo Memo { get; private set; }

    /// <summary>
    /// CTOR
    /// </summary>
    public OpenMemoMessage(Memo memo)
      : base("open-document-message")
    {
      this.Memo = memo;
    }
  }
}