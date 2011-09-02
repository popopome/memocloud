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
using Microsoft.Silverlight.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TapfishCore.Resources;

namespace MemoPadTest.Tests
{
  [TestClass]
  public class Test_WorkspaceFileOp : SilverlightTest
  {

    const string ORIGIN_FILE_NAME = "a.txt";
    const string NEW_FILE_NAME = "newa.txt";
    const string ORIGIN_FILE_NAME_DELETED = "a.txt.deleted";

    [TestMethod]
    public void Rename()
    {
      DeleteAllTestFiles();

      WorkspaceFileOp.NewFile(ORIGIN_FILE_NAME);
      WorkspaceFileOp.Rename(NEW_FILE_NAME, ORIGIN_FILE_NAME);

      Assert.AreEqual(false, StorageIo.Exists(ORIGIN_FILE_NAME));
      Assert.AreEqual(true, StorageIo.Exists(ORIGIN_FILE_NAME_DELETED));
      Assert.AreEqual(true, StorageIo.Exists(NEW_FILE_NAME));
    }

    [TestMethod]
    public void DeleteVirtual()
    {
      DeleteAllTestFiles();

      StorageIo.NewFile(ORIGIN_FILE_NAME_DELETED);

      WorkspaceFileOp.NewFile(ORIGIN_FILE_NAME);

      Assert.AreEqual(false, StorageIo.Exists(ORIGIN_FILE_NAME_DELETED));
      Assert.AreEqual(true, StorageIo.Exists(ORIGIN_FILE_NAME));
    }

    public void StripShadowDeleteMark()
    {
      var s = WorkspaceFileOp.StripShadowDeleteMark("a.x.deleted");
      Assert.AreEqual("a.x", s);
    }

    #region Helper Functions

    static void DeleteAllTestFiles()
    {
      StorageIo.Delete(ORIGIN_FILE_NAME);
      StorageIo.Delete(NEW_FILE_NAME);
      StorageIo.Delete(ORIGIN_FILE_NAME_DELETED);
    }

    #endregion Helper Functions

  }
}