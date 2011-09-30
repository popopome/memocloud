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
using MemoPadCore.Common.Messages;
using MemoPadCore.Model;

namespace MemoPad
{
  public class PhotoViewerPageViewModel
  {
    public Memo Memo { get; set; }

    /// <summary>
    /// CTOR
    /// </summary>
    public PhotoViewerPageViewModel()
    {
      Messenger.Default.Register<OpenMemoMessage>
        (this,
        OnOpenMemoMessage);
    }

    /// <summary>
    /// Open memo
    /// </summary>
    /// <param name="msg">Open memo message</param>
    void OnOpenMemoMessage(OpenMemoMessage msg)
    {
      if (msg.Memo.IsPhotoMemo == false)
        return;

      Memo = msg.Memo;
      Memo.Open();
    }
  }

}