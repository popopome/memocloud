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
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TapfishCore.Platform;

namespace MemoPadTest.Tests
{
  [TestClass]
  public class Test_PathUtil
  {
    [TestMethod]
    public void PathOnlyIsValid()
    {
      var s = PathUtil.PathOnly("/");
      Assert.AreEqual("/", s);

      var s2 = PathUtil.PathOnly("/abcd/bbb");
      Assert.AreEqual("/abcd/", s2);
    }

    [TestMethod]
    public void Extension()
    {
      var s = PathUtil.Extension("a.txt");
      Assert.AreEqual(".txt", s);
    }
  }
}