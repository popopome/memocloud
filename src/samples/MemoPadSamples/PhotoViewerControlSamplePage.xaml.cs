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
using Microsoft.Phone.Controls;
using TapfishCore.Resources;

namespace MemoPadSamples
{
  public partial class PhotoViewerControlSamplePage : PhoneApplicationPage
  {
    public PhotoViewerControlSamplePage()
    {
      InitializeComponent();

      var bmp = BitmapUtils.CreateBitmapImmediately("Images/sample/a.jpg");
      _viewer.Build("a.jpg", bmp);
    }
  }
}