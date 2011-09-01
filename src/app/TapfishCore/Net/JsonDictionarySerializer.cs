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

namespace TapfishCore.Net
{
  public class JsonDictionarySerializer<T>
    where T : new()
  {
    public T Value { get; set; }

    public JsonDictionarySerializer()
    {
      Value = new T();
    }
  }
}