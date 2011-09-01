using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.IsolatedStorage;
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
using DropNet;
using DropNet.Exceptions;
using DropNet.Models;
using MemoPadCore.Common;
using TapfishCore.Platform;
using TapfishCore.Resources;

namespace MemoPadCore.Model
{
  /// <summary>
  /// This class has a reponsibility to sync remote DROPBOX folder
  /// and local WORKSPACE.
  /// </summary>
  public class DropboxSync
  {
    const string LOGTAG = "[DropboxSync]:";

    Workspace _ws;
    DropNetClient _client;

    int _totaluploadings;
    int _totaldownloadings;

    List<string> _uploadingfiles;
    List<MetaData> _downloadingfiles;

    public event EventHandler<DropboxSyncEventArgs> SyncStarted;
    public event EventHandler<DropboxSyncEventArgs> SyncStepped;
    public event EventHandler<DropboxSyncEventArgs> Finished;

    /// <summary>
    /// CTOR
    /// </summary>
    /// <param name="ws">Workspace</param>
    public DropboxSync(Workspace ws)
    {
      _ws = ws;
    }

    /// <summary>
    /// Start dropbox sync
    /// </summary>
    public void Start()
    {
      Debug.Assert(_ws.HasDropBoxToken);

      _client = new DropNetClient(
                  AppSetting.DROPBOX_API_KEY,
                  AppSetting.DROPBOX_API_SECRET,
                  _ws.DropBoxToken,
                  _ws.DropBoxSecret);

      //
      // is first time sync?
      //
      bool isfirstsync = (_ws.HasLastSyncHashCode == false);
      if (isfirstsync)
      {
        Log("First sync:{0}...", _ws.Name);
        _client.GetMetaDataAsync(_ws.DropBoxPath,
                                 OnFirstSyncMetaDataReceived,
                                 OnFirstSyncMetaDataError);
        return;
      }

      Log("Sync was done once before");
      SyncFiles();
    }

    /// <summary>
    /// Meta data
    /// </summary>
    /// <param name="meta">meta data</param>
    void OnFirstSyncMetaDataReceived(MetaData meta)
    {
      //
      // This is a first sync,
      // but remote folder is already deleted.
      //
      // Let's create new dropbox folder
      // with current workspace name.
      //
      if (meta.Is_Deleted)
      {
        Log("First sync but remote folder is already removed");
        Log("Let's create new folder => {0}", _ws.Name);

        ThreadUtil.Execute(CreateDropboxFolder);
        return;
      }

      //
      // This is FIRST SYNC.
      // But remote site already has duplicated FOLDER.
      //
      // So let's show warning message to user
      //
      ThreadUtil.UiCall(() =>
        {
          Log("Duplicated folder is found!");

          var msg =
            string.Format("A {0} folder already exists on your DropBox. Click \"OK\" if you want to continue sync and it will replace all files under {0} Dropbox folder with local device version.\nAre you sure to continue syncing anyway?",
                          _ws.DropBoxPath);
          var result =
            MessageBox.Show(msg,
                          "Confirmation",
                          MessageBoxButton.OKCancel);
          if (result == MessageBoxResult.Cancel)
          {
            Log("User canceled to sync with duplicated remote folder");

            //
            // Receiver should remove DROPBOX information
            // from workspace file.
            //
            Finished(this,
                     new DropboxSyncEventArgs
                     {
                       Result = DropboxSyncResult.DuplicateFolderFound
                     });
            return;
          }

          //
          // Let's sync anyway.
          //
          Log("Dup folder. But Keep going to sync!");

          ThreadUtil.Execute(SyncFiles);
        });
    }

    /// <summary>
    /// Folder meta data could not found.
    /// </summary>
    void OnFirstSyncMetaDataError(DropboxException err)
    {
      //
      // Remote site has no such an folder.
      // This is expected response from server.
      //
      // Let's create dropbox folder and
      // start to sync between REMOTE and LOCAL.
      //
      if (err.StatusCode == HttpStatusCode.NotFound)
      {
        Log("No folder on remote side. Let's create new folder:{0}",
            _ws.Name);
        ThreadUtil.Execute(CreateDropboxFolder);
        return;
      }

      FireFinishedEvent(DropboxSyncResult.UnknownError,
                        err.Response.ErrorMessage);
    }

    /// <summary>
    /// Create dropbox folder
    /// </summary>
    void CreateDropboxFolder()
    {
      _client.CreateFolderAsync(
        _ws.DropBoxPath,
        (meta) =>
        {
          Log("Success to create remote folder:{0}",
              _ws.Name);
          ThreadUtil.Execute(SyncFiles);
        },
      (err) =>
      {
        Log("Unable to create remote folder:{0}", _ws.Name);
        FireFinishedEvent(
            DropboxSyncResult.UnableToCreateFolder,
            err.Response.ErrorMessage);
      });
    }

    /// <summary>
    /// Fire finish event with given result code
    /// </summary>
    /// <param name="result">Dropbox sync result code</param>
    void FireFinishedEvent(DropboxSyncResult result,
                           string errormessage)
    {
      Finished(this,
        new DropboxSyncEventArgs
        {
          Result = result,
          ErrorMessage = errormessage
        });
    }

