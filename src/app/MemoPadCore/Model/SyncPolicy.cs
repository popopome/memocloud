using System;
using System.Collections.Generic;
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
using DropNet.Models;
using MemoPadCore.Common;
using TapfishCore.Platform;
using TapfishCore.Resources;

namespace MemoPadCore.Model
{
  #region LocalFileMeta Class Definition

  public class LocalFileMeta
  {
    public string Name { get; set; }
    public string Path { get; set; }
    public DateTime ModifiedUtc { get; set; }
    public bool IsDeleted { get; set; }

    public static LocalFileMeta Create(
                          string path,
                          string date,
                          bool isdeleted)
    {
      return new LocalFileMeta
      {
        Name = WorkspaceFileOp.StripShadowDeleteMark(PathUtil.NameOnly(path)),
        Path = path,
        ModifiedUtc = DateTime.ParseExact(date, "yyyyMMdd HH:mm:ss", null),
        IsDeleted = isdeleted
      };
    }
  }

  #endregion LocalFileMeta Class Definition

  #region DropboxFileMeta Class Definition

  /// <summary>
  /// Short version of MetaData of DropNet library
  /// </summary>
  public class DropboxFileMeta
  {
    public string Path { get; set; }
    public bool IsDir { get; set; }
    public bool IsDeleted { get; set; }
    public string Extension { get; set; }
    public string Name { get; set; }
    public DateTime LastModifiedUtc { get; set; }

    public static DropboxFileMeta Create(MetaData m)
    {
      return new DropboxFileMeta
      {
        IsDir = m.Is_Dir,
        IsDeleted = m.Is_Deleted,
        Extension = m.Extension,
        Name = m.Name,
        LastModifiedUtc = m.UTCDateModified,
        Path = m.Path
      };
    }

    public static DropboxFileMeta Create(
              string path,
              string modifieddate,
              bool isdir,
              bool isdeleted)
    {
      return new DropboxFileMeta
      {
        IsDeleted = isdeleted,
        IsDir = isdir,
        Extension = PathUtil.Extension(path),
        Name = PathUtil.NameOnly(path),
        Path = path,
        LastModifiedUtc = DateTime.ParseExact(modifieddate, "yyyyMMdd HH:mm:ss", null)
      };
    }
  }

  #endregion DropboxFileMeta Class Definition

  #region SyncUpDnList Class Definition

  public class SyncUpDnList
  {
    public List<LocalFileMeta> UploadingList { get; set; }
    public List<DropboxFileMeta> DownloadingList { get; set; }

    public bool IsUploadingEmpty { get { return UploadingList.Count == 0; } }
    public bool IsDownloadingEmpty { get { return DownloadingList.Count == 0; } }

    public int DownloadCount { get { return DownloadingList.Count; } }
    public int UploadCount { get { return UploadingList.Count; } }

    public LocalFileMeta PopFrontUploading()
    {
      if (IsUploadingEmpty)
        return null;

      var v = UploadingList[0];
      UploadingList.RemoveAt(0);
      return v;
    }

    public DropboxFileMeta PopFrontDownloading()
    {
      if (IsDownloadingEmpty)
        return null;

      var v = DownloadingList[0];
      DownloadingList.RemoveAt(0);
      return v;
    }
  }

  #endregion SyncUpDnList Class Definition

  public class SyncPolicy
  {
    struct Pair
    {
      public LocalFileMeta LocalFileMeta { get; set; }
      public DropboxFileMeta RemoteFileMeta { get; set; }
    }

    static void DeleteAllLocalOnlyDeleteFiles(List<LocalFileMeta> localfiles)
    {
      if (localfiles.Count == 0)
        return;

      localfiles.ForEach(local =>
        {
          if (local.IsDeleted)
          {
            StorageIo.Delete(local.Path);
            StorageIo.Delete(WorkspaceFileOp.GetDeleteShadowFilePath(local.Path));
          }
        });
    }

    static List<LocalFileMeta> DropDeleteFilesInList(List<LocalFileMeta> localfiles)
    {
      for (int i = localfiles.Count - 1; i >= 0; --i)
      {
        var local = localfiles[i];
        if (local.IsDeleted)
          localfiles.RemoveAt(i);
      }

      return localfiles;
    }

    public static SyncUpDnList MakeUpDnFileList(
                  List<LocalFileMeta> localfiles,
                  List<DropboxFileMeta> remotefiles)
    {
      var localonlyfiles =
        (from local in localfiles
         where
          remotefiles.FirstOrDefault(x => WorkspaceFileOp.HasLocalRemoteSameName(local.Name, x.Name)) == null
         select local).ToList();

      //
      // Let's remove any redundant files at here.
      // This is not a good position to place this code.
      //
      DeleteAllLocalOnlyDeleteFiles(localonlyfiles);
      localonlyfiles = DropDeleteFilesInList(localonlyfiles);

      var remoteonlyfiles =
        (from remote in remotefiles
         where
           remote.IsDir == false
           && remote.IsDeleted == false
           && WorkspaceFileOp.IsValidMemoContent(remote.Extension)
           && localfiles.FirstOrDefault(x => WorkspaceFileOp.HasLocalRemoteSameName(x.Name, remote.Name)) == null
         select remote).ToList();

      var founds =
        (from local in localfiles
         from remote in remotefiles
         where
           remote.IsDir == false &&
           local.Name == remote.Name
         select new Pair
         {
           LocalFileMeta = local,
           RemoteFileMeta = remote
         }).ToList();

      var moreuploadingfiles =
        (from v in founds
         where
           DiffInSeconds(v.LocalFileMeta.ModifiedUtc, v.RemoteFileMeta.LastModifiedUtc) > 15
         select v.LocalFileMeta);
      localonlyfiles.AddRange(moreuploadingfiles);

      var moredownloadingfiles =
        (from v in founds
         where
           DiffInSeconds(v.LocalFileMeta.ModifiedUtc, v.RemoteFileMeta.LastModifiedUtc) < -15
         select v.RemoteFileMeta);
      remoteonlyfiles.AddRange(moredownloadingfiles);

      return new SyncUpDnList
      {
        UploadingList = localonlyfiles,
        DownloadingList = remoteonlyfiles
      };
    }

    static int DiffInSeconds(DateTime d1, DateTime d2)
    {
      return (int)(d1.Subtract(d2).TotalSeconds);
    }
  }
}