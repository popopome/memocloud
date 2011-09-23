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
      Delete(shadowpath);

      StorageIo.WriteTextFile(path, "");
      StorageIo.WriteLastModifiedTime(path);
    }

    /// <summary>
    /// Rename
    /// </summary>
    /// <param name="newpath">New path</param>
    /// <param name="oldpath">Old path</param>
    public static void Rename(string newpath,
                              string oldpath)
    {
      if (Exists(oldpath) == false)
        return;

      if (newpath.ToLower() == oldpath.ToLower())
        return;

      Copy(newpath, oldpath);
      DeleteVirtual(oldpath);
    }

    public static void Copy(string dstpath,
                            string srcpath)
    {
      StorageIo.CopyFile(dstpath, srcpath);
      StorageIo.WriteLastModifiedTime(dstpath);
    }

    public static void DeleteVirtual(string path)
    {
      Delete(path);
      NewFile(GetDeleteShadowFilePath(path));
    }

    public static string GetDeleteShadowFilePath(string path)
    {
      return string.Concat(path, AppSetting.DELETE_MEMO_EXT);
    }

    public static void Delete(string path)
    {
      StorageIo.Delete(path);
    }

    public static bool Exists(string path)
    {
      return StorageIo.Exists(path);
    }

    public static bool IsVisibleMemoFile(string fn)
    {
      var lowerfn = fn.ToLower();
      return
        lowerfn.EndsWith(AppSetting.TEXT_DOCUMENT_EXT)
        || lowerfn.EndsWith(AppSetting.PHOTO_DOCUMENT_EXT);
    }

    public static bool IsMemoFile(string fn)
    {
      var lowerfn = fn.ToLower();
      return lowerfn.EndsWith(AppSetting.TEXT_DOCUMENT_EXT)
        || lowerfn.EndsWith(AppSetting.DELETE_MEMO_EXT)
        || lowerfn.EndsWith(AppSetting.PHOTO_DOCUMENT_EXT);
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
  }
}