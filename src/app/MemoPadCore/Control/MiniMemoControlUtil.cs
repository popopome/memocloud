using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using TapfishCore.Resources;
using TapfishCore.Ui;

namespace MemoPadCore.Control
{
  public class MiniMemoControlUtil
  {
    public const int DESIRED_WIDTH = 200;
    public const int DESIRED_HEIGHT = 200;

    public const int BUTTON_IMAGE_WIDTH = 90;
    public const int BUTTON_IMAGE_HEIGHT = 90;

    const int BUTTON_CANCEL_X = 100;
    const int BUTTON_CANCEL_Y = 91;

    const int BUTTON_TRASH_X = 10;
    const int BUTTON_TRASH_Y = 1;

    const int MORE_COMMANDS_WIDTH = 60;
    const int MORE_COMMANDS_HEIGHT = 60;
    const int MORE_COMMANDS_RIGHT_MARGIN = 0;
    const int MORE_COMMANDS_BOTTOM_MARGIN = 0;

    public static BitmapImage FlipBackground { get; set; }
    public static BitmapImage MoreCommandsBmp { get; set; }
    public static BitmapImage FlipTrashBmp { get; set; }
    public static BitmapImage FlipArrowBmp { get; set; }

    static MiniMemoControlUtil()
    {
      FlipBackground = BitmapUtils.CreateBitmapImmediately("Images/memo-list/memo-summary-flip-back.png");
      FlipTrashBmp = BitmapUtils.CreateBitmapImmediately("Images/memo-list/memo-summary-trash.png");
      FlipArrowBmp = BitmapUtils.CreateBitmapImmediately("Images/memo-list/memo-summary-arrow-selected.png");

      MoreCommandsBmp = BitmapUtils.CreateBitmapImmediately("Images/memo-list/more-commands.png");
    }

    /// <summary>
    /// Create more buttons
    /// </summary>
    public static Image CreateMoreCommandsImage()
    {
      var img = new Image
      {
        Width = MORE_COMMANDS_WIDTH,
        Height = MORE_COMMANDS_HEIGHT,
        HorizontalAlignment = HorizontalAlignment.Right,
        VerticalAlignment = VerticalAlignment.Bottom,
        Source = MoreCommandsBmp,
        Margin = new Thickness(0, 0, MORE_COMMANDS_RIGHT_MARGIN, MORE_COMMANDS_BOTTOM_MARGIN)
      };
      return img;
    }

    /// <summary>
    /// Create image button
    /// </summary>
    /// <param name="parent"></param>
    /// <param name="normalimgpath"></param>
    /// <param name="focusimgpath"></param>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    public static ImageButton CreateImageButton(
      Panel parent,
      string normalimgpath,
      string focusimgpath,
      double x,
      double y)
    {
      var btn = new ImageButton();
      btn.Create(BUTTON_IMAGE_WIDTH,
                 BUTTON_IMAGE_HEIGHT,
                 normalimgpath,
                 focusimgpath);
      btn.SetXYWithMargin(x, y);
      parent.Children.Add(btn);
      btn.Hide();
      return btn;
    }

    public static ImageButton CreateCancelImageButton(Panel parent)
    {
      return CreateImageButton(
              parent,
              "Images/memo-list/memo-summary-arrow.png",
              "Images/memo-list/memo-summary-arrow-selected.png",
              12,
              94);
    }

    public static ImageButton CreateTrashImageButton(Panel parent)
    {
      return CreateImageButton(
              parent,
              "Images/memo-list/memo-summary-trash.png",
              "Images/memo-list/memo-summary-trash-selected.png",
              12,
              1);
    }
  }
}