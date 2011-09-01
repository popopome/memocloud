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
using MemoPadCore;
using Microsoft.Phone.Controls;
using TapfishCore.Platform;
using TapfishCore.Ui;

namespace MemoPad
{
  public partial class TextEditorPage : PhoneApplicationPage
  {
    TextEditorViewModel _vm;
    TapAndHoldRepeater _leftrepeater;
    TapAndHoldRepeater _rightrepeater;

    /// <summary>
    /// CTOR
    /// </summary>
    public TextEditorPage()
    {
      InitializeComponent();

      _leftrepeater = new TapAndHoldRepeater();

      _leftrepeater.Attach(_left,
          isfinal =>
          {
            MoveCaretBy(-1);

            if (isfinal)
              _text.Focus();
          });

      _rightrepeater = new TapAndHoldRepeater();
      _rightrepeater.Attach(
          _right,
          isfinal =>
          {
            MoveCaretBy(1);
            if (isfinal)
              _text.Focus();
          });

      _title.LostFocus += new RoutedEventHandler(OnTitleLostFocus);

      this.LayoutRoot.Children.Remove(_popup);

      this.Loaded += new RoutedEventHandler(OnLoaded);
      this.Unloaded += new RoutedEventHandler(OnUnloaded);
    }

    /// <summary>
    /// Loaded
    /// </summary>
    /// <param name="sender">Event sender</param>
    /// <param name="e">Event parameter</param>
    void OnLoaded(object sender, RoutedEventArgs e)
    {
      _popup.IsOpen = true;
    }

    /// <summary>
    /// Unloaded parameter
    /// </summary>
    /// <param name="sender">Sender</param>
    /// <param name="e">Event parameter</param>
    void OnUnloaded(object sender, RoutedEventArgs e)
    {
      _popup.IsOpen = false;
    }

    /// <summary>
    /// Navigated to
    /// </summary>
    /// <param name="e">Navigation event</param>
    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
      base.OnNavigatedTo(e);

      _vm = ViewModelLocator.TextEditorVm;
      _title.Text = _vm.Doc.Title;
      _text.Text = _vm.Doc.Text;
    }

    /// <summary>
    /// TOMBSTONING, DORMANT
    /// </summary>
    /// <param name="e">Navigation event</param>
    protected override void OnNavigatedFrom(NavigationEventArgs e)
    {
      base.OnNavigatedFrom(e);

      _vm.UpdateAndSave(_title.Text.Trim(),
                        _text.Text);
      _vm.Doc.IsRevised = true;
    }

    /// <summary>
    /// Move caret by one point
    /// </summary>
    /// <param name="delta"></param>
    void MoveCaretBy(int delta)
    {
      int x = _text.SelectionStart + delta;
      x = Math.Max(0, x);
      x = Math.Min(_text.Text.Length, x);
      _text.Select(x, 0);
    }

    /// <summary>
    /// Title lost key-input focus
    /// </summary>
    /// <param name="sender">Event sender</param>
    /// <param name="e">Event parameter</param>
    void OnTitleLostFocus(object sender, RoutedEventArgs e)
    {
      var title = _title.Text.Trim();
      if (title.IsEmpty())
        _title.Text = _vm.Doc.Title;
    }

    /// <summary>
    /// Title edit box got focus
    /// </summary>
    /// <param name="sender">Event sender</param>
    /// <param name="e">Event parameter</param>
    void OnTitleGotFocus(object sender, System.Windows.RoutedEventArgs e)
    {
      _title.SelectAll();
    }
  }
}