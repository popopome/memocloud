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
using System.Windows.Shapes;
using MemoPadCore.Model;
using Microsoft.Silverlight.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MemoPadTest.Tests
{
  [TestClass]
  /*[Tag("a")]*/
  public class Test_SyncPolicy : SilverlightTest
  {

    [TestMethod]
    public void UpDownList()
    {
      var locallist = new List<LocalFileMeta>
      {
        LocalFileMeta.Create("/a.txt", "20110801 01:01:01", false),
        LocalFileMeta.Create("/b.txt", "20110801 01:01:01", false),
        LocalFileMeta.Create("/c.txt", "20110801 01:01:01", false)
      };

      var remotelist = new List<DropboxFileMeta>
      {
        DropboxFileMeta.Create("/a.txt", "20110801 01:01:01", false, false),
        DropboxFileMeta.Create("/b.txt", "20110901 01:01:01", false, false),
        DropboxFileMeta.Create("/c.txt", "20110701 01:01:01", false, false),
      };

      var r = SyncPolicy.MakeUpDnFileList(locallist, remotelist);
      Assert.AreEqual(1, r.UploadingList.Count);
      Assert.AreEqual("/c.txt", r.UploadingList[0].Path);

      Assert.AreEqual(1, r.DownloadingList.Count);
      Assert.AreEqual("/b.txt", r.DownloadingList[0].Path);
    }

    [TestMethod]
    /*[Tag("a")]*/
    public void UpDownList_DeleteByLocal()
    {
      var locallist = new List<LocalFileMeta>
      {
        LocalFileMeta.Create("/a.txt", "20110801 01:01:01", false),
        LocalFileMeta.Create("/b.txt.deleted", "20110801 01:01:01", false),
        LocalFileMeta.Create("/c.txt.deleted", "20110801 01:01:01", true)
      };

      var remotelist = new List<DropboxFileMeta>
      {
        DropboxFileMeta.Create("/a.txt", "20110801 01:01:01", false, false),
        DropboxFileMeta.Create("/b.txt", "20110901 01:01:01", false, false),
        DropboxFileMeta.Create("/c.txt", "20110701 01:01:01", false, false),
      };

      var r = SyncPolicy.MakeUpDnFileList(locallist, remotelist);
      Assert.AreEqual(1, r.UploadingList.Count);
      Assert.AreEqual("/c.txt.deleted", r.UploadingList[0].Path);

      Assert.AreEqual(1, r.DownloadingList.Count);
      Assert.AreEqual("/b.txt", r.DownloadingList[0].Path);

    }
  }
}