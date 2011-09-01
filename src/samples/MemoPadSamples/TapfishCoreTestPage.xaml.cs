using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using TapfishCore.Platform;
using TapfishCore.Resources;

namespace MemoPadSamples
{
  public partial class TapfishCoreTestPage : PhoneApplicationPage
  {
    public TapfishCoreTestPage()
    {
      InitializeComponent();

      Test_PathUtil_SplitFunction();

      Test_StorageIo_Rename();
    }

    private static void Test_StorageIo_Rename()
    {
      StorageIo.DeleteDir("\\abc");
      StorageIo.DeleteDir("\\abcd");
      StorageIo.CreateDir("\\abc");
      StorageIo.WriteTextFile("\\abc\\a.txt", "TEST FILE");
      StorageIo.WriteTextFile("\\abc\\b.txt", "TEST FILE");
      StorageIo.WriteTextFile("\\abc\\c.txt", "TEST FILE");

      StorageIo.RenameDir("\\abc", "\\abcd");
      Debug.Assert(false == StorageIo.Exists("\\abc\\a.txt"));
      Debug.Assert(true == StorageIo.Exists("\\abcd\\a.txt"));
    }

    private static void Test_PathUtil_SplitFunction()
    {
      var list = PathUtil.Split("/images/workspace\\ok");
      string last = list.Last();
      Debug.WriteLine("Last=>{0}", last);
    }
  }
}