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
  public partial class MemoSummaryControlSamplePage : PhoneApplicationPage
  {
    public MemoSummaryControlSamplePage()
    {
      InitializeComponent();

      var ws = new Workspace();
      ws.Open("/workspaces/memoit");
      var vm = ViewModelLocator.MemoListPageVm;
      vm.OpenWorkspace(ws);

      var doc = vm.Docs[1];
      /*doc.ChangeTitle("Hello There");*/

      _a.Open(doc);
      /*_b.Open(vm.Docs[1]);*/
    }
  }
}