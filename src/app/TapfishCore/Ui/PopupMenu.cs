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
    Canvas _menucontent;

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

    #region MenuBackground Dependency property

    /// <summary>
    /// The <see cref="MenuBackground" /> dependency property's name.
    /// </summary>
    public const string MenuBackgroundPropertyName = "MenuBackground";

    /// <summary>
    /// Gets or sets the value of the <see cref="MenuBackground" />
    /// property. This is a dependency property.
    /// </summary>
    public BitmapSource MenuBackground
    {
      get
      {
        return (BitmapSource)GetValue(MenuBackgroundProperty);
      }
      set
      {
        SetValue(MenuBackgroundProperty, value);
      }
    }

    /// <summary>
    /// Identifies the <see cref="MenuBackground" /> dependency property.
    /// </summary>
    public static readonly DependencyProperty MenuBackgroundProperty = DependencyProperty.Register(
        MenuBackgroundPropertyName,
        typeof(BitmapSource),
        typeof(PopupMenu),
        new PropertyMetadata((x, xe) =>
          {
            var mnu = (x as PopupMenu);
            var bmp = (xe.NewValue as BitmapSource);
            mnu._menucontent.Background = new ImageBrush
            {
              ImageSource = bmp
            };
            mnu._menucontent.Width = bmp.PixelWidth;
            mnu._menucontent.Height = bmp.PixelHeight;
          }));

    #endregion MenuBackground Dependency property

    public int PopupWidth
    {
      get
      {
        return MenuBackground.PixelWidth;
      }
    }

    public int PopupHeight
    {
      get
      {
        return MenuBackground.PixelHeight;
      }
    }

    /// <summary>
    /// CTOR
    /// </summary>
    public PopupMenu()
    {
      this.Width = 800;
      this.Height = 800;
      this.Background = new SolidColorBrush(Colors.Transparent);

      _bmppool = new Dictionary<int, BitmapImage>();
      _items = new List<ImageButton>();

      _menucontent = new Canvas();
      this.Children.Add(_menucontent);

      this.ManipulationCompleted += new EventHandler<ManipulationCompletedEventArgs>(OnManipCompleted);
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

    /// <summary>
    /// Get bitmap from bitmap pool
    /// </summary>
    /// <param name="bmpid">Bitmap id</param>
    /// <returns>Bitmap image</returns>
    BitmapImage GetBitmap(int bmpid)
    {
      if (_bmppool.ContainsKey(bmpid))
        return _bmppool[bmpid];

      return null;
    }

    public void AddMenuItem(int itemid,
                            int marginx,
                            int marginy,
                            string normalbmpPoolId,
                            string pressbmpPoolId)
    {
      AddMenuItem(itemid, marginx, marginy,
                  BitmapPool.Bmp(normalbmpPoolId),
                  BitmapPool.Bmp(pressbmpPoolId));
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
                    BitmapSource normalbmp,
                    BitmapSource focusbmp)
    {
      var imgbutton = new ImageButton();

      Debug.Assert(normalbmp != null);
      Debug.Assert(focusbmp != null);

      imgbutton.Create(normalbmp, focusbmp);
      imgbutton.Tag = itemid;

      var pt = ComputeNextItemPoint();
      imgbutton.SetXY(pt.X + marginx, pt.Y + marginy);
      imgbutton.Clicked += new EventHandler(OnImageButtonClicked);

      _menucontent.Children.Add(imgbutton);
      _items.Add(imgbutton);
    }

    /// <summary>
    /// Image button is clicked
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    void OnImageButtonClicked(object sender, EventArgs e)
    {
      var imgbutton = (sender as ImageButton);
      int itemid = (int)imgbutton.Tag;

      if (ItemClicked != null)
        ItemClicked(this,
                    new PopupMenuEventArgs
                    {
                      MenuId = itemid
                    });

      CloseMenu();
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
    public void ShowMenu(double x, double y)
    {
      _menucontent.SetXY(x, y);
      this.Show();
      BackKeyHandler.InstallBackKeyHandler(this.GetHashCode(),
        () =>
        {
          this.CloseMenu();
          return true;
        });
    }

    /// <summary>
    /// Click other area.
    /// Close popup menu
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    void OnManipCompleted(
          object sender,
          ManipulationCompletedEventArgs e)
    {
      CloseMenu();
    }

    /// <summary>
    /// Close menu
    /// </summary>
    public void CloseMenu()
    {
      this.Hide();
      BackKeyHandler.UninstallBackKeyHandler(this.GetHashCode());
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