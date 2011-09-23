using System;
using System.IO;
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
using TapfishCore.Platform;
using TapfishCore.Resources;

namespace MemoPadCore.Model
{
  public class FileTimeDb
  {
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

    public static void Delete(string path)
    {
      var fullpath = GetLastModifiedFilePath(path);
      if (false == StorageIo.Exists(fullpath))
        return;

      StorageIo.Delete(fullpath);
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
      StorageIo.EnsureDir(LAST_MODIFIED_DB_PATH);
      var newpath = path.Replace("-", "--");
      string p2 = newpath.Replace('\\', '/');
      p2 = path.Replace('/', '-');
      string fullpath = PathUtil.MakePath(LAST_MODIFIED_DB_PATH, p2);
      return fullpath;
    }
  }
}