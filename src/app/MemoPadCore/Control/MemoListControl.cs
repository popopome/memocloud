using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using MemoPadCore.Model;
using TapfishCore.Ui;

namespace MemoPadCore.Control
{
  public class MemoListControl : Canvas
  {
    public const int DESIRED_WIDTH = 480;
    public const int DESIRED_HEIGHT = 800;
    const int MAX_NUM_ROWS_PER_COL = 3;

    const int MEMO_Y_POSITION = 80;
    const int MEMO_Y_MARGIN = 8;

    public event EventHandler<MemoClickedEventArgs> MemoClicked;

    #region Fields

    List<MemoSummaryControl> _list;
    CompositeTransform _trans;

    DoubleAnimation _alignani;
    Storyboard _sbalign;

    double _beginx;
    double _endx;

    double _downx;

    #endregion Fields

    #region X DependencyProperty

    /// <summary>
    /// The <see cref="X" /> dependency property's name.
    /// </summary>
    public const string XPropertyName = "X";

    /// <summary>
    /// Gets or sets the value of the <see cref="X" />
    /// property. This is a dependency property.
    /// </summary>
    public double X
    {
      get
      {
        return (double)GetValue(XProperty);
      }
      set
      {
        SetValue(XProperty, value);
      }
    }

    /// <summary>
    /// Identifies the <see cref="X" /> dependency property.
    /// </summary>
    public static readonly DependencyProperty XProperty = DependencyProperty.Register(
        XPropertyName,
        typeof(double),
        typeof(MemoListControl),
        new PropertyMetadata(0.0, OnXChanged));

    static void OnXChanged(DependencyObject sender,
                           DependencyPropertyChangedEventArgs args)
    {
      (sender as MemoListControl).OnXChanged((double)args.NewValue);
    }

    void OnXChanged(double x)
    {
      /*_trans.TranslateX = (double)args.NewValue;*/
      int cnt = _list.Count;
      for (int i = 0; i < cnt; ++i)
      {
        var c = _list[i];
        var trans = c.RenderTransform as TranslateTransform;
        trans.X = x;
      }
    }

    #endregion X DependencyProperty

    /// <summary>
    /// CTOR
    /// </summary>
    public MemoListControl()
    {
      this.Background = new SolidColorBrush(Colors.Transparent);
      _trans = new CompositeTransform();
      this.RenderTransform = _trans;

      this.ManipulationStarted += new EventHandler<ManipulationStartedEventArgs>(OnManipulationStarted);
      this.ManipulationDelta += new EventHandler<ManipulationDeltaEventArgs>(OnManipulationDelta);
      this.ManipulationCompleted += new EventHandler<ManipulationCompletedEventArgs>(OnManipulationCompleted);
    }

    /// <summary>
    /// Build list control with given document data
    /// </summary>
    /// <param name="docs"></param>
    public void Build(List<TextDocument> docs)
    {
      RemoveAllChildren();

      _list = new List<MemoSummaryControl>();
      docs.ForEach(d =>
        {
          var c = CreateAndAddSummaryControl(d);
          _list.Add(c);
        });

      ArrangeMemoControls();

      UpdateScrollRange();

      X = _beginx;
      OnXChanged(X);
    }

    /// <summary>
    /// Arrange memo controls
    /// </summary>
    void ArrangeMemoControls()
    {
      double x = 0;
      double y = MEMO_Y_POSITION;
      int numrow = 0;
      _list.ForEach(c =>
      {
        c.SetXY(x, y);

        ++numrow;
        if (MAX_NUM_ROWS_PER_COL == numrow)
        {
          x += c.Width;
          y = MEMO_Y_POSITION;
          numrow = 0;
        }
        else
          y += c.Height + MEMO_Y_MARGIN;

      });
    }

    /// <summary>
    /// Remove all children
    /// </summary>
    void RemoveAllChildren()
    {
      int cnt = this.Children.Count;
      for (int i = cnt - 1; i >= 0; --i)
      {
        this.Children.RemoveAt(i);
      }
    }

