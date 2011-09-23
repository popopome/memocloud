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
        using (var reader = new StreamReader(stm))
        {
          var buf = new char[len];
          int nread = reader.Read(buf, 0, buf.Length);
          return new string(buf);
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
      CreateDir(newname);

      // Copy all files to new folder
      var srcfiles = Files(oldname, "*");
      foreach (var src in srcfiles)
      {
        var dstfn = PathUtil.MakePath(newname, src);
        var srcfn = PathUtil.MakePath(oldname, src);

        CopyFile(dstfn, srcfn);
      }

      DeleteDir(oldname);
    }

    /// <summary>
    /// Copy two file
    /// </summary>
    /// <param name="dstfn">Destination function</param>
    /// <param name="srcfn">Source function</param>
    public static void CopyFile(string dstfn, string srcfn)
    {
      var stg = IsolatedStorageFile.GetUserStoreForApplication();

      using (var dststm = stg.CreateFile(dstfn))
      {
        using (var srcstm = stg.OpenFile(srcfn, FileMode.Open))
        {
          const int BUFFER_SIZE = 4096;
          byte[] buf = new byte[BUFFER_SIZE];

          long remaining = srcstm.Length;
          while (remaining > 0)
          {
            int readbytes = System.Math.Min((int)remaining, buf.Length);
            int nread = srcstm.Read(buf, 0, readbytes);
            dststm.Write(buf, 0, nread);

            remaining -= readbytes;
          }

          srcstm.Close();
        }

        dststm.Close();
      }
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

    const string LAST_MODIFIED_DB_PATH = "/lastmodified";

    public static void WriteLastModifiedTime(string path)
    {
      WriteLastModifiedTime(path, DateTime.Now.ToUniversalTime());
    }

    public static void WriteLastModifiedTime(string path,
        DateTime dt)
    {
      string fullpath = GetLastModifiedFilePath(path);
      long value = dt.Ticks;
      var stg = IsolatedStorageFile.GetUserStoreForApplication();

      Stream stm = null;
      if (StorageIo.Exists(fullpath))
        stm = stg.OpenFile(fullpath, FileMode.Open);
      else
        stm = stg.CreateFile(fullpath);
      using (stm)
      {
        using (var writer = new StreamWriter(stm))
        {
          writer.Write(value);
          writer.Close();
        }
      }
    }

    public static DateTime ReadLastModifiedTime(string path)
    {
      var fullpath = GetLastModifiedFilePath(path);
      if (false == StorageIo.Exists(fullpath))
        return DateTime.Now;

      var stg = IsolatedStorageFile.GetUserStoreForApplication();
      using (var stm = stg.OpenFile(fullpath, FileMode.Open))
      {
        using (var reader = new StreamReader(stm))
        {
          var val = reader.ReadToEnd();
          long tick = long.Parse(val);
          return new DateTime(tick);
        }
      }
    }

    static string GetLastModifiedFilePath(string path)
    {
      EnsureDir(LAST_MODIFIED_DB_PATH);
      var newpath = path.Replace("-", "--");
      string p2 = newpath.Replace('\\', '/');
      p2 = path.Replace('/', '-');
      string fullpath = PathUtil.MakePath(LAST_MODIFIED_DB_PATH, p2);
      return fullpath;
    }

    public static string[] Dirs(string path)
    {
      var stg = IsolatedStorageFile.GetUserStoreForApplication();

      var pattern = PathUtil.MakePath(path, "*");
      return stg.GetDirectoryNames(pattern);
    }
  }
}