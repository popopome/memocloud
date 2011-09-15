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
using TapfishCore.Ui;

namespace MemoPadSamples
{
  public partial class PopupMenuSamplePage : PhoneApplicationPage
  {
    PopupMenu _mnu;

    public PopupMenuSamplePage()
    {
      InitializeComponent();

      BitmapPool.AddBitmap("back", "Images/menu/dropbox/dropbox-menu-back.png");
      BitmapPool.AddBitmap("sync", "Images/menu/dropbox/dropbox-menu-item-sync.png");
      BitmapPool.AddBitmap("syncsel", "Images/menu/dropbox/dropbox-menu-item-sync-selected.png");
      BitmapPool.AddBitmap("signout", "Images/menu/dropbox/dropbox-menu-item-signout.png");
      BitmapPool.AddBitmap("signoutsel", "Images/menu/dropbox/dropbox-menu-item-signout-selected.png");

      _mnu = new PopupMenu
      {
        MenuBackground = BitmapPool.Bmp("back")
      };
      _mnu.ItemClicked +=
          (x, xe) =>
          {
          };

      _mnu.AddMenuItem(0, 22, 9, BitmapPool.Bmp("sync"), BitmapPool.Bmp("syncsel"));
      _mnu.AddMenuItem(1, 0, 0, BitmapPool.Bmp("signout"), BitmapPool.Bmp("signoutsel"));

      this.LayoutRoot.Children.Add(_mnu);
      _mnu.Hide();
    }

    private void _popup_Click(object sender, System.Windows.RoutedEventArgs e)
    {
      _mnu.ShowMenu(30, 30);
    }
  }
}