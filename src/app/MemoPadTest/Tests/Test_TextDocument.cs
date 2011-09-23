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
using MemoPadCore.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TapfishCore.Resources;

namespace MemoPadTest.Tests
{
  [TestClass]
  public class Test_TextDocument
  {
    [TestMethod]
    public void ChangeTitle_And_CheckIt()
    {
      var doc = new Memo("a.txt", MemoKind.Text);
      doc.ChangeTitle("newtitle");
      doc.Save();

      Assert.AreEqual(true, StorageIo.Exists("newtitle.txt"));
      Assert.AreEqual(false, StorageIo.Exists("a.txt"));
    }
  }
}