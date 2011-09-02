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
using DropNet;
using MemoPadCore.Common;
using MemoPadCore.Model;
using MemoPadTest.Helper;
using Microsoft.Silverlight.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TapfishCore.Resources;

namespace MemoPadTest.Tests
{
  [TestClass]
  public class Test_DropboxSync : SilverlightTest
  {
    [Ignore]
    [TestMethod]
    [Asynchronous]
    public void OverwritingTest()
    {
      DropboxLoginHelper.Login(
        (token, secret) =>
        {
          Workspace ws = new Workspace();
          ws.Open("/memopad");
          ws.DropBoxToken = token;
          ws.DropBoxSecret = secret;

          StorageIo.WriteTextFile("/memopad/a.txt", "THIS IS MY TEST");
          StorageIo.WriteLastModifiedTime("/memopad/a.txt", DateTime.Now);

          StorageIo.WriteTextFile("/memopad/b.txt", "THIS IS MY TEST");
          StorageIo.WriteLastModifiedTime("/memopad/b.txt", DateTime.Now);

          StorageIo.WriteTextFile("/memopad/c.txt", "THIS IS MY TEST");
          StorageIo.WriteLastModifiedTime("/memopad/c.txt", DateTime.Now);

          DropboxSync sync = new DropboxSync(ws);
          sync.Finished += (x, xe) =>
            {
              base.EnqueueTestComplete();
            };
          sync.Start();

        });
    }

    [TestMethod]
    public void UploadFile()
    {

    }
  }
}