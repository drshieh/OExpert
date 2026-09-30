using HelixToolkit.Wpf;
using OExpert.UILibraryWPF;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace OExpert
{
    public class WRule_Locat
    {

        /*###################### LOCAT ######################################*/

        /*-----------------------------------------------------------------
        oLocatRuleInRect(bx,by,ex,ey,xrat,yrat,bSelPtr,bRule,tRect)
         Find the Element that is pointed by the cursor
         return the pointer of the element
        -----------------------------------------------------------------*/
        public static int oLocatRuleInRect(
           double bx, double by, double ex, double ey, /* cursor Rect: Absolute to the Screen */
           List<object> aSelPtrList,    /* returned selected element pointer */
           XRuleNet aRuleNet  /* list of RuleHead Pointer */
           )
        {
            if (aRuleNet == null || aRuleNet.aRuleList == null) return -1;
            WCoord.oXyCoordSwap(ref bx, ref by, ref ex, ref ey);
            if (MainWindow.tp.fShowRule)
            {
                foreach (XRule aRule in aRuleNet.aRuleList)
                {
                    if (aRule.X >= bx && aRule.Y >= by &&
                        (aRule.X + aRule.Width) <= ex && (aRule.Y + aRule.Height) <= ey)
                    {
                        aSelPtrList.Add(aRule);
                    }
                }
            }
            if (MainWindow.tp.fShowFact)
            {
                foreach (XFact aFact in aRuleNet.aFactList)
                {
                    if (aFact.X >= bx && aFact.Y >= by &&
                        (aFact.X + aFact.Width) <= ex && (aFact.Y + aFact.Height) <= ey)
                    {
                        aSelPtrList.Add(aFact);
                    }
                }
            }
            if (MainWindow.tp.fShowConclusion)
            {
                foreach (XConclusion aCon in aRuleNet.aConclusionList)
                {
                    if (aCon.X >= bx && aCon.Y >= by &&
                        (aCon.X + aCon.Width) <= ex && (aCon.Y + aCon.Height) <= ey)
                    {
                        aSelPtrList.Add(aCon);
                    }
                }
            }
            return (10);
        }

        /*-----------------------------------------------------------------
        FUNCT: oLocatRule(cx,cy,bSelPtr,bRule,tRect)
         Find the Element that is pointed by the cursor
         return the pointer of the element
        -----------------------------------------------------------------*/
        public static int oLocatRule(
                  double cx, double cy, /* cursor position: Absolute to the Screen */
                  List<object> aSelPtrList,    /* returned selected element pointer */
                  XRuleNet aRuleNet)
        {
            aSelPtrList.Clear();
            if (aRuleNet == null || aRuleNet.aRuleList == null) return -1;
            if (MainWindow.tp.fShowRule)
            {
                for (int i = aRuleNet.aRuleList.Count - 1; i >= 0; i--)
                {
                    if (cx >= aRuleNet.aRuleList[i].X - GConstants.MARK &&
                        cy >= aRuleNet.aRuleList[i].Y - GConstants.MARK &&
                        cx <= (aRuleNet.aRuleList[i].X + aRuleNet.aRuleList[i].Width + GConstants.MARK) &&
                        cy <= (aRuleNet.aRuleList[i].Y + aRuleNet.aRuleList[i].Height + GConstants.MARK))
                    {
                        aSelPtrList.Add(aRuleNet.aRuleList[i]);
                        return (oLocatStub(cx, cy, aRuleNet.aRuleList[i].X, aRuleNet.aRuleList[i].Y, aRuleNet.aRuleList[i].Width, aRuleNet.aRuleList[i].Height));
                    }
                }
            }
            if (MainWindow.tp.fShowFact)
            {
                for (int i = aRuleNet.aFactList.Count - 1; i >= 0; i--)
                {
                    if (cx >= aRuleNet.aFactList[i].X - GConstants.MARK &&
                        cy >= aRuleNet.aFactList[i].Y - GConstants.MARK &&
                        cx <= (aRuleNet.aFactList[i].X + aRuleNet.aFactList[i].Width + GConstants.MARK) &&
                        cy <= (aRuleNet.aFactList[i].Y + aRuleNet.aFactList[i].Height + GConstants.MARK))
                    {
                        aSelPtrList.Add(aRuleNet.aFactList[i]);
                        return (oLocatStub(cx, cy, aRuleNet.aFactList[i].X, aRuleNet.aFactList[i].Y, aRuleNet.aFactList[i].Width, aRuleNet.aFactList[i].Height));
                    }
                }
            }
            if (MainWindow.tp.fShowConclusion)
            {
                for (int i = aRuleNet.aConclusionList.Count - 1; i >= 0; i--)
                {
                    if (cx >= aRuleNet.aConclusionList[i].X - GConstants.MARK &&
                        cy >= aRuleNet.aConclusionList[i].Y - GConstants.MARK &&
                        cx <= (aRuleNet.aConclusionList[i].X + aRuleNet.aConclusionList[i].Width + GConstants.MARK) &&
                        cy <= (aRuleNet.aConclusionList[i].Y + aRuleNet.aConclusionList[i].Height + GConstants.MARK))
                    {
                        aSelPtrList.Add(aRuleNet.aConclusionList[i]);
                        return (oLocatStub(cx, cy, aRuleNet.aConclusionList[i].X, aRuleNet.aConclusionList[i].Y, aRuleNet.aConclusionList[i].Width, aRuleNet.aConclusionList[i].Height));
                    }
                }
            }
            return -1;
        }

        /*-----------------------------------------------------------------
        FUNCT: oLocatRule(cx,cy,bSelPtr,bRule,tRect)
         Find the Element that is pointed by the cursor
         return the pointer of the element
        -----------------------------------------------------------------*/
        public static object oLocatRuleX(
                  double cx, double cy, /* cursor position: Absolute to the Screen */
                  XRuleNet aRuleNet)
        {
            if (aRuleNet == null || aRuleNet.aRuleList == null) return null;
            if (MainWindow.tp.fShowRule)
            {
                for (int i = aRuleNet.aRuleList.Count - 1; i >= 0; i--)
                {
                    if (cx >= aRuleNet.aRuleList[i].X - GConstants.MARK &&
                        cy >= aRuleNet.aRuleList[i].Y - GConstants.MARK &&
                        cx <= (aRuleNet.aRuleList[i].X + aRuleNet.aRuleList[i].Width + GConstants.MARK) &&
                        cy <= (aRuleNet.aRuleList[i].Y + aRuleNet.aRuleList[i].Height + GConstants.MARK))
                    {
                        return aRuleNet.aRuleList[i];
                    }
                }
            }
            if (MainWindow.tp.fShowFact)
            {
                for (int i = aRuleNet.aFactList.Count - 1; i >= 0; i--)
                {
                    if (cx >= aRuleNet.aFactList[i].X - GConstants.MARK &&
                        cy >= aRuleNet.aFactList[i].Y - GConstants.MARK &&
                        cx <= (aRuleNet.aFactList[i].X + aRuleNet.aFactList[i].Width + GConstants.MARK) &&
                        cy <= (aRuleNet.aFactList[i].Y + aRuleNet.aFactList[i].Height + GConstants.MARK))
                    {
                        return aRuleNet.aFactList[i];
                    }
                }
            }
            if (MainWindow.tp.fShowConclusion)
            {
                for (int i = aRuleNet.aConclusionList.Count - 1; i >= 0; i--)
                {
                    if (cx >= aRuleNet.aConclusionList[i].X - GConstants.MARK &&
                        cy >= aRuleNet.aConclusionList[i].Y - GConstants.MARK &&
                        cx <= (aRuleNet.aConclusionList[i].X + aRuleNet.aConclusionList[i].Width + GConstants.MARK) &&
                        cy <= (aRuleNet.aConclusionList[i].Y + aRuleNet.aConclusionList[i].Height + GConstants.MARK))
                    {
                        return aRuleNet.aConclusionList[i];
                    }
                }
            }
            return null;
        }

        /*-----------------------------------------------------------------
        oLocatStub(cx,cy,bx,by)
           ---2----
           |      3
           1  10  |
           |      4
           ---5----
        -----------------------------------------------------------------*/
        public static int oLocatStub(
                  double cx, double cy,  //--Cursor location
                  double bx, double by, double ex, double ey)
        {
            double tx, ty;

            ty = (by + ey) / 2;
            if (cx >= bx - GConstants.MARK && cy >= ty - GConstants.MARK &&
               cx <= bx + GConstants.MARK && cy <= ty + GConstants.MARK) return (1);
            tx = (bx + ex) / 2;
            if (cx >= tx - GConstants.MARK && cy >= by - GConstants.MARK &&
               cx <= tx + GConstants.MARK && cy <= by + GConstants.MARK) return (2);
            ty = by + (ey - by) / 3;
            if (cx >= ex - GConstants.MARK && cy >= ty - GConstants.MARK &&
               cx <= ex + GConstants.MARK && cy <= ty + GConstants.MARK) return (3);
            ty = ey - (ey - by) / 3;
            if (cx >= ex - GConstants.MARK && cy >= ty - GConstants.MARK &&
               cx <= ex + GConstants.MARK && cy <= ty + GConstants.MARK) return (4);
            if (cx >= tx - GConstants.MARK && cy >= ey - GConstants.MARK &&
               cx <= tx + GConstants.MARK && cy <= ey + GConstants.MARK) return (5);

            return (10);
        }

        /*-----------------------------------------------------------------
        oSetPointBySelMark(iSelMark,tRect,bx,by)
           ---2----
           |      3
           1  10  |
           |      4
           ---5----
        -----------------------------------------------------------------*/
        public static void oSetPointBySelMark(
                  int iSelMark,
                  WRECT tRect,
                  ref double bx,
                  ref double by)
        {
            if (iSelMark == 1)
            {
                bx = tRect.left;
                by = (tRect.top + tRect.bottom) / 2;
            }
            else if (iSelMark == 2)
            {
                bx = (tRect.left + tRect.right) / 2;
                by = tRect.top;
            }
            else if (iSelMark == 3)
            {
                bx = tRect.right;
                by = tRect.top + (tRect.bottom - tRect.top) / 3;
            }
            else if (iSelMark == 4)
            {
                bx = tRect.right;
                by = tRect.bottom - (tRect.bottom - tRect.top) / 3;
            }
            else if (iSelMark == 5)
            {
                bx = (tRect.left + tRect.right) / 2;
                by = tRect.bottom;
            }
            else
            {
                bx = (tRect.left + tRect.right) / 2;
                by = (tRect.top + tRect.bottom) / 2;
            }
        }

        //-----------------------------------------------------
        public static List<Point> GetSelectedRect(List<object> aSelObjectList)
        {
            if (aSelObjectList == null) return null;
            double posBX = 0, posEX = 0, posBY = 0, posEY = 0;
            double bx = Double.MaxValue;
            double by = Double.MaxValue;
            double ex = Double.MinValue;
            double ey = Double.MinValue;
            foreach (object a in aSelObjectList)
            {
                GetRuleBOX(a, ref posBX, ref posBY, ref posEX, ref posEY);
                if (bx > posBX) bx = posBX;
                if (ex < posEX) ex = posEX;
                if (by > posBY) by = posBY;
                if (ey < posEY) ey = posEY;
            }
            List<Point> aPtList = new List<Point>();
            aPtList.Add(new Point(bx, by));
            aPtList.Add(new Point(ex, ey));

            return aPtList;
        }

        //-------------------------------------------------------------
        public static void GetRuleBOX(object a, ref double posBX, ref double posBY, ref double posEX, ref double posEY)
        {
            if (a == null) return;
            if (a.GetType() == typeof(XRule))
            {
                XRule x = a as XRule;
                posBX = x.X;
                posEX = x.X + x.Width;
                posBY = x.Y;
                posEY = x.Y + x.Height;
            }
            else if (a.GetType() == typeof(XFact))
            {
                XFact x = a as XFact;
                posBX = x.X;
                posEX = x.X + x.Width;
                posBY = x.Y;
                posEY = x.Y + x.Height;
            }
            else if (a.GetType() == typeof(XConclusion))
            {
                XConclusion x = a as XConclusion;
                posBX = x.X;
                posEX = x.X + x.Width;
                posBY = x.Y;
                posEY = x.Y + x.Height;
            }
        }

        //###################### MOUSE MOVE #################################
        //-----------------------------------------------------------------------
        public static void RuleMoveResize(WDrawingCanvas aCanvas, XRuleNet aRuleNet, double dx, double dy, WSettings aSetting)
        {
            if (dx == 0 && dy == 0) return;
            if (aRuleNet == null || aRuleNet.aRuleList == null) return;
            if (aRuleNet.aSelectedObjectList != null && aRuleNet.aSelectedObjectList.Count > 0)  //--Selected
            {
                if (aSetting.iLeftButtonDown)
                {
                    if (aSetting.iOperMode == OperMode.Move)
                    {
                        XMoveRuleList(aRuleNet.aSelectedObjectList, dx, dy);
                        XRuleNet.GetRuleNetSize(aRuleNet);
                        WDrawRule.DrawRuleNet(aCanvas, aRuleNet, aRuleNet.aSelectedObjectList, aSetting.zoomFactorX, aSetting.zoomFactorY,
                                              GConstants.WIN_NORMAL);
                    }
                    else if (aSetting.iOperMode == OperMode.Resize)
                    {
                        XResizeRuleList(aRuleNet.aSelectedObjectList, dx, dy, aSetting.iSelPos);
                        XRuleNet.GetRuleNetSize(aRuleNet);
                        WDrawRule.DrawRuleNet(aCanvas, aRuleNet, aRuleNet.aSelectedObjectList, 
                                              aSetting.zoomFactorX, aSetting.zoomFactorY, GConstants.WIN_NORMAL);
                    }
                    aSetting.prevCursorPos = aSetting.cursorPos;
                    return;
                }
            }
            if (aSetting.aPointList == null || aSetting.aPointList.Count < 1) return;
            if (aSetting.aWorkVisual == null) return;
            #region --Elmt mouse move
            var pen = new Pen(new SolidColorBrush(Color.FromRgb(200, 10, 20)), 2);
            pen.DashStyle = DashStyles.Solid;
            aSetting.aPointList[aSetting.aPointList.Count - 1] = new Point(aSetting.cursorPos.X, aSetting.cursorPos.Y);

            if (aSetting.fMultiSelect)
            {
                using (DrawingContext dc = aSetting.aWorkVisual.RenderOpen())
                {
                    WDrawingVisual.WDrawRectangle(dc, Brushes.Transparent, pen, aSetting.aPointList[0].X, aSetting.aPointList[0].Y, aSetting.aPointList[1].X, aSetting.aPointList[1].Y);
                }
            }
            #endregion
        }

        //--------------------------------------------------
        public static void XMoveRuleList(List<object> aSelList, double dx, double dy)
        {
            if (aSelList == null) return;
            MainWindow.tp.fChanged = true;

            foreach (object a in aSelList)
            {
                if (a.GetType() == typeof(XRule))
                {
                    XRule aRule = a as XRule;
                    if (aRule != null)
                    {
                        aRule.X += dx;
                        aRule.Y += dy;
                    }
                }
                else if (a.GetType() == typeof(XFact))
                {
                    XFact aFact = a as XFact;
                    if (aFact != null)
                    {
                        aFact.X += dx;
                        aFact.Y += dy;
                    }
                }
                else if (a.GetType() == typeof(XConclusion))
                {
                    XConclusion aCon = a as XConclusion;
                    if (aCon != null)
                    {
                        aCon.X += dx;
                        aCon.Y += dy;
                    }
                }
            }
        }
        public static WRECT AdjustCoordByOpt(double x, double y, double width, double height, double dx, double dy, int iOpt)
        {
            double ex = x + width;
            double ey = y + height;
            WRECT r = new WRECT(x, y, ex, ey);
            if (iOpt == 0)
            {
                x += dx;
                y += dy;
                r = new WRECT(x, y, ex, ey);
            }
            else if (iOpt == 1)
            {
                y += dy;
                r = new WRECT(x, y, ex, ey);
            }
            else if (iOpt == 2)
            {
                ex += dx;
                y += dy;
                r = new WRECT(x, y, ex, ey);
            }
            else if (iOpt == 3)
            {
                ex += dx;
                r = new WRECT(x, y, ex, ey);
            }
            else if (iOpt == 4)
            {
                ex += dx;
                ey += dy;
                r = new WRECT(x, y, ex, ey);
            }
            else if (iOpt == 5)
            {
                ey += dy;
                r = new WRECT(x, y, ex, ey);
            }
            else if (iOpt == 6)
            {
                x += dx;
                ey += dy;
                r = new WRECT(x, y, ex, ey);
            }
            else if (iOpt == 7)
            {
                x += dx;
                r = new WRECT(x, y, ex, ey);
            }

            return r;
        }

        //--------------------------------------------------
        public static void XResizeRuleList(List<object> aSelList, double dx, double dy, int iOpt)
        {
            if (aSelList == null) return;
            MainWindow.tp.fChanged = true;

            foreach (object a in aSelList)
            {
                if (a.GetType() == typeof(XRule))
                {
                    XRule aRule = a as XRule;
                    if (aRule != null)
                    {
                        WRECT r = AdjustCoordByOpt(aRule.X, aRule.Y, aRule.Width, aRule.Height, dx, dy, iOpt);
                        aRule.X = r.left;
                        aRule.Y = r.top;
                        aRule.Width = r.right - r.left;
                        aRule.Height = r.bottom - r.top;
                    }
                }
                else if (a.GetType() == typeof(XFact))
                {
                    XFact aFact = a as XFact;
                    if (aFact != null)
                    {
                        WRECT r = AdjustCoordByOpt(aFact.X, aFact.Y, aFact.Width, aFact.Height, dx, dy, iOpt);
                        aFact.X = r.left;
                        aFact.Y = r.top;
                        aFact.Width = r.right - r.left;
                        aFact.Height = r.bottom - r.top;
                    }
                }
                else if (a.GetType() == typeof(XConclusion))
                {
                    XConclusion aCon = a as XConclusion;
                    if (aCon != null)
                    {
                        WRECT r = AdjustCoordByOpt(aCon.X, aCon.Y, aCon.Width, aCon.Height, dx, dy, iOpt);
                        aCon.X = r.left;
                        aCon.Y = r.top;
                        aCon.Width = r.right - r.left;
                        aCon.Height = r.bottom - r.top;
                    }
                }
            }
        }

        //-----------------------------------------------------------------------------------------------
        public static int oLocatRuleInSelected(Point cursorPos, List<object> aSelectedObjList, ref object aCmp)
        {
            aCmp = null;
            int iSelPos = -1;  //--Selected position
            double posBX = 0;
            double posBY = 0;
            double posEX = 0;
            double posEY = 0;

            if (aSelectedObjList != null && aSelectedObjList.Count > 0)
            {
                foreach (object a in aSelectedObjList)
                {
                    GetRuleBOX(a, ref posBX, ref posBY, ref posEX, ref posEY);
                    if (cursorPos.X >= posBX - 3 && cursorPos.X <= posEX + 3 &&
                        cursorPos.Y >= posBY - 3 && cursorPos.Y <= posEY + 3)
                    {
                        aCmp = a;
                        return LocatInRuleSTUB(cursorPos, posBX, posBY, posEX, posEY);
                    }
                }
            }
            return iSelPos;
        }

        //-----------------------------------------------------------------
        public static int LocatInRuleSTUB(Point cursorPos, double posBX, double posBY, double posEX, double posEY)
        {
            //--Check the bounding rect stub
            List<Point> aPointList = new List<Point>();
            aPointList.Add(new Point(posBX, posBY));  //--0
            aPointList.Add(new Point((posBX + posEX) / 2, posBY));  //--1
            aPointList.Add(new Point(posEX, posBY));                //--2
            aPointList.Add(new Point(posEX, (posBY + posEY) / 2));  //--3
            aPointList.Add(new Point(posEX, posEY));                //--4
            aPointList.Add(new Point((posBX + posEX) / 2, posEY));  //--5
            aPointList.Add(new Point(posBX, posEY));                //--6
            aPointList.Add(new Point(posBX, (posBY + posEY) / 2));  //--7
            for (int i = 0; i < aPointList.Count; i++)
            {
                if (cursorPos.X >= aPointList[i].X - 3 && cursorPos.X <= aPointList[i].X + 3 &&
                   cursorPos.Y >= aPointList[i].Y - 3 && cursorPos.Y <= aPointList[i].Y + 3) return i;
            }
            return 10;
        }

        //-----------------------------------------------------------------
        public static int LocatInRuleSTUB(object aCmp, Point cursorPos)
        {
            double posBX = 0;
            double posBY = 0;
            double posEX = 0;
            double posEY = 0;
            GetRuleBOX(aCmp, ref posBX, ref posBY, ref posEX, ref posEY);
            //--Check the bounding rect stub
            List<Point> aPointList = new List<Point>();
            aPointList.Add(new Point(posBX, posBY));  //--0
            aPointList.Add(new Point((posBX + posEX) / 2, posBY));  //--1
            aPointList.Add(new Point(posEX, posBY));                //--2
            aPointList.Add(new Point(posEX, (posBY + posEY) / 2));  //--3
            aPointList.Add(new Point(posEX, posEY));                //--4
            aPointList.Add(new Point((posBX + posEX) / 2, posEY));  //--5
            aPointList.Add(new Point(posBX, posEY));                //--6
            aPointList.Add(new Point(posBX, (posBY + posEY) / 2));  //--7
            for (int i = 0; i < aPointList.Count; i++)
            {
                if (cursorPos.X >= aPointList[i].X - 3 && cursorPos.X <= aPointList[i].X + 3 &&
                   cursorPos.Y >= aPointList[i].Y - 3 && cursorPos.Y <= aPointList[i].Y + 3) return i;
            }
            return 10;
        }

    }
}