    /// <summary>
    /// Upload single file
    /// </summary>
    void UploadSingleFile()
    {
      if (_uploadingfiles.Count == 0)
      {
        FireSyncSteppedEvent("Uploading...");

        Log("No more uploading files");
        SyncLatestMetaData();
        return;
      }

      string name = _uploadingfiles[0];
      string path = _ws.GetFullPath(name);
      _uploadingfiles.RemoveAt(0);

      Log("Let's upload single file:{0}==>remote:{1}",
          path,
          _ws.DropBoxPath);

      byte[] filedata = StorageIo.ReadBinaryFile(path);
      if (null == filedata)
      {
        Log("Unable to read local file:{0}", path);
        FireFinishedEvent(DropboxSyncResult.UnableToReadFile,
                          string.Format("Failed to read local file:{0}",
                                        path));
        return;
      }

      _client.UploadFileAsync(
        _ws.DropBoxPath,
        name,
        filedata,
        (resp) =>
        {
          Log("Upload succeeded!:{0}", path);
          ThreadUtil.Execute(UploadSingleFile);
        },
        (err) =>
        {
          Log("Upload failed!:{0}", ErrorMessage(err));
          FireFinishedEvent(
              DropboxSyncResult.UnableToUploadFile,
              ErrorMessage(err));
        });
    }

    /// <summary>
    /// Update last hash code
    /// </summary>
    void SyncLatestMetaData()
    {
      _client.GetMetaDataAsync(
        _ws.DropBoxPath,
        (meta) =>
        {
          _ws.SyncHashCode = meta.Hash;
          var remotefiles = meta.Contents;
          TouchLocalFilesModifiedTime(remotefiles);
          _ws.SaveConfigData();

          Log("Sync Succeeded!:{0}", _ws.DropBoxPath);
          FireFinishedEvent(
              DropboxSyncResult.Success,
              "");
        },
        (err) =>
        {
          Log("SyncMetaData failed:{0}", ErrorMessage(err));
          FireFinishedEvent(
            DropboxSyncResult.UnableToSyncMetaData,
            ErrorMessage(err));
        });
    }

    /// <summary>
    /// Error message
    /// </summary>
    /// <param name="err">Dropbox exception</param>
    /// <returns>Error message from exception object</returns>
    string ErrorMessage(DropboxException err)
    {
      if (err == null
        || err.Response == null)
        return "Unknown error";

      return err.Response.Content;
    }

    /// <summary>
    /// Touch local files
    /// </summary>
    /// <param name="remotefiles">Remote file data</param>
    void TouchLocalFilesModifiedTime(List<MetaData> remotefiles)
    {
      var found =
        from local in _ws.GetMemoFiles()
        from remote in remotefiles
        where remote.Name == local
        select new Pair
        {
          LocalFilePath = local,
          RemoteFileMeta = remote
        };

      foreach (var v in found)
      {
        var fullpath = PathUtil.MakePath(_ws.GetPath(), v.LocalFilePath);
        StorageIo.WriteLastModifiedTime(fullpath, v.RemoteFileMeta.ModifiedDate);
      }
    }

    /// <summary>
    /// Keep doing sync.
    /// </summary>
    void SyncFiles()
    {
      Debug.Assert(_ws.HasLastSyncHashCode == true);

      _client.GetMetaDataAsync(
        _ws.DropBoxPath,
        (meta) =>
        {
          Log("Success to get folder metadata. Let's continue sync...");
          var localfiles = _ws.GetMemoFiles();
          DetermineUploadingDownloadingFiles(
            localfiles,
            meta.Contents);

          _totaldownloadings = _downloadingfiles.Count;
          _totaluploadings = _uploadingfiles.Count;

          FireSyncStartedEvent();

          ThreadUtil.Execute(DownloadSingleFile);
        },
        (err) =>
        {
          if (err.StatusCode == HttpStatusCode.NotModified)
          {
            Log("Given workspace was not modifed since last sync");
            FireFinishedEvent(DropboxSyncResult.Success, "");
            return;
          }

          Log("Unable to get folder metadata:{0},{1}",
            _ws.DropBoxPath,
            ErrorMessage(err));
          FireFinishedEvent(DropboxSyncResult.UnableToSyncMetaData,
            ErrorMessage(err));
        });
    }

    /// <summary>
    /// Fire sync started event
    /// </summary>
    void FireSyncStartedEvent()
    {
      if (null == SyncStarted)
        return;

      SyncStarted(this,
        new DropboxSyncEventArgs
        {
          Message = "Now syncing...",
          TotalDownloadingFiles = _totaldownloadings,
          TotalUploadingFiles = _totaluploadings,
          NumDownloadedFiles = 0,
          NumUploadedFiles = 0
        });
    }