    /// <summary>
    /// Update scroll range
    /// </summary>
    void UpdateScrollRange()
    {
      _beginx = (DESIRED_WIDTH - MemoSummaryControl.DESIRED_WIDTH) / 2;
      _endx = _beginx - (((_list.Count + MAX_NUM_ROWS_PER_COL - 1) / MAX_NUM_ROWS_PER_COL) - 1) * MemoSummaryControl.DESIRED_WIDTH;
    }

    /// <summary>
    /// Create summary control
    /// </summary>
    /// <returns>summary control</returns>
    MemoSummaryControl CreateAndAddSummaryControl(TextDocument d)
    {
      var c = new MemoSummaryControl
      {
        Width = MemoSummaryControl.DESIRED_WIDTH,
        Height = MemoSummaryControl.DESIRED_HEIGHT,
        RenderTransform = new TranslateTransform(),
        CacheMode = new BitmapCache()
      };
      c.Open(d);
      c.Click += new EventHandler<MemoClickedEventArgs>(OnMemoClicked);

      this.Children.Add(c);
      return c;
    }

    /// <summary>
    /// Manipulation started
    /// </summary>
    /// <param name="sender">Event sender</param>
    /// <param name="e">Event parameter</param>
    void OnManipulationStarted(object sender, ManipulationStartedEventArgs e)
    {
      if (_sbalign != null)
      {
        double curx = X;
        _sbalign.Stop();
        X = curx;
      }

      _downx = X;
    }

    /// <summary>
    /// Manipulation delta
    /// </summary>
    /// <param name="sender">Event sender</param>
    /// <param name="e">Event parameter</param>
    void OnManipulationDelta(object sender, ManipulationDeltaEventArgs e)
    {
      var dx = e.CumulativeManipulation.Translation.X / 2;
      X = _downx + dx;
    }

    /// <summary>
    /// Manipulation completed
    /// </summary>
    /// <param name="sender">Event sender</param>
    /// <param name="e">Event parameter</param>
    void OnManipulationCompleted(object sender, ManipulationCompletedEventArgs e)
    {
      double tx = ComputeNextX(e);
      BeginAlignAnimationTo(tx);
    }

    /// <summary>
    /// Prepare storyboard
    /// </summary>
    void PrepareStoryboard()
    {
      if (_sbalign == null)
      {
        _alignani = new DoubleAnimation
        {
          Duration = TimeSpan.FromMilliseconds(500),
          EasingFunction = new CubicEase
          {
            EasingMode = EasingMode.EaseOut
          }
        };
        Storyboard.SetTarget(_alignani, this);
        Storyboard.SetTargetProperty(_alignani, new PropertyPath("X"));

        _sbalign = new Storyboard();
        _sbalign.Children.Add(_alignani);
      }
    }

    /// <summary>
    /// Begin align animation to given position
    /// </summary>
    /// <param name="tx">Target position</param>
    void BeginAlignAnimationTo(double tx)
    {
      PrepareStoryboard();

      //
      // If X and tx is equal,
      // any transformation is not happened.
      //
      // This can make problem,
      // cause the app updates rendertransform of
      // each children.
      //
      // For newly added children should update
      // its render transform in order to be
      // displayed in right position.
      //
      // So by making intentionally X value different,
      // app has always a chance to update
      // render transform of its children.
      //
      // TODO:
      // - Program design should cover this case.
      //
      if (X == tx)
      {
        X = tx - 1;
      }
      _alignani.To = tx;
      _sbalign.Begin();
    }

    /// <summary>
    /// Compute next x
    /// </summary>
    /// <param name="e">Manipulation completed event</param>
    /// <returns>New target x</returns>
    double ComputeNextX(ManipulationCompletedEventArgs e)
    {
      if (X > _beginx)
        return _beginx;

      if (X < _endx)
        return _endx;

      double tx = 0;
      if (UiUtils.IsTapped(e))
        tx = Align(X);
      else
      {
        double dx = e.TotalManipulation.Translation.X;
        if (dx > 0)
          tx = Align(X + MemoSummaryControl.DESIRED_WIDTH + MemoSummaryControl.DESIRED_WIDTH / 2);
        else
          tx = Align(_downx - MemoSummaryControl.DESIRED_WIDTH);
      }

      if (tx > _beginx)
        tx = _beginx;
      else if (tx < _endx)
        tx = _endx;

      return tx;
    }

