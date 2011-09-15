using System;
using System.Collections.Generic;
using System.Diagnostics;
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

namespace TapfishCore.Resources
{
  /// <summary>
  /// Bitmap pool
  /// </summary>
  public class BitmapPool
  {
    static Dictionary<string, BitmapSource> Pool { get; set; }

    static BitmapPool()
    {
      Pool = new Dictionary<string, BitmapSource>();
    }

    /// <summary>
    /// Bitmap from resource
    /// </summary>
    /// <param name="assemblyName"></param>
    /// <param name="path"></param>
    /// <returns></returns>
    public static BitmapSource BitmapFromResource(
                    string assemblyName,
                    string path)
    {
      return BitmapFromResource(
                  assemblyName,
                  path,
                  BitmapCreateOptions.DelayCreation);
    }

    /// <summary>
    /// Add bitmap
    /// </summary>
    /// <param name="id">Bitmap id</param>
    /// <param name="path">Bitmap path</param>
    public static void AddBitmap(string id, string path)
    {
      var bmp = BitmapUtils.CreateBitmapImmediately(path);
      Debug.Assert(bmp != null);
      AddBitmap(id, bmp);
    }

    /// <summary>
    /// Add bitmap
    /// </summary>
    /// <param name="id">Bitmap id</param>
    /// <param name="bmp">Bitmap object</param>
    public static void AddBitmap(string id, BitmapSource bmp)
    {
      Debug.Assert(bmp != null);
      if (Pool.ContainsKey(id))
        Pool[id] = bmp;
      else
        Pool.Add(id, bmp);
    }

    /// <summary>
    /// Access bitmap image
    /// </summary>
    /// <param name="id">Bitmap id</param>
    /// <returns>Bitmap object</returns>
    public static BitmapSource Bmp(string id)
    {
      if (Pool.ContainsKey(id))
        return Pool[id];

      return null;
    }

    /// <summary>
    /// Get bitmap from resource
    /// </summary>
    /// <param name="assemblyName"></param>
    /// <param name="path"></param>
    /// <returns></returns>
    public static BitmapSource BitmapFromResource(
            string assemblyName,
            string path,
            BitmapCreateOptions option)
    {
      string pathToResource =
                string.Format("/{0};component{1}",
                              assemblyName,
                              path);
      if (Pool.ContainsKey(pathToResource))
        return Pool[pathToResource];

      BitmapImage img =
                new BitmapImage(new Uri(pathToResource, UriKind.Relative));
      img.CreateOptions = option;
      return img;
    }
  }
}