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
using MemoPad;
using MemoPadCore.Common;
using MemoPadCore.Model;
using Microsoft.Silverlight.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TapfishCore.Resources;

namespace MemoPadTest.Tests
{
  [TestClass]
  public class Test_WorkspaceListPageViewModel
  {
    [TestMethod]
    [Tag("xx")]
    public void DefaultWorkspaceShouldBeCreated()
    {
      StorageIo.DeleteDir(Workspace.WORKSPACE_BASEPATH);

      var vm = new WorkspaceListPageViewModel();
      Assert.AreEqual(1, vm.Workspaces.Count);
      Assert.AreEqual(AppSetting.DEFAULT_WORKSPACE_NAME,
                      vm.Workspaces[0].Name);
    }
  }
}