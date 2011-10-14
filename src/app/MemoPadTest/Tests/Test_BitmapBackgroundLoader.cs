using System;
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
using Microsoft.Silverlight.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TapfishCore.Resources;

namespace MemoPadTest.Tests
{
  [TestClass]
  public class Test_BitmapBackgroundLoader : SilverlightTest
  {
    [TestMethod]
    [Asynchronous]
    [Tag("bmpbkgnd")]
    public void LoadResourceBitmap()
    {
      BitmapBackgroundLoader.LoadResourceBitmapAsync(
        "Images/photo-rename-button.png",
        (result) =>
        {
          Debug.WriteLine("Bitmap loaded");

          Assert.AreEqual(true, result.Succeeded);
          Assert.AreEqual(64, result.Width);
          Assert.AreEqual(64, result.Height);

          EnqueueTestComplete();
        });
    }

    [TestMethod]
    [Asynchronous]
    [Tag("bmpbkgnd")]
    public void LoadNonExistFile()
    {
      BitmapBackgroundLoader.LoadIsoBitmapAsync("no-exist-file.jpg",
        result =>
        {
          Assert.AreEqual(false, result.Succeeded);
          EnqueueTestComplete();
        });
    }
  }
}