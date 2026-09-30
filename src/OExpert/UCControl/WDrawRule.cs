using OExpert.UILibraryWPF;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.Eventing.Reader;
//using System.Drawing;
using System.Windows;
using System.Windows.Media;

namespace OExpert
{
    public class WDrawRule
    {
        //---------------------------------------------------------------
        //--xrat, yrat is used to calculate the Canvas size
        public static void DrawRuleNet(WDrawingCanvas aCanvas, XRuleNet aRuleNet,
              List<object> aSelList, double xrat, double yrat, int operMode)
        {
            aCanvas.Clear();

            bool fSelected = false;
            if (aRuleNet == null || aRuleNet.aRuleList == null) return;
            XRuleNet.SetCanvasSize(aCanvas, aRuleNet, xrat, yrat);

            double incD = 0;
            foreach (XRule aRule in aRuleNet.aRuleList)  //--Width/Height <= 0
            {
                SetRuleSize(aRule, 1, incD);
            }

            #region --Links------------------------------------
            //--XFact
            if (MainWindow.tp.fShowFact && MainWindow.tp.fShowRule && MainWindow.tp.fShowFactLink)
            {
                foreach (XFact a in aRuleNet.aFactList)
                {
                    if (a.aDrawVisualX != null) a.aDrawVisualX = null;
                    if (aSelList != null && aSelList.Contains(a)) fSelected = true;
                    else fSelected = false;
                    DrawFactLinkXCV(aCanvas, a, 0, 0, fSelected);
                }
            }
            //--XConclusion
            if (MainWindow.tp.fShowConclusion && MainWindow.tp.fShowRule && MainWindow.tp.fShowConclusionLink)
            {
                foreach (XConclusion a in aRuleNet.aConclusionList)
                {
                    if (aSelList != null && aSelList.Contains(a)) fSelected = true;
                    else fSelected = false;
                    DrawConclusionLinkXCV(aCanvas, a, 0, 0, fSelected);
                }
            }

            //--XRule
            if (MainWindow.tp.fShowRule && MainWindow.tp.fShowRuleLink)
            {
                foreach (XRule a in aRuleNet.aRuleList)
                {
                    if (aSelList != null && aSelList.Contains(a)) fSelected = true;
                    else fSelected = false;
                    DrawRuleLinkXCV(aCanvas, a, 0, 0, fSelected);
                }
            }
            #endregion

            #region --Items-----------------------------------
            if (MainWindow.tp.fShowFact)
            {
                foreach (XFact a in aRuleNet.aFactList)
                {
                    if (aSelList != null && aSelList.Contains(a)) fSelected = true;
                    else fSelected = false;
                    DrawFactXCV(aCanvas, a, 0, 0, fSelected, operMode, 0,
                               (aRuleNet.aSelectedObject4Info != null && aRuleNet.aSelectedObject4Info == a) ? true : false);
                }
            }
            if (MainWindow.tp.fShowConclusion)
            {
                foreach (XConclusion a in aRuleNet.aConclusionList)
                {
                    if (aSelList != null && aSelList.Contains(a)) fSelected = true;
                    else fSelected = false;
                    DrawConclusionXCV(aCanvas, a, 0, 0, fSelected, operMode, 0,
                        (aRuleNet.aSelectedObject4Info != null && aRuleNet.aSelectedObject4Info == a) ? true : false);
                }
            }
            if (MainWindow.tp.fShowRule)
            {
                foreach (XRule a in aRuleNet.aRuleList)
                {
                    if (aSelList != null && aSelList.Contains(a)) fSelected = true;
                    else fSelected = false;
                    DrawRuleXCV(aCanvas, a, 0, 0, fSelected, operMode, 0,
                        (aRuleNet.aSelectedObject4Info != null && aRuleNet.aSelectedObject4Info == a) ? true : false);
                }
            }
            #endregion
        }

