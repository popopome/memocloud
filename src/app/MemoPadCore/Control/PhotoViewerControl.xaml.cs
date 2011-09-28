using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using TapfishCore.Math;

namespace MemoPadCore.Control
{
  public partial class PhotoViewerControl : UserControl
  {
    const int FLICK_DURATION_IN_MILLIS = 700;

    #region Fields

    Matrix33 _fitmat;
    BitmapSource _bmp;
    Matrix33 _mat;
    IDisposable _subscription;

    Point _pinchdown;
    Matrix33 _pinchdownmat;

    Matrix33 _anibeginmat;
    Matrix33 _aniendmat;

    Storyboard _sbflick;

    #endregion Fields

    #region AniTimeline DependencyProperty

    /// <summary>
    /// The <see cref="AniTimeline" /> dependency property's name.
    /// </summary>
    public const string AniTimelinePropertyName = "AniTimeline";

    /// <summary>
    /// Gets or sets the value of the <see cref="AniTimeline" />
    /// property. This is a dependency property.
    /// </summary>
    public double AniTimeline
    {
      get
      {
        return (double)GetValue(AniTimelineProperty);
      }
      set
      {
        SetValue(AniTimelineProperty, value);
      }
    }

    /// <summary>
    /// Identifies the <see cref="AniTimeline" /> dependency property.
    /// </summary>
    public static readonly DependencyProperty AniTimelineProperty = DependencyProperty.Register(
        AniTimelinePropertyName,
        typeof(double),
        typeof(PhotoViewerControl),
        new PropertyMetadata(0.0, (x, xe) =>
          {
            var pv = x as PhotoViewerControl;

            var m =
            Matrix33.Interpolate(
                              pv._anibeginmat,
                              pv._aniendmat,
                              (double)xe.NewValue);
            pv.UpdateMatrix(m);

          }));

    #endregion AniTimeline DependencyProperty

    #region CTOR

    public PhotoViewerControl()
    {
      InitializeComponent();

      _subscription = Observable.FromEvent<SizeChangedEventHandler, SizeChangedEventArgs>(
        a =>
        {
          return new SizeChangedEventHandler((x, xe) => a(xe));
        },
        h => this.SizeChanged += h,
        h => this.SizeChanged -= h)
        .Select(evtpat => evtpat.NewSize)
        .Subscribe(
        sz =>
        {
          if (_bmp == null)
            return;

          this.BitmapToFit(_bmp);
        }
        );
    }

    #endregion CTOR

    void PhotoViewerControl_Loaded(object sender, RoutedEventArgs e)
    {

    }

    /// <summary>
    /// Build photo viewer
    /// </summary>
    /// <param name="title"></param>
    /// <param name="bmp"></param>
    public void Build(
                string title,
                BitmapSource bmp)
    {

      Debug.Assert(bmp.PixelWidth > 0);
      Debug.Assert(bmp.PixelHeight > 0);

      _bmp = bmp;
      _title.Text = title;

      if (this.ActualWidth != 0
        && this.ActualHeight != 0)
        BitmapToFit(bmp);
    }

    /// <summary>
    /// Bitmap to fit
    /// </summary>
    /// <param name="bmp"></param>
    void BitmapToFit(BitmapSource bmp)
    {
      _img.Width = bmp.PixelWidth;
      _img.Height = bmp.PixelHeight;
      _img.Source = bmp;
      _mat = Matrix33
              .Create()
              .SetRectToRect(new Rect(0, 0, bmp.PixelWidth, bmp.PixelHeight),
                             new Rect(0, 0, 480, 800),
                             Stretch.Uniform);
      _trans.Matrix = _mat.ToMatrix();
      _fitmat = _mat;
    }

    /// <summary>
    /// Pinch started
    /// </summary>
    /// <param name="sender">Event sender</param>
    /// <param name="e">Event parameter</param>
    void GestureListener_PinchStarted(
            object sender,
            PinchStartedGestureEventArgs e)
    {
      _pinchdownmat = _mat;
      _pinchdown = _mat.Transform(e.GetPosition(_img));
    }

    /// <summary>
    /// Pinch delta
    /// </summary>
    /// <param name="sender">Event sender</param>
    /// <param name="e">Event parameter</param>
    void GestureListener_PinchDelta(
            object sender,
            PinchGestureEventArgs e)
    {
      var scale = e.DistanceRatio;
      var mat = _pinchdownmat;
      mat.PostTranslate(-_pinchdown.X, -_pinchdown.Y);
      mat.PostScale(scale, scale);
      mat.PostTranslate(_pinchdown.X, _pinchdown.Y);

      _mat = mat;
      AlignPhoto();
      if (_mat.Sx < _fitmat.Sx
        || _mat.Sy < _fitmat.Sy)
      {
        UpdateMatrix(_fitmat);
      }
    }

    /// <summary>
    /// Update matrix
    /// </summary>
    /// <param name="mat">Matrix33 object</param>
    void UpdateMatrix(Matrix33 mat)
    {
      _mat = mat;
      _trans.Matrix = _mat.ToMatrix();
    }

