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
    /// Interaction logic for UCModel.xaml
    /// </summary>
    public partial class UCModel : UserControl
    {
        public WDrawingCanvas aModelCanvas = null;

        public UCModel()
        {
            InitializeComponent();
            aModelCanvas = this.ModelCanvas;
        }

        //--------------------------------------------------
        public static UCModel Create(XWindow tWindow)
        {
            UCModel ctrl = new UCModel();
            ctrl.Name = "CTRL_" + tWindow.Id;
            ctrl.HorizontalAlignment = GConstants.GetHorizontalAlignment(tWindow.HorizontalAlignment);
            ctrl.VerticalAlignment = GConstants.GetVerticalAlignment(tWindow.VerticalAlignment);
            ctrl.Margin = new Thickness(tWindow.PosBX, tWindow.PosBY, tWindow.PosBY, tWindow.PosEY);
            ctrl.Foreground = (WPen.GetPen(tWindow.aWpfPen)).Brush;
            ctrl.Background = WBrush.GetBrush(tWindow.aWpfBrush);
            ctrl.Tag = tWindow;
            WinEventAction.HookEventAction(ctrl, tWindow.aMsgActList);
            return ctrl;
        }


        //----------------------------------------------------------------------------------
        private void ModelCanvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            fMultiSelect = false;
            iLeftButtonDown = true;
            iRightButtonDown = false;
            prevCursorPos = e.GetPosition(ModelCanvas);
            Point prevCursorPosX = new Point(prevCursorPos.X / zoomFactor, prevCursorPos.Y / zoomFactor);
            labStatus.Text = "Zoom:" + zoomFactor + " X:" + prevCursorPos.X + " Y:" + prevCursorPos.Y;

            #region - Mouse down - Gph_None
            if (iGphElmtType == GConstants.GPH_NONE)
            {
                #region - double-click
                if (e.ClickCount == 2)
                {
                    iOperMode = OperMode.None;
                    ShowCmpData(aVMMdlHead, aSelectedObjectList);
                    iLeftButtonDown = false;
                    return;
                }
                #endregion

                #region - Ctrl key
                if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
                {
                    VMMdlCmp aCmp = null;
                    if (Model_Locate.GetMdlCmpByCursor(prevCursorPosX, aVMMdlHead, ref aCmp, ref aRubPointList) >= 0)
                    {
                        aRubPointList = WCoord.oResizePoints(aRubPointList, zoomFactor, zoomFactor);
                        if (aSelectedObjectList == null) aSelectedObjectList = new List<object>();
                        if (aCmp != null && aSelectedObjectList.Contains(aCmp)) aSelectedObjectList.Remove(aCmp);
                        else
                        {
                            if (aVMMdlCmp != null && aVMMdlCmp != aCmp && !aSelectedObjectList.Contains(aVMMdlCmp)) aSelectedObjectList.Add(aVMMdlCmp);
                            if (aCmp != null && !aSelectedObjectList.Contains(aCmp)) aSelectedObjectList.Add(aCmp);
                            aVMMdlCmp = null;
                            aVMGphElmt = null;
                        }
                        WDrawModel.DrawModel(ModelCanvas, aVMMdlHead, aVMMdlCmp, aSelectedObjectList, zoomFactor, zoomFactor);
                        return;
                    }
                }
                #endregion
                #region - with selected
                if (aSelectedObjectList != null && aSelectedObjectList.Count > 0)
                {
                    aVMMdlCmp = null;
                    aVMGphElmt = null;
                    aRubPointList = null;
                    Point cursorPosX = new Point(cursorPos.X / zoomFactor, cursorPos.Y / zoomFactor);
                    iSelPos = Model_Locate.GetMdlCmpInSelectedByCursor(cursorPosX, aSelectedObjectList, ref aVMMdlCmp, ref aRubPointList);
                    aRubPointList = WCoord.oResizePoints(aRubPointList, zoomFactor, zoomFactor);
                    if (iSelPos == 10)  //--Move
                    {
                        Model_Back.Add(aModelBackList, aVMMdlHead, aSelectedObjectList);
                        iOperMode = OperMode.Move;
                        //                        aVMGphElmt = null;
                        //                        aVMModelPort = null;
                    }
                    else if (iSelPos >= 0 && iSelPos <= 7) //-Resize
                    {
                        Model_Back.Add(aModelBackList, aVMMdlHead, aSelectedObjectList);
                        iOperMode = OperMode.Resize;
                    }
                    else if (iSelPos >= 100)  //--Selected Point, update Point
                    {
                        Model_Back.Add(aModelBackList, aVMMdlHead, aSelectedObjectList);
                        LogFile.WriteLine("Select SELECTEDLIST: iSelPOs: " + iSelPos);
                        aVMGphElmt = null;
                        iOperMode = OperMode.Individual;
                        aWorkVisual = new DrawingVisual();
                        ModelCanvas.AddVisual(aWorkVisual);
                    }
                    else  //--none selected
                    {
                        aSelectedObjectList = null;
                        aVMMdlCmp = null;
                        aVMGphElmt = null;

                        fMultiSelect = true;
                        if (aPointList == null) aPointList = new List<Point>();
                        aPointList.Add(prevCursorPos);
                        aPointList.Add(prevCursorPos);
                        aWorkVisual = new DrawingVisual();
                        ModelCanvas.AddVisual(aWorkVisual);

                        Pen pen = new Pen(Brushes.Black, 3);
                        using (DrawingContext dc = aWorkVisual.RenderOpen())
                        {
                            WDrawingVisual.WDrawRectangle(dc, Brushes.Transparent, pen, aPointList[0].X, aPointList[0].Y, aPointList[1].X, aPointList[1].Y);
                        }
                    }
                    return;
                }
                #endregion

                aSelectedObjectList = null;
                aVMMdlCmp = null;
                aVMGphElmt = null;

                iSelPos = Model_Locate.GetMdlCmpByCursor(prevCursorPosX, aVMMdlHead, ref aVMMdlCmp, ref aRubPointList);
                aRubPointList = WCoord.oResizePoints(aRubPointList, zoomFactor, zoomFactor);

                if (iSelPos < 0)
                {
                    aSelectedObjectList = null;
                    aVMMdlCmp = null;
                    aVMGphElmt = null;

                    fMultiSelect = true;
                    if (aPointList == null) aPointList = new List<Point>();
                    aPointList.Add(prevCursorPos);
                    aPointList.Add(prevCursorPos);
                    aWorkVisual = new DrawingVisual();
                    ModelCanvas.AddVisual(aWorkVisual);

                    Pen pen = new Pen(Brushes.Black, 3);
                    using (DrawingContext dc = aWorkVisual.RenderOpen())
                    {
                        WDrawingVisual.WDrawRectangle(dc, Brushes.Transparent, pen, aPointList[0].X, aPointList[0].Y, aPointList[1].X, aPointList[1].Y);
                    }
                }
                else
                {
                    aSelectedObjectList = new List<object>();
                    if (aVMMdlCmp != null) aSelectedObjectList.Add(aVMMdlCmp);
                    if (aVMGphElmt != null) aSelectedObjectList.Add(aVMGphElmt);

                    if (iSelPos == 10)  //--Move
                    {
                        Model_Back.Add(aModelBackList, aVMMdlHead, aSelectedObjectList);
                        iOperMode = OperMode.Move;
                    }
                    else if (iSelPos >= 0 && iSelPos <= 7) //-Resize
                    {
                        Model_Back.Add(aModelBackList, aVMMdlHead, aSelectedObjectList);
                        iOperMode = OperMode.Resize;
                    }
                    else if (iSelPos >= 100)  //--Selected Point, update Point
                    {
                        Model_Back.Add(aModelBackList, aVMMdlHead, aSelectedObjectList);
                        aVMGphElmt = null;
                        iOperMode = OperMode.Individual;
                        aWorkVisual = new DrawingVisual();
                        ModelCanvas.AddVisual(aWorkVisual);
                    }

                    WDrawModel.DrawModel(ModelCanvas, aVMMdlHead, null, aSelectedObjectList, zoomFactor, zoomFactor);
                }
            }
            #endregion

            #region -- Mouse down - GElmt
            if (iGphElmtType == GConstants.GPH_LINE)
            {
                if (aPointList == null) aPointList = new List<Point>();
                aPointList.Add(prevCursorPos);
                aPointList.Add(prevCursorPos);
                Pen pen = new Pen(Brushes.Black, 3);
                using (DrawingContext dc = aWorkVisual.RenderOpen())
                {
                    WDrawingVisual.WDrawLine(dc, pen, aPointList[0].X, aPointList[0].Y, aPointList[1].X, aPointList[1].Y);
                }
            }
            else if (iGphElmtType == GConstants.GPH_TEXT)
            {
                if (aPointList == null) aPointList = new List<Point>();
                aPointList.Add(prevCursorPos);
                aPointList.Add(new Point(prevCursorPos.X + 80, prevCursorPos.Y + 30));
                Brush brush = Brushes.Black;
                string sText = "Test TEXT";
                TextInput dlg = new ObjLogic.TextInput(sText);
                if (dlg.ShowDialog() == true)
                {
                    SaveGElmtText(dlg.sText, aPointList);
                    iGphElmtType = GConstants.GPH_NONE;
                    SetGphElmtPalleteSelected(iGphElmtType);
                }
            }
            else if (iGphElmtType == GConstants.GPH_RECTANGLE)
            {
                if (aPointList == null) aPointList = new List<Point>();
                aPointList.Add(prevCursorPos);
                aPointList.Add(prevCursorPos);
                Pen pen = new Pen(Brushes.Black, 3);
                using (DrawingContext dc = aWorkVisual.RenderOpen())
                {
                    Brush brush = Brushes.Wheat;
                    WDrawingVisual.WDrawRectangle(dc, brush, pen, aPointList[0].X, aPointList[0].Y, aPointList[1].X, aPointList[1].Y);
                }
            }
            else if (iGphElmtType == GConstants.GPH_ROUNDRECT)
            {
                if (aPointList == null) aPointList = new List<Point>();
                aPointList.Add(prevCursorPos);
                aPointList.Add(prevCursorPos);
                Pen pen = new Pen(Brushes.Black, 3);
                using (DrawingContext dc = aWorkVisual.RenderOpen())
                {
                    Brush brush = Brushes.Wheat;
                    WDrawingVisual.WDrawRoundedRectangle(dc, brush, pen, aPointList[0].X, aPointList[0].Y, aPointList[1].X, aPointList[1].Y, 0, 0);
                }
            }
            else if (iGphElmtType == GConstants.GPH_ARC)
            {
                if (aPointList == null)
                {
                    aPointList = new List<Point>();
                    aPointList.Add(prevCursorPos);
                    aPointList.Add(prevCursorPos);
                }
                else aPointList.Add(prevCursorPos);
                Pen pen = new Pen(Brushes.Black, 3);
                using (DrawingContext dc = aWorkVisual.RenderOpen())
                {
                    Brush brush = Brushes.Transparent;
                    double CX = (aPointList[0].X + aPointList[1].X) / 2;
                    double CY = (aPointList[0].Y + aPointList[1].Y) / 2;

                    WDrawingVisual.WDrawEllipse(dc, brush, pen, aPointList[0].X, aPointList[0].Y, aPointList[1].X, aPointList[1].Y);
                    if (aPointList.Count > 2) WDrawingVisual.WDrawLine(dc, pen, CX, CY, aPointList[2].X, aPointList[2].Y);
                    if (aPointList.Count > 3) WDrawingVisual.WDrawLine(dc, pen, CX, CY, aPointList[3].X, aPointList[3].Y);
                }
            }
            else if (iGphElmtType == GConstants.GPH_CHORD)
            {
                if (aPointList == null)
                {
                    aPointList = new List<Point>();
                    aPointList.Add(prevCursorPos);
                    aPointList.Add(prevCursorPos);
                }
                else aPointList.Add(prevCursorPos);
                Pen pen = new Pen(Brushes.Black, 3);
                using (DrawingContext dc = aWorkVisual.RenderOpen())
                {
                    Brush brush = Brushes.Wheat;
                    WDrawingVisual.WDrawEllipse(dc, brush, pen, aPointList[0].X, aPointList[0].Y, aPointList[1].X, aPointList[1].Y);
                    double CX = (aPointList[0].X + aPointList[1].X) / 2;
                    double CY = (aPointList[0].Y + aPointList[1].Y) / 2;

                    WDrawingVisual.WDrawEllipse(dc, brush, pen, aPointList[0].X, aPointList[0].Y, aPointList[1].X, aPointList[1].Y);
                    if (aPointList.Count > 2) WDrawingVisual.WDrawLine(dc, pen, CX, CY, aPointList[2].X, aPointList[2].Y);
                    if (aPointList.Count > 3) WDrawingVisual.WDrawLine(dc, pen, CX, CY, aPointList[3].X, aPointList[3].Y);
                }
            }
            else if (iGphElmtType == GConstants.GPH_PIE)
            {
                if (aPointList == null)
                {
                    aPointList = new List<Point>();
                    aPointList.Add(prevCursorPos);
                    aPointList.Add(prevCursorPos);
                }
                else aPointList.Add(prevCursorPos);
                Pen pen = new Pen(Brushes.Black, 3);
                using (DrawingContext dc = aWorkVisual.RenderOpen())
                {
                    Brush brush = Brushes.Wheat;
                    WDrawingVisual.WDrawEllipse(dc, brush, pen, aPointList[0].X, aPointList[0].Y, aPointList[1].X, aPointList[1].Y);
                    double CX = (aPointList[0].X + aPointList[1].X) / 2;
                    double CY = (aPointList[0].Y + aPointList[1].Y) / 2;

                    WDrawingVisual.WDrawEllipse(dc, brush, pen, aPointList[0].X, aPointList[0].Y, aPointList[1].X, aPointList[1].Y);
                    if (aPointList.Count > 2) WDrawingVisual.WDrawLine(dc, pen, CX, CY, aPointList[2].X, aPointList[2].Y);
                    if (aPointList.Count > 3) WDrawingVisual.WDrawLine(dc, pen, CX, CY, aPointList[3].X, aPointList[3].Y);
                }
            }
            else if (iGphElmtType == GConstants.GPH_BITMAP)
            {
                if (aPointList == null) aPointList = new List<Point>();
                aPointList.Add(prevCursorPos);
                aPointList.Add(prevCursorPos);
                Pen pen = new Pen(Brushes.Red, 3);
                using (DrawingContext dc = aWorkVisual.RenderOpen())
                {
                    Brush brush = Brushes.Transparent;
                    WDrawingVisual.WDrawRectangle(dc, brush, pen, aPointList[0].X, aPointList[0].Y, aPointList[1].X, aPointList[1].Y);
                }
            }
            else if (iGphElmtType == GConstants.GPH_ELLIPSE)
            {
                if (aPointList == null) aPointList = new List<Point>();
                aPointList.Add(prevCursorPos);
                aPointList.Add(prevCursorPos);
                Pen pen = new Pen(Brushes.Black, 3);
                using (DrawingContext dc = aWorkVisual.RenderOpen())
                {
                    Brush brush = Brushes.Wheat;
                    WDrawingVisual.WDrawEllipse(dc, brush, pen, aPointList[0].X, aPointList[0].Y, aPointList[1].X, aPointList[1].Y);
                }
            }
            else if (iGphElmtType == GConstants.GPH_POLYLINE || iGphElmtType == GConstants.GPH_POLYGON)
            {
                if (aPointList == null)
                {
                    aPointList = new List<Point>();
                    aPointList.Add(prevCursorPos);
                    aPointList.Add(prevCursorPos);
                }
                else
                {
                    aPointList.Add(prevCursorPos);
                }
                Pen pen = new Pen(Brushes.Black, 3);
                using (DrawingContext dc = aWorkVisual.RenderOpen())
                {
                    if (iGphElmtType == GConstants.GPH_POLYLINE)
                    {
                        Brush brush = Brushes.Transparent;
                        WDrawingVisual.WDrawPolyLine(dc, brush, pen, aPointList);
                    }
                    else if (iGphElmtType == GConstants.GPH_POLYGON)
                    {
                        Brush brush = Brushes.Wheat;
                        WDrawingVisual.WDrawPolygon(dc, brush, pen, aPointList);
                    }
                }
                if (e.ClickCount == 2) //--double click
                {
                    //SaveGElmtPoly();
                    SaveMdlCmp(aPointList, true);
                    iGphElmtType = GConstants.GPH_NONE;
                    SetGphElmtPalleteSelected(iGphElmtType);
                }
            }
            else if (iGphElmtType == GConstants.GPH_BEZIERCURVE)
            {
                if (aPointList == null)
                {
                    aPointList = new List<Point>();
                    aPointList.Add(prevCursorPos);
                    aPointList.Add(prevCursorPos);
                }
                else
                {
                    aPointList.Add(prevCursorPos);
                }
                Pen pen = new Pen(Brushes.Black, 3);
                using (DrawingContext dc = aWorkVisual.RenderOpen())
                {
                    Brush brush = Brushes.Transparent;
                    //                    WDrawingVisual.WDrawBezierCurve(dc, brush, pen, aPointList);
                    WDrawingVisual.WDrawPolyLine(dc, brush, pen, aPointList);
                }
                if (e.ClickCount == 2) //--double click
                {
                    //SaveGElmtPoly();
                    SaveMdlCmp(aPointList, true);
                    iGphElmtType = GConstants.GPH_NONE;
                    SetGphElmtPalleteSelected(iGphElmtType);
                }
            }
            else if (iGphElmtType == GConstants.GPH_QUADBEZIERCURVE)
            {
                if (aPointList == null)
                {
                    aPointList = new List<Point>();
                    aPointList.Add(prevCursorPos);
                    aPointList.Add(prevCursorPos);
                }
                else
                {
                    aPointList.Add(prevCursorPos);
                }
                Pen pen = new Pen(Brushes.Black, 3);
                using (DrawingContext dc = aWorkVisual.RenderOpen())
                {
                    Brush brush = Brushes.Transparent;
                    //                    WDrawingVisual.WDrawQuadraticBezierCurve(dc, brush, pen, aPointList);
                    WDrawingVisual.WDrawPolyLine(dc, brush, pen, aPointList);
                }
                if (e.ClickCount == 2) //--double click
                {
                    //SaveGElmtPoly();
                    SaveMdlCmp(aPointList, true);
                    iGphElmtType = GConstants.GPH_NONE;
                    SetGphElmtPalleteSelected(iGphElmtType);
                }
            }
            else if (iGphElmtType == GConstants.GPH_MDLCONN)
            {
                if (aPointList == null)
                {
                    aPointList = new List<Point>();
                    aPointList.Add(prevCursorPos);
                    aPointList.Add(prevCursorPos);
                }
                else
                {
                    aPointList.Add(prevCursorPos);
                }
                Pen pen = new Pen(Brushes.Black, 3);
                using (DrawingContext dc = aWorkVisual.RenderOpen())
                {
                    Brush brush = Brushes.Transparent;
                    WDrawingVisual.WDrawPolyLine(dc, brush, pen, aPointList);
                }
                if (e.ClickCount == 2) //--double click
                {
                    //SaveGElmtPoly();
                    SaveMdlCmp(aPointList, true);
                    iGphElmtType = GConstants.GPH_NONE;
                    SetGphElmtPalleteSelected(iGphElmtType);
                }
            }
            else if (iGphElmtType == GConstants.GPH_MDLOBJ)
            {
                if (aPointList == null) aPointList = new List<Point>();
                aPointList.Add(prevCursorPos);
                aPointList.Add(prevCursorPos);
                Pen pen = new Pen(Brushes.Black, 3);
                using (DrawingContext dc = aWorkVisual.RenderOpen())
                {
                    Brush brush = Brushes.Wheat;
                    WDrawingVisual.WDrawRectangle(dc, brush, pen, aPointList[0].X, aPointList[0].Y, aPointList[1].X, aPointList[1].Y);
                }
            }
            else if (iGphElmtType == GConstants.GPH_MDLCLASS)
            {
                if (aPointList == null) aPointList = new List<Point>();
                aPointList.Add(prevCursorPos);
                aPointList.Add(prevCursorPos);
                Pen pen = new Pen(Brushes.Black, 3);
                using (DrawingContext dc = aWorkVisual.RenderOpen())
                {
                    Brush brush = Brushes.Wheat;
                    WDrawingVisual.WDrawRectangle(dc, brush, pen, aPointList[0].X, aPointList[0].Y, aPointList[1].X, aPointList[1].Y);
                }
            }
            #endregion
        }

        //-----------------------------------------------------------------------
        private void ModelCanvas_MouseMove(object sender, MouseEventArgs e)
        {
            cursorPos = e.GetPosition(ModelCanvas);
            if (cursorPos.X < 0) cursorPos.X = 0;
            if (cursorPos.Y < 0) cursorPos.Y = 0;

            double dx = (cursorPos.X - prevCursorPos.X) / zoomFactor;
            double dy = (cursorPos.Y - prevCursorPos.Y) / zoomFactor;

            if (dx == 0 && dy == 0) return;
            double posBX = 0;
            double posBY = 0;
            double posEX = 0;
            double posEY = 0;
            if (aVMMdlCmp != null || aVMGphElmt != null || (aSelectedObjectList != null && aSelectedObjectList.Count > 0))  //--Selected
            {
                if (iLeftButtonDown)
                {
                    if (iOperMode == OperMode.Move)
                    {
                        Model_Locate.MoveMdlCmpList(aVMMdlHead, aSelectedObjectList, dx, dy);
                        WDrawModel.DrawModel(ModelCanvas, aVMMdlHead, null, aSelectedObjectList, zoomFactor, zoomFactor);
                    }
                    else if (iOperMode == OperMode.Resize)
                    {
                        Model_Locate.ResizeMdlCmpList(aVMMdlHead, aSelectedObjectList, iSelPos, dx, dy);
                        WDrawModel.DrawModel(ModelCanvas, aVMMdlHead, null, aSelectedObjectList, zoomFactor, zoomFactor);
                    }
                    else if (iOperMode == OperMode.Individual)
                    {
                        var penx = new Pen(new SolidColorBrush(Color.FromRgb(200, 10, 20)), 2);
                        penx.DashStyle = DashStyles.Solid;
                        using (DrawingContext dc = aWorkVisual.RenderOpen())
                        {
                            if (aRubPointList != null && aRubPointList.Count > 0)
                            {
                                aRubPointList[aRubPointList.Count - 1] = new Point(cursorPos.X, cursorPos.Y);
                                if (aRubPointList.Count == 1)
                                {
                                    VMMdlCmp.GetMdlCmpBOX(aVMMdlCmp, ref posBX, ref posBY, ref posEX, ref posEY);
                                    WDrawingVisual.WDrawRoundedRectangle(dc, Brushes.Wheat, penx, posBX, posBY, posEX, posEY, Math.Abs(aRubPointList[0].X - posBX), Math.Abs(aRubPointList[0].Y - posBY));
                                }
                                else if (aRubPointList.Count == 2)
                                {
                                    WDrawingVisual.WDrawLine(dc, penx, aRubPointList[0].X, aRubPointList[0].Y, aRubPointList[1].X, aRubPointList[1].Y);
                                }
                                else
                                {
                                    WDrawingVisual.WDrawLine(dc, penx, aRubPointList[0].X, aRubPointList[0].Y, aRubPointList[2].X, aRubPointList[2].Y);
                                    WDrawingVisual.WDrawLine(dc, penx, aRubPointList[1].X, aRubPointList[1].Y, aRubPointList[2].X, aRubPointList[2].Y);
                                }
                            }
                        }

                    }
                    prevCursorPos = cursorPos;
                    return;
                }
            }
            if (aPointList == null || aPointList.Count < 1) return;
            if (aWorkVisual == null) return;
            #region --Elmt mouse move
            var pen = new Pen(new SolidColorBrush(Color.FromRgb(200, 10, 20)), 2);
            pen.DashStyle = DashStyles.Solid;
            aPointList[aPointList.Count - 1] = new Point(cursorPos.X, cursorPos.Y);

            if (iGphElmtType == GConstants.GPH_LINE)
            {
                using (DrawingContext dc = aWorkVisual.RenderOpen())
                {
                    WDrawingVisual.WDrawLine(dc, pen, aPointList[0].X, aPointList[0].Y, aPointList[1].X, aPointList[1].Y);
                }
            }
            else if (iGphElmtType == GConstants.GPH_TEXT)
            {
                using (DrawingContext dc = aWorkVisual.RenderOpen())
                {
                    Brush brush = Brushes.Transparent;
                    WDrawingVisual.WDrawRectangle(dc, brush, pen, aPointList[0].X, aPointList[0].Y, aPointList[1].X, aPointList[1].Y);
                }
            }
            else if (iGphElmtType == GConstants.GPH_RECTANGLE)
            {
                using (DrawingContext dc = aWorkVisual.RenderOpen())
                {
                    Brush brush = Brushes.Wheat;
                    WDrawingVisual.WDrawRectangle(dc, brush, pen, aPointList[0].X, aPointList[0].Y, aPointList[1].X, aPointList[1].Y);
                }
            }
            else if (iGphElmtType == GConstants.GPH_ROUNDRECT)
            {
                using (DrawingContext dc = aWorkVisual.RenderOpen())
                {
                    Brush brush = Brushes.Wheat;
                    if (aPointList.Count <= 2)
                    {
                        WDrawingVisual.WDrawRoundedRectangle(dc, brush, pen, aPointList[0].X, aPointList[0].Y, aPointList[1].X, aPointList[1].Y, 0, 0);
                    }
                    else
                    {
                        aPointList[aPointList.Count - 1] = new Point(cursorPos.X, cursorPos.Y);
                        double radiusX = Math.Abs(cursorPos.X - aPointList[1].X);
                        double radiusY = Math.Abs(cursorPos.Y - aPointList[1].Y);
                        WDrawingVisual.WDrawRoundedRectangle(dc, brush, pen, aPointList[0].X, aPointList[0].Y, aPointList[1].X, aPointList[1].Y, radiusX, radiusY);
                    }
                }
            }

            else if (iGphElmtType == GConstants.GPH_ARC)
            {
                using (DrawingContext dc = aWorkVisual.RenderOpen())
                {
                    Brush brush = Brushes.Transparent;
                    if (aPointList.Count <= 2)
                    {
                        WDrawingVisual.WDrawEllipse(dc, brush, pen, aPointList[0].X, aPointList[0].Y, aPointList[1].X, aPointList[1].Y);
                    }
                    else
                    {
                        aPointList[aPointList.Count - 1] = new Point(cursorPos.X, cursorPos.Y);
                        WDrawingVisual.WDrawEllipse(dc, brush, pen, aPointList[0].X, aPointList[0].Y, aPointList[1].X, aPointList[1].Y);
                        double CX = (aPointList[0].X + aPointList[1].X) / 2;
                        double CY = (aPointList[0].Y + aPointList[1].Y) / 2;
                        if (aPointList.Count > 2) WDrawingVisual.WDrawLine(dc, pen, CX, CY, aPointList[2].X, aPointList[2].Y);
                        if (aPointList.Count > 3) WDrawingVisual.WDrawLine(dc, pen, CX, CY, aPointList[3].X, aPointList[3].Y);
                    }
                }
            }
            else if (iGphElmtType == GConstants.GPH_CHORD)
            {
                using (DrawingContext dc = aWorkVisual.RenderOpen())
                {
                    Brush brush = Brushes.Wheat;
                    if (aPointList.Count <= 2)
                    {
                        WDrawingVisual.WDrawEllipse(dc, brush, pen, aPointList[0].X, aPointList[0].Y, aPointList[1].X, aPointList[1].Y);
                    }
                    else
                    {
                        aPointList[aPointList.Count - 1] = new Point(cursorPos.X, cursorPos.Y);
                        WDrawingVisual.WDrawEllipse(dc, brush, pen, aPointList[0].X, aPointList[0].Y, aPointList[1].X, aPointList[1].Y);
                        double CX = (aPointList[0].X + aPointList[1].X) / 2;
                        double CY = (aPointList[0].Y + aPointList[1].Y) / 2;
                        if (aPointList.Count > 2) WDrawingVisual.WDrawLine(dc, pen, CX, CY, aPointList[2].X, aPointList[2].Y);
                        if (aPointList.Count > 3) WDrawingVisual.WDrawLine(dc, pen, CX, CY, aPointList[3].X, aPointList[3].Y);
                    }
                }
            }
            else if (iGphElmtType == GConstants.GPH_PIE)
            {
                using (DrawingContext dc = aWorkVisual.RenderOpen())
                {
                    Brush brush = Brushes.Wheat;
                    if (aPointList.Count <= 2)
                    {
                        WDrawingVisual.WDrawEllipse(dc, brush, pen, aPointList[0].X, aPointList[0].Y, aPointList[1].X, aPointList[1].Y);
                    }
                    else
                    {
                        aPointList[aPointList.Count - 1] = new Point(cursorPos.X, cursorPos.Y);
                        WDrawingVisual.WDrawEllipse(dc, brush, pen, aPointList[0].X, aPointList[0].Y, aPointList[1].X, aPointList[1].Y);
                        double CX = (aPointList[0].X + aPointList[1].X) / 2;
                        double CY = (aPointList[0].Y + aPointList[1].Y) / 2;
                        if (aPointList.Count > 2) WDrawingVisual.WDrawLine(dc, pen, CX, CY, aPointList[2].X, aPointList[2].Y);
                        if (aPointList.Count > 3) WDrawingVisual.WDrawLine(dc, pen, CX, CY, aPointList[3].X, aPointList[3].Y);
                    }
                }
            }
            else if (iGphElmtType == GConstants.GPH_BITMAP)
            {
                using (DrawingContext dc = aWorkVisual.RenderOpen())
                {
                    Brush brush = Brushes.Transparent;
                    WDrawingVisual.WDrawRectangle(dc, brush, pen, aPointList[0].X, aPointList[0].Y, aPointList[1].X, aPointList[1].Y);
                }
            }
            else if (iGphElmtType == GConstants.GPH_ELLIPSE)
            {
                using (DrawingContext dc = aWorkVisual.RenderOpen())
                {
                    Brush brush = Brushes.Wheat;
                    WDrawingVisual.WDrawEllipse(dc, brush, pen, aPointList[0].X, aPointList[0].Y, aPointList[1].X, aPointList[1].Y);
                }
            }
            else if (iGphElmtType == GConstants.GPH_POLYGON || iGphElmtType == GConstants.GPH_POLYLINE)
            {
                using (DrawingContext dc = aWorkVisual.RenderOpen())
                {
                    if (iGphElmtType == GConstants.GPH_POLYLINE)
                    {
                        Brush brush = Brushes.Transparent;
                        WDrawingVisual.WDrawPolyLine(dc, brush, pen, aPointList);
                    }
                    else if (iGphElmtType == GConstants.GPH_POLYGON)
                    {
                        Brush brush = Brushes.Wheat;
                        WDrawingVisual.WDrawPolygon(dc, brush, pen, aPointList);
                    }
                }
            }
            else if (iGphElmtType == GConstants.GPH_BEZIERCURVE)
            {
                using (DrawingContext dc = aWorkVisual.RenderOpen())
                {
                    Brush brush = Brushes.Transparent;
                    //                        WDrawingVisual.WDrawBezierCurve(dc, brush, pen, aPointList);
                    WDrawingVisual.WDrawBezierCurve(dc, brush, pen, aPointList);
                }
            }
            else if (iGphElmtType == GConstants.GPH_QUADBEZIERCURVE)
            {
                using (DrawingContext dc = aWorkVisual.RenderOpen())
                {
                    Brush brush = Brushes.Transparent;
                    //                        WDrawingVisual.WDrawQuadraticBezierCurve(dc, brush, pen, aPointList);
                    WDrawingVisual.WDrawQuadraticBezierCurve(dc, brush, pen, aPointList);
                }
            }
            else if (iGphElmtType == GConstants.GPH_MDLOBJ)
            {
                using (DrawingContext dc = aWorkVisual.RenderOpen())
                {
                    Brush brush = Brushes.Wheat;
                    WDrawingVisual.WDrawRectangle(dc, brush, pen, aPointList[0].X, aPointList[0].Y, aPointList[1].X, aPointList[1].Y);
                }
            }
            else if (iGphElmtType == GConstants.GPH_MDLCLASS)
            {
                using (DrawingContext dc = aWorkVisual.RenderOpen())
                {
                    Brush brush = Brushes.Wheat;
                    WDrawingVisual.WDrawRectangle(dc, brush, pen, aPointList[0].X, aPointList[0].Y, aPointList[1].X, aPointList[1].Y);
                }
            }
            else if (iGphElmtType == GConstants.GPH_MDLCONN)
            {
                using (DrawingContext dc = aWorkVisual.RenderOpen())
                {
                    Brush brush = Brushes.Transparent;
                    WDrawingVisual.WDrawPolyLine(dc, brush, pen, aPointList);
                }
            }

            else if (fMultiSelect)
            {
                using (DrawingContext dc = aWorkVisual.RenderOpen())
                {
                    WDrawingVisual.WDrawRectangle(dc, Brushes.Transparent, pen, aPointList[0].X, aPointList[0].Y, aPointList[1].X, aPointList[1].Y);
                }
            }
            #endregion
            DisplayStatus(dx, dy);
        }
        //-----------------------------------------------------------------
        private void ModelCanvas_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            iLeftButtonDown = false;
            iRightButtonDown = false;
            if (iGphElmtType != GConstants.GPH_NONE)  //--Adding point to special ELmt
            {
                #region -- Continue for extra Point 
                if (iGphElmtType == GConstants.GPH_ARC)
                {
                    if (aPointList.Count < 5)
                    {
                        if (aPointList.Count == 2)
                        {
                            aPointList.Add(aPointList[1]);
                        }
                        return;
                    }
                }
                else if (iGphElmtType == GConstants.GPH_CHORD)
                {
                    if (aPointList.Count < 5)
                    {
                        if (aPointList.Count == 2)
                        {
                            aPointList.Add(aPointList[1]);
                        }
                        return;
                    }
                }
                else if (iGphElmtType == GConstants.GPH_PIE)
                {
                    if (aPointList.Count < 5)
                    {
                        if (aPointList.Count == 2)
                        {
                            aPointList.Add(aPointList[1]);
                        }
                        return;
                    }
                }
                else if (iGphElmtType == GConstants.GPH_ROUNDRECT)
                {
                    if (aPointList.Count < 3)
                    {
                        if (aPointList.Count == 2)
                        {
                            aPointList.Add(aPointList[1]);
                        }
                        return;
                    }
                }
                aVMMdlCmp = SaveMdlCmp(aPointList, true);
                #endregion
            }
            else
            {
                if (fMultiSelect)
                {
                    aVMGphElmt = null;
                    aVMMdlCmp = null;
                    if (aPointList != null)
                    {
                        List<Point> aPointListX = WCoord.oResizePoints(aPointList, 1 / zoomFactor, 1 / zoomFactor);
                        if (Math.Abs(aPointListX[0].X - aPointListX[1].X) < 3 && Math.Abs(aPointListX[0].Y - aPointListX[1].Y) < 3)
                        {
                            aSelectedObjectList = null;
                            if (Model_Locate.GetMdlCmpByCursor(aPointListX[0], aVMMdlHead, ref aVMMdlCmp, ref aRubPointList) >= 0)
                            {
                                aRubPointList = WCoord.oResizePoints(aRubPointList, zoomFactor, zoomFactor);
                                aSelectedObjectList = new List<object>();
                                if (aVMMdlCmp != null) aSelectedObjectList.Add(aVMMdlCmp);
                                //???//                          if (aVMGphElmt != null) aSelectedObjectList.Add(aVMGphElmt);
                            }
                        }
                        else
                        {
                            aSelectedObjectList = Model_Locate.FindMdlCmpInRect(aVMMdlHead, aPointListX);
                        }
                    }
                    ModelCanvas.DeleteVisual(aWorkVisual);
                    //                   fMultiSelect = false;
                }
                else
                {
                    if (iOperMode == OperMode.Individual && aRubPointList != null)
                    {
                        List<Point> aRubPointListX = WCoord.oResizePoints(aRubPointList, 1 / zoomFactor, 1 / zoomFactor);
                        Model_Arrange.UpdatePoly(aVMMdlHead, aVMMdlCmp, 1.0, 1.0, iSelPos - 100, aRubPointListX);
                        ModelCanvas.DeleteVisual(aWorkVisual);
                        aRubPointList = null;
                        iOperMode = OperMode.None;
                    }
                }
                if (aSelectedObjectList != null)
                {
                    if (aMdlCmpDataDlg != null)
                    {
                        aVMMdlCmp = aSelectedObjectList[0] as VMMdlCmp;
                        aMdlCmpDataDlg.ShowData(aVMMdlCmp);
                    }
                }
                aPointList = null;
                WDrawModel.DrawModel(ModelCanvas, aVMMdlHead, aVMMdlCmp, aSelectedObjectList, zoomFactor, zoomFactor);
            }
        }

        //-----------------------------------------------------------------
        private void ModelCanvas_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (iGphElmtType == GConstants.GPH_NONE)
            {
                //--Context Menu?
                if (aVMGphElmt != null)
                {
                    if (aVMGphElmt.GEType == GConstants.GPH_TEXT)
                    {
                        TextInput dlg = new ObjLogic.TextInput(aVMGphElmt.MxyDB);
                        if (dlg.ShowDialog() == true)
                        {
                            aVMGphElmt.MxyDB = dlg.sText;
                            Size size = WDrawingVisual.WMeasureString(aVMGphElmt.MxyDB, "Arial", 12, 0, aVMGphElmt.PosEX, aVMGphElmt.PosEY);
                            aVMGphElmt.PosEX = aVMGphElmt.PosBX + size.Width;
                            aVMGphElmt.PosEY = aVMGphElmt.PosBY + size.Height;
                            aVMMdlCmp.CmpType = GConstants.OBJ_GELMT;
                            aVMMdlCmp.CmpObjPtrX = aVMGphElmt;
                            WDrawModel.DrawModel(ModelCanvas, aVMMdlHead, aVMMdlCmp, null, zoomFactor, zoomFactor);
                        }
                        return;
                    }
                }
                VMMdlCmp aCmp = null;
                iRightButtonDown = true;
                prevCursorPos = e.GetPosition(ModelCanvas);
                Point prevCursorPosX = new Point(prevCursorPos.X / zoomFactor, prevCursorPos.Y / zoomFactor);
                if (Model_Locate.GetMdlCmpByCursor(prevCursorPosX, aVMMdlHead, ref aCmp, ref aRubPointList) >= 0)
                {
                    aRubPointList = WCoord.oResizePoints(aRubPointList, zoomFactor, zoomFactor);
                    aVMMdlCmp = null;
                    aVMGphElmt = null;
                    if (aSelectedObjectList == null) aSelectedObjectList = new List<object>();
                    if (aCmp != null && aSelectedObjectList.Contains(aCmp)) aSelectedObjectList.Remove(aCmp);
                    else
                    {
                        if (aCmp != null && !aSelectedObjectList.Contains(aCmp)) aSelectedObjectList.Add(aCmp);
                        aVMMdlCmp = aCmp;
                        aVMGphElmt = Icon_Init.Cmp2GElmt(aCmp);
                    }
                    WDrawModel.DrawModel(ModelCanvas, aVMMdlHead, null, aSelectedObjectList, zoomFactor, zoomFactor);
                }
                else
                {
                    fMultiSelect = true;
                    if (aPointList == null) aPointList = new List<Point>();
                    aPointList.Add(prevCursorPos);
                    aPointList.Add(prevCursorPos);
                    aWorkVisual = new DrawingVisual();
                    ModelCanvas.AddVisual(aWorkVisual);

                    Pen pen = new Pen(Brushes.Black, 3);
                    using (DrawingContext dc = aWorkVisual.RenderOpen())
                    {
                        WDrawingVisual.WDrawRectangle(dc, Brushes.Transparent, pen, aPointList[0].X, aPointList[0].Y, aPointList[1].X, aPointList[1].Y);
                    }
                }
                return;
            }

            if (iGphElmtType == GConstants.GPH_POLYGON || iGphElmtType == GConstants.GPH_POLYLINE ||
                     iGphElmtType == GConstants.GPH_BEZIERCURVE || iGphElmtType == GConstants.GPH_QUADBEZIERCURVE
                      || iGphElmtType == GConstants.GPH_MDLCONN)
            {
                //                VMGphElmt aElmt = SaveGElmtPoly();
                aVMMdlCmp = SaveMdlCmp(aPointList, false);
            }
            iGphElmtType = GConstants.GPH_NONE;
            SetGphElmtPalleteSelected(iGphElmtType);
        }

        //-----------------------------------------------------------------
        private void ModelCanvas_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            iRightButtonDown = false;
            if (iGphElmtType == GConstants.GPH_NONE && fMultiSelect == true)
            {
                List<Point> aPointListX = new List<Point>();
                foreach (Point p in aPointList) aPointListX.Add(new Point(p.X / zoomFactor, p.Y / zoomFactor));
                List<object> aSelList = Model_Locate.FindMdlCmpInRect(aVMMdlHead, aPointListX);
                if (aSelList != null)
                {
                    if (aSelectedObjectList == null || aSelectedObjectList.Count <= 0) aSelectedObjectList = aSelList;
                    else
                    {
                        foreach (object o in aSelList)
                        {
                            if (aSelectedObjectList.Contains(o)) aSelectedObjectList.Remove(o);
                            else aSelectedObjectList.Add(o);
                        }
                    }
                }
                ModelCanvas.DeleteVisual(aWorkVisual);
                aPointList = null;
                WDrawModel.DrawModel(ModelCanvas, aVMMdlHead, null, aSelectedObjectList, zoomFactor, zoomFactor);
            }
            fMultiSelect = false;
        }

    }
}