        //---------------------------------------------------------------
        //--xrat, yrat is used to calculate the Canvas size
        public static void DrawRuleNet0(WDrawingCanvas aCanvas, XRuleNet aRuleNet,
              List<object> aSelList, double xrat, double yrat, int operMode)
        {
            aCanvas.Clear();

            bool fSelected = false;
            if (aRuleNet == null || aRuleNet.aRuleList == null) return;
            XRuleNet.SetCanvasSize(aCanvas, aRuleNet, xrat, yrat);

            double incD = 0;
            foreach (XRule aRule in aRuleNet.aRuleList)  //--Width/Height <= 0
            {
                SetRuleSize(aRule, 1, incD);
            }
            if (MainWindow.tp.fShowFact)
            {
                foreach (XFact a in aRuleNet.aFactList)
                {
                    if (aSelList != null && aSelList.Contains(a)) fSelected = true;
                    else fSelected = false;
                    DrawFactXC(aCanvas, a, 0, 0, fSelected, operMode, 0,
                        (aRuleNet.aSelectedObject4Info != null && aRuleNet.aSelectedObject4Info == a) ? true : false);
                }
            }
            if (MainWindow.tp.fShowConclusion)
            {
                foreach (XConclusion a in aRuleNet.aConclusionList)
                {
                    if (aSelList != null && aSelList.Contains(a)) fSelected = true;
                    else fSelected = false;
                    DrawConclusionXC(aCanvas, a, 0, 0, fSelected, operMode, 0,
                        (aRuleNet.aSelectedObject4Info != null && aRuleNet.aSelectedObject4Info == a) ? true : false);
                }
            }
            if (MainWindow.tp.fShowRule)
            {
                foreach (XRule a in aRuleNet.aRuleList)
                {
                    if (aSelList != null && aSelList.Contains(a)) fSelected = true;
                    else fSelected = false;
                    DrawRuleXC(aCanvas, a, 0, 0, fSelected, operMode, 0,
                        (aRuleNet.aSelectedObject4Info != null && aRuleNet.aSelectedObject4Info == a) ? true : false);
                }
            }
        }

        /*==========================================================================
        oDrawModelInRectP(cMdl,tRect,wMode,fFrame,fIcon,tRat,mx,my)
        --Used in the Print 
        ==========================================================================*/
        public static bool DrawRuleNetInRect(
                            DrawingContext dc,
                            XRuleNet aRuleNet,
                            WRECT tRect,
                            char wMode,
                            int fFrame, /* 1:box only, 2:box+title, 0:none */
                            int fIcon,  /* If has draw icon flag: 1000:NO, other yes */
                            double tRat)  /* icon title ratio */
        {
            if (aRuleNet == null || aRuleNet.aRuleList == null) return false;
            XRuleNet.GetRuleNetSize(aRuleNet);

            double xrat = (tRect.right - tRect.left) / MainWindow.aXRuleNet.xMax;
            double yrat = (tRect.bottom - tRect.top) / MainWindow.aXRuleNet.yMax;

            DrawRuleNetDC(dc, aRuleNet, xrat, yrat, wMode);
            if (fFrame != 0)
            {
                WDrawingVisual.WDrawRectangle(dc, GlobalVarX.hDefBrush, GlobalVarX.hDefPen, tRect.left, tRect.top, tRect.right, tRect.bottom);
                if (fFrame > 1)
                {
                    XFont aXFont = new XFont();
                    if (xrat < 1.0) aXFont.FontSize = 16 * xrat;
                    else aXFont.FontSize = 16;
                    string stext = aRuleNet.Name;
                    double bx = tRect.left;
                    double by = tRect.top;
                    double ex = tRect.right;
                    double ey = tRect.bottom;
                    bool fSizeChanged = false;
                    FormattedText formattedText = WDrawingVisual.GetProperFormattedText(ref stext,
                                ref bx, ref by, ref ex, ref ey, aXFont, GlobalVarX.hDefPen, ref fSizeChanged);

                    WDrawingVisual.WDrawText(dc, formattedText, stext, bx, by, ex, ey, aXFont, GConstants.DT_CENTER | GConstants.DT_VCENTER, 0);
                }
            }
            return true;
        }

        //---------------------------------------------------------------
        //--xrat, yrat is used to calculate the Canvas size
        public static void DrawRuleNetDC(DrawingContext dc, XRuleNet aRuleNet,
              double xrat, double yrat, int operMode)
        {
            #region -- Links
            //--XFact
            if (MainWindow.tp.fShowFact && MainWindow.tp.fShowRule && MainWindow.tp.fShowFactLink)
            {
                foreach (XFact a in aRuleNet.aFactList)
                {
                    oDrawFactLinksDC(dc, a, 0, 0, "Green");
                }
            }

            //--XConclusion
            if (MainWindow.tp.fShowConclusion && MainWindow.tp.fShowRule && MainWindow.tp.fShowConclusionLink)
            {
                foreach (XConclusion a in aRuleNet.aConclusionList)
                {
                    oDrawConclusionLinksDC(a, dc, 0, 0, "Blue");
                }
            }
            //--XRule
            if (MainWindow.tp.fShowRule && MainWindow.tp.fShowRuleLink)
            {
                foreach (XRule a in aRuleNet.aRuleList)
                {
                    oDrawRuleLinksDC(dc, a, 0, 0, "Red");
                }
            }
            #endregion

            #region -- Items
            //--XFact
            if (MainWindow.tp.fShowFact)
            {
                foreach (XFact a in aRuleNet.aFactList)
                {
                    DrawFactX(dc, a, 0, 0, operMode, 0, false, false);
                }
            }

            //--XConclusion
            if (MainWindow.tp.fShowConclusion)
            {
                foreach (XConclusion a in aRuleNet.aConclusionList)
                {
                    DrawConclusionX(dc, a, 0, 0, operMode, 0, false, false);
                }
            }
            //--XRule
            if (MainWindow.tp.fShowRule)
            {
                foreach (XRule a in aRuleNet.aRuleList)
                {
                    DrawRuleX(dc, a, 0, 0, operMode, 0, false, false);
                }
            }
            #endregion
        }

