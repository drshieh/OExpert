//--KEEP:
using OExpert.UILibraryWPF;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;

namespace OExpert
{
    /// <summary>
    /// Interaction logic for DrawArea.xaml
    /// </summary>
    public partial class UCDrawArea : UserControl
    {
        public WSettings aSetting = new WSettings();

        public XRuleNet aXRuleNetBackTemp = null;

        //--------------------------------------------------
        public UCDrawArea()
        {
            InitializeComponent();
            CustomTooltip.IsOpen = false;
        }

        //--------------------------------------------------
        public UCDrawArea(XRuleNet _ruleNet)
        {
            InitializeComponent();
            //           aRuleNet = _ruleNet;
            CustomTooltip.IsOpen = false;
        }
        //--------------------------------------------------
        public UCDrawArea(bool fDisableEvents)
        {
            InitializeComponent();
            if (fDisableEvents) DisableMouseEvents();
            CustomTooltip.IsOpen = false;
        }

        //--------------------------------------------------
        public void DisableMouseEvents()
        {
            this.MouseLeftButtonDown -= DACanvas_MouseLeftButtonDown;
            this.MouseLeftButtonUp -= DACanvas_MouseLeftButtonUp;
            this.MouseMove -= DACanvas_MouseMove;
            this.MouseRightButtonDown -= DACanvas_MouseRightButtonDown;
            this.MouseRightButtonUp -= DACanvas_MouseRightButtonUp;
        }

        //----------------------------------------------------------------------------------
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (MainWindow.aXRuleNet != null)
            {
                WDrawRule.DrawRuleNet(DACanvas, MainWindow.aXRuleNet, MainWindow.aXRuleNet.aSelectedObjectList,
                                      aSetting.zoomFactorX, aSetting.zoomFactorY, GConstants.WIN_NORMAL);
            }
        }

        //----------------------------------------------------------------------------------
        public void UpdateControl(object aHead)
        {
            Console.WriteLine("UpdateControl: UCDrawArea..");
            if (aSetting == null) return;

            WDrawRule.DrawRuleNet(DACanvas, MainWindow.aXRuleNet, MainWindow.aXRuleNet.aSelectedObjectList,
                                  aSetting.zoomFactorX, aSetting.zoomFactorY, GConstants.WIN_NORMAL);
        }

        private Point prevCursorPosP = new Point(0, 0);  //--Keep the previous cursor position, for double-click
        //----------------------------------------------------------------------------------
        private void DACanvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Console.WriteLine("DACanvas_MouseLeftButtonDown");
            aSetting.iLeftButtonDown = true;
            aSetting.iRightButtonDown = false;
            aSetting.prevCursorPos = e.GetPosition(DACanvas);
            bool fDoubleClick = false;
            if (aSetting.prevCursorPos.X == prevCursorPosP.X &&
                aSetting.prevCursorPos.Y == prevCursorPosP.Y) fDoubleClick = true;  //--Same position (like double click)
            prevCursorPosP.X = aSetting.prevCursorPos.X;
            prevCursorPosP.Y = aSetting.prevCursorPos.Y;
            #region - Ctrl key
            if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
            {
                List<object> aSelObjectList = new List<object>();
                int iSelPos = WRule_Locat.oLocatRule(aSetting.prevCursorPos.X, aSetting.prevCursorPos.Y,
                                   aSelObjectList, MainWindow.aXRuleNet);
                if (aSelObjectList.Count > 0)
                {  //--Toggle selection of the object
                    object aCmp = aSelObjectList[0];
                    if (MainWindow.aXRuleNet.aSelectedObjectList == null) MainWindow.aXRuleNet.aSelectedObjectList = new List<object>();
                    if (MainWindow.aXRuleNet.aSelectedObjectList.Contains(aCmp)) MainWindow.aXRuleNet.aSelectedObjectList.Remove(aCmp);
                    else
                    {
                        MainWindow.aXRuleNet.aSelectedObjectList.Add(aCmp);
                    }
                    WDrawRule.DrawRuleNet(DACanvas, MainWindow.aXRuleNet, MainWindow.aXRuleNet.aSelectedObjectList,
                                          aSetting.zoomFactorX, aSetting.zoomFactorY, GConstants.WIN_NORMAL);
                    return;
                }
            }
            #endregion

