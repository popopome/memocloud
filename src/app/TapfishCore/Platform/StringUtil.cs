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

namespace TapfishCore.Platform
{
  public static class StringUtil
  {
    public static bool IsEmpty(this string v)
    {
      return v.Length == 0;
    }
  }
}