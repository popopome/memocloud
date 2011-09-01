using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace TapfishCore.Ui
{
  public static class FrameworkExtension
  {
    /// <summary>
    /// Show
    /// </summary>
    /// <param name="el"></param>
    public static void Show(this FrameworkElement el)
    {
      el.Visibility = Visibility.Visible;
    }

    /// <summary>
    /// Hide
    /// </summary>
    /// <param name="el"></param>
    public static void Hide(this FrameworkElement el)
    {
      el.Visibility = Visibility.Collapsed;
    }

    public static bool IsLoaded(this FrameworkElement element)
    {
      DependencyObject root = Application.Current.RootVisual;
      DependencyObject el = element;
      while (el != null)
      {
        if (el == root)
          return true;
        el = VisualTreeHelper.GetParent(el);
      }
      return false;
    }

    public static bool IsVisible(this FrameworkElement element)
    {
      return element.Visibility == Visibility.Visible;
    }

    public static void SetXY(this UIElement el,
                             double x,
                             double y)
    {
      Canvas.SetLeft(el, x);
      Canvas.SetTop(el, y);
    }

    public static Point GetXY(this UIElement el)
    {
      return new Point
      {
        X = Canvas.GetLeft(el),
        Y = Canvas.GetTop(el)
      };
    }

    public static void MoveRelByX(this UIElement el, double dx)
    {
      var x = Canvas.GetLeft(el);
      Canvas.SetLeft(el, x + dx);
    }
  }
}