    /// <summary>
    /// Download single file
    /// </summary>
    void DownloadSingleFile()
    {
      if (_downloadingfiles.Count == 0)
      {
        Log("Download done");
        /*ThreadUtil.Execute(UploadSingleFile);*/
        UploadSingleFile();
        return;
      }

      var dn = _downloadingfiles[0];
      _downloadingfiles.RemoveAt(0);

      if (dn.Is_Deleted)
      {
        Log("Remote file is already deleted:{0}",
          dn.Name);
        var localpath = _ws.GetFullPath(dn.Name);
        StorageIo.Delete(localpath);
        ThreadUtil.Execute(DownloadSingleFile);
        return;
      }

      _client.GetFileAsync(
        dn.Path,
        (resp) =>
        {
          Log("Success to download file stream:{0}", dn.Path);
          var localpath = _ws.GetFullPath(dn.Name);

          StorageIo.WriteBinaryFile(localpath, resp.RawBytes);
          StorageIo.WriteLastModifiedTime(localpath,
                                          dn.ModifiedDate);

          FireSyncSteppedEvent("Downloading...");

          ThreadUtil.Execute(DownloadSingleFile);
        },
        (err) =>
        {
          Log("Failed to download file:{0},{1}",
              dn.Name,
              ErrorMessage(err));
          FireFinishedEvent(DropboxSyncResult.UnableToDownloadFile,
            ErrorMessage(err));
        });
    }

    /// <summary>
    /// Fire sync stepped event
    /// </summary>
    void FireSyncSteppedEvent(string msg)
    {
      if (SyncStepped == null)
        return;

      SyncStepped(this,
        new DropboxSyncEventArgs
        {
          Message = msg,
          TotalDownloadingFiles = _totaldownloadings,
          TotalUploadingFiles = _totaluploadings,
          NumDownloadedFiles = _totaldownloadings - _downloadingfiles.Count,
          NumUploadedFiles = _totaluploadings - _uploadingfiles.Count
        });
    }

    struct Pair
    {
      public string LocalFilePath { get; set; }
      public MetaData RemoteFileMeta { get; set; }
    }

    internal void DetermineUploadingDownloadingFiles(
                  string[] localfiles,
                  List<MetaData> remotefiles)
    {
      var localonlyfiles =
        (from local in localfiles
         where remotefiles.FirstOrDefault((x) => x.Name == local) == null
         select local).ToList();

      var remoteonlyfiles =
        (from remote in remotefiles
         where
           remote.Is_Dir == false
           && remote.Is_Deleted == false
           && remote.Extension.ToLower() == ".txt"
           && localfiles.FirstOrDefault(x => x == remote.Name) == null
         select remote).ToList();

      var founds =
        (from local in localfiles
         from remote in remotefiles
         where
           remote.Is_Dir == false &&
           local == remote.Name
         select new Pair
         {
           LocalFilePath = local,
           RemoteFileMeta = remote
         }).ToList();

      var moreuploadingfiles =
        (from v in founds
         let fulllocalpath = PathUtil.MakePath(_ws.GetPath(), v.LocalFilePath)
         where
           DiffInSeconds(StorageIo.ReadLastModifiedTime(fulllocalpath), v.RemoteFileMeta.ModifiedDate) > 15
         select v.LocalFilePath);
      localonlyfiles.AddRange(moreuploadingfiles);

      var moredownloadingfiles =
        (from v in founds
         let fulllocalpath = PathUtil.MakePath(_ws.GetPath(), v.LocalFilePath)
         where
           DiffInSeconds(StorageIo.ReadLastModifiedTime(fulllocalpath), v.RemoteFileMeta.ModifiedDate) < -15
         select v.RemoteFileMeta);
      remoteonlyfiles.AddRange(moredownloadingfiles);

      _uploadingfiles = localonlyfiles;
      _downloadingfiles = remoteonlyfiles;
    }

    int DiffInSeconds(DateTime d1, DateTime d2)
    {
      return (int)(d1.Subtract(d2).TotalSeconds);
    }

    /// <summary>
    /// This function leaves log message
    /// </summary>
    /// <param name="fmt">String format</param>
    /// <param name="args">Format parameter</param>
    void Log(string fmt, params string[] args)
    {
#if DEBUG
      var msg = string.Format(fmt, args);
      Debug.WriteLine("{0}{1}", LOGTAG, msg);
#endif
    }
  }

  public enum DropboxSyncResult
  {
    DuplicateFolderFound,
    UnableToCreateFolder,
    UnableToGetFileMetaData,
    UnableToDownloadFile,
    UnableToUploadFile,
    UnableToGetFolderMetaData,
    UnableToSyncMetaData,
    UnableToReadFile,
    UnknownError,
    Success
  }

  /// <summary>
  /// Dropbox sync event arguments
  /// </summary>
  public class DropboxSyncEventArgs : EventArgs
  {
    public DropboxSyncResult Result { get; set; }
    public string ErrorMessage { get; set; }

    public string Message { get; set; }
    public int TotalDownloadingFiles { get; set; }
    public int NumDownloadedFiles { get; set; }
    public int TotalUploadingFiles { get; set; }
    public int NumUploadedFiles { get; set; }
  }
}