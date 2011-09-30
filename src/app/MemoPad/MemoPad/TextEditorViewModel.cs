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
using TapfishCore.Platform;

namespace MemoPad
{
  public class TextEditorViewModel
  {
    public Memo Memo { get; private set; }

    /// <summary>
    /// CTOR
    /// </summary>
    public TextEditorViewModel()
    {
      Messenger.Default.Register<OpenMemoMessage>(this,
                                                      OnOpenMemoMessage);
    }

    /// <summary>
    /// Open document message
    /// </summary>
    /// <param name="msg">Message from others</param>
    void OnOpenMemoMessage(OpenMemoMessage msg)
    {
      //
      // Only accept text memo
      //
      if (msg.Memo.IsTextMemo == false)
        return;

      Memo = msg.Memo;
      Memo.Open();
    }

    /// <summary>
    /// Update and save
    /// </summary>
    /// <param name="title">Title</param>
    /// <param name="text">Text</param>
    public void UpdateAndSave(string title, string text)
    {
      if (title.IsEmpty())
        title = Memo.Title;

      string oldtitle = Memo.Title;
      string oldtext = Memo.Text;

      bool ismodified = false;
      if (oldtitle != Memo.Title)
        ismodified = true;
      else if (oldtext != text)
        ismodified = true;

      if (ismodified == false)
        return;

      Memo.Text = text;
      if (Memo.Title != title)
        Memo.Delete();

      Memo.RenameTo(title);
      Memo.Save();
    }
  }
}