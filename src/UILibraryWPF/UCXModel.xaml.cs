using Com.DRS.Helper;
using Com.DRS.ObjLogic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Com.DRS.UILibraryWPF
{
    /// <summary>
    /// Interaction logic for XModel.xaml
    /// </summary>
    public partial class UCXModel : UserControl
    {
        public WDrawingCanvas aModelCanvas = null;

        public UCXModel()
        {
            InitializeComponent();
            aModelCanvas = ModelCanvas;
        }

        //--------------------------------------------------
        public static UCXModel Create(XWindow tWindow)
        {
            UCXModel ctrl = new UCXModel();
            ctrl.Name = "CTRL_" + tWindow.Id;
            ctrl.HorizontalAlignment = GConstants.GetHorizontalAlignment(tWindow.HorizontalAlignment);
            ctrl.VerticalAlignment = GConstants.GetVerticalAlignment(tWindow.VerticalAlignment);
            ctrl.Margin = new Thickness(tWindow.PosBX, tWindow.PosBY, tWindow.PosBY, tWindow.PosEY);
            ctrl.Foreground = (WPen.GetPen(tWindow.aWpfPen)).Brush;
            ctrl.Background = WBrush.GetBrush(tWindow.aWpfBrush);
            ctrl.Content = tWindow.textStr;
            ctrl.Tag = tWindow;
            WinEventAction.HookEventAction(ctrl, tWindow.aMsgActList);
            return ctrl;
        }

    }
}