        //------------------------------------------------------------
        public static void DrawRuleItem(WDrawingCanvas aCanvas, object a, int iOperMode, int iVisit)
        {
            if (a == null) return;
            if (a.GetType() == typeof(Rule))
            {
                XRule aRule = a as XRule;
                if (aRule != null)
                {
                    WDrawRule.DrawRuleXC(aCanvas, aRule, 0, 0, false, iOperMode, iVisit, false);
                }
            }
            else if (a.GetType() == typeof(XFact))
            {
                XFact aFact = a as XFact;
                if (aFact != null)
                {
                    WDrawRule.DrawFactXC(aCanvas, aFact, 0, 0, false, iOperMode, iVisit, false);
                }
            }
            else if (a.GetType() == typeof(XConclusion))
            {
                XConclusion aCon = a as XConclusion;
                if (aCon != null)
                {
                    WDrawRule.DrawConclusionXC(aCanvas, aCon, 0, 0, false, iOperMode, iVisit, false);
                }
            }
        }

        //------------------------------------------------------------
        public static void SetRuleSize(XRule aRule, double frat, double incD)
        {
            if (aRule.Width <= 0 || aRule.Height <= 0)
            {
                SetRuleSizeX(aRule, frat, incD);
            }
        }

        //------------------------------------------------------------
        public static void SetRuleSizeX(XRule aRule, double frat, double incD)
        {
            Pen pen = new Pen(new SolidColorBrush(WColor.GetColor(GlobalVarX.sRulePenColor)), 2);
            Brush brush = new SolidColorBrush(WColor.GetColor(GlobalVarX.sRuleBrushColor));

            double width = 0;
            double height = 0;
            WDrawingVisual.WMeasureString(pen, aRule.Conclusion, GlobalVarX.aRuleDefXFont, 0, ref width, ref height);
            aRule.Width = width + incD;
            aRule.Height = height + incD;
        }

        //---------------------------------------------------------------
        public static void DrawRuleXC(WDrawingCanvas aCanvas, XRule aRule, double adjx, double adjy, bool fSelected, int operMode, int iVisit, bool fInfoSelect)
        {
            DrawingVisual visual = new DrawingVisual();
            aRule.aDrawVisualX = visual;
            using (DrawingContext dc = visual.RenderOpen())
            {
                DrawRuleX(dc, aRule, adjx, adjy, operMode, iVisit, fSelected, fInfoSelect);
                if (fSelected)
                {
                    WDrawMarks(dc, aRule.X, aRule.Y, aRule.Width, aRule.Height, adjx, adjy);
                }
                if (MainWindow.tp.fShowRuleLink)
                {
                    if (fSelected) oDrawRuleLinksDC(dc, aRule, adjx, adjy, "Magenta");
                    else oDrawRuleLinksDC(dc, aRule, adjx, adjy, "Red");
                }
            }
            aCanvas.AddVisual(visual);
        }

        //---------------------------------------------------------------
        public static void DrawRuleXCV(WDrawingCanvas aCanvas, XRule aRule, double adjx, double adjy, bool fSelected, int operMode, int iVisit, bool fInfoSelect)
        {
            DrawingVisual visual = new DrawingVisual();
            aRule.aDrawVisualX = visual;
            using (DrawingContext dc = aRule.aDrawVisualX.RenderOpen())
            {
                DrawRuleX(dc, aRule, adjx, adjy, operMode, iVisit, fSelected, fInfoSelect);
                if (fSelected)
                {
                    WDrawMarks(dc, aRule.X, aRule.Y, aRule.Width, aRule.Height, adjx, adjy);
                }
            }
            aCanvas.AddVisual(aRule.aDrawVisualX);
        }

        //---------------------------------------------------------------
        public static void DrawRuleLinkXCV(WDrawingCanvas aCanvas, XRule aRule, double adjx, double adjy, bool fSelected)
        {
            DrawingVisual visual = new DrawingVisual();
            using (DrawingContext dc = visual.RenderOpen())
            {
                if (fSelected) oDrawRuleLinksDC(dc, aRule, adjx, adjy, "Magenta");
                else oDrawRuleLinksDC(dc, aRule, adjx, adjy, "Red");
            }
            aCanvas.AddVisual(visual);
        }