    double Align(double x)
    {
      return
        Math.Floor(((x - _beginx) / MemoSummaryControl.DESIRED_WIDTH)) * MemoSummaryControl.DESIRED_WIDTH + _beginx;
    }

    /// <summary>
    /// Memo clicked
    /// </summary>
    /// <param name="sender">Event sender</param>
    /// <param name="e"></param>
    void OnMemoClicked(object sender, MemoClickedEventArgs e)
    {
      if (MemoClicked != null)
        MemoClicked(this, e);
    }

    /// <summary>
    /// Refresh revised document
    /// </summary>
    public void RefreshRevisedDocuments()
    {
      _list.ForEach(
        c => c.ReloadRevisedDocument());
    }

    /// <summary>
    /// Add new memo to front
    /// </summary>
    /// <param name="doc"></param>
    public void AddNewMemoToFront(TextDocument doc)
    {
      /*MoveMemosByRel(MemoSummaryControl.DESIRED_WIDTH);*/
      var c = CreateAndAddSummaryControl(doc);
      _list.Add(c);

      ArrangeMemoControls();
      UpdateScrollRange();
      if (_list.Count == 1)
        BeginAlignAnimationTo(_beginx);
      else
        BeginAlignAnimationTo(_endx);
    }

    /// <summary>
    /// Move memo controls by delta-x
    /// </summary>
    /// <param name="dx">Delta x</param>
    void MoveMemosByRel(List<MemoSummaryControl> list,
                        double dx)
    {
      list.ForEach(c => c.MoveRelByX(dx));
    }

    /// <summary>
    /// Delete most visible document
    /// </summary>
    public TextDocument DeleteMostVisibleDocument()
    {
      int delindex = MostVisibleIndex();
      if (-1 == delindex)
        return null;

      var c = _list[delindex];
      var doc = c.Doc;

      this.Children.Remove(c);
      var nextsiblings = _list.GetRange(delindex + 1, _list.Count - delindex - 1);
      MoveMemosByRel(nextsiblings, -MemoSummaryControl.DESIRED_WIDTH);
      _list.Remove(c);

      UpdateScrollRange();

      double tx = Align(X);
      if (tx > _beginx)
        tx = _beginx;
      else if (tx < _endx)
        tx = _endx;

      BeginAlignAnimationTo(tx);
      return doc;
    }

    /// <summary>
    /// Compute most visible index
    /// </summary>
    /// <returns></returns>
    int MostVisibleIndex()
    {
      double x = X;
      double end = x + MemoSummaryControl.DESIRED_WIDTH;
      int i = 0;
      for (; end < 0 && i < _list.Count; ++i)
      {
        x += MemoSummaryControl.DESIRED_WIDTH;
        end += MemoSummaryControl.DESIRED_WIDTH;
      }

      double maxvisible =
        (x < 0)
        ? end
        : end - x;
      int maxvisibleindex = i;

      x += MemoSummaryControl.DESIRED_WIDTH;
      end += MemoSummaryControl.DESIRED_WIDTH;
      ++i;
      for (; x < DESIRED_WIDTH && i < _list.Count; ++i)
      {
        double curvisible =
          (end > DESIRED_WIDTH)
          ? DESIRED_WIDTH - x
          : end - x;

        if (curvisible > maxvisible)
        {
          maxvisibleindex = i;
        }

        x += MemoSummaryControl.DESIRED_WIDTH;
        end += MemoSummaryControl.DESIRED_WIDTH;
      }

      if (maxvisibleindex >= _list.Count)
        return -1;

      return maxvisibleindex;
    }
  }
}