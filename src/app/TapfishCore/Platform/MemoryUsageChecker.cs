using System;
using System.Diagnostics;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Threading;
using Microsoft.Phone.Info;

namespace TapfishCore.Platform
{
  public class MemoryUsageChecker
  {
    public static void Start()
    {
      var timer = new DispatcherTimer();
      timer.Interval = TimeSpan.FromMilliseconds(500);
      timer.Tick += (x, xe) =>
        {
          long deviceTotalMemory = (long)DeviceExtendedProperties.GetValue("DeviceTotalMemory");
          long currentMemoryUsage = (long)DeviceExtendedProperties.GetValue("ApplicationCurrentMemoryUsage");
          long peakMemoryUsage = (long)DeviceExtendedProperties.GetValue("ApplicationPeakMemoryUsage");

          Debug.WriteLine("TOTAL:{0}kb, CUR:{1}kb, PEAK:{2}lb",
            deviceTotalMemory / 1000,
            currentMemoryUsage / 1000,
            peakMemoryUsage / 1000);

        };
      timer.Start();
    }
  }
}