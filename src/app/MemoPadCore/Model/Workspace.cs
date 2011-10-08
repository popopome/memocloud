using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
using Newtonsoft.Json;
using TapfishCore.Platform;
using TapfishCore.Resources;

namespace MemoPadCore.Model
{
  /// <summary>
  /// This class represents WORKSPACE object.
  /// </summary>
  public class Workspace
  {
    const string LOGTAG = "[Workspace]:";
    public const string WORKSPACE_BASEPATH = "\\workspaces";
    public const string WORKSPACE_BASEPATH_BS = WORKSPACE_BASEPATH + "\\";
    const string CONFIG_FILE_NAME = "cfg";
    const string THUMB_FILE_NAME = "thumb.jpg";

    public string Name { get; set; }
    public string DropBoxToken { get; set; }
    public string DropBoxSecret { get; set; }
    public string DropBoxPath { get; set; }

    public DateTime LastUpdated { get; set; }
    public string SyncHashCode { get; set; }
    public BitmapImage Thumb { get; private set; }
    public string ThumbPath
    {
      get
      {
        return PathUtil.MakePath(_path, THUMB_FILE_NAME);
      }
    }

    public bool HasLastSyncHashCode
    {
      get
      {
        return string.IsNullOrEmpty(SyncHashCode) == false;
      }
    }
    public bool HasDropBoxToken
    {
      get
      {
        return string.IsNullOrEmpty(DropBoxToken) == false;
      }
    }

    string _path;

    /// <summary>
    /// Create workspace
    /// </summary>
    /// <param name="name">Workspace name</param>
    /// <returns>Workspace object</returns>
    public static Workspace Create(string name)
    {
      var ws = new Workspace();
      ws.Open(WORKSPACE_BASEPATH_BS + name);
      return ws;
    }

    /// <summary>
    /// Check given workspace exists
    /// </summary>
    /// <param name="name">Name of workspace</param>
    /// <returns>True if exists, otherwise False</returns>
    public static bool Exists(string name)
    {
      var path = WORKSPACE_BASEPATH_BS + name;
      return StorageIo.DirExists(path);
    }

    /// <summary>
    /// Open workspace
    /// </summary>
    /// <param name="name">Workspace name</param>
    public void Open(string path)
    {
      SyncHashCode = "";
      _path = path;
      StorageIo.EnsureDir(_path);

      //
      // For initial case,
      // make DropBoxPath to be same with CurrentName.
      //
      UpdateName();
      DropBoxPath = PathUtil.MakePath("/", Name);

      LoadConfigData();
    }

    /// <summary>
    /// Update name
    /// </summary>
    /// <param name="name">workspace name</param>
    void UpdateName()
    {
      var list = PathUtil.Split(_path);
      Name = list[list.Length - 1];

    }

    /// <summary>
    /// Load configuration file data
    /// </summary>
    void LoadConfigData()
    {
      var cfgpath = PathUtil.MakePath(_path, CONFIG_FILE_NAME);
      var json = StorageIo.ReadTextFile(cfgpath);
      if (string.IsNullOrEmpty(json))
        return;

      try
      {
        var workspace = JsonConvert.DeserializeObject<Workspace>(json);
        this.Name = workspace.Name;
        this.DropBoxToken = workspace.DropBoxToken;
        this.DropBoxSecret = workspace.DropBoxSecret;
        this.DropBoxPath = workspace.DropBoxPath;
        this.SyncHashCode = workspace.SyncHashCode ?? "";
      }
      catch (System.Exception e)
      {
      }
    }

    /// <summary>
    /// Save configuration data for current workspace.
    /// </summary>
    public void SaveConfigData()
    {
      var cfgpath = PathUtil.MakePath(_path, CONFIG_FILE_NAME);

      var json = JsonConvert.SerializeObject(this);
      StorageIo.WriteTextFile(cfgpath, json);
    }

    /// <summary>
    /// Clear dropbox setting
    /// </summary>
    public void ClearDropBoxSetting()
    {
      this.DropBoxToken = "";
      this.DropBoxSecret = "";
      this.SyncHashCode = "";
      SaveConfigData();
    }

    /// <summary>
    /// Get path
    /// </summary>
    /// <returns></returns>
    public string GetPath()
    {
      return _path;
    }

    public string GetFullPath(string fn)
    {
      return PathUtil.MakePath(_path, fn);
    }

    /// <summary>
    /// Get syncpath
    /// </summary>
    /// <returns>Return a path which is not prefixed with
    /// BASEPATH</returns>
    public string GetSyncPath()
    {
      return _path.Replace(WORKSPACE_BASEPATH, "");
    }

