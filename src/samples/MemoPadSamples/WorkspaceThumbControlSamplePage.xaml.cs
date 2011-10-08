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
using MemoPadCore.Model;
using Microsoft.Phone.Controls;

namespace MemoPadSamples
{
  public partial class WorkspaceThumbControlSamplePage : PhoneApplicationPage
  {
    public WorkspaceThumbControlSamplePage()
    {
      InitializeComponent();

      var ws = new Workspace();

    }
  }
}