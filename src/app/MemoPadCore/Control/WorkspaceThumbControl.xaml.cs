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

namespace MemoPadCore.Control
{
  public partial class WorkspaceThumbControl : UserControl
  {
    public WorkspaceThumbControl()
    {
      InitializeComponent();
    }

    /// <summary>
    /// Open workspace
    /// </summary>
    /// <param name="ws">Workspace object</param>
    public void OpenWorkspace(Workspace ws)
    {
      if (ws.Thumb != null)
      {
        _img.Source = ws.Thumb;
      }
      else
      {
        ws.LoadThumb(() =>
          {
            _img.Source = ws.Thumb;
          });
      }

      _name.Text = ws.Name;
      _lastupdated.Text = string.Concat("Last updated:",
                                   ws.LastUpdated.ToString());
    }
  }
}