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
using System.Windows.Navigation;
using System.Windows.Shapes;
using MemoPadCore.Control;
using MemoPadCore.Model;
using Microsoft.Phone.Controls;
using TapfishCore.Platform;
using TapfishCore.Resources;
using TapfishCore.Ui;

namespace MemoPad
{
  public partial class PhotoViewerPage : PhoneApplicationPage
  {
    public const string BMP_ID_RENAME_BUTTON = "rename-button";
    const int PAGE_WIDTH = 480;
    const int PAGE_HEIGHT = 800;

    PhotoViewerPageViewModel _vm;
    RenameInputDialog _filenameinputdlg;

    #region CTOR

    /// <summary>
    /// CTOR
    /// </summary>
    public PhotoViewerPage()
    {
      InitializeComponent();

      _renamebutton.Create(BitmapPool.Bmp(BMP_ID_RENAME_BUTTON),
                           BitmapPool.Bmp(BMP_ID_RENAME_BUTTON));
      _renamebutton.Clicked += new EventHandler(OnRenameClicked);
    }

    #endregion CTOR

    /// <summary>
    /// Rename button is clicked
    /// </summary>
    /// <param name="sender">Event sender</param>
    /// <param name="e">Event parameter</param>
    void OnRenameClicked(object sender, EventArgs e)
    {
      if (_filenameinputdlg == null)
      {
        _filenameinputdlg = new RenameInputDialog
        {
          Width = PAGE_WIDTH,
          Height = PAGE_HEIGHT
        };

        _filenameinputdlg.Changed += new EventHandler<RenameInputDialogEventArgs>(OnRenameDlg_NameChanged);
        _filenameinputdlg.Canceled += new EventHandler(OnRenameDlg_Canceled);

        LayoutRoot.Children.Add(_filenameinputdlg);
      }

      _pv.DisableTouch();
      _filenameinputdlg.SetFileName(_vm.Memo.FullPath);
      _filenameinputdlg.Show();
    }

    /// <summary>
    /// Name changed
    /// </summary>
    /// <param name="sender">Event sender</param>
    /// <param name="e">Event parameter</param>
    void OnRenameDlg_NameChanged(
            object sender,
            RenameInputDialogEventArgs e)
    {
      if (_vm.Memo != null)
      {
        _vm.Memo.RenameTo(e.NameOnlyNoExt);
        _title.Text = _vm.Memo.Title;
      }

      _filenameinputdlg.Hide();
      _pv.EnableTouch();
    }

    /// <summary>
    /// Rename dialog is canceled
    /// </summary>
    /// <param name="sender">Event sender</param>
    /// <param name="e">Event parameter</param>
    void OnRenameDlg_Canceled(object sender, EventArgs e)
    {
      _filenameinputdlg.Hide();
      _pv.EnableTouch();
    }

    /// <summary>
    /// Become Active Page
    /// </summary>
    /// <param name="e"></param>
    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
      base.OnNavigatedTo(e);

      _vm = ViewModelLocator.PhotoViewerPageVm;
      _title.Text = _vm.Memo.Title;
      _pv.Build(_vm.Memo.FullBitmap);
    }
  }
}