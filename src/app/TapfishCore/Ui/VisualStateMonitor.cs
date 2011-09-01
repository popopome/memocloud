using System;
using System.Collections;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace TapfishCore.Ui
{
  public class VisualStateMonitor
  {
    public static void Install(FrameworkElement el,
                               Action<string, string> callback)
    {
      IList groups =
          VisualStateManager.GetVisualStateGroups(el);
      if (null == groups
          || 0 == groups.Count)
        return;

      VisualStateGroup grp = groups[0] as VisualStateGroup;
      if (null == grp)
        return;

      grp.CurrentStateChanged +=
        (x, xe) =>
        {
          string oldName = xe.OldState == null
             ? ""
             : xe.OldState.Name;

          callback(oldName,
                   xe.NewState.Name);
        };
    }

  }
}