    /// <summary>
    /// Pinch completed
    /// </summary>
    /// <param name="sender">Event sender</param>
    /// <param name="e">Event parameter</param>
    void GestureListener_PinchCompleted(
            object sender,
            PinchGestureEventArgs e)
    {
      AlignPhoto();
    }

    /// <summary>
    /// Drag started
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    void GestureListener_DragStarted(
            object sender,
            DragStartedGestureEventArgs e)
    {
      double val = AniTimeline;
      if (_sbflick != null)
        _sbflick.Stop();
      AniTimeline = val;
    }

    /// <summary>
    /// Drag delta
    /// </summary>
    /// <param name="sender">Event sender</param>
    /// <param name="e">Event parameter</param>
    void GestureListener_DragDelta(
            object sender,
            DragDeltaGestureEventArgs e)
    {
      _mat.PostTranslate(e.HorizontalChange,
                         e.VerticalChange);
      _trans.Matrix = _mat.ToMatrix();
    }

    /// <summary>
    /// Drag completed
    /// </summary>
    /// <param name="sender">Event sender</param>
    /// <param name="e">Event parameter</param>
    void GestureListener_DragCompleted(
            object sender,
            DragCompletedGestureEventArgs e)
    {
      Point offset = CalcOffsetToAlign(_mat);
      if (offset.X == 0)
        offset.X = e.HorizontalVelocity / 4;
      if (offset.Y == 0)
        offset.Y = e.VerticalVelocity / 4;

      if (offset.X == 0
        && offset.Y == 0)
        return;

      //
      // Compute offset again with final matrix.
      // If final matrix is not aligned,
      // let's make it align.
      // So the empty area should not be visible
      // as possible as it can.
      //
      var m = _mat;
      m.PostTranslate(offset.X, offset.Y);
      var testoffset = CalcOffsetToAlign(m);
      offset.X += testoffset.X;
      offset.Y += testoffset.Y;

      StartFlickAni(offset);
    }

    void StartFlickAni(Point offset)
    {
      var begin = _mat;
      var end = _mat;
      end.PostTranslate(offset.X, offset.Y);
      BeginMatrixAnimation(begin, end);
    }

    void BeginMatrixAnimation(Matrix33 beginmat, Matrix33 endmat)
    {
      _anibeginmat = beginmat;
      _aniendmat = endmat;
      if (_sbflick == null)
      {
        var ani = new DoubleAnimation
        {
          From = 0,
          To = 1,
          EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut },
          Duration = TimeSpan.FromMilliseconds(FLICK_DURATION_IN_MILLIS)
        };

        Storyboard.SetTarget(ani, this);
        Storyboard.SetTargetProperty(ani, new PropertyPath(AniTimelinePropertyName));

        _sbflick = new Storyboard();
        _sbflick.Children.Add(ani);
      }

      _sbflick.Begin();
    }

    /// <summary>
    /// Align photo
    /// </summary>
    void AlignPhoto()
    {
      var offset = CalcOffsetToAlign(_mat);

      _mat.PostTranslate(offset.X, offset.Y);
      _trans.Matrix = _mat.ToMatrix();
    }

    /// <summary>
    /// Calc offset to align
    /// </summary>
    /// <returns></returns>
    Point CalcOffsetToAlign(Matrix33 m)
    {
      double offx = 0;
      double offy = 0;

      var lt = m.Transform(0, 0);
      var rb = m.Transform(_bmp.PixelWidth, _bmp.PixelHeight);

      var dx = rb.X - lt.X;
      var dy = rb.Y - lt.Y;

      if (dx < this.ActualWidth)
      {
        var x = this.ActualWidth / 2 - dx / 2;
        offx = x - lt.X;
      }
      else
      {
        if (lt.X > 0)
          offx = -lt.X;
        if (rb.X < this.ActualWidth)
          offx = this.ActualWidth - rb.X;
      }

      if (dy < this.ActualHeight)
      {
        var y = this.ActualHeight / 2 - dy / 2;
        offy = y - lt.Y;
      }
      else
      {
        if (lt.Y > 0)
          offy = -lt.Y;

        if (rb.Y < this.ActualHeight)
          offy = this.ActualHeight - rb.Y;
      }

      return new Point(offx, offy);
    }

    void GestureListener_DoubleTap(
            object sender,
            Microsoft.Phone.Controls.GestureEventArgs e)
    {
      if (_mat.IsSame(_fitmat))
      {
        var m = Matrix33
              .Create()
              .SetRectToRect(new Rect(0, 0, _bmp.PixelWidth, _bmp.PixelHeight),
                             new Rect(0, 0, 480, 800),
                             Stretch.UniformToFill);
        /*var bbox = _mat.Transform(0, 0, _bmp.PixelWidth, _bmp.PixelHeight);
        var cx = bbox.Width / 2;
        var cy = bbox.Height / 2;
        var m = _mat;
        m.PostTranslate(-cx, -cy);
        m.PostScale(1.5, 1.5);
        m.PostTranslate(cx, cy);*/
        BeginMatrixAnimation(_mat, m);
      }
      else
      {
        BeginMatrixAnimation(_mat, _fitmat);
      }

    }
  }
}