    /// <summary>
    /// Get memo files from given workspace
    /// </summary>
    /// <returns>Array of filenames</returns>
    public string[] GetMemoFiles(WorkspaceFileAccessMode access)
    {
      var files = StorageIo.Files(_path, "*");
      var sortedfiles = SortByModifiedTime(files);

      if (access == WorkspaceFileAccessMode.All)
        return
          (from fn in sortedfiles
           where WorkspaceFileOp.IsMemoFile(fn)
           select fn).ToArray();

      return
          (from fn in sortedfiles
           where WorkspaceFileOp.IsVisibleMemoFile(fn)
           select fn).ToArray();
    }

    struct FileMeta
    {
      public string FullPath;
      public string Name;
      public DateTime ModifiedTime;
    };

    /// <summary>
    /// Sort by modified time
    /// </summary>
    /// <param name="files"></param>
    string[] SortByModifiedTime(string[] files)
    {
      var paths =
        (from fn in files
         let fullpath = this.GetFullPath(fn)
         select new FileMeta
         {
           FullPath = fullpath,
           Name = fn,
           ModifiedTime = FileTimeDb.ReadLastModifiedTime(fullpath)
         }).ToList();

      paths.Sort(new Comparison<FileMeta>(
        (p1, p2) =>
        {
          return p2.ModifiedTime.CompareTo(p1.ModifiedTime);
        }));

      return
        (from p in paths
         select p.Name).ToArray();
    }

    /// <summary>
    /// Change workspace name
    /// </summary>
    /// <param name="oldname">Old workspace name</param>
    /// <param name="newname">New workspace name</param>
    public void ChangeDropboxPath(string newpath)
    {
      if (0 == string.Compare(DropBoxPath, newpath, StringComparison.InvariantCultureIgnoreCase))
        return;

      string p = newpath.Replace('\\', '/');
      p = newpath.Replace('/', '-');

      if (p[0] == '-')
        p = p.Remove(0, 1);

      string newlocalpath = PathUtil.MakePath(WORKSPACE_BASEPATH, p);
      StorageIo.RenameDir(this._path, newlocalpath);
      this._path = newlocalpath;

      var list = PathUtil.Split(newpath);
      this.Name = list[list.Length - 1];
      this.DropBoxPath = newpath;

      SaveConfigData();
    }

    /// <summary>
    /// Seek new file name from given workspace
    /// </summary>
    string SeekNewFileName(string ext)
    {
      const string NEW_FILE_NAME_PREFIX = "Untitled-";

      string fn = "";
      string fullpath = "";

      for (int i = 0; ; ++i)
      {
        fn = string.Concat(NEW_FILE_NAME_PREFIX, i, ext);
        fullpath = this.GetFullPath(fn);
        if (false == StorageIo.Exists(fullpath))
          break;
      }

      return fullpath;
    }

    /// <summary>
    /// Create new text document
    /// </summary>
    /// <returns>Text document object</returns>
    public Memo NewTextDocument()
    {
      var fullpath = SeekNewFileName(AppSetting.TEXT_MEMO_EXT);
      var doc = new Memo(fullpath, MemoKind.Text)
      {
        WorkspaceName = Name
      };
      doc.NewTextMemo();

      return doc;
    }

    /// <summary>
    /// Create new photo memo
    /// </summary>
    /// <param name="photo">Bitmap object</param>
    /// <returns>Create new photo memo</returns>
    public Memo NewPhotoMemo(
              BitmapImage bmp)
    {
      var fullpath = SeekNewFileName(AppSetting.JPEG_EXT);
      var memo = new Memo(fullpath, MemoKind.Photo)
      {
        WorkspaceName = Name
      };
      memo.NewPhotoMemo(bmp);
      return memo;
    }

    /// <summary>
    /// Load thumb
    /// </summary>
    /// <param name="callback">Callback</param>
    /// <returns>Handle for the loading action. If Dispose() is called,
    /// loading will be canceled.</returns>
    public IDisposable LoadThumb(Action callback)
    {
      return BitmapBackgroundLoader.LoadIsoBitmapAsync(
        ThumbPath,
        (result) =>
        {
          if (result.Succeeded)
            callback();
        });
    }

    /// <summary>
    /// Delete workspace
    /// </summary>
    /// <param name="name">Workspace name</param>
    public void Delete()
    {
      StorageIo.DeleteDir(GetPath());
    }
  }
}