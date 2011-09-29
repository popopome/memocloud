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

namespace MemoPad
{
  public partial class PhotoViewerPage : PhoneApplicationPage
  {
    public const string BMP_ID_RENAME_BUTTON = "rename-button";

    #region CTOR

    public PhotoViewerPage()
    {
      InitializeComponent();

      _renamebutton.Create(BitmapPool.Bmp(BMP_ID_RENAME_BUTTON),
                           BitmapPool.Bmp(BMP_ID_RENAME_BUTTON));
      _renamebutton.Clicked += new EventHandler(OnRenameClicked);

      _pv.Build(BitmapUtils.CreateBitmapImmediately("Images/sample/a.jpg"));
    }

    #endregion CTOR

    /// <summary>
    /// Rename button is clicked
    /// </summary>
    /// <param name="sender">Event sender</param>
    /// <param name="e">Event parameter</param>
    void OnRenameClicked(object sender, EventArgs e)
    {
    }
  }
}