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
using System.Windows.Navigation;
using System.Windows.Shapes;
using GalaSoft.MvvmLight.Messaging;
using MemoPadCore.Common.Messages;
using MemoPadCore.Control;
using MemoPadCore.Model;
using Microsoft.Phone.Controls;
using TapfishCore.Platform;
using TapfishCore.Resources;
using TapfishCore.Ui;

namespace MemoPad
{
  public partial class MemoListPage : PhoneApplicationPage
  {
    #region Constants

    const string VS_NORMAL = "Normal";
    const string VS_DELETECONFIRM = "DeleteConfirm";
    const string VS_SYNC = "SyncState";

    const int MNU_ID_SYNC = 1;
    const int MNU_ID_DROPBOX_LOGOUT = 2;

    const string BMP_ID_SYNC_MENU_BACK = "syncmenuback";
    const string BMP_ID_DROPBOX_SYNC = "sync";
    const string BMP_ID_DROPBOX_SYNC_SEL = "syncsel";
    const string BMP_ID_DROPBOX_SIGNOUT = "signout";
    const string BMP_ID_DROPBOX_SIGNOUT_SEL = "signoutsel";

    #endregion Constants

    #region Fields

    MemoListPageViewModel _vm;

    PopupMenu _syncmenu;

    #endregion Fields

    #region CTOR

    /// <summary>
    /// CTOR
    /// </summary>
    public MemoListPage()
    {
      InitializeComponent();

      _vm = ViewModelLocator.MemoListPageVm;

      //
      // I donot want to show empty page.
      // Here let's add empty memo
      //
      if (_vm.Docs.Count == 0)
      {
        _vm.AddNewDocumentToFront();
      }

      _memolist.Build(_vm.Docs);
      _memolist.MemoClicked +=
        new EventHandler<MemoClickedEventArgs>(OnMemoClicked);
      _memolist.MemoDeleteClicked += new EventHandler<MemoClickedEventArgs>(OnMemoDeleteClicked);

      _dropboxsignin.SigninSucceeded += new EventHandler<DropboxSigninEventArgs>(OnDropboxSigninSucceeded);
      _dropboxsignin.SigninFailed += new EventHandler<DropboxSigninEventArgs>(OnDropboxSigninFailed);
      _dropboxsignin.Canceled += new EventHandler(OnDropboxSigninCanceled);

      string[] bmpdata = new string[]
      {
        BMP_ID_SYNC_MENU_BACK, "Images/menu/dropbox/dropbox-menu-back.png",
        BMP_ID_DROPBOX_SYNC, "Images/menu/dropbox/dropbox-menu-item-sync.png",
        BMP_ID_DROPBOX_SYNC_SEL, "Images/menu/dropbox/dropbox-menu-item-sync-selected.png",
        BMP_ID_DROPBOX_SIGNOUT, "Images/menu/dropbox/dropbox-menu-item-signout.png",
        BMP_ID_DROPBOX_SIGNOUT_SEL, "Images/menu/dropbox/dropbox-menu-item-signout-selected.png"
      };
      for (int i = 0; i < bmpdata.Length; i += 2)
        BitmapPool.AddBitmap(bmpdata[i], bmpdata[i + 1]);

      InitializeSyncMenu();

      _syncbox.DoneButtonClicked += new EventHandler(OnSyncBoxDoneClicked);
    }

    #endregion CTOR

    /// <summary>
    /// Initialize sync menu
    /// </summary>
    void InitializeSyncMenu()
    {
      _syncmenu = new PopupMenu
      {
        MenuBackground = BitmapPool.Bmp(BMP_ID_SYNC_MENU_BACK)
      };
      _syncmenu.ItemClicked += new EventHandler<PopupMenuEventArgs>(OnSyncMenuItemClicked);
      _syncmenu.AddMenuItem(MNU_ID_SYNC,
                            22, 9,
                            BMP_ID_DROPBOX_SYNC,
                            BMP_ID_DROPBOX_SYNC_SEL);
      _syncmenu.AddMenuItem(MNU_ID_DROPBOX_LOGOUT,
                            0, 0,
                            BMP_ID_DROPBOX_SIGNOUT,
                            BMP_ID_DROPBOX_SIGNOUT_SEL);

      this.LayoutRoot.Children.Add(_syncmenu);
      _syncmenu.Hide();
    }

