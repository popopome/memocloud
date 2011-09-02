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
    const string VS_NORMAL = "Normal";
    const string VS_DELETECONFIRM = "DeleteConfirm";
    const string VS_SYNC = "SyncState";

    MemoListPageViewModel _vm;

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

      _dropboxsignin.SigninSucceeded += new EventHandler<DropboxSigninEventArgs>(OnDropboxSigninSucceeded);
      _dropboxsignin.SigninFailed += new EventHandler<DropboxSigninEventArgs>(OnDropboxSigninFailed);
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
        _dropboxsignin.Show();

        return;
      }

      Sync();
    }

    /// <summary>
    /// Begin sync
    /// </summary>
    void Sync()
    {
      GoToVisualState(VS_SYNC);

      var sync = new DropboxSync(_vm.Workspace);
      sync.Finished += new EventHandler<DropboxSyncEventArgs>(OnSyncFinished);
      sync.Start();
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
          }
          else
          {
            MessageBox.Show("Succeeded");
            Refresh();
          }

          GoToVisualState(VS_NORMAL);
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
      _vm.Workspace.DropBoxToken = e.UserToken;
      _vm.Workspace.DropBoxSecret = e.UserSecret;

      _vm.Workspace.ChangeDropboxPath(e.RemoteFolderName);
      _vm.Workspace.SaveConfigData();

      Sync();
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
  }
}