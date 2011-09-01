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
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TapfishCore.Resources;

namespace MemoPadTest.Tests
{
  [TestClass]
  public class Test_WorkspaceListPageViewModel
  {
    [TestMethod]
    public void Test_WorkspaceInitializes()
    {
      var vm = new WorkspaceListPageViewModel();
      Assert.AreEqual(1, vm.Workspaces.Count);
    }
  }
}