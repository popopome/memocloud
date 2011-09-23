﻿using System;
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
using MemoPad;
using MemoPadCore.Model;
using Microsoft.Phone.Controls;
using TapfishCore.Resources;

namespace MemoPadSamples
{
  public partial class MemoSummaryControlSamplePage : PhoneApplicationPage
  {
    public MemoSummaryControlSamplePage()
    {
      InitializeComponent();

      const string IMAGE_FILE_PATH = "/HAHAHA.image";
      var bmp = BitmapUtils.CreateBitmapImmediately("Images/sample/a.jpg");
      BitmapUtils.SaveBitmapToIso(IMAGE_FILE_PATH, bmp);

      var thumbpath = Memo.ThumbPathFromFullPath(IMAGE_FILE_PATH);
      BitmapUtils.SaveBitmapToIso(thumbpath, bmp);

      var photomemo = new Memo(IMAGE_FILE_PATH,
                               MemoKind.Photo);

      const string FULLPATH = "/HAHAHA.txt";
      StorageIo.WriteTextFile(
        FULLPATH,
        "Here is what I have so far.  Each team leaders should review them and consult with Frank Kim for possible adjustment.");
      var memo = new Memo(FULLPATH,
                          MemoKind.Text);

      _a.Open(photomemo);
    }
  }
}