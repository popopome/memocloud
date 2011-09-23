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
using TapfishCore.Resources;

namespace MemoPadSamples
{
  public partial class MiniPhotoMemoControlSamplePage : PhoneApplicationPage
  {
    public MiniPhotoMemoControlSamplePage()
    {
      InitializeComponent();

      const string IMAGE_FILE_PATH = "/HAHAHA.image";
      var bmp = BitmapUtils.CreateBitmapImmediately("Images/sample/a.jpg");
      BitmapUtils.SaveBitmapToIso(IMAGE_FILE_PATH, bmp);

      var thumbpath = Memo.ThumbPathFromFullPath(IMAGE_FILE_PATH);
      BitmapUtils.SaveBitmapToIso(thumbpath, bmp);

      Memo memo = new Memo(
        IMAGE_FILE_PATH,
        MemoKind.Photo);

      _memo0.Open(memo);
    }
  }
}