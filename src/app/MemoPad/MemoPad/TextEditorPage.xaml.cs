using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
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

    bool _istextfocused;
    Point _downpt;

    double _downoffset;

    /// <summary>
    /// The <see cref="VOffset" /> dependency property's name.
    /// </summary>
    public const string VOffsetPropertyName = "VOffset";

    /// <summary>
    /// Gets or sets the value of the <see cref="VOffset" />
    /// property. This is a dependency property.
    /// </summary>
    public double VOffset
    {
      get
      {
        return (double)GetValue(VOffsetProperty);
      }
      set
      {
        SetValue(VOffsetProperty, value);
      }
    }

    /// <summary>
    /// Identifies the <see cref="VOffset" /> dependency property.
    /// </summary>
    public static readonly DependencyProperty VOffsetProperty = DependencyProperty.Register(
        VOffsetPropertyName,
        typeof(double),
        typeof(TextEditorPage),
        new PropertyMetadata((x, xe) =>
          {
            double val = (double)xe.NewValue;
            Debug.WriteLine("vertical->{0}", val);

            var pg = x as TextEditorPage;
            if (pg._istextfocused)
              pg._focusthief.Focus();

          }));

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

      _text.GotFocus += (x, xe) => _istextfocused = true;
      _text.LostFocus += (x, xe) => _istextfocused = false;

      _scrollviewer.ManipulationStarted += new EventHandler<ManipulationStartedEventArgs>(OnTextManipStarted);
      _scrollviewer.ManipulationDelta += new EventHandler<ManipulationDeltaEventArgs>(OnTextManipDelta);

      var binding = new Binding();
      binding.Source = _scrollviewer;
      binding.Path = new PropertyPath("VerticalOffset");
      this.SetBinding(TextEditorPage.VOffsetProperty,
                      binding);

      this.Loaded += new RoutedEventHandler(OnLoaded);
      this.Unloaded += new RoutedEventHandler(OnUnloaded);
    }

    void OnTextManipStarted(object sender, ManipulationStartedEventArgs e)
    {
      _downoffset = _scrollviewer.VerticalOffset;
      /*var trans = e.ManipulationContainer.TransformToVisual(this.LayoutRoot);
      _downpt = trans.Transform(e.ManipulationOrigin);*/
    }

    void OnTextManipDelta(object sender, ManipulationDeltaEventArgs e)
    {
      if (_istextfocused == false)
        return;

      /*var trans = e.ManipulationContainer.TransformToVisual(this.LayoutRoot);
      var curpt = trans.Transform(e.ManipulationOrigin);

      var dy = Math.Abs(_downpt.Y - curpt.Y);*/

      var dy = Math.Abs(_downoffset - _scrollviewer.VerticalOffset);
      if (dy >= 64)
      {
        _focusthief.Focus();
      }
    }

    /// <summary>
    /// Loaded
    /// </summary>
    /// <param name="sender">Event sender</param>
    /// <param name="e">Event parameter</param>
    void OnLoaded(object sender, RoutedEventArgs e)
    {
      _istextfocused = false;
      /*_popup.IsOpen = true;*/
    }

    /// <summary>
    /// Unloaded parameter
    /// </summary>
    /// <param name="sender">Sender</param>
    /// <param name="e">Event parameter</param>
    void OnUnloaded(object sender, RoutedEventArgs e)
    {
      /*_popup.IsOpen = false;*/
    }

    /// <summary>
    /// Navigated to
    /// </summary>
    /// <param name="e">Navigation event</param>
    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
      base.OnNavigatedTo(e);

      _vm = ViewModelLocator.TextEditorVm;
      _title.Text = _vm.Memo.Title;
      _text.Text = _vm.Memo.Text;
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
      _vm.Memo.IsRevised = true;
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
        _title.Text = _vm.Memo.Title;
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