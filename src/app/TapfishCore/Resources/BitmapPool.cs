using System;
using System.Collections.Generic;
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
  public class BitmapPool
  {
    static Dictionary<string, BitmapImage> Pool { get; set; }

    static BitmapPool()
    {
      Pool = new Dictionary<string, BitmapImage>();
    }

    /// <summary>
    /// Bitmap from resource
    /// </summary>
    /// <param name="assemblyName"></param>
    /// <param name="path"></param>
    /// <returns></returns>
    public static BitmapImage BitmapFromResource(
                    string assemblyName,
                    string path)
    {
      return BitmapFromResource(
                  assemblyName,
                  path,
                  BitmapCreateOptions.DelayCreation);
    }

    /// <summary>
    /// Get bitmap from resource
    /// </summary>
    /// <param name="assemblyName"></param>
    /// <param name="path"></param>
    /// <returns></returns>
    public static BitmapImage BitmapFromResource(
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