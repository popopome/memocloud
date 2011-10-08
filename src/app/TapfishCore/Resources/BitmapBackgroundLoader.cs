using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using TapfishCore.Platform;

namespace TapfishCore.Resources
{

  public class BitmapBackgroundLoader
  {
    static object _locker;
    static Subject<BitmapRequest> _requestob;
    static Subject<BitmapRequest> _streamob;
    static Subject<BitmapRequest> _requestdoneob;
    static bool _exitflag;

    public struct LoadingResult
    {
      public bool Succeeded { get; set; }
      public string ErrorMessage { get; set; }
      public string Path { get; set; }
      public BitmapImage Bmp { get; set; }
      public int Width { get; set; }
      public int Height { get; set; }
    }

    private class BitmapRequest
    {
      public string Id { get; set; }
      public string IsoPath { get; set; }
      public string ResourcePath { get; set; }
      public Stream Stream { get; set; }
      public BitmapImage Bmp { get; set; }
      public int BmpWidth { get; set; }
      public int BmpHeight { get; set; }

      public BooleanDisposable Handle { get; set; }
      public IDisposable BmpLoadingSubscription { get; set; }
      public bool Succeeded { get; set; }
      public string ErrorMessage { get; set; }
      public Action<LoadingResult> Callback { get; set; }

      public LoadingResult CreateLoadingResult()
      {
        return new LoadingResult
        {
          Bmp = this.Bmp,
          Path = IsoPath ?? ResourcePath,
          Succeeded = this.Succeeded,
          ErrorMessage = this.ErrorMessage,
          Width = this.BmpWidth,
          Height = this.BmpHeight
        };
      }

      public void CleanUp()
      {
        if (Stream != null)
        {
          try
          {
            Stream.Close();
            Stream.Dispose();
          }
          catch (System.Exception e)
          {
          }
          Stream = null;
        }

        if (BmpLoadingSubscription != null)
        {
          if (Thread.CurrentThread.IsBackground)
          {
            ThreadUtil.UiCall(() =>
            {
              BmpLoadingSubscription.Dispose();
              BmpLoadingSubscription = null;
            });
          }
          else
          {
            BmpLoadingSubscription.Dispose();
            BmpLoadingSubscription = null;
          }
        }
      }
    }

    static BitmapBackgroundLoader()
    {
      _requestob = new Subject<BitmapRequest>();
      _requestob
            .ObserveOn(Scheduler.ThreadPool)
            .Subscribe(GenerateStreamAndPush);

      _streamob = new Subject<BitmapRequest>();
      _streamob
        .ObserveOnDispatcher()
        .Subscribe(GenerateBackgroundBitmapLoadingAndPush);

      _requestdoneob = new Subject<BitmapRequest>();
      _requestdoneob
        .ObserveOn(Scheduler.ThreadPool)
        .Subscribe(NotifyJobDone);
    }

    /// <summary>
    /// Load resource bitmap in asynchronous mode
    /// </summary>
    /// <param name="path">Path to resource</param>
    /// <returns>Disposable handle</returns>
    public static IDisposable LoadResourceBitmapAsync(
                  string path,
                  Action<LoadingResult> callback)
    {
      var req = new BitmapRequest
      {
        Id = Guid.NewGuid().ToString(),
        Handle = new BooleanDisposable(),
        ResourcePath = path,
        Callback = callback
      };

      _requestob.OnNext(req);
      return req.Handle;
    }

    /// <summary>
    /// Create memory stream from given stream
    /// </summary>
    /// <param name="src">Source stream</param>
    /// <returns></returns>
    static MemoryStream MemoryStreamFromStream(Stream src)
    {
      int len = (int)src.Length;
      byte[] buf = new byte[len];
      src.Read(buf, 0, buf.Length);

      return new MemoryStream(buf);
    }

    /// <summary>
    /// Generate STREAM and
    /// push it to bitmap loading observable
    /// </summary>
    /// <param name="req"></param>
    static void GenerateStreamAndPush(BitmapRequest req)
    {
      if (req.Handle.IsDisposed)
      {
        req.CleanUp();
        return;
      }

      if (string.IsNullOrEmpty(req.ResourcePath) == false)
      {
        var stminfo = Application.GetResourceStream(new Uri(req.ResourcePath, UriKind.Relative));
        if (null == stminfo
          || null == stminfo.Stream)
        {
          req.Stream = null;
          req.Succeeded = false;
          req.ErrorMessage = "Failed to get resource stream";

          _requestdoneob.OnNext(req);
          return;
        }

        req.Stream = MemoryStreamFromStream(stminfo.Stream);
        _streamob.OnNext(req);
        return;
      }

      if (string.IsNullOrEmpty(req.IsoPath) == false)
      {
        byte[] buf = StorageIo.ReadBinaryFile(req.IsoPath);
        req.Stream = new MemoryStream(buf);
        _streamob.OnNext(req);
        return;
      }

      //
      // Unrecharable.
      //
      Debug.Assert(false);
    }

    /// <summary>
    /// Generate bitmap image and start background loading
    /// </summary>
    /// <param name="req"></param>
    static void GenerateBackgroundBitmapLoadingAndPush(BitmapRequest req)
    {
      if (req.Handle.IsDisposed)
      {
        req.CleanUp();
        return;
      }

      var bmp = new BitmapImage
      {
        CreateOptions = BitmapCreateOptions.BackgroundCreation
      };

      var opensucceeded = Observable.FromEventPattern<RoutedEventArgs>(
            h => bmp.ImageOpened += h,
            h => bmp.ImageOpened -= h)
            .Select<EventPattern<RoutedEventArgs>, EventArgs>(x => x.EventArgs as EventArgs);
      var openfailed =
        Observable.FromEventPattern<ExceptionRoutedEventArgs>(
                          h => bmp.ImageFailed += h,
                          h => bmp.ImageFailed -= h)
                  .Select<EventPattern<ExceptionRoutedEventArgs>, EventArgs>(x => x.EventArgs as EventArgs);

      req.BmpLoadingSubscription =
        Observable.Amb(opensucceeded, openfailed)
                  .Subscribe
                    (args =>
                      {
                        if (req.Handle.IsDisposed)
                        {
                          req.CleanUp();
                          return;
                        }

                        if (args is ExceptionRoutedEventArgs)
                        {
                          req.Succeeded = false;
                          req.ErrorMessage = (args as ExceptionRoutedEventArgs).ErrorException.ToString();
                          _requestdoneob.OnNext(req);
                          return;
                        }

                        if (args is RoutedEventArgs)
                        {
                          req.Succeeded = true;
                          req.BmpWidth = req.Bmp.PixelWidth;
                          req.BmpHeight = req.Bmp.PixelHeight;
                          _requestdoneob.OnNext(req);
                          return;
                        }
                      }
                    );

      req.Bmp = bmp;
      bmp.SetSource(req.Stream);
    }

    static void NotifyJobDone(BitmapRequest req)
    {
      if (req.Handle.IsDisposed
        || req.Callback == null)
      {
        req.CleanUp();
        return;
      }

      req.Callback(req.CreateLoadingResult());
      req.CleanUp();
    }
  }
}