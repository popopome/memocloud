using System;
using System.Collections.Generic;
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

namespace TapfishCore.Platform
{
  public class PathUtil
  {
    public static string AddBackslash(string path)
    {
      char ch = path.Last();
      if (ch == '/'
          || ch == '\\')
        return path;

      return string.Concat(path, "\\");
    }

    public static string RemoveBackslash(string path)
    {
      char ch = path.Last();
      if (ch == '/'
          || ch == '\\')
        return path.Substring(0, path.Length - 1);

      return path;
    }

    /// <summary>
    /// Make path
    /// </summary>
    /// <param name="pathlist">Path list</param>
    /// <returns>Concatened path</returns>
    public static string MakePath(params string[] pathlist)
    {
      var builder = new StringBuilder();
      foreach (var s in pathlist)
      {
        var ss =
          (s == pathlist[0])
          ? s
          : PathUtil.TrimFirstBackslash(s);

        var bs = PathUtil.AddBackslash(ss);
        builder.Append(bs);
      }

      var path = builder.ToString();
      string newpath = path.Replace('\\', '/');
      newpath = RemoveBackslash(newpath);
      return InsertFirstBackslash(newpath);
    }

    /// <summary>
    /// Find name part only
    /// </summary>
    /// <param name="fn"></param>
    /// <returns></returns>
    public static string NameOnlyNoExt(string input)
    {
      var fn = NameOnly(input);
      int index = fn.LastIndexOf('.');
      if (-1 == index)
        return fn;

      return fn.Substring(0, index);
    }

    public static string NameOnly(string fn)
    {
      int index = fn.LastIndexOfAny(new char[] { '/', '\\' });
      if (-1 == index)
        return fn;

      return fn.Substring(index + 1);
    }

    /// <summary>
    /// This function generates path-only object.
    /// </summary>
    /// <param name="fn">File name</param>
    /// <returns></returns>
    public static string PathOnly(string fn)
    {
      int index = fn.LastIndexOfAny(new char[] { '/', '\\' });
      if (-1 == index)
        return fn;

      return AddBackslash(fn.Substring(0, index + 1));
    }

    /// <summary>
    /// Split path into several parts
    /// </summary>
    /// <param name="path">Path to split</param>
    /// <returns>List of each part of path</returns>
    public static string[] Split(string path)
    {
      var list = path.Split(new char[] { '/', '\\' });

      var fulllist =
        from n in list
        where string.IsNullOrEmpty(n) == false
        select n;

      return fulllist.ToArray();
    }

    public static string TrimFirstBackslash(string s)
    {
      if (string.IsNullOrEmpty(s))
        return s;

      if (s[0] == '\\'
        || s[0] == '/')
        return s.Remove(0, 1);

      return s;
    }

    public static string InsertFirstBackslash(string s)
    {
      if (s.Length > 0)
      {
        if (s[0] != '/')
          return s.Insert(0, "/");
      }

      return s;
    }

    public static string Extension(string s)
    {
      var n = NameOnly(s);
      int index = n.LastIndexOf('.');
      if (-1 == index)
        return "";

      return n.Substring(index);
    }
  }
}