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
      const string TEST_NAME = "abcdefg";
      Workspace w = new Workspace();
      w.Open(TEST_NAME);

      Assert.AreEqual("abcdefg", w.Name);
      Assert.AreEqual("/abcdefg", w.DropBoxPath);
      Assert.AreEqual(true, Workspace.Exists(TEST_NAME));
      w.Delete();
      Assert.AreEqual(false, Workspace.Exists(TEST_NAME));
    }

    [TestMethod]
    [Tag("x")]
    public void Create()
    {
      var ws = Workspace.Create("aaa");
      Assert.AreEqual(Workspace.WORKSPACE_BASEPATH_BS + "aaa", ws.GetPath());
      ws.Delete();
      Assert.AreEqual(false, Workspace.Exists("aaa"));
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

    static void Assert_ChangeDropBoxPath(
                          string localpath,
                          string dropboxpath,
                          string newname)
    {
      var w = new Workspace();
      w.Open(PathUtil.NameOnly(localpath));

      w.ChangeDropboxPath(dropboxpath);
      Assert.AreEqual(dropboxpath, w.DropBoxPath);
      Assert.AreEqual(newname, w.Name);

      Assert.AreEqual(false, StorageIo.DirExists(localpath));
      Assert.AreEqual(true, StorageIo.DirExists(w.GetPath()));

      StorageIo.DeleteDir(localpath);
      StorageIo.DeleteDir(w.GetPath());
    }

    /*[TestMethod]
    public void DeleteTextDocument()
    {
      var ws = new Workspace();
      ws.Open("/workspaces/test-doc");

      var doc = ws.NewTextDocument();
      doc.ChangeTitle();
    }*/
  }
}