            #region - in selected
            if (MainWindow.aXRuleNet.aSelectedObjectList != null && MainWindow.aXRuleNet.aSelectedObjectList.Count > 0)
            {
                object aCmp = null;
                //??          Point cursorPosX = new Point(prevCursorPosP.X / aSetting.zoomFactorX, prevCursorPosP.Y / aSetting.zoomFactorY);
                Point cursorPosX = new Point(prevCursorPosP.X, prevCursorPosP.Y);
                aSetting.iSelPos = WRule_Locat.oLocatRuleInSelected(cursorPosX, MainWindow.aXRuleNet.aSelectedObjectList, ref aCmp);
                if (fDoubleClick && aCmp != null)
                {
                    aSetting.iLeftButtonDown = false;
                    MainWindow.tp.ucRuleNetwork.ShowRuleItemInfo(aCmp);
                    return;
                }
                if (aSetting.iSelPos == 10)  //--Move
                {
                    aXRuleNetBackTemp = XRuleNet.Copy(MainWindow.aXRuleNet);
                    aSetting.iOperMode = OperMode.Move;
                    ShowItemInfo(false, aCmp);
                    return;
                }
                else if (aSetting.iSelPos >= 0 && aSetting.iSelPos <= 7) //-Resize
                {
                    aXRuleNetBackTemp = XRuleNet.Copy(MainWindow.aXRuleNet);
                    aSetting.iOperMode = OperMode.Resize;
                    ShowItemInfo(false, aCmp);
                    return;
                }
            }
            #endregion