        //---------------------------------------------------------------
        public static void DrawRuleXDC(DrawingContext dc, XRule aRule, double adjx, double adjy, int operMode, int iVisit, bool fSelected, bool fInfoSelect)
        {
            DrawRuleX(dc, aRule, adjx, adjy, operMode, iVisit, fSelected, fInfoSelect);
            if (MainWindow.tp.fShowRuleLink)
            {
                if (fSelected) oDrawRuleLinksDC(dc, aRule, adjx, adjy, "Magenta");
                else oDrawRuleLinksDC(dc, aRule, adjx, adjy, "Red");
            }
        }

        // -----------------------------------------------------------------
        public static void oDrawRuleLinksDC(DrawingContext dc, XRule aRule, double adjx, double adjy, string lineColor)
        {
            if (aRule == null) return;
            if (aRule.aFromRules == null || aRule.aFromRules.Count <= 0) return;

            Pen pen = new Pen(new SolidColorBrush(WColor.GetColor(lineColor)), 1);
            double bx0 = adjx + aRule.X;
            double by0 = adjy + aRule.Y;
            double ex0 = bx0 + aRule.Width;
            double ey0 = by0 + aRule.Height;
            foreach (XRule r in aRule.aFromRules)
            {
                double bx1 = adjx + r.X;
                double by1 = adjy + r.Y;
                double ex1 = bx1 + r.Width;
                double ey1 = by1 + r.Height;
                List<Point> pointList = GetBestLinkLine(bx0, by0, ex0, ey0, bx1, by1, ex1, ey1);

                WDrawingVisual.WDrawBezierCurve(dc, null, pen, pointList);
                Brush brush = new SolidColorBrush(WColor.GetColor("Blue"));
                int i = 1;
                WDrawingVisual.WDrawArrowHead(dc, brush, pen, pointList[i].X, pointList[i].Y, pointList[i - 1].X, pointList[i - 1].Y, adjx, adjy, 1);
                //                WDrawingVisual.WDrawRectangle(dc, brush, pen, pointList[0].X - 2, pointList[0].Y - 2, pointList[0].X + 2, pointList[0].Y + 2);
            }
        }

        //---------------------------------------------------------------
        public static List<Point> GetBestLinkLine(double aBX, double aBY, double aEX, double aEY, double bBX, double bBY, double bEX, double bEY)
        {
            List<Point> aList = new List<Point>();
            if (bBX > aEX)
            {
                if (aBY - bEY > 30)
                {
                    aList.Add(new Point((aBX + aEX) / 2, aBY));
                    aList.Add(new Point((aBX + aEX) / 2, (aEY + bBY) / 2));
                    aList.Add(new Point((bBX + bEX) / 2, (aEY + bBY) / 2));
                    aList.Add(new Point((bBX + bEX) / 2, bEY));
                }
                else if (bBY - aEY > 30)
                {
                    aList.Add(new Point((aBX + aEX) / 2, aEY));
                    aList.Add(new Point((aBX + aEX) / 2, (aEY + bBY) / 2));
                    aList.Add(new Point((bBX + bEX) / 2, (aEY + bBY) / 2));
                    aList.Add(new Point((bBX + bEX) / 2, bBY));
                }
                else
                {
                    aList.Add(new Point(aEX, (aBY + aEY) / 2));
                    aList.Add(new Point((aEX + bBX) / 2, (aBY + aEY) / 2));
                    aList.Add(new Point((aEX + bBX) / 2, (bBY + bEY) / 2));
                    aList.Add(new Point(bBX, (bBY + bEY) / 2));
                }
            }
            else if (bEX < aBX)
            {
                if (aBY - bEY > 30)
                {
                    aList.Add(new Point((aBX + aEX) / 2, aBY));
                    aList.Add(new Point((aBX + aEX) / 2, (aEY + bBY) / 2));
                    aList.Add(new Point((bBX + bEX) / 2, (aEY + bBY) / 2));
                    aList.Add(new Point((bBX + bEX) / 2, bEY));
                }
                else if (bBY - aEY > 30)
                {
                    aList.Add(new Point((aBX + aEX) / 2, aEY));
                    aList.Add(new Point((aBX + aEX) / 2, (aEY + bBY) / 2));
                    aList.Add(new Point((bBX + bEX) / 2, (aEY + bBY) / 2));
                    aList.Add(new Point((bBX + bEX) / 2, bBY));
                }
                else
                {
                    aList.Add(new Point(aBX, (aBY + aEY) / 2));
                    aList.Add(new Point((aBX + bEX) / 2, (aBY + aEY) / 2));
                    aList.Add(new Point((aBX + bEX) / 2, (bBY + bEY) / 2));
                    aList.Add(new Point(bEX, (bBY + bEY) / 2));
                }
            }
            else if (bEY < aBY)
            {
                aList.Add(new Point((aBX + aEX) / 2, aBY));
                aList.Add(new Point((aBX + aEX) / 2, (aEY + bBY) / 2));
                aList.Add(new Point((bBX + bEX) / 2, (aEY + bBY) / 2));
                aList.Add(new Point((bBX + bEX) / 2, bEY));
            }
            else
            {
                aList.Add(new Point((aBX + aEX) / 2, aEY));
                aList.Add(new Point((aBX + aEX) / 2, (aEY + bBY) / 2));
                aList.Add(new Point((bBX + bEX) / 2, (aEY + bBY) / 2));
                aList.Add(new Point((bBX + bEX) / 2, bBY));
            }
            return aList;
        }

