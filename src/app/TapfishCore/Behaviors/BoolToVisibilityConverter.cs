using System;
using System.Globalization;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace TapfishCore.Behaviors
{
  public class BoolToVisibilityConverter : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      bool isBool = (value is bool);
      if (false == isBool)
        return value;

      Visibility visible = Visibility.Collapsed;
      if ((bool)value)
        visible = Visibility.Visible;

      if ((parameter is bool)
        && ((bool)parameter))
      {
        visible = (visible == Visibility.Visible)
          ? Visibility.Collapsed
          : Visibility.Visible;
      }

      return visible;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      throw new NotImplementedException();
    }

  }
}