            if (MainWindow.aXRuleNet.aPreSelectedObjectList != null) MainWindow.aXRuleNet.aPreSelectedObjectList.Clear();
            else MainWindow.aXRuleNet.aPreSelectedObjectList = new List<object>();
            if (MainWindow.aXRuleNet.aSelectedObjectList != null)
            {
                foreach (object a in MainWindow.aXRuleNet.aSelectedObjectList)
                {
                    MainWindow.aXRuleNet.aPreSelectedObjectList.Add(a);
                }
            }
            aSetting.iSelPos = WRule_Locat.oLocatRule(aSetting.prevCursorPos.X, aSetting.prevCursorPos.Y,
                                 MainWindow.aXRuleNet.aSelectedObjectList, MainWindow.aXRuleNet);
            if (aSetting.iSelPos >= 0)
            {
                aXRuleNetBackTemp = XRuleNet.Copy(MainWindow.aXRuleNet);
                ShowItemInfo(false, MainWindow.aXRuleNet.aSelectedObjectList[0]);

                if (aSetting.iSelPos >= 0 && aSetting.iSelPos <= 7) aSetting.iOperMode = OperMode.Resize;
                else aSetting.iOperMode = OperMode.Move;

                aSetting.fMultiSelect = false;
                WDrawRule.DrawRuleNet(DACanvas, MainWindow.aXRuleNet, MainWindow.aXRuleNet.aSelectedObjectList,
                                      aSetting.zoomFactorX, aSetting.zoomFactorY, GConstants.WIN_NORMAL);
                if (fDoubleClick)
                {
                    aSetting.iLeftButtonDown = false;
                    MainWindow.tp.ucRuleNetwork.ShowRuleItemInfo(MainWindow.aXRuleNet.aSelectedObjectList[0]);
                }
            }
            else
            {
                aSetting.fMultiSelect = true;
                if (aSetting.aPointList == null) aSetting.aPointList = new List<Point>();
                aSetting.aPointList.Add(aSetting.prevCursorPos);
                aSetting.aPointList.Add(aSetting.prevCursorPos);
                aSetting.aWorkVisual = new DrawingVisual();
                DACanvas.AddVisual(aSetting.aWorkVisual);

                Pen pen = new Pen(Brushes.Black, 3);
                using (DrawingContext dc = aSetting.aWorkVisual.RenderOpen())
                {
                    WDrawingVisual.WDrawRectangle(dc, Brushes.Transparent, pen, aSetting.aPointList[0].X, aSetting.aPointList[0].Y, aSetting.aPointList[1].X, aSetting.aPointList[1].Y);
                }
            }
        }

        //-----------------------------------------------------------------------
        private void DACanvas_MouseMove(object sender, MouseEventArgs e)
        {
            //            Console.WriteLine("DACanvas_MouseMove");
            if (!aSetting.iLeftButtonDown && !aSetting.iRightButtonDown)  //--Show Tooltip
            {
                if (MainWindow.tp.fShowToolTip)
                {
                    Point cp = e.GetPosition(DACanvas);
                    object o = WRule_Locat.oLocatRuleX(cp.X, cp.Y, MainWindow.aXRuleNet);
                    if (o != null)
                    {
                        CustomTooltip.HorizontalOffset = cp.X + 15;
                        CustomTooltip.VerticalOffset = cp.Y + 15;
       //                 tbTip.Text = XRule_UI.GetItemStr(o);
                        XRule_UI.SetItemTip(o, tbTipHead, tbTip, tbTipRulesHead1, tbTipRules1, tbTipRulesHead2, tbTipRules2);
                        CustomTooltip.IsOpen = true;
                    }
                    else
                    {
                        CustomTooltip.IsOpen = false;
                    }
                }
                return;
            }

            //            aSetting.iOperMode = OperMode.Move;

            aSetting.cursorPos = e.GetPosition(DACanvas);
            if (aSetting.cursorPos.X < 0) aSetting.cursorPos.X = 0;
            if (aSetting.cursorPos.Y < 0) aSetting.cursorPos.Y = 0;

            double dx = (aSetting.cursorPos.X - aSetting.prevCursorPos.X) / aSetting.zoomFactorX;
            double dy = (aSetting.cursorPos.Y - aSetting.prevCursorPos.Y) / aSetting.zoomFactorY;

            if (dx == 0 && dy == 0) return;

            if (aSetting.iLeftButtonDown)
            {
                if (aSetting.iOperMode == OperMode.Move || aSetting.iOperMode == OperMode.Resize)
                {
                    if (MainWindow.aXRuleNet.aSelectedObjectList != null && MainWindow.aXRuleNet.aSelectedObjectList.Count > 0)
                    {
                        WRule_Locat.RuleMoveResize(DACanvas, MainWindow.aXRuleNet, dx, dy, aSetting);
                    }
                }
                else if (aSetting.fMultiSelect)
                {
                    using (DrawingContext dc = aSetting.aWorkVisual.RenderOpen())
                    {
                        aSetting.aPointList[1] = new Point(aSetting.cursorPos.X, aSetting.cursorPos.Y);
                        var pen = new Pen(new SolidColorBrush(Color.FromRgb(200, 10, 20)), 2);
                        WDrawingVisual.WDrawRectangle(dc, Brushes.Transparent, pen,
                                aSetting.aPointList[0].X, aSetting.aPointList[0].Y,
                                aSetting.aPointList[1].X, aSetting.aPointList[1].Y);
                    }
                }
            }
            else if (aSetting.iRightButtonDown)
            {
                if (aSetting.fMultiSelect)
                {
                    using (DrawingContext dc = aSetting.aWorkVisual.RenderOpen())
                    {
                        aSetting.aPointList[1] = new Point(aSetting.cursorPos.X, aSetting.cursorPos.Y);
                        var pen = new Pen(new SolidColorBrush(Color.FromRgb(200, 10, 20)), 2);
                        WDrawingVisual.WDrawRectangle(dc, Brushes.Transparent, pen,
                                aSetting.aPointList[0].X, aSetting.aPointList[0].Y,
                                aSetting.aPointList[1].X, aSetting.aPointList[1].Y);
                    }
                }
            }
        }

        //-----------------------------------------------------------------
        private void DACanvas_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Console.WriteLine("DACanvas_MouseLeftButtonUp");
            if (aSetting.iOperMode == OperMode.Resize || aSetting.iOperMode == OperMode.Move)
            {
                MainWindow.aXRuleNetBack = aXRuleNetBackTemp;
            }

            aSetting.iOperMode = OperMode.None;
            if (aSetting.fMultiSelect)
            {
                if (MainWindow.aXRuleNet.aSelectedObjectList != null) MainWindow.aXRuleNet.aSelectedObjectList.Clear();
                WRule_Locat.oLocatRuleInRect(aSetting.aPointList[0].X, aSetting.aPointList[0].Y,
                                                aSetting.aPointList[1].X, aSetting.aPointList[1].Y,
                                                MainWindow.aXRuleNet.aSelectedObjectList,
                                                MainWindow.aXRuleNet);
                if (aSetting.aPointList != null) aSetting.aPointList.Clear();
                aSetting.fMultiSelect = false;
                if (aSetting.aWorkVisual != null)
                {
                    DACanvas.DeleteVisual(aSetting.aWorkVisual);
                    aSetting.aWorkVisual = null;
                }
            }

            aSetting.iLeftButtonDown = false;
            aSetting.iRightButtonDown = false;
            WDrawRule.DrawRuleNet(DACanvas, MainWindow.aXRuleNet, MainWindow.aXRuleNet.aSelectedObjectList,
                                  aSetting.zoomFactorX, aSetting.zoomFactorY, GConstants.WIN_NORMAL);
        }

        //-----------------------------------------------------------------
        private void DACanvas_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            Console.WriteLine("DACanvas_MouseRightButtonDown");
            aSetting.iLeftButtonDown = false;
            aSetting.iRightButtonDown = true;

            aSetting.prevCursorPos = e.GetPosition(DACanvas);
            List<object> aSelObjectList = new List<object>();
            int iSelPos = WRule_Locat.oLocatRule(aSetting.prevCursorPos.X, aSetting.prevCursorPos.Y, aSelObjectList, MainWindow.aXRuleNet);
            if (aSelObjectList.Count > 0)
            {
                ShowItemInfo(true, aSelObjectList[0]);
                WDrawRule.DrawRuleNet(DACanvas, MainWindow.aXRuleNet, MainWindow.aXRuleNet.aSelectedObjectList,
                                      aSetting.zoomFactorX, aSetting.zoomFactorY, GConstants.WIN_NORMAL);
            }
            else
            {
                aSetting.fMultiSelect = true;
                if (aSetting.aPointList == null) aSetting.aPointList = new List<Point>();
                aSetting.aPointList.Add(aSetting.prevCursorPos);
                aSetting.aPointList.Add(aSetting.prevCursorPos);
                aSetting.aWorkVisual = new DrawingVisual();
                DACanvas.AddVisual(aSetting.aWorkVisual);

                Pen pen = new Pen(Brushes.Black, 3);
                using (DrawingContext dc = aSetting.aWorkVisual.RenderOpen())
                {
                    WDrawingVisual.WDrawRectangle(dc, Brushes.Transparent, pen, aSetting.aPointList[0].X, aSetting.aPointList[0].Y, aSetting.aPointList[1].X, aSetting.aPointList[1].Y);
                }
            }
        }

        //-----------------------------------------------------------------
        public void ShowItemInfo(bool fCreate, object o)
        {
            if (MainWindow.tp.wItemInfoDlg == null)
            {
                if (fCreate)
                {
                    MainWindow.tp.wItemInfoDlg = new ItemInfoDlg(o);
                    MainWindow.tp.wItemInfoDlg.Owner = MainWindow.tp;
                    MainWindow.tp.wItemInfoDlg.Show();
                }
            }
            else
            {
                MainWindow.tp.wItemInfoDlg.SetItem(o);
                MainWindow.tp.wItemInfoDlg.Activate();
            }
        }

        //-----------------------------------------------------------------
        private void DACanvas_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            Console.WriteLine("DACanvas_MouseRightButtonUp");
            if (aSetting.iRightButtonDown && aSetting.fMultiSelect)
            {
                List<object> aSelList = new List<object>();
                WRule_Locat.oLocatRuleInRect(aSetting.aPointList[0].X, aSetting.aPointList[0].Y,
                                                aSetting.aPointList[1].X, aSetting.aPointList[1].Y,
                                                aSelList,
                                                MainWindow.aXRuleNet);
                if (MainWindow.aXRuleNet.aSelectedObjectList == null) MainWindow.aXRuleNet.aSelectedObjectList = aSelList;
                else
                {
                    foreach(object a in aSelList)
                    {
                        if (!MainWindow.aXRuleNet.aSelectedObjectList.Contains(a)) MainWindow.aXRuleNet.aSelectedObjectList.Add(a);
                        else MainWindow.aXRuleNet.aSelectedObjectList.Remove(a);
                    }
                }
                if (aSetting.aPointList != null) aSetting.aPointList.Clear();

                aSetting.fMultiSelect = false;
                if (aSetting.aWorkVisual != null)
                {
                    DACanvas.DeleteVisual(aSetting.aWorkVisual);
                    aSetting.aWorkVisual = null;
                }
            }

            aSetting.iOperMode = OperMode.None;
            aSetting.iLeftButtonDown = false;
            aSetting.iRightButtonDown = false;
            WDrawRule.DrawRuleNet(DACanvas, MainWindow.aXRuleNet, MainWindow.aXRuleNet.aSelectedObjectList,
                                  aSetting.zoomFactorX, aSetting.zoomFactorY, GConstants.WIN_NORMAL);
        }

        //-----------------------------------------------------------------
        private void DACanvas_KeyUp(object sender, KeyEventArgs e)
        {
            Console.WriteLine("DACanvas_KeyUp");
            aSetting.iOperMode = OperMode.None;
            aSetting.iLeftButtonDown = false;
            aSetting.iRightButtonDown = false;
            //         aWCanvasMAction.KeyUp(this.DACanvas, this.aSetting, e);
        }

        private void DAScrollViewer_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            Console.WriteLine("DAScrollViewer_SizeChanged");
            XRuleNet.SetCanvasSize(DACanvas, MainWindow.aXRuleNet, aSetting.zoomFactorX, aSetting.zoomFactorY);
        }

        private void DACanvas_MouseLeave(object sender, MouseEventArgs e)
        {
            if (CustomTooltip.IsOpen) CustomTooltip.IsOpen = false;
        }
    }
}