        //---------------------------------------------------------------
        public static void DrawRuleX(DrawingContext dc, XRule aRule, double adjx, double adjy, int operMode, int iVisit, bool fselected, bool fInfoSelect)
        {
            double bx = adjx + aRule.X;
            double by = adjy + aRule.Y;
            double ex = bx + aRule.Width;
            double ey = by + aRule.Height;

            Pen pen = new Pen(new SolidColorBrush(WColor.GetColor(GlobalVarX.sRulePenColor)), 1);
            if (fInfoSelect)
            {
                pen = new Pen(new SolidColorBrush(WColor.GetColor("Red")), 1);
            }
            string sBColor = "White";
            if (fselected) sBColor = "LightCyan";
            else if (aRule.Value >= 1) sBColor = "LightGreen";
            else if (aRule.Value == 0) sBColor = "Pink";
            Brush brushX = new SolidColorBrush(WColor.GetColor(sBColor));
            WDrawingVisual.WDrawTextBox(dc, brushX, pen, aRule.GetRuleValue(),
                                   bx, by, ex, ey,
                                   GlobalVarX.aRuleDefXFont, GConstants.DT_CENTER | GConstants.DT_VCENTER, 0);
            if (operMode == GConstants.WIN_RUN && iVisit != 0)
            {
                pen = new Pen(new SolidColorBrush(Color.FromRgb(200, 10, 20)), 2);
                Brush brush = new SolidColorBrush(Colors.Yellow);
                brush.Opacity = 0;
                WDrawingVisual.WDrawRectangle(dc, null, pen, bx, by, ex, ey);
            }
        }

        //---------------------------------------------------------------
        public static void DrawFactXC(WDrawingCanvas aCanvas, XFact aFact, double adjx, double adjy, bool fSelected, int operMode, int iVisit, bool fInfoSelect)
        {
            DrawingVisual visual = new DrawingVisual();
            aFact.aDrawVisualX = visual;
            using (DrawingContext dc = visual.RenderOpen())
            {
                DrawFactX(dc, aFact, adjx, adjy, operMode, iVisit, fSelected, fInfoSelect);
                if (fSelected)
                {
                    WDrawMarks(dc, aFact.X, aFact.Y, aFact.Width, aFact.Height, adjx, adjy);
                }
                if (MainWindow.tp.fShowRule && MainWindow.tp.fShowFactLink)
                {
                    if (fSelected) oDrawFactLinksDC(dc, aFact, adjx, adjy, "Magenta");
                    else oDrawFactLinksDC(dc, aFact, adjx, adjy, "Green");
                }
            }
            aCanvas.AddVisual(visual);
        }

        //---------------------------------------------------------------
        //--Visual, XFact only
        public static void DrawFactLinkXCV(WDrawingCanvas aCanvas, XFact aFact, double adjx, double adjy, bool fSelected)
        {
            DrawingVisual visual = new DrawingVisual();
            using (DrawingContext dc = visual.RenderOpen())
            {
                if (fSelected) oDrawFactLinksDC(dc, aFact, adjx, adjy, "Magenta");
                else oDrawFactLinksDC(dc, aFact, adjx, adjy, "Green");
            }
            aCanvas.AddVisual(visual);
        }

        //------------------------------------------------------------
        public static void DrawFactXCV(WDrawingCanvas aCanvas, XFact aFact, double adjx, double adjy, bool fSelected, int operMode, int iVisit, bool fInfoSelect)
        {
            DrawingVisual visual = new DrawingVisual();
            aFact.aDrawVisualX = visual;

            using (DrawingContext dc = aFact.aDrawVisualX.RenderOpen())
            {
                DrawFactX(dc, aFact, adjx, adjy, operMode, iVisit, fSelected, fInfoSelect);
                if (fSelected)
                {
                    WDrawMarks(dc, aFact.X, aFact.Y, aFact.Width, aFact.Height, adjx, adjy);
                }
            }
            aCanvas.AddVisual(aFact.aDrawVisualX);
        }

