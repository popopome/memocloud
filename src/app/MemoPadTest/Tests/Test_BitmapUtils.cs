using System;
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
using TapfishCore.Math;
using TapfishCore.Resources;

namespace MemoPadTest.Tests
{
  [TestClass]
  public class Test_BitmapUtils
  {
    [TestMethod]
    [Tag("a")]
    public void ResizeBitmap()
    {
      var wbmp = new WriteableBitmap(320, 240);
      var output = BitmapUtils.ResizeBitmap(wbmp, 32, 32);
      Assert.AreEqual(32, output.PixelWidth);
      Assert.AreEqual(24, output.PixelHeight);
    }
  }
}