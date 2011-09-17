using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reactive.Linq;
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
  public partial class SyncBoxSamplePage : PhoneApplicationPage
  {
    public SyncBoxSamplePage()
    {
      InitializeComponent();

      this.Loaded += new RoutedEventHandler(SyncBoxSamplePage_Loaded);
      _syncbox.SetSyncInfo(10, 10);

    }

    void SyncBoxSamplePage_Loaded(object sender, RoutedEventArgs e)
    {
      _syncbox.ShowSyncBox();

      Observable.Interval(TimeSpan.FromSeconds(1))
                .ObserveOnDispatcher()
                .Subscribe((val) =>
                {
                  _syncbox.IncreaseUploading();
                });

      Observable.Interval(TimeSpan.FromMilliseconds(1010))
        .ObserveOnDispatcher()
        .Subscribe(v => _syncbox.IncreaseDownloading());
    }
  }
}