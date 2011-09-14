using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using TapfishCore.Resources;

namespace TapfishCore.Ui
{
  public class PopupMenu : Canvas
  {
    public event EventHandler<PopupMenuEventArgs> ItemClicked;

    Dictionary<int, BitmapImage> _bmppool;
    List<ImageButton> _items;

    #region Orientation DependencyProperty

    /// <summary>
    /// The <see cref="Orientation" /> dependency property's name.
    /// </summary>
    public const string OrientationPropertyName = "Orientation";

    /// <summary>
    /// Gets or sets the value of the <see cref="Orientation" />
    /// property. This is a dependency property.
    /// </summary>
    public Orientation Orientation
    {
      get
      {
        return (Orientation)GetValue(OrientationProperty);
      }
      set
      {
        SetValue(OrientationProperty, value);
      }
    }

    /// <summary>
    /// Identifies the <see cref="Orientation" /> dependency property.
    /// </summary>
    public static readonly DependencyProperty OrientationProperty = DependencyProperty.Register(
        OrientationPropertyName,
        typeof(Orientation),
        typeof(PopupMenu),
        new PropertyMetadata(Orientation.Vertical));

    #endregion Orientation DependencyProperty

    /// <summary>
    /// CTOR
    /// </summary>
    public PopupMenu()
    {
      _bmppool = new Dictionary<int, BitmapImage>();
      _items = new List<ImageButton>();
    }

    /// <summary>
    /// Add bitmap to pool
    /// </summary>
    /// <param name="bmpid">Bitmap id</param>
    /// <param name="path">Bitmap path</param>
    public void AddBitmap(int bmpid, string path)
    {
      if (_bmppool.ContainsKey(bmpid))
        return;

      var bmp = BitmapUtils.CreateBitmapImmediately(path);
      Debug.Assert(bmp != null);

      _bmppool.Add(bmpid, bmp);
    }

    BitmapImage GetBitmap(int bmpid)
    {
      if (_bmppool.ContainsKey(bmpid))
        return _bmppool[bmpid];

      return null;
    }

    /// <summary>
    /// Add menu item
    /// </summary>
    /// <param name="itemid"></param>
    /// <param name="marginx"></param>
    /// <param name="marginy"></param>
    /// <param name="normalbmpid"></param>
    /// <param name="focusbmpid"></param>
    public void AddMenuItem(
                    int itemid,
                    int marginx,
                    int marginy,
                    int normalbmpid,
                    int focusbmpid)
    {
      var imgbutton = new ImageButton();

      var normalbmp = GetBitmap(normalbmpid);
      var focusbmp = GetBitmap(focusbmpid);
      imgbutton.Create(normalbmp,
                       focusbmp);
      imgbutton.Tag = itemid;

      var pt = ComputeNextItemPoint();
      imgbutton.SetXY(pt.X + marginx, pt.Y + marginy);

      this.Children.Add(imgbutton);
      _items.Add(imgbutton);
    }

    /// <summary>
    /// Compute the point where next item is positioned.
    /// </summary>
    /// <returns>Position for next item being placed.</returns>
    Point ComputeNextItemPoint()
    {
      if (_items.Count == 0)
        return default(Point);

      var last = _items.Last();
      if (null == last)
        return default(Point);

      var pt = last.GetXY();
      if (this.Orientation == Orientation.Vertical)
        pt.Y += last.Height;
      else
        pt.X += last.Width;

      return pt;
    }

    /// <summary>
    /// Show menu
    /// </summary>
    public void ShowMenu()
    {
      this.Show();
    }
  }

  /// <summary>
  /// This class defines event arguments
  /// </summary>
  public class PopupMenuEventArgs : EventArgs
  {
    public int MenuId { get; set; }
  }
}