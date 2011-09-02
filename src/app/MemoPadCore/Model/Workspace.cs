using System;
using System.Linq;
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
    const string WORKSPACE_BASEPATH = "\\workspaces";
    const string CONFIG_FILE_NAME = "cfg";

    public string Name { get; set; }
    public string DropBoxToken { get; set; }
    public string DropBoxSecret { get; set; }
    public string DropBoxPath { get; set; }

    public string SyncHashCode { get; set; }

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
      var files = StorageIo.Files(_path, "*.txt");
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

    /// <summary>
    /// Sort by modified time
    /// </summary>
    /// <param name="files"></param>
    string[] SortByModifiedTime(string[] files)
    {
      //
      // WP7.5 is possible.
      //
      /*var stg = IsolatedStorageFile.GetUserStoreForApplication();

      var details = new List<KeyValuePair<string, DateTime>>();
      foreach (var fn in files)
      {
      }*/

      return files;
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
    string SeekNewFileName()
    {
      const string NEW_FILE_NAME_PREFIX = "Untitled-";

      string fn = "";
      string fullpath = "";

      for (int i = 0; ; ++i)
      {
        fn = string.Concat(NEW_FILE_NAME_PREFIX, i, AppSetting.TEXT_DOCUMENT_EXT);
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
    public TextDocument NewTextDocument()
    {
      var fullpath = SeekNewFileName();
      var doc = new TextDocument(fullpath)
      {
        WorkspaceName = Name
      };
      doc.New();

      return doc;
    }
  }
}