    /// <summary>
    /// Memo clicked
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    void OnMemoClicked(object sender, MemoClickedEventArgs e)
    {
      Messenger.Default.Send<OpenDocumentMessage>(
        new OpenDocumentMessage(e.Document));

      Uri uri = new Uri("/TextEditorPage.xaml", UriKind.Relative);
      NavigationService.Navigate(uri);
    }

    /// <summary>
    /// This page is activated
    /// </summary>
    /// <param name="e">Event parameter</param>
    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
      base.OnNavigatedTo(e);

      _memolist.RefreshRevisedDocuments();
    }

    /// <summary>
    /// New memobutton is clicked
    /// </summary>
    /// <param name="sender">Event sender</param>
    /// <param name="e">Event parameter</param>
    void OnNewMemoClicked(
            object sender,
            ManipulationCompletedEventArgs e)
    {
      if (UiUtils.IsTapped(e))
      {
        NewMemo();
      }
    }

    /// <summary>
    /// Create new memo
    /// </summary>
    void NewMemo()
    {
      var doc = _vm.AddNewDocumentToFront();
      _memolist.AddNewMemoToFront(doc);
    }

    /// <summary>
    /// Delete memo clicked
    /// </summary>
    /// <param name="sender">Event sender</param>
    /// <param name="e">Event parameter</param>
    void OnDeleteMemoClicked(
          object sender,
          ManipulationCompletedEventArgs e)
    {
      GoToVisualState(VS_DELETECONFIRM);

    }

    /// <summary>
    /// Go to visual state
    /// </summary>
    /// <param name="statename"></param>
    void GoToVisualState(string statename)
    {
      VisualStateManager.GoToState(this, statename, true);
    }

    /// <summary>
    /// Delete confirm button canceled
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    void OnDeleteConfirmCanceled(object sender,
                                 ManipulationCompletedEventArgs e)
    {
      GoToVisualState(VS_NORMAL);
    }

    /// <summary>
    /// Delete confirm button clicked
    /// </summary>
    /// <param name="sender">Event sender</param>
    /// <param name="e">Event parameter</param>
    void OnDeleteConfirmed(object sender,
                           ManipulationCompletedEventArgs e)
    {
      e.Handled = true;
      if (UiUtils.IsTapped(e) == false)
        return;

      var doc = _memolist.DeleteMostVisibleDocument();
      if (null == doc)
        return;

      _vm.DeleteDocument(doc);

      if (_vm.Docs.Count == 0)
      {
        NewMemo();
      }

      GoToVisualState(VS_NORMAL);
    }

    /// <summary>
    /// Sync button clicked
    /// </summary>
    /// <param name="sender">Event sender</param>
    /// <param name="e">Event parameter</param>
    void OnSyncButtonClicked(
                    object sender,
                    ManipulationCompletedEventArgs e)
    {
      if (_vm.Workspace.HasDropBoxToken == false)
      {
        //
        // Let's show dropbox account control.
        //
        _dropboxsignin.SetSyncFolderName(_vm.Workspace.DropBoxPath);
        _dropboxsignin.Activate();
        return;
      }

      var trans = _syncbutton.TransformToVisual(this.LayoutRoot);
      var pt = trans.Transform(new Point
      {
        X = _syncbutton.ActualWidth - 12,
        Y = 0
      });

      _syncmenu.SetTopMost(this.LayoutRoot);
      _syncmenu.ShowMenu(pt.X, pt.Y);

      /*Sync();*/
    }

    /// <summary>
    /// Begin sync
    /// </summary>
    void Sync()
    {
      GoToVisualState(VS_SYNC);

      _syncbox.SetSyncInfo(0, 0);
      _syncbox.HideSyncBox();
      _syncbox.UpdateDescription("Prepare synchronization...");

      var sync = new DropboxSync(_vm.Workspace);
      sync.SyncStarted += new EventHandler<DropboxSyncEventArgs>(OnSyncStarted);
      sync.SyncStepped += new EventHandler<DropboxSyncEventArgs>(OnSyncStepped);
      sync.Finished += new EventHandler<DropboxSyncEventArgs>(OnSyncFinished);
      sync.Start();

      _syncbox.ShowSyncBox();
    }

