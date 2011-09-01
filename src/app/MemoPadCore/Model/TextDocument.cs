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
using TapfishCore.Platform;
using TapfishCore.Resources;

namespace MemoPadCore.Model
{
  /// <summary>
  /// This class represents TEXT document object.
  /// </summary>
  public class TextDocument
  {
    public string Title { get; private set; }
    public string Text { get; set; }
    public string WorkspaceName { get; set; }
    public string FullPath { get; private set; }
    public string Summary { get; set; }
    public bool IsRevised { get; set; }

    /// <summary>
    /// CTOR
    /// </summary>
    /// <param name="fullpath">Full path of text document</param>
    public TextDocument(string fullpath)
    {
      FullPath = PathUtil.InsertFirstBackslash(fullpath);
      Title = PathUtil.NameOnlyNoExt(fullpath);
    }

    /// <summary>
    /// Open text document.
    /// </summary>
    public void Open()
    {
      Text = StorageIo.ReadTextFile(FullPath) ?? "";
    }

    /// <summary>
    /// Load summary only.
    /// </summary>
    public void LoadSummary()
    {
      const int READ_LEN = 300;
      Summary = StorageIo.ReadTextFile(FullPath, READ_LEN);
    }

    /// <summary>
    /// Save
    /// </summary>
    public void Save()
    {
      var dir = PathUtil.ExtractDirectory(FullPath);
      if (StorageIo.DirExists(dir) == false)
        StorageIo.CreateDir(dir);

      StorageIo.WriteTextFile(FullPath, Text);
      StorageIo.WriteLastModifiedTime(FullPath);
    }

    /// <summary>
    /// Delete document
    /// </summary>
    public void Delete()
    {
      StorageIo.Delete(FullPath);
    }

    /// <summary>
    /// Change title
    /// </summary>
    /// <param name="newtitle"></param>
    public void ChangeTitle(string newtitle)
    {
      Title = newtitle;

      var basepath = PathUtil.PathOnly(FullPath);
      FullPath =
        string.Concat(PathUtil.MakePath(basepath, Title),
                      AppSetting.TEXT_DOCUMENT_EXT);
    }
  }
}