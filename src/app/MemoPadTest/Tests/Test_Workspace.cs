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
using TapfishCore.Platform;
using TapfishCore.Resources;

namespace MemoPadTest.Tests
{
  [TestClass]
  public class Test_Workspace
  {
    [TestMethod]
    public void Workspace_InitialCreation()
    {
      const string TEST_PATH = "//workspaces//abcdefg";
      Workspace w = new Workspace();
      w.Open(TEST_PATH);

      Assert.AreEqual("abcdefg", w.Name);
      Assert.AreEqual("/abcdefg", w.DropBoxPath);
      Assert.AreEqual(true, StorageIo.DirExists(TEST_PATH));

      StorageIo.DeleteDir(TEST_PATH);
      Assert.AreEqual(false, StorageIo.DirExists(TEST_PATH));

      const string TEST_PATH2 = "//workspaces\\abcdefg";

      var w2 = new Workspace();
      w2.Open(TEST_PATH2);

      Assert.AreEqual("abcdefg", w2.Name);
      Assert.AreEqual("/abcdefg", w2.DropBoxPath);
      Assert.AreEqual(true, StorageIo.DirExists(TEST_PATH2));

      StorageIo.DeleteDir(TEST_PATH2);
      Assert.AreEqual(false, StorageIo.DirExists(TEST_PATH2));
    }

    [TestMethod]
    public void Workspace_DropBoxSyncSetup()
    {
      const string TEST_PATH = "/workspaces/a";
      const string NEW_PATH = "/workspaces/writings";
      Assert_ChangeDropBoxPath(TEST_PATH,
                               "/writings",
                               "writings");
    }

    [TestMethod]
    public void Workspace_DropBoxSync_WithDeepFolderPath()
    {
      const string TEST_PATH = "/workspaces/a";
      Assert_ChangeDropBoxPath(TEST_PATH,
                               "/a/b/c/d/e/f/g",
                               "g");
    }

    private static void Assert_ChangeDropBoxPath(
                          string localpath,
                          string dropboxpath,
                          string newname)
    {
      var w = new Workspace();
      w.Open(localpath);

      w.ChangeDropboxPath(dropboxpath);
      Assert.AreEqual(dropboxpath, w.DropBoxPath);
      Assert.AreEqual(newname, w.Name);

      Assert.AreEqual(false, StorageIo.DirExists(localpath));
      Assert.AreEqual(true, StorageIo.DirExists(w.GetPath()));

      StorageIo.DeleteDir(localpath);
      StorageIo.DeleteDir(w.GetPath());
    }
  }
}