        //---------------------------------------------------------------
        public static void DrawFactXDC(DrawingContext dc, XFact aFact, double adjx, double adjy, int operMode, int iVisit, bool fSelect, bool fInfoSelect)
        {
            DrawFactX(dc, aFact, adjx, adjy, operMode, iVisit, fSelect, fInfoSelect);
            if (MainWindow.tp.fShowRule && MainWindow.tp.fShowFactLink)
            {
                if (fSelect) oDrawFactLinksDC(dc, aFact, adjx, adjy, "Magenta");
                else oDrawFactLinksDC(dc, aFact, adjx, adjy, "Green");
            }
        }

        //---------------------------------------------------------------
        public static void DrawFactX(DrawingContext dc, XFact aFact, double adjx, double adjy, int operMode, int iVisit, bool fselected, bool fInfoSelect)
        {
            double bx = adjx + aFact.X;
            double by = adjy + aFact.Y;
            double ex = adjx + (aFact.X + aFact.Width);
            double ey = adjy + (aFact.Y + aFact.Height);

            Pen pen = new Pen(new SolidColorBrush(WColor.GetColor(GlobalVarX.sRulePenColor)), 1);
            if (fInfoSelect)
            {
                pen = new Pen(new SolidColorBrush(WColor.GetColor("Red")), 1);
            }
            string sBColor = "White";
            if (fselected) sBColor = "LightCyan";
            else if (!String.IsNullOrEmpty(aFact.Value)) sBColor = "LightGreen";
            Brush brushX = new SolidColorBrush(WColor.GetColor(sBColor));
            WDrawingVisual.WDrawTextBox(dc, brushX, pen, aFact.GetFactValue(), bx, by, ex, ey,
                GlobalVarX.aRuleDefXFont, GConstants.DT_CENTER | GConstants.DT_VCENTER, 0);
            if (operMode == GConstants.WIN_RUN && iVisit != 0)
            {
                pen = new Pen(new SolidColorBrush(Color.FromRgb(200, 10, 20)), 2);
                //     Brush brush = new SolidColorBrush(Colors.Yellow);
                //     brush.Opacity = 0;
                WDrawingVisual.WDrawRectangle(dc, null, pen, bx, by, ex, ey);
            }
        }

        //---------------------------------------------------------------
        public static void DrawConclusionXC(WDrawingCanvas aCanvas, XConclusion aCon, double adjx, double adjy, bool fSelected, int operMode, int iVisit, bool fInfoSelect)
        {
            DrawingVisual visual = new DrawingVisual();
            aCon.aDrawVisualX = visual;
            using (DrawingContext dc = visual.RenderOpen())
            {
                DrawConclusionX(dc, aCon, adjx, adjy, operMode, iVisit, fSelected, fInfoSelect);
                if (fSelected)
                {
                    WDrawMarks(dc, aCon.X, aCon.Y, aCon.Width, aCon.Height, adjx, adjy);
                }
                if (MainWindow.tp.fShowRule && MainWindow.tp.fShowConclusionLink)
                {
                    if (fSelected) oDrawConclusionLinksDC(aCon, dc, adjx, adjy, "Magenta");
                    else oDrawConclusionLinksDC(aCon, dc, adjx, adjy, "Blue");
                }
            }

            aCanvas.AddVisual(visual);
        }

        //---------------------------------------------------------------
        public static void DrawConclusionXCV(WDrawingCanvas aCanvas, XConclusion aCon, double adjx, double adjy, bool fSelected, int operMode, int iVisit, bool fInfoSelect)
        {
            DrawingVisual visual = new DrawingVisual();
            aCon.aDrawVisualX = visual;

            using (DrawingContext dc = aCon.aDrawVisualX.RenderOpen())
            {
                DrawConclusionX(dc, aCon, adjx, adjy, operMode, iVisit, fSelected, fInfoSelect);
                if (fSelected)
                {
                    WDrawMarks(dc, aCon.X, aCon.Y, aCon.Width, aCon.Height, adjx, adjy);
                }
            }

            aCanvas.AddVisual(aCon.aDrawVisualX);
        }

        //---------------------------------------------------------------
        public static void DrawConclusionLinkXCV(WDrawingCanvas aCanvas, XConclusion aCon, double adjx, double adjy, bool fSelected)
        {
            DrawingVisual visual = new DrawingVisual();
            using (DrawingContext dc = visual.RenderOpen())
            {
                if (fSelected) oDrawConclusionLinksDC(aCon, dc, adjx, adjy, "Magenta");
                else oDrawConclusionLinksDC(aCon, dc, adjx, adjy, "Blue");
            }

            aCanvas.AddVisual(visual);
        }

