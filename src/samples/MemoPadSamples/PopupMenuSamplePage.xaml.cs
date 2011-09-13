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

namespace MemoPadSamples
{
    public partial class PopupMenuSamplePage : PhoneApplicationPage
    {
        public PopupMenuSamplePage()
        {
            InitializeComponent();
        }

        private void _popup_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            /*var mnu = new PopupMenu(bgimage);
            mnu.AddBitmap(1, "/Images/menu/dropbox/dropbox-menu-back.png");
            mnu.AddBitmap(2, "/Images/menu/dropbox/dropbox-menu-item-sync.png");
            mnu.AddBitmap(3, "/Images/menu/dropbox/dropbox-menu-item-sync-selected.png");
            mnu.AddBitmap(4, "/Images/menu/dropbox/dropbox-menu-item-signout.png");
            mnu.AddBitmap(5, "/Images/menu/dropbox/dropbox-menu-item-signout-selected.png");

            mnu.ItemClicked +=
                (x, xe) =>
                {
                };

            mnu.AddItem(0, 8, 2, 3);
            mnu.AddItem(1, 8, 4, 5);

            mnu.Show(20, 20);*/

        }
    }
}