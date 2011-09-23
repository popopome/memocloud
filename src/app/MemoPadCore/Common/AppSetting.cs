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
using Newtonsoft.Json;
using TapfishCore.Resources;

namespace MemoPadCore.Common
{
  public class AppSetting
  {
    #region Constants

    const string SETTING_FN = "appsetting.app";

    public const string DROPBOX_API_KEY = "j38j44e7tnpmfhc";
    public const string DROPBOX_API_SECRET = "em18wmbvqytur02";
    public const string WORKSPACE_BASE_PATH = "\\workspaces";
    public const string DEFAULT_WORKSPACE_NAME = "memoit";
    public const string TEXT_MEMO_EXT = ".txt";
    public const string DELETE_MEMO_EXT = ".deleted";
    public const string JPEG_EXT = ".jpg";
    public const string PNG_EXT = ".png";

    #endregion Constants

    public static readonly FontFamily FONT_SEGOE_WP_SEMILIGHT = new FontFamily("Segoe WP SemiLight");
    public static readonly FontFamily FONT_SEGOE_WP_LIGHT = new FontFamily("Segoe WP Light");

    public static readonly SolidColorBrush COLOR_SUMMARY_TITLE = new SolidColorBrush(Color.FromArgb(255, 110, 110, 110));
    public static readonly SolidColorBrush COLOR_SUMMARY_BODY =
      new SolidColorBrush(Color.FromArgb(255, 136, 136, 136));

    public string LastWorkspaceName { get; set; }

    #region Singletone implementation

    static AppSetting _instance;
    public static AppSetting Instance
    {
      get
      {
        if (_instance == null)
        {
          _instance = new AppSetting();
        }

        return _instance;
      }
    }

    #endregion Singletone implementation

    public AppSetting()
    {

    }

    /// <summary>
    /// Load configuration
    /// </summary>
    public void Load()
    {
      var json = StorageIo.ReadTextFile(SETTING_FN);
      if (string.IsNullOrEmpty(json))
      {
        DefaultSetting();
        return;
      }

      try
      {
        var app = JsonConvert.DeserializeObject<AppSetting>(json);
        this.LastWorkspaceName = app.LastWorkspaceName;
      }
      catch (System.Exception e)
      {
        DefaultSetting();
      }
    }

    /// <summary>
    /// Load default setting
    /// </summary>
    void DefaultSetting()
    {
      LastWorkspaceName = DEFAULT_WORKSPACE_NAME;
    }
  }
}