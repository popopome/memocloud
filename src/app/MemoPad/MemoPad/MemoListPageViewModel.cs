using System;
using System.Collections.Generic;
using System.IO;
using System.IO.IsolatedStorage;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
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

    public List<Memo> MemoList { get; private set; }
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
      BuildMemoList();
    }

    /// <summary>
    /// Build document list
    /// </summary>
    void BuildMemoList()
    {
      MemoList = new List<Memo>();

      foreach (var fn in Workspace.GetMemoFiles(WorkspaceFileAccessMode.Visible))
      {
        var fullpath = Workspace.GetFullPath(fn);
        var kind =
          Memo.IsPhotoMemoFile(fn)
          ? MemoKind.Photo
          : MemoKind.Text;

        var doc = new Memo(fullpath, kind)
        {
          WorkspaceName = Workspace.Name
        };

        MemoList.Add(doc);
      }
    }

    /// <summary>
    /// Refresh workspace
    /// </summary>
    public void RefreshWorkspace()
    {
      BuildMemoList();
    }

    /// <summary>
    /// Add new document to front
    /// </summary>
    public Memo AddNewTextMemoToFront()
    {
      var doc = Workspace.NewTextDocument();
      doc.Save();
      MemoList.Add(doc);
      return doc;
    }

    /// <summary>
    /// Add new photo to front side
    /// </summary>
    /// <param name="photo">Bitmap object</param>
    /// <returns>Create memo</returns>
    public Memo AddNewPhotoMemoToFront(
        BitmapImage bmp,
        Stream stm)
    {
      var memo = Workspace.NewPhotoMemo(bmp, stm);
      memo.Save();
      MemoList.Add(memo);
      return memo;
    }

    /// <summary>
    /// Delete given document
    /// </summary>
    /// <param name="doc">Text document</param>
    public void DeleteMemo(Memo doc)
    {
      MemoList.Remove(doc);
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