        //---------------------------------------------------------------
        public static void DrawConclusionXDC(DrawingContext dc, XConclusion aCon, double adjx, double adjy, int operMode, int iVisit, bool fSelected, bool fInfoSelect)
        {
            DrawConclusionX(dc, aCon, adjx, adjy, operMode, iVisit, fSelected, fInfoSelect);
            if (MainWindow.tp.fShowRule && MainWindow.tp.fShowConclusionLink)
            {
                if (fSelected) oDrawConclusionLinksDC(aCon, dc, adjx, adjy, "Magenta");
                else oDrawConclusionLinksDC(aCon, dc, adjx, adjy, "Blue");
            }
        }

        //---------------------------------------------------------------
        public static void DrawConclusionX(DrawingContext dc, XConclusion aCon, double adjx, double adjy, int operMode, int iVisit, bool fselected, bool fInfoSelect)
        {
            double bx = adjx + aCon.X;
            double by = adjy + aCon.Y;
            double ex = adjx + (aCon.X + aCon.Width);
            double ey = adjy + (aCon.Y + aCon.Height);

            Pen pen = new Pen(new SolidColorBrush(WColor.GetColor(GlobalVarX.sRulePenColor)), 1);
            if (fInfoSelect)
            {
                pen = new Pen(new SolidColorBrush(Colors.Red), 1);
            }
            string sBColor = "White";
            if (fselected) sBColor = "LightCyan";
            else if (aCon.Value >= 1) sBColor = "LightGreen";
            else if (aCon.Value == 0) sBColor = "LightCyan";
            Brush brushX = new SolidColorBrush(WColor.GetColor(sBColor));
            WDrawingVisual.WDrawTextBox(dc, brushX, pen, aCon.GetConclusionValue(), bx, by, ex, ey,
                               GlobalVarX.aRuleDefXFont, GConstants.DT_CENTER | GConstants.DT_VCENTER, 0);
            if (operMode == GConstants.WIN_RUN && iVisit != 0)
            {
                pen = new Pen(new SolidColorBrush(Color.FromRgb(200, 10, 20)), 2);
                Brush brush = new SolidColorBrush(Colors.Yellow);
                brush.Opacity = 0;
                WDrawingVisual.WDrawRectangle(dc, null, pen, bx, by, ex, ey);
            }
        }

        /*---------------------------------------------------------------
         *         WDrawMarks(hDC,bx,by,ex,ey)
           ---1----
           |      2
           0      |
           |      3
           ---4----
        -----------------------------------------------------------------*/
        public static void WDrawMarks(DrawingContext dc, double X, double Y, double Width, double Height, double adjx, double adjy)
        {
            SolidColorBrush hBrush = new SolidColorBrush(Colors.Green);
            hBrush.Opacity = 0.3;
            var pen = new Pen(hBrush, 1);
            pen.DashStyle = DashStyles.Solid;
            WDrawBoxMarks(dc, pen, X, Y, Width, Height, adjx, adjy);
        }

        //---------------------------------------------------------------
        public static void WDrawBoxMarks(DrawingContext dc, Pen pen, double X, double Y, double Width, double Height, double adjx, double adjy)
        {
            double bx = adjx + X;
            double by = adjy + Y;
            double ex = bx + Width;
            double ey = by + Height;
            WDrawingVisual.WDrawRectangle(dc, Brushes.Transparent, pen, bx, by, ex, ey);
            List<Point> aPointList = new List<Point>();
            aPointList.Add(new Point(bx, by));  //--0
            aPointList.Add(new Point((bx + ex) / 2, by));  //--1
            aPointList.Add(new Point(ex, by));  //--2
            aPointList.Add(new Point(ex, (by + ey) / 2));  //--3
            aPointList.Add(new Point(ex, ey));  //--4
            aPointList.Add(new Point((bx + ex) / 2, ey));  //--5
            aPointList.Add(new Point(bx, ey));  //--6
            aPointList.Add(new Point(bx, (by + ey) / 2));  //--7
            foreach (Point a in aPointList)
            {
                WDrawingVisual.WDrawRectangle(dc, Brushes.Black, pen, a.X - 2, a.Y - 2, a.X + 2, a.Y + 2);
            }
        }

        //---------------------------------------------------------------
        public static void WDrawPointsMarks(DrawingContext dc, Pen pen, List<Point> aPointList)
        {
            if (aPointList == null || aPointList.Count <= 0) return;
            foreach (Point a in aPointList)
            {
                WDrawingVisual.WDrawRectangle(dc, Brushes.Red, pen, a.X - 2, a.Y - 2, a.X + 2, a.Y + 2);
            }
        }