    void OnSyncStarted(object sender, DropboxSyncEventArgs e)
    {
      _syncbox.SetSyncInfo(e.TotalUploadingFiles,
                           e.TotalDownloadingFiles);
    }

    void OnSyncStepped(object sender, DropboxSyncEventArgs e)
    {
      ThreadUtil.UiCall(() =>
        {
          _syncbox.UpdateDescription(e.Message);
          _syncbox.UpdateProgress(e.TotalUploadingFiles,
                                  e.NumUploadedFiles,
                                  e.TotalDownloadingFiles,
                                  e.NumDownloadedFiles);
        });
    }

    /// <summary>
    /// Sync finished
    /// </summary>
    /// <param name="sender">Event sender</param>
    /// <param name="e">Event parameter</param>
    void OnSyncFinished(object sender, DropboxSyncEventArgs e)
    {
      ThreadUtil.UiCall(() =>
        {
          if (e.Result != DropboxSyncResult.Success)
          {
            var errmsg =
              string.Format("{0}\nResult:{1}",
                      e.ErrorMessage,
                      e.Result.ToString());
            MessageBox.Show(errmsg,
                            "Sync Failed",
                            MessageBoxButton.OK);

            GoToVisualState(VS_NORMAL);
            return;
          }

          _syncbox.SyncFinished();
        });
    }

    /// <summary>
    /// Refresh
    /// </summary>
    void Refresh()
    {
      _vm.RefreshWorkspace();
      _memolist.Build(_vm.Docs);
    }

    /// <summary>
    /// Dropbox sign-in succeeded
    /// </summary>
    /// <param name="sender">Event sender</param>
    /// <param name="e">Event parameter</param>
    void OnDropboxSigninSucceeded(object sender, DropboxSigninEventArgs e)
    {
      var ws = _vm.Workspace;
      ws.DropBoxToken = e.UserToken;
      ws.DropBoxSecret = e.UserSecret;

      ws.ChangeDropboxPath(e.RemoteFolderName);
      ws.SaveConfigData();

      _dropboxsignin.Deactivate();

      Sync();
    }

    /// <summary>
    /// Signin canceled
    /// </summary>
    /// <param name="sender">Event sender</param>
    /// <param name="e">Event parameter</param>
    void OnDropboxSigninCanceled(object sender, EventArgs e)
    {
      _dropboxsignin.Deactivate();
    }

    /// <summary>
    /// Signin failed
    /// </summary>
    /// <param name="sender">Event sender</param>
    /// <param name="e">Event parameter</param>
    void OnDropboxSigninFailed(object sender, DropboxSigninEventArgs e)
    {
      MessageBox.Show(e.ErrorMessage);
    }

    /// <summary>
    /// Sync menu item is clicked
    /// </summary>
    /// <param name="sender">Event sender</param>
    /// <param name="e">Clicked menu item</param>
    void OnSyncMenuItemClicked(object sender, PopupMenuEventArgs e)
    {
      switch (e.MenuId)
      {
        case MNU_ID_SYNC:
          Sync();
          break;
        case MNU_ID_DROPBOX_LOGOUT:
          var result = MessageBox.Show("Are you sure to sign out DropBox sync?",
                                       "Confirmation",
                                       MessageBoxButton.OKCancel);
          if (result == MessageBoxResult.OK)
            _vm.ClearSync();

          break;
      }
    }

    /// <summary>
    /// Syncbox done button is clicked
    /// </summary>
    /// <param name="sender">Event sender</param>
    /// <param name="e">Event parameter</param>
    void OnSyncBoxDoneClicked(object sender, EventArgs e)
    {
      Refresh();
      GoToVisualState(VS_NORMAL);
    }

    /// <summary>
    /// Memo delete clicked
    /// </summary>
    /// <param name="sender">Event sender</param>
    /// <param name="e">Event parameter</param>
    void OnMemoDeleteClicked(
            object sender,
            MemoClickedEventArgs e)
    {
      _vm.DeleteDocument(e.Document);
      if (_vm.Docs.Count == 0)
      {
        NewMemo();
      }
      GoToVisualState(VS_NORMAL);
    }
  }
}