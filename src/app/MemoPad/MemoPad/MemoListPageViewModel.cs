using System;
using System.Collections.Generic;
using System.IO.IsolatedStorage;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using MemoPadCore.Common;
using MemoPadCore.Model;
using TapfishCore.Platform;
using TapfishCore.Resources;

namespace MemoPad
{
  /// <summary>
  /// This class is viewmodel for MemoListPage
  /// </summary>
  public class MemoListPageViewModel
  {
    const string BASE_PATH = "\\workspaces";

    public List<TextDocument> Docs { get; private set; }
    public Workspace Workspace { get; private set; }

    /// <summary>
    /// CTOR
    /// </summary>
    public MemoListPageViewModel()
    {
      StorageIo.EnsureDir(BASE_PATH);
    }

    /// <summary>
    /// Open workspace
    /// </summary>
    /// <param name="workspacepath">Workspace name</param>
    public void OpenWorkspace(Workspace ws)
    {
      Workspace = ws;
      BuildDocumentList();
    }

    /// <summary>
    /// Build document list
    /// </summary>
    void BuildDocumentList()
    {
      Docs = new List<TextDocument>();

      foreach (var fn in Workspace.GetMemoFiles(WorkspaceFileAccessMode.Visible))
      {
        var fullpath = Workspace.GetFullPath(fn);
        var doc = new TextDocument(fullpath)
        {
          WorkspaceName = Workspace.Name
        };

        Docs.Add(doc);
      }
    }

    /// <summary>
    /// Refresh workspace
    /// </summary>
    public void RefreshWorkspace()
    {
      BuildDocumentList();
    }

    /// <summary>
    /// Add new document to front
    /// </summary>
    public TextDocument AddNewDocumentToFront()
    {
      var doc = Workspace.NewTextDocument();

      doc.Save();
      Docs.Add(doc);
      return doc;
    }

    /// <summary>
    /// Delete given document
    /// </summary>
    /// <param name="doc">Text document</param>
    public void DeleteDocument(TextDocument doc)
    {
      Docs.Remove(doc);
      doc.Delete();
    }

    /// <summary>
    /// Clear sync
    /// </summary>
    public void ClearSync()
    {
      Workspace.ClearDropBoxSetting();
    }
  }
}