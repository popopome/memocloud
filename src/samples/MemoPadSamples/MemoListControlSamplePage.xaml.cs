using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using MemoPad;
using MemoPadCore.Model;
using Microsoft.Phone.Controls;

namespace MemoPadSamples
{
  public partial class MemoListControlSamplePage : PhoneApplicationPage
  {
    public MemoListControlSamplePage()
    {
      InitializeComponent();

      var vm = ViewModelLocator.MemoListPageVm;
      var ws = new Workspace();
      ws.Open("/workspaces/memoit");
      vm.OpenWorkspace(ws);

      _memolist.Build(vm.MemoList);
    }
  }
}