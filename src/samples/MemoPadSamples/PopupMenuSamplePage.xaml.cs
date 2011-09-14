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
using TapfishCore.Ui;

namespace MemoPadSamples
{
  public partial class PopupMenuSamplePage : PhoneApplicationPage
  {
    PopupMenu _mnu;

    public PopupMenuSamplePage()
    {
      InitializeComponent();

      _mnu = new PopupMenu();
      _mnu.AddBitmap(1, "Images/menu/dropbox/dropbox-menu-back.png");
      _mnu.AddBitmap(2, "Images/menu/dropbox/dropbox-menu-item-sync.png");
      _mnu.AddBitmap(3, "Images/menu/dropbox/dropbox-menu-item-sync-selected.png");
      _mnu.AddBitmap(4, "Images/menu/dropbox/dropbox-menu-item-signout.png");
      _mnu.AddBitmap(5, "Images/menu/dropbox/dropbox-menu-item-signout-selected.png");

      _mnu.ItemClicked +=
          (x, xe) =>
          {
          };

      _mnu.AddMenuItem(0, 0, 8, 2, 3);
      _mnu.AddMenuItem(1, 0, 8, 4, 5);

      this.LayoutRoot.Children.Add(_mnu);
      _mnu.Hide();
    }

    private void _popup_Click(object sender, System.Windows.RoutedEventArgs e)
    {
      _mnu.ShowMenu();
    }
  }
}