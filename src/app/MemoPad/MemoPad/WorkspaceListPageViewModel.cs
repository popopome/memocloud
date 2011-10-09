using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using MemoPadCore.Common;
using MemoPadCore.Model;
using TapfishCore.Platform;
using TapfishCore.Resources;

namespace MemoPad
{
  public class WorkspaceListPageViewModel
  {
    public List<Workspace> Workspaces { get; private set; }

    /// <summary>
    /// There is no workspaces
    /// </summary>
    public bool IsEmptyWorkspace
    {
      get
      {
        return Workspaces.Count == 0;
      }
    }

    /// <summary>
    /// Initialize constructor
    /// </summary>
    public WorkspaceListPageViewModel()
    {
      Workspaces = new List<Workspace>();

      StorageIo.EnsureDir(AppSetting.WORKSPACE_BASE_PATH);
      var paths =
        StorageIo.Dirs(AppSetting.WORKSPACE_BASE_PATH)
                 .ToList();
      foreach (var p in paths)
      {
        var ws = new Workspace();
        ws.Open(p);
        Workspaces.Add(ws);
      }

      if (IsEmptyWorkspace)
        CreateDefaultWorkspace();
    }

    /// <summary>
    /// Create default workspace
    /// </summary>
    void CreateDefaultWorkspace()
    {
      var ws = new Workspace();
      ws.Open(AppSetting.DEFAULT_WORKSPACE_NAME);
      ws.SaveConfigData();
      Workspaces.Add(ws);
    }

    /// <summary>
    /// Find workspace by name.
    /// </summary>
    /// <param name="name">Workspace name</param>
    /// <returns>Workspace object</returns>
    public Workspace FindWorkspace(string name)
    {
      return Workspaces.FirstOrDefault(w => w.Name == name);
    }
  }
}