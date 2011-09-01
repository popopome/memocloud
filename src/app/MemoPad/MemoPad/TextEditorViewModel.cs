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
    public TextDocument Doc { get; private set; }

    /// <summary>
    /// CTOR
    /// </summary>
    public TextEditorViewModel()
    {
      Messenger.Default.Register<OpenDocumentMessage>(this,
                                                      OnOpenDocument);
    }

    /// <summary>
    /// Open document message
    /// </summary>
    /// <param name="msg">Message from others</param>
    void OnOpenDocument(OpenDocumentMessage msg)
    {
      Doc = msg.Doc;
      Doc.Open();
    }

    /// <summary>
    /// Update and save
    /// </summary>
    /// <param name="title">Title</param>
    /// <param name="text">Text</param>
    public void UpdateAndSave(string title, string text)
    {
      if (title.IsEmpty())
        title = Doc.Title;

      Doc.Text = text;
      if (Doc.Title != title)
        Doc.Delete();

      Doc.ChangeTitle(title);
      Doc.Save();
    }
  }
}