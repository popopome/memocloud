using System;
using System.IO;
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
using TapfishCore.Platform;
using TapfishCore.Resources;

namespace MemoPadCore.Model
{
  /// <summary>
  /// This class represents TEXT document object.
  /// </summary>
  public class Memo
  {
    const string THUMB_EXTENSION = ".thumb";
    const int MAX_THUMB_WIDTH = 120;
    const int MAX_THUMB_HEIGHT = 120;

    public string Title { get; private set; }
    public string Text { get; set; }
    public string WorkspaceName { get; set; }
    public string FullPath { get; private set; }
    public string Summary { get; set; }
    public bool IsRevised { get; set; }
    public BitmapSource Thumb { get; private set; }
    public BitmapSource FullBitmap { get; private set; }
    public MemoKind Kind { get; private set; }
    public bool IsTextMemo
    {
      get
      {
        return Kind == MemoKind.Text;
      }
    }

    public bool IsPhotoMemo
    {
      get
      {
        return Kind == MemoKind.Photo;
      }
    }

    public string ThumbPath
    {
      get
      {
        return ThumbPathFromFullPath(FullPath);
      }
    }

    /// <summary>
    /// CTOR
    /// </summary>
    /// <param name="fullpath">Full path of text document</param>
    public Memo(string fullpath,
                MemoKind kind)
    {
      FullPath = PathUtil.InsertFirstBackslash(fullpath);
      Title = PathUtil.NameOnlyNoExt(fullpath);
      Text = "";
      Kind = kind;

      Thumb = null;
      FullBitmap = null;
    }

    /// <summary>
    /// Create new text document
    /// </summary>
    public void NewTextMemo()
    {
      WorkspaceFileOp.NewFile(FullPath);
      Kind = MemoKind.Text;
    }

    /// <summary>
    /// New photo memo
    /// </summary>
    /// <param name="img"></param>
    public void NewPhotoMemo(BitmapSource img)
    {
      FullBitmap = img;
      BitmapUtils.SaveBitmapToIso(this.FullPath, FullBitmap);
      Thumb = CreateThumbFile(FullPath, img);
      Kind = MemoKind.Photo;
    }

    /// <summary>
    /// Open text document.
    /// </summary>
    public void Open()
    {
      if (IsTextMemo)
        Text = StorageIo.ReadTextFile(FullPath) ?? "";
      else if (IsPhotoMemo)
      {

      }
    }

    /// <summary>
    /// Load summary only.
    /// </summary>
    public void OpenSummary()
    {
      const int READ_LEN = 300;
      Summary = StorageIo.ReadTextFile(FullPath, READ_LEN);
    }

    /// <summary>
    /// Open thumbnail image
    /// </summary>
    public void OpenThumb()
    {
      var thumbpath = ThumbPathFromFullPath(FullPath);
      Thumb = BitmapUtils.LoadBitmapFromIso(thumbpath);
      if (null == Thumb)
      {
        FullBitmap = BitmapUtils.LoadBitmapFromIso(FullPath, BitmapCreateOptions.None);
        Thumb = CreateThumbFile(FullPath, FullBitmap);
      }
    }

    /// <summary>
    /// Save
    /// </summary>
    public void Save()
    {
      var dir = PathUtil.PathOnly(FullPath);
      if (StorageIo.DirExists(dir) == false)
        StorageIo.CreateDir(dir);

      if (IsTextMemo)
        StorageIo.WriteTextFile(FullPath, Text);
      else if (IsPhotoMemo)
      {
        BitmapUtils.SaveBitmapToIso(ThumbPathFromFullPath(FullPath),
                                    Thumb);
        BitmapUtils.SaveBitmapToIso(FullPath, FullBitmap);
      }

      FileTimeDb.WriteLastModifiedTime(FullPath);
    }

    /// <summary>
    /// Delete document
    /// </summary>
    public void Delete()
    {
      WorkspaceFileOp.DeleteVirtual(FullPath);
    }

    /// <summary>
    /// Change title
    /// </summary>
    /// <param name="newtitle"></param>
    public void ChangeTitle(string newtitle)
    {
      if (newtitle == Title)
        return;

      Title = newtitle;

      var oldpath = FullPath;

      var basepath = PathUtil.PathOnly(FullPath);
      var newpath =
          string.Concat(PathUtil.MakePath(basepath, Title),
                        AppSetting.TEXT_MEMO_EXT);

      WorkspaceFileOp.Rename(oldpath, newpath);
      FullPath = newpath;
    }

    #region Utility functions

    /// <summary>
    /// Get thumbnail path from full-path
    /// </summary>
    /// <param name="fullpath">Full path to specific bitmap image</param>
    /// <returns></returns>
    public static string ThumbPathFromFullPath(string fullpath)
    {
      return fullpath + THUMB_EXTENSION;
    }

    public static void CreateThumbBitmapFile(string fullpath)
    {
      var bmp = BitmapUtils.LoadBitmapFromIso(
                    fullpath,
                    BitmapCreateOptions.None);
      CreateThumbFile(fullpath, bmp);
    }

    static BitmapSource CreateThumbFile(string fullpath,
                                 BitmapSource bmp)
    {
      var thumb =
        BitmapUtils.ResizeBitmap(
            bmp,
            MAX_THUMB_WIDTH,
            MAX_THUMB_HEIGHT);
      BitmapUtils.SaveBitmapToIso(ThumbPathFromFullPath(fullpath), thumb);
      return thumb;
    }

    #endregion Utility functions
  }
}