        /* -----------------------------------------------------------------
        oDrawFactLinksDC(hDC,bSelPtr,adjx,adjy,xrat,yrat,wMode)
         ----------------------------------------------------------------- */
        public static void oDrawFactLinksDC(DrawingContext dc, XFact aFact, double adjx, double adjy, string lineColor)
        {
            if (aFact == null || aFact.ruleList == null) return;

            Pen pen = new Pen(new SolidColorBrush(WColor.GetColor(lineColor)), 1);
            double bx0 = adjx + aFact.X;
            double ex0 = bx0 + aFact.Width;
            double by0 = adjy + aFact.Y;
            double ey0 = by0 + +aFact.Height;
            if (aFact.ruleList != null)
            {
                foreach (XRule r in aFact.ruleList)
                {
                    double bx1 = adjx + r.X;
                    double ex1 = bx1 + r.Width;
                    double by1 = adjy + r.Y;
                    double ey1 = by1 + r.Height;
                    List<Point> pointList = GetBestLinkLine(bx0, by0, ex0, ey0, bx1, by1, ex1, ey1);
                    //               WDrawingVisual.WDrawPolyLine(dc, null, pen, pointList, fArrow);
                    WDrawingVisual.WDrawBezierCurve(dc, null, pen, pointList);
                    Brush brush = new SolidColorBrush(WColor.GetColor("Blue"));
                    int i = pointList.Count - 1;
                    WDrawingVisual.WDrawArrowHead(dc, brush, pen, pointList[i - 1].X, pointList[i - 1].Y, pointList[i].X, pointList[i].Y, adjx, adjy, 1);
                }
            }
        }

        /* -----------------------------------------------------------------
        oDrawConclusionLinksDC(hDC,bSelPtr,adjx,adjy,xrat,yrat,wMode)
         ----------------------------------------------------------------- */
        public static void oDrawConclusionLinksDC(XConclusion aCon, DrawingContext dc, double adjx, double adjy, string lineColor)
        {
            if (aCon == null || aCon.ruleList == null) return;

            Pen pen = new Pen(new SolidColorBrush(WColor.GetColor(lineColor)), 1);
            DrawingVisual visual = aCon.aDrawVisualX as DrawingVisual;
            double bx0 = adjx + aCon.X;
            double ex0 = bx0 + aCon.Width;
            double by0 = adjy + aCon.Y;
            double ey0 = by0 + +aCon.Height;
            if (aCon.ruleList != null)
            {
                foreach (XRule r in aCon.ruleList)
                {
                    double bx1 = adjx + r.X;
                    double ex1 = bx1 + r.Width;
                    double by1 = adjy + r.Y;
                    double ey1 = by1 + r.Height;
                    List<Point> pointList = GetBestLinkLine(bx0, by0, ex0, ey0, bx1, by1, ex1, ey1);
                    //       WDrawingVisual.WDrawPolyLine(dc, null, pen, pointList, fArrow);
                    WDrawingVisual.WDrawBezierCurve(dc, null, pen, pointList);
                    Brush brush = new SolidColorBrush(WColor.GetColor("Blue"));
                    int i = 1;
                    WDrawingVisual.WDrawArrowHead(dc, brush, pen, pointList[i].X, pointList[i].Y, pointList[i - 1].X, pointList[i - 1].Y, adjx, adjy, 1);
                }
            }
        }

        /*-----------------------------------------------------------------
        oGetLineCOORDS(itype,bx,by,ex,ey,lbx,lby,lex,ley)
        -----------------------------------------------------------------*/
        public static void oGetLineCOORDS(char itype, double bx, double by, double width, double height,
                         XRule aRule, ref double lbx, ref double lby, ref double lex, ref double ley)
        {
            if (itype == 'P')
            {
                lbx = aRule.X + aRule.Width / 2;
                lby = aRule.Y;
                lex = bx + width / 2;
                ley = by + height / 2;
            }
            else if (itype == 'T')
            {
                lbx = aRule.X + aRule.Width;
                lby = aRule.Y + aRule.Height / 3;
                lex = bx + width;
                ley = by + height / 2;
            }
            else if (itype == 'F')
            {
                lbx = aRule.X + aRule.Width;
                lby = aRule.Y + aRule.Height * 2 / 3;
                lex = bx + width;
                ley = by + height / 2;
            }
            else
            {
                lbx = aRule.X + aRule.Width;
                lby = aRule.Y + aRule.Height / 2;
                lex = bx + width;
                ley = by + height;
            }
        }
    }
}
