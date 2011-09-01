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
using DropNet;
using MemoPadCore.Common;
using Newtonsoft.Json;
using TapfishCore.Platform;
using TapfishCore.Resources;

namespace MemoPadTest.Helper
{
  public class DropboxLoginHelper
  {
    const string LOGIN_HELPER_FILE = "helperfile.txt";

    public static void Login(Action<string, string> callback)
    {
      var json = StorageIo.ReadTextFile(LOGIN_HELPER_FILE);
      if (string.IsNullOrEmpty(json))
      {
        var client = new DropNetClient(AppSetting.DROPBOX_API_KEY,
                                       AppSetting.DROPBOX_API_SECRET);
        client.LoginAsync(
          "popopome@gmail.com",
          "emfkqqkrtm",
          (login) =>
          {
            string[] credintial = new string[]
            {
              login.Token,
              login.Secret
            };
            var jsonsaving = JsonConvert.SerializeObject(credintial);
            StorageIo.WriteTextFile(LOGIN_HELPER_FILE, jsonsaving);

            ThreadUtil.UiCall(() => callback(login.Token, login.Secret));
          },
          (err) =>
          {
            MessageBox.Show("err happened:" + err.Response);
          }
        );

        return;
      }

      string[] info = JsonConvert.DeserializeObject<string[]>(json);
      callback(info[0], info[1]);
    }
  }
}