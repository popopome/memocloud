using System;
using System.Globalization;
using System.Net;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Interactivity;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Expression.Samples.Interactivity;
using Microsoft.Phone.Controls;

namespace TapfishCore.Behaviors
{
  /// <summary>
  /// Call method on control
  /// </summary>
  public class CallMethodOnControl : TriggerAction<FrameworkElement>
  {
    private MethodInfo Info { get; set; }

    #region MethodName DependencyProperty

    /// <summary>
    /// The <see cref="MethodName" /> dependency property's name.
    /// </summary>
    public const string MethodNamePropertyName = "MethodName";
    public string MethodName
    {
      get { return (string)GetValue(MethodNameProperty); }
      set { SetValue(MethodNameProperty, value); }
    }

    public static readonly DependencyProperty MethodNameProperty = DependencyProperty.Register(
        MethodNamePropertyName,
        typeof(string),
        typeof(CallMethodOnControl),
        new PropertyMetadata(OnMethodNameChanged));

    private static void OnMethodNameChanged(
        DependencyObject sender,
        DependencyPropertyChangedEventArgs args)
    {
      (sender as CallMethodOnControl).OnMethodNameChanged(args);
    }

    private void OnMethodNameChanged(DependencyPropertyChangedEventArgs args)
    {
      UpdateMethodInfo();
    }

    #endregion MethodName DependencyProperty

    /// <summary>
    /// Attached.
    /// </summary>
    protected override void OnAttached()
    {
      base.OnAttached();

      if (this.AssociatedObject.IsLoaded())
        this.UpdateMethodInfo();
      else
      {
        this.AssociatedObject.Loaded += new RoutedEventHandler(AssociatedObject_Loaded);
      }
    }

    void AssociatedObject_Loaded(object sender, RoutedEventArgs e)
    {
      this.AssociatedObject.Loaded -= new RoutedEventHandler(AssociatedObject_Loaded);
      this.UpdateMethodInfo();
    }

    /// <summary>
    /// Invoke method
    /// </summary>
    /// <param name="parameter"></param>
    protected override void Invoke(object parameter)
    {
      if (this.Info == null)
        return;

      FrameworkElement target = GetControlOrPage();
      ParameterInfo[] p = this.Info.GetParameters();
      if (p.Length == 0
        && target != null)
        this.Info.Invoke(target, null);
      else if (p.Length == 2 && this.AssociatedObject != null)
      {
        if (p[0].ParameterType.IsAssignableFrom(this.AssociatedObject.GetType()))
        {
          bool isAssignable = true;
          if (parameter != null)
          {
            isAssignable = p[1].ParameterType.IsAssignableFrom(parameter.GetType());
          }

          if (isAssignable)
            Info.Invoke(target,
                        new object[] { this.AssociatedObject, parameter });
        }
      }
    }

    /// <summary>
    /// Update method info
    /// </summary>
    private void UpdateMethodInfo()
    {
      FrameworkElement target = GetControlOrPage();
      if (target != null && !string.IsNullOrEmpty(this.MethodName))
      {
        Type targetType = target.GetType();

        MethodInfo methodInfo = targetType.GetMethod(MethodName, Type.EmptyTypes);
        if (methodInfo == null)
        {
          methodInfo = targetType.GetMethod(MethodName, new Type[] { typeof(object), typeof(object) });
          if (null == methodInfo)
            throw new ArgumentException(
                string.Format(CultureInfo.CurrentCulture,
                              ExceptionStringTable.CallMethodActionMethodDoesNotExistMessage,
                              this.MethodName,
                              targetType.Name));
        }

        if (methodInfo.GetParameters().Length != 0
          && methodInfo.GetParameters().Length != 2)
        {
          throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, ExceptionStringTable.CallMethodActionZeroParametersOnlyMessage));
        }

        this.Info = methodInfo;
      }
      else
      {
        this.Info = null;
      }
    }

    FrameworkElement GetControlOrPage()
    {
      FrameworkElement el = AssociatedObject;
      while (el != null)
      {
        if (el is UserControl
          || el is PhoneApplicationPage)
          return el;

        el = VisualTreeHelper.GetParent(el) as FrameworkElement;
      }
      return null;
    }
  }
}