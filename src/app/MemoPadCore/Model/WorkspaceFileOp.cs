using System;
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
using TapfishCore.Resources;

namespace MemoPadCore.Model
{
  public enum WorkspaceFileAccessMode
  {
    Visible,
    All
  };

  public static class WorkspaceFileOp
  {

    /// <summary>
    /// Create new file
    /// </summary>
    /// <param name="path">New file path</param>
    public static void NewFile(string path)
    {
      var shadowpath = GetDeleteShadowFilePath(path);
      StorageIo.Delete(shadowpath);

      StorageIo.WriteTextFile(path, "");
      FileTimeDb.WriteLastModifiedTime(path);
    }

    /// <summary>
    /// Rename
    /// </summary>
    /// <param name="newpath">New path</param>
    /// <param name="oldpath">Old path</param>
    public static void Rename(string oldpath,
                              string newpath)
    {
      if (Exists(oldpath) == false)
        return;

      if (newpath.ToLower() == oldpath.ToLower())
        return;

      Copy(oldpath, newpath);
      DeleteVirtual(oldpath);
    }

    public static void Copy(string srcpath,
                            string dstpath)
    {
      StorageIo.CopyFile(srcpath, dstpath);
      FileTimeDb.WriteLastModifiedTime(dstpath);
    }

    /// <summary>
    /// Delete a file virtually.
    /// This is a critical step to sync well.
    /// </summary>
    /// <param name="path"></param>
    public static void DeleteVirtual(string path)
    {
      var shadowpath = GetDeleteShadowFilePath(path);
      StorageIo.RenameFile(path, shadowpath);
      FileTimeDb.WriteLastModifiedTime(shadowpath);
      FileTimeDb.Delete(path);
    }

    /// <summary>
    /// Get shadow file path
    /// </summary>
    /// <param name="path">original path</param>
    /// <returns>Delete marked path</returns>
    public static string GetDeleteShadowFilePath(string path)
    {
      return string.Concat(path, AppSetting.DELETE_MEMO_EXT);
    }

    /// <summary>
    /// Check existence
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public static bool Exists(string path)
    {
      return StorageIo.Exists(path);
    }

    public static bool IsPhotoMemoFile(string path)
    {
      var lower = path.ToLower();
      return
        lower.EndsWith(AppSetting.JPEG_EXT)
        || lower.EndsWith(AppSetting.PNG_EXT);
    }

    public static bool IsTextMemoFile(string path)
    {
      var lower = path.ToLower();
      return lower.EndsWith(AppSetting.TEXT_MEMO_EXT);
    }

    public static bool IsVisibleMemoFile(string fn)
    {
      return
        IsTextMemoFile(fn)
        || IsPhotoMemoFile(fn);
    }

    public static bool IsMemoFile(string fn)
    {
      var lowerfn = fn.ToLower();
      return IsVisibleMemoFile(lowerfn)
        || IsDeleteShadowFile(fn);
    }

    public static bool IsDeleteShadowFile(string fn)
    {
      var lowerfn = fn.ToLower();
      return lowerfn.EndsWith(AppSetting.DELETE_MEMO_EXT);
    }

    public static string StripShadowDeleteMark(string fn)
    {
      if (false == IsDeleteShadowFile(fn))
        return fn;

      return fn.Substring(0, fn.Length - AppSetting.DELETE_MEMO_EXT.Length);
    }

    public static bool IsValidMemoContent(string fn)
    {
      var tmp = fn.ToLower();
      return tmp == AppSetting.TEXT_MEMO_EXT
        || tmp == AppSetting.JPEG_EXT
        || tmp == AppSetting.PNG_EXT;
    }

    public static bool HasLocalRemoteSameName(
        string local,
        string remote)
    {
      if (WorkspaceFileOp.IsDeleteShadowFile(local))
      {
        var fn = StripShadowDeleteMark(local);
        return fn.ToLower() == remote.ToLower();
      }

      return local.ToLower() == remote.ToLower();
    }
  }
}