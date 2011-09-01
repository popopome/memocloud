using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Interactivity;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using TapfishCore.Ui;

namespace TapfishCore.Behaviors
{
  public class TapTrigger : TriggerBase<FrameworkElement>
  {
    Point Down { get; set; }
    bool IsDraggingDetected { get; set; }
    Transform OriginalTransform { get; set; }

    #region OffsetXAtDown DependencyProperty

    public double OffsetXAtDown
    {
      get { return (double)GetValue(OffsetXAtDownProperty); }
      set { SetValue(OffsetXAtDownProperty, value); }
    }
    public static readonly DependencyProperty OffsetXAtDownProperty =
        DependencyProperty.Register(
          "OffsetXAtDown",
          typeof(double),
          typeof(TapTrigger),
          new PropertyMetadata(0.0));

    #endregion OffsetXAtDown DependencyProperty

    #region OffsetYAtDown DependencyProperty

    public double OffsetYAtDown
    {
      get { return (double)GetValue(OffsetYAtDownProperty); }
      set { SetValue(OffsetYAtDownProperty, value); }
    }
    public static readonly DependencyProperty OffsetYAtDownProperty =
        DependencyProperty.Register(
          "OffsetYAtDown",
          typeof(double),
          typeof(TapTrigger),
          new PropertyMetadata(0.0));

    #endregion OffsetYAtDown DependencyProperty

    protected override void OnAttached()
    {
      base.OnAttached();

      AssociatedObject.ManipulationStarted += new EventHandler<ManipulationStartedEventArgs>(AssociatedObject_ManipulationStarted);
      AssociatedObject.ManipulationDelta += new EventHandler<ManipulationDeltaEventArgs>(AssociatedObject_ManipulationDelta);
      AssociatedObject.ManipulationCompleted += new EventHandler<ManipulationCompletedEventArgs>(AssociatedObject_ManipulationCompleted);
    }

    void AssociatedObject_ManipulationStarted(object sender, ManipulationStartedEventArgs e)
    {
      Down = e.ManipulationOrigin;
      IsDraggingDetected = false;

      if (OffsetXAtDown != 0.0
        || OffsetYAtDown != 0.0)
      {
        OriginalTransform = AssociatedObject.RenderTransform;
        if (OriginalTransform != null)
        {
          TransformGroup grp = new TransformGroup();
          grp.Children.Add(OriginalTransform);
          grp.Children.Add(
              new TranslateTransform
              {
                X = OffsetXAtDown,
                Y = OffsetYAtDown
              });
          AssociatedObject.RenderTransform = grp;
        }
      }
    }

    void AssociatedObject_ManipulationDelta(object sender, ManipulationDeltaEventArgs e)
    {
      if (IsDraggingDetected)
        return;

      Point cur = e.ManipulationOrigin;
      var absDeltaX = System.Math.Abs(cur.X - Down.X);
      var absDeltaY = System.Math.Abs(cur.Y - Down.Y);
      var maxDelta = System.Math.Max(absDeltaX, absDeltaY);
      if (maxDelta >= UiUtils.TAP_THRESHOLD)
      {
        IsDraggingDetected = true;
        RevertTransform();
      }
    }

    /// <summary>
    /// Revert RenderTransform to original one
    /// </summary>
    void RevertTransform()
    {
      if (OriginalTransform != null)
      {
        AssociatedObject.RenderTransform = OriginalTransform;
        OriginalTransform = null;
      }
    }

    void AssociatedObject_ManipulationCompleted(object sender, ManipulationCompletedEventArgs e)
    {
      RevertTransform();

      if (false == IsDraggingDetected)
        this.InvokeActions(null);

      IsDraggingDetected = false;
    }

    protected override void OnDetaching()
    {
      base.OnDetaching();

      AssociatedObject.ManipulationStarted -= new EventHandler<ManipulationStartedEventArgs>(AssociatedObject_ManipulationStarted);
      AssociatedObject.ManipulationDelta -= new EventHandler<ManipulationDeltaEventArgs>(AssociatedObject_ManipulationDelta);
      AssociatedObject.ManipulationCompleted -= new EventHandler<ManipulationCompletedEventArgs>(AssociatedObject_ManipulationCompleted);
    }
  }
}