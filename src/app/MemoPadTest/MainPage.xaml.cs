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
using Microsoft.Phone.Shell;
using Microsoft.Silverlight.Testing;

namespace MemoPadTest
{
  public partial class MainPage : PhoneApplicationPage
  {
    // Constructor
    public MainPage()
    {
      InitializeComponent();

      this.Loaded += new RoutedEventHandler(MainPage_Loaded);
    }

    void MainPage_Loaded(object sender, RoutedEventArgs e)
    {
      SystemTray.IsVisible = false;

      var testpage = UnitTestSystem.CreateTestPage() as IMobileTestPage;
      BackKeyPress += (x, xe) => xe.Cancel = testpage.NavigateBack();
      (Application.Current.RootVisual as PhoneApplicationFrame).Content = testpage;
    }
  }
}