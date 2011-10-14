using System;
using System.Diagnostics;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Net;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using TapfishCore.Platform;

namespace TapfishCore.Resources
{
  public class StorageIo
  {
    const string LOGTAG = "[StorageIo]:";

    public static void NewFile(string path)
    {
      var stg = IsolatedStorageFile.GetUserStoreForApplication();
      if (stg.FileExists(path))
        stg.DeleteFile(path);

      var stm = stg.CreateFile(path);
      stm.Close();
    }

    public static bool Exists(string path)
    {
      var stg = IsolatedStorageFile.GetUserStoreForApplication();
      return (stg.FileExists(path));
    }

    public static void Delete(string path)
    {
      var stg = IsolatedStorageFile.GetUserStoreForApplication();
      if (stg.FileExists(path))
        stg.DeleteFile(path);
    }

    /// <summary>
    /// Read text file.
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public static string ReadTextFile(string path)
    {
      var stg = IsolatedStorageFile.GetUserStoreForApplication();
      if (stg.FileExists(path) == false)
        return null;

      using (var stm = stg.OpenFile(path, FileMode.Open))
      {
        using (var reader = new StreamReader(stm))
        {
          return reader.ReadToEnd();
        }
      }
    }

    public static string ReadTextFile(string path, int len)
    {
      var stg = IsolatedStorageFile.GetUserStoreForApplication();
      if (stg.FileExists(path) == false)
        return null;

      using (var stm = stg.OpenFile(path, FileMode.Open))
      {
        if (stm.Length == 0)
        {
          Debug.WriteLine("{0}{1}", LOGTAG, "File is empty");
          return "";
        }

        using (var reader = new StreamReader(stm))
        {
          var text = reader.ReadToEnd();
          if (text.Length < len)
            return text;

          return text.Substring(0, len);
        }
      }
    }

    /// <summary>
    /// Write text file
    /// </summary>
    /// <param name="path"></param>
    /// <param name="text"></param>
    public static void WriteTextFile(string path, string text)
    {
      var stg = IsolatedStorageFile.GetUserStoreForApplication();
      if (stg.FileExists(path))
        stg.DeleteFile(path);

      using (var stm = stg.CreateFile(path))
      {
        using (var writer = new StreamWriter(stm))
        {
          writer.Write(text);
        }
      }
    }

    /// <summary>
    /// Read binary from resource
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public static byte[] ReadBinaryFromResource(string path)
    {
      var stminfo = Application.GetResourceStream(new Uri(path, UriKind.Relative));
      if (null == stminfo)
        return null;

      var stm = stminfo.Stream;
      if (null == stm)
        return null;

      byte[] buffer = new byte[stm.Length];
      stm.Read(buffer, 0, buffer.Length);
      return buffer;

    }

    /// <summary>
    /// Read binary file
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public static byte[] ReadBinaryFile(string path)
    {
      var stg = IsolatedStorageFile.GetUserStoreForApplication();
      if (stg.FileExists(path) == false)
        return null;

      using (var stm = stg.OpenFile(path, FileMode.Open))
      {
        byte[] buffer = new byte[stm.Length];
        stm.Read(buffer, 0, buffer.Length);

        return buffer;
      }
    }

    /// <summary>
    /// Write binary file
    /// </summary>
    /// <param name="path"></param>
    /// <param name="data"></param>
    public static void WriteBinaryFile(string path, byte[] data)
    {
      var stg = IsolatedStorageFile.GetUserStoreForApplication();
      if (stg.FileExists(path))
        stg.DeleteFile(path);

      using (var stm = stg.CreateFile(path))
      {
        stm.Write(data, 0, data.Length);
      }
    }

    public static void WriteBinaryFile(string path, Stream stm)
    {
      try
      {
        byte[] buf = new byte[stm.Length];
        stm.Seek(0, SeekOrigin.Begin);
        stm.Read(buf, 0, buf.Length);

        WriteBinaryFile(path, buf);
      }
      catch (System.Exception e)
      {
        Debug.WriteLine("******************EXCEPTION: To write binary file:" + e);
      }

    }

    public static bool DirExists(string dir)
    {
      var stg = IsolatedStorageFile.GetUserStoreForApplication();
      return stg.DirectoryExists(dir);
    }

    public static void CreateDir(string dirpath)
    {
      var stg = IsolatedStorageFile.GetUserStoreForApplication();
      var dirpath_nobs = PathUtil.RemoveBackslash(dirpath);
      stg.CreateDirectory(dirpath_nobs);
    }

    /// <summary>
    /// This function checks whether given direction exists or not.
    /// If it does not exist,
    /// create it.
    /// </summary>
    /// <param name="dirpath">Directory path</param>
    public static void EnsureDir(string dirpath)
    {
      if (DirExists(dirpath) == false)
        CreateDir(dirpath);
    }

    /// <summary>
    /// List up files
    /// </summary>
    /// <param name="dirpath">Directory path</param>
    /// <param name="pattern">Pattern to find</param>
    /// <returns>List of file names</returns>
    public static string[] Files(string dirpath, string pattern)
    {
      var stg = IsolatedStorageFile.GetUserStoreForApplication();
      var searchpattern = PathUtil.MakePath(dirpath, pattern);
      return stg.GetFileNames(
                  PathUtil.RemoveBackslash(searchpattern));
    }

    /// <summary>
    /// Rename directory
    /// </summary>
    /// <param name="oldname">oldname</param>
    /// <param name="newname">newname</param>
    public static void RenameDir(string oldname,
                                 string newname)
    {
      var stg = IsolatedStorageFile.GetUserStoreForApplication();
      stg.MoveDirectory(oldname, newname);
    }

    /// <summary>
    /// Copy two file
    /// </summary>
    /// <param name="dstfn">Destination function</param>
    /// <param name="srcfn">Source function</param>
    public static void CopyFile(string srcfn, string dstfn)
    {
      var stg = IsolatedStorageFile.GetUserStoreForApplication();
      stg.CopyFile(srcfn, dstfn);

    }

    public static void RenameFile(string srcfn, string dstfn)
    {
      var stg = IsolatedStorageFile.GetUserStoreForApplication();
      if (stg.FileExists(dstfn))
        stg.DeleteFile(dstfn);

      stg.MoveFile(srcfn, dstfn);
    }

    /// <summary>
    /// Delete directory
    /// </summary>
    /// <param name="path"></param>
    public static void DeleteDir(string path)
    {
      if (DirExists(path) == false)
        return;

      var stg = IsolatedStorageFile.GetUserStoreForApplication();

      var pathbs = PathUtil.AddBackslash(path);
      var searchpattern = PathUtil.MakePath(path, "*");
      var files = stg.GetFileNames(searchpattern);
      foreach (var fn in files)
      {
        var fullpath = pathbs + fn;
        try
        {
          stg.DeleteFile(fullpath);
        }
        catch (System.Exception e)
        {
          Debug.WriteLine("Failed to delete file:{0}", fullpath);
        }
      }

      var dirs = stg.GetDirectoryNames(searchpattern);
      foreach (var dir in dirs)
      {
        var fullpath = pathbs + dir;
        DeleteDir(fullpath);
      }

      var pathnobs = PathUtil.RemoveBackslash(path);
      try
      {
        stg.DeleteDirectory(pathnobs);
      }
      catch (System.Exception e)
      {
        Debug.WriteLine("Failed to delete directory:{0}", path);
      }
    }

    public static string[] Dirs(string path)
    {
      var stg = IsolatedStorageFile.GetUserStoreForApplication();

      var pattern = PathUtil.MakePath(path, "*");
      return stg.GetDirectoryNames(pattern);
    }
  }
}