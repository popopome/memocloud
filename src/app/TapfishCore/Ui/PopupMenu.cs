using System;
using System.Collections.Generic;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace TapfishCore.Ui
{
    public class PopupMenu
    {
        Dictionary<int, BitmapImage> _bmppool;

        public void AddBitmap(int bmpid, string path)
        {
            var bmp =
                new BitmapImage
                {
                    CreateOptions = BitmapCreateOptions.None,
                    UriSource = new Uri(path, UriKind.Relative)
                };

            _bmppool.Add(bmpid, bmp);
        }
    }
}