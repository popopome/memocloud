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

namespace MemoPad
{
  public class ViewModelLocator
  {
    public static TextEditorViewModel TextEditorVm;
    public static MemoListPageViewModel MemoListPageVm;
    public static WorkspaceListPageViewModel WorkspaceListPageVm;
    public static PhotoViewerPageViewModel PhotoViewerPageVm;

    static public void Initialize()
    {
      WorkspaceListPageVm = new WorkspaceListPageViewModel();
      TextEditorVm = new TextEditorViewModel();
      MemoListPageVm = new MemoListPageViewModel();
      PhotoViewerPageVm = new PhotoViewerPageViewModel();
    }

  }
}