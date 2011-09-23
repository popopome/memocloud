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
  public partial class MiniTextMemoControlSamplePage : PhoneApplicationPage
  {
    public MiniTextMemoControlSamplePage()
    {
      InitializeComponent();

      Memo memo = new Memo("HAHAHA.txt",
        MemoKind.Text)
      {
        Summary = "Here is what I have so far.  Each team leaders should review them and consult with Frank Kim for possible adjustment."
      };
      _memoctrl.Open(memo);
      _memoctrl.Width = 200;
      _memoctrl.Height = 200;

      _memo2.Open(memo);
      _memo2.ShowBackSide();
    }
  }
}