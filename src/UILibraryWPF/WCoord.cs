using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace OExpert.UILibraryWPF
{
    public class WCoord
    {
        public static int gMARK = 3;

        /*--------------------------------------------------------------------
        oGetTextExt(hDC,iFont,iSize,fStyle,textstr,flag,angle)
         * char flag;  -- interpretation flag 
        --------------------------------------------------------------------*/
        public static void oGetTextExt(string FontName, double iFontSize, string textstr, ref double lx, ref double ly)
        {
            /* want interpret the string */
            Size siz = WDrawingVisual.WMeasureString(textstr, FontName, iFontSize, 0, lx, ly);
            lx = siz.Width;
            ly = siz.Height;
        }

        /*--------------------------------------------------------------------
        oGetTextExt(hDC,iFont,iSize,fStyle,textstr,flag,angle)
        * char flag;  -- interpretation flag 
        --------------------------------------------------------------------*/
        public static void oGetTextExt(string FontName, double iFontSize, int fStyle, string textstr, int angle, ref double lx, ref double ly)
        {
            /* want interpret the string */
            Size siz = WDrawingVisual.WMeasureString(textstr, FontName, iFontSize, fStyle, lx, ly);
            lx = siz.Width;
            ly = siz.Height;
        }

        /*-----------------------------------------------------------------
        oRotateXYCoords(px,py,CX,CY,xrot,yrot)
        -----------------------------------------------------------------*/
        public static void oRotateXYCoords(
             ref double px,
             ref double py,  /* to be adjusted coords */
             double CX,
             double CY,  /* rotation center */
             double xrot,
             double yrot)  /* rotation ratios: xrot=9999:flip Hori, yrot=9999:flip Vert */
        {
            double nx, ny;

            if (xrot == 9999.0 || yrot == 9999.0)
            {  // flip Horizontal
                if (xrot == 9999.0)
                {  // flip Horizontal
                    nx = px - CX;
                    px = CX - nx;
                }
                if (yrot == 9999.0)
                {  // flip Vertical
                    ny = py - CY;
                    py = CY - ny;
                }
            }
            else if (xrot != 0.0 || yrot != 0.0)
            {
                nx = px - CX;
                ny = py - CY;
                px = (CX + nx * xrot + ny * yrot);
                py = (CY + ny * xrot - nx * yrot);
            }
        }

        /*==========================================================================
        oGetRotateParm(RotAngle,bx,by,ex,ey,CX,CY,xrot,yrot)
        ==========================================================================*/
        public static bool oGetRotateParm(
                            double RotAngle,
                            double bx,
                            double by,
                            double ex,
                            double ey,
                            ref double CX,
                            ref double CY,
                            ref double xrot,
                            ref double yrot)
        {
            double gAng;
            long x;

            CX = CY = 0;
            xrot = yrot = 0.0;
            if (RotAngle != 0.0)
            {
                CX = (bx + ex) / 2;
                CY = (by + ey) / 2;
                if (RotAngle < 9000)
                {
                    gAng = RotAngle / 180.0 * Math.PI;
                    x = (long)(100000 * Math.Cos(gAng));
                    xrot = (double)x / 100000;
                    x = (long)(100000 * Math.Sin(gAng));
                    yrot = (double)x / 100000;
                }
                else
                {
                    if (((int)(RotAngle - 9000) & 1) > 0)
                    {
                        xrot = 9999.0;
                    }
                    if (((int)(RotAngle - 9000) & 2) > 0)
                    {
                        yrot = 9999.0;
                    }
                }
                return true;
            }
            else return false;
        }

        /*--------------------------------------------------------------------
        FUNCT: oCorrectPoly(oPoly,adjx,adjy,xrat,yrat)
          Calculate the Polygon's enclosing rectangle
        ----------------------------------------------------------------------*/
        public static void oCorrectPoly(List<WPOINT> ptList,
             double adjx,
             double adjy,
             double xrat,
             double yrat)
        {
            foreach (WPOINT p in ptList)
            {
                p.X = (adjx + xrat * p.X);
                p.Y = (adjy + yrat * p.Y);
            }
        }

        /*--------------------------------------------------------------------
        FUNCT: oCalcPolyRect(oPoly,tRect,xrat,yrat)
          Calculate the Polygon's enclosing rectangle
          as well as adjust to relative
        ----------------------------------------------------------------------*/
        public static WRECT oCalcPolyRect(List<WPOINT> ptList, double xrat, double yrat)
        {
            /* calc the poly binding rect */
            WRECT tRect = new WRECT(0, 0, 0, 0);
            if (ptList == null || ptList.Count <= 0) return tRect;
            double bx = 30000;
            double by = 30000;
            double ex = -30000;
            double ey = -30000;
            foreach (WPOINT p in ptList)
            {
                if (p.X < bx) bx = p.X;
                if (p.X > ex) ex = p.X;

                if (p.Y < by) by = p.Y;
                if (p.Y > ey) ey = p.Y;
            }
            tRect.left = (bx / xrat);
            tRect.top = (by / yrat);
            tRect.right = (ex / xrat);
            tRect.bottom = (ey / yrat);
            /* recalc the poly coords for relative to rect */
            foreach (WPOINT p in ptList)
            {
                p.X = ((p.X - tRect.left) / xrat);
                p.Y = ((p.Y - tRect.top) / yrat);
            }
            return tRect;
        }

        /*--------------------------------------------------------------------
        FUNCT: CalcPolyRect(oPoly,xrat,yrat)
          Calculate the Polyline's enclosing rectangle
          as well as adjust to relative
        ----------------------------------------------------------------------*/
        public static Rect CalcPolyRect(List<Point> ptList, double xrat, double yrat)
        {
            /* calc the poly binding rect */
            if (ptList == null || ptList.Count <= 0) return new Rect(0, 0, 0, 0);
            double bx = Double.MaxValue;
            double by = Double.MaxValue;
            double ex = Double.MinValue;
            double ey = Double.MinValue;
            foreach (Point p in ptList)
            {
                if (p.X < bx) bx = p.X;
                if (p.X > ex) ex = p.X;

                if (p.Y < by) by = p.Y;
                if (p.Y > ey) ey = p.Y;
            }
            Rect tRect = new Rect(bx / xrat, by / yrat, ex / xrat, ey / yrat);
            /* recalc the poly coords for relative to rect */
            for (int i = 0; i < ptList.Count; i++)
            {
                ptList[i] = new Point((ptList[i].X - tRect.Left) / xrat, (ptList[i].Y - tRect.Top) / yrat);
            }
            return tRect;
        }

        /*--------------------------------------------------------------------
        FUNCT: GetPolyRect(oPoly,xrat,yrat)
          Calculate the Polyline's enclosing rectangle
          as well as adjust to relative
        ----------------------------------------------------------------------*/
        public static Rect GetPolyRect(List<Point> ptList)
        {
            /* calc the poly binding rect */
            if (ptList == null || ptList.Count <= 0) return new Rect(0, 0, 0, 0);
            double bx = Double.MaxValue;
            double by = Double.MaxValue;
            double ex = Double.MinValue;
            double ey = Double.MinValue;
            foreach (Point p in ptList)
            {
                if (p.X < bx) bx = p.X;
                if (p.X > ex) ex = p.X;

                if (p.Y < by) by = p.Y;
                if (p.Y > ey) ey = p.Y;
            }
            return new Rect(bx, by, ex - bx, ey - by);
        }

        /*--------------------------------------------------------------------
        oResizePolyXY(bParm,xrat,yrat)
        ----------------------------------------------------------------------*/
        public static bool oResizePolyXY(string xyStr, double xrat, double yrat)
        {
            if (String.IsNullOrEmpty(xyStr)) return false;
            List<WPOINT> aPtList = WPOINT.Parse(xyStr);
            oResizePolyXY(aPtList, xrat, yrat);
            return true;
        }

        /*--------------------------------------------------------------------
        oResizePolyXY(bParm,xrat,yrat)
        ----------------------------------------------------------------------*/
        public static bool oResizePolyXY(List<WPOINT> ptList, double xrat, double yrat)
        {
            if (ptList == null || ptList.Count <= 0) return false;
            foreach (WPOINT p in ptList)
            {
                p.X = (p.X * xrat);
                p.Y = (p.Y * yrat);
            }
            return true;
        }

        /*--------------------------------------------------------------------
        oResizePolyXY(bParm,xrat,yrat)
        ----------------------------------------------------------------------*/
        public static List<Point> oResizePoints(List<Point> ptList, double xrat, double yrat)
        {
            if (ptList == null || ptList.Count <= 0) return null;
            List<Point> aNewPtList = new List<Point>();
            foreach (Point p in ptList)
            {
                aNewPtList.Add(new Point(p.X * xrat, p.Y * yrat));
            }
            return aNewPtList;
        }

        /*---------------------------------------------------------------------------
        oResizeCoords(iSel,bx,by,ex,ey,xrat,yrat)
         1---2---3
         |       |
         8       4
         |       |
         7---6---5
        ---------------------------------------------------------------------------*/
        public static void oResizeCoords(int iSel, ref double bx, ref double by, ref double ex, ref double ey, double xrat, double yrat)
        {
            if (iSel == 1)
            {
                bx = ex - ((ex - bx) * xrat);
                by = ey - ((ey - by) * yrat);
            }
            else if (iSel == 2)
            {
                by = ey - ((ey - by) * yrat);
            }
            else if (iSel == 3)
            {
                ex = bx + ((ex - bx) * xrat);
                by = ey - ((ey - by) * yrat);
            }
            else if (iSel == 4)
            {
                ex = bx + ((ex - bx) * xrat);
            }
            else if (iSel == 5)
            {
                ex = bx + ((ex - bx) * xrat);
                ey = by + ((ey - by) * yrat);
            }
            else if (iSel == 6)
            {
                ey = by + ((ey - by) * yrat);
            }
            else if (iSel == 7)
            {
                bx = ex - ((ex - bx) * xrat);
                ey = by + ((ey - by) * yrat);
            }
            else if (iSel == 8)
            {
                bx = ex - ((ex - bx) * xrat);
            }
            else
            {
                bx = (bx * xrat);
                by = (by * yrat);
                ex = (ex * xrat);
                ey = (ey * yrat);
            }
        }

        /*==========================================================================
        oAdjRectByRotate(RotAngle,bx,by,ex,ey)
        ==========================================================================*/
        public static void oAdjRectByRotate(double RotAngle, ref double bx, ref double by, ref double ex, ref double ey)
        {
            double CX, CY;
            double xrot, yrot, gAng;

            if (RotAngle != 0.0 && RotAngle < 9000)
            {
                CX = (bx + ex) / 2;
                CY = (by + ey) / 2;
                gAng = RotAngle / 180.0 * Math.PI;
                xrot = Math.Cos(gAng);
                yrot = Math.Sin(gAng);
                oRotateXYCoords(ref bx, ref by, CX, CY, xrot, yrot);
                oRotateXYCoords(ref ex, ref ey, CX, CY, xrot, yrot);
            }
            oXyCoordSwap(ref bx, ref by, ref ex, ref ey);
        }

        /*--------------------------------------------------------------------
        oBoundPtInRect(px,py,bx,by,ex,ey)
        ---------------------------------------------------------------------*/
        public static void oBoundPtInRect(ref double px, ref double py, double bx, double by, double ex, double ey)
        {
            if (px < bx) px = bx;
            if (px > ex) px = ex;
            if (py < by) py = by;
            if (py > ey) py = ey;
        }

        /*-----------------------------------------------------------------
        oXyCoordSwap(bx,by,ex,ey)
        -----------------------------------------------------------------*/
        public static void oXyCoordSwap(ref double bx, ref double by, ref double ex, ref double ey)
        {
            double tvalue;

            if (bx > ex)
            {   /* swap x coord */
                tvalue = bx;
                bx = ex;
                ex = tvalue;
            }
            if (by > ey)
            {   /* swap y coord */
                tvalue = by;
                by = ey;
                ey = tvalue;
            }
        }

        /*-----------------------------------------------------------------
        oRectCoordSwap(Rect)
        -----------------------------------------------------------------*/
        public static void oRectCoordSwap(WRECT tRect)
        {
            double tvalue;

            if (tRect.left > tRect.right)
            {   /* swap x coord */
                tvalue = tRect.left;
                tRect.left = tRect.right;
                tRect.right = tvalue;
            }
            if (tRect.top > tRect.bottom)
            {   /* swap y coord */
                tvalue = tRect.top;
                tRect.top = tRect.bottom;
                tRect.bottom = tvalue;
            }
        }


        /*---------------------------------------------------------------------
        MaxCoords(mRect,x,y)
        -------------------------------------------------------------------- */
        public static void MaxCoords(
                        WRECT mRect,  /*min/max rect coords */
                        double x,
                        double y) /*to be tested coords */
        {
            if (x < mRect.left) mRect.left = x;
            if (x > mRect.right) mRect.right = x;
            if (y < mRect.top) mRect.top = y;
            if (y > mRect.bottom) mRect.bottom = y;
        }

        /*---------------------------------------------------------------------
        MaxCoords2(mRect,x,y)
        -------------------------------------------------------------------- */
        public static void MaxCoords2(
                        WRECT mRect,  /*min/max rect coords */
                        double x,
                        double y,
                        double x1,
                        double y1) /*to be tested coords */
        {
            if (x < mRect.left) mRect.left = x;
            if (x > mRect.right) mRect.right = x;
            if (y < mRect.top) mRect.top = y;
            if (y > mRect.bottom) mRect.bottom = y;
            if (x1 < mRect.left) mRect.left = x1;
            if (x1 > mRect.right) mRect.right = x1;
            if (y1 < mRect.top) mRect.top = y1;
            if (y1 > mRect.bottom) mRect.bottom = y1;
        }

        /*------------------------------------------------------------------
        FUNCT: oChkMinMax(bx,by,MinX,MinY,MaxX,MaxY)
          Pauses for a specified number of microseconds.
        ------------------------------------------------------------------*/
        public static bool oChkMinMax(
                        double bx,
                        double by,  /* to be checked coords */
                        ref double MinX,
                        ref double MinY, /* to be checked against min coords */
                        ref double MaxX,
                        ref double MaxY) /* to be checked against max coords */
        {
            bool iupdate = false;

            if (bx < MinX)
            {
                iupdate = true;
                MinX = bx;
            }
            if (bx > MaxX)
            {
                iupdate = true;
                MaxX = bx;
            }
            if (by < MinY)
            {
                iupdate = true;
                MinY = by;
            }
            if (by > MaxY)
            {
                iupdate = true;
                MaxY = by;
            }
            return iupdate;
        }

        /*------------------------------------------------------------------
        FUNCT: oChkMinMax2(bx,by,ex,ey,MinX,MinY,MaxX,MaxY)
          Pauses for a specified number of microseconds.
        ------------------------------------------------------------------*/
        public static bool oChkMinMax2(
                        double bx,
                        double by,
                        double ex,
                        double ey,  /* to be checked coords */
                        ref double MinX,
                        ref double MinY, /* to be checked against min coords */
                        ref double MaxX,
                        ref double MaxY) /* to be checked against max coords */
        {
            bool iupdate = false;
            oXyCoordSwap(ref bx, ref by, ref ex, ref ey);

            if (bx < MinX)
            {
                iupdate = true;
                MinX = bx;
            }
            if (by < MinY)
            {
                iupdate = true;
                MinY = by;
            }
            if (ex > MaxX)
            {
                iupdate = true;
                MaxX = ex;
            }
            if (ey > MaxY)
            {
                iupdate = true;
                MaxY = ey;
            }
            return iupdate;
        }

        /*--------------------------------------------------------------------
        FUNCT: GetWPOINTsRect(oPoly,mGphElmt)
        ---------------------------------------------------------------------*/
        public static void GetWPOINTsRect(List<WPOINT> ptList, ref double bx, ref double by, ref double ex, ref double ey)
        {
            WRECT tRect = oCalcPolyRect(ptList, 1.0, 1.0);
            bx = tRect.left;
            by = tRect.top;
            ex = tRect.right;
            ey = tRect.bottom;
        }

        //-------------------------------------------------------------
        public static WRECT oUnionRect(WRECT r1, WRECT r2)
        {
            WRECT x = new WRECT(r1);
            if (x.left > r2.left) x.left = r2.left;
            if (x.right < r2.right) x.right = r2.right;
            if (x.top > r2.top) x.top = r2.top;
            if (x.bottom < r2.bottom) x.bottom = r2.bottom;
            return x;
        }

        /*--------------------------------------------------------------------
        FUNCT: oAdjCoords(bx,by,ex,ey,iSel,xrat,yrat)
               1---2---3
               8-     -4
               7---6---5
        ----------------------------------------------------------------------*/
        public static void oAdjCoords(
            ref double bx,
            ref double by,
            ref double ex,
            ref double ey,
            int iSel,
            double xrat,
            double yrat)
        {
            if (iSel == 1)
            {
                bx = (ex) - ((ex - bx) * xrat);
                by = (ey) - ((ey - by) * yrat);
            }
            else if (iSel == 2)
            {
                by = (ey) - ((ey - by) * yrat);
            }
            else if (iSel == 3)
            {
                ex = (bx) + ((ex - bx) * xrat);
                by = (ey) - ((ey - by) * yrat);
            }
            else if (iSel == 4)
            {
                ex = (bx) + ((ex - bx) * xrat);
            }
            else if (iSel == 5)
            {
                ex = (bx) + ((ex - bx) * xrat);
                ey = (by) + ((ey - by) * yrat);
            }
            else if (iSel == 6)
            {
                ey = (by) + ((ey - by) * yrat);
            }
            else if (iSel == 7)
            {
                bx = (ex) - ((ex - bx) * xrat);
                ey = (by) + ((ey - by) * yrat);
            }
            else if (iSel == 8)
            {
                bx = (ex) - ((ex - bx) * xrat);
            }
            else
            {
                Console.WriteLine("ERROR: wrong iSel: " + iSel);
            }
        }


        /*---------------------------------------------------------------------------
        oFlipXyToOrg(iSel,x0,y0,x1,y1)
          1---2---3
          |       |
          8       4
          |       |
          7---6---5
        ---------------------------------------------------------------------------*/
        public static void oFlipXyToOrg(
         int iSel,
         ref double x0,
            ref double y0,
            ref double x1,
            ref double y1)
        {
            double tvalue;

            if (iSel == 1)
            {  /* flip x0=>x1, y0=>y1 */
                tvalue = x1;
                x1 = x0;
                x0 = tvalue;
                tvalue = y1;
                y1 = y0;
                y0 = tvalue;
            }
            else if (iSel == 2 || iSel == 3)
            {  /* flip y0=>y1 */
                tvalue = y1;
                y1 = y0;
                y0 = tvalue;
            }
            else if (iSel == 4 || iSel == 5 || iSel == 6) return;
            else if (iSel == 7 || iSel == 8)
            {  /* flip x0=>x1 */
                tvalue = x1;
                x1 = x0;
                x0 = tvalue;
            }
        }


        //-------------------------------------------------------------------------
        //--This is a rectangle (2 points)
        public static List<Point> FlipPoints(List<Point> aList)
        {
            bool fchange = false;
            Point p1 = aList[0];
            Point p2 = aList[1];
            double x1 = p1.X, x2 = p2.X;
            double y1 = p1.Y, y2 = p2.Y;
            if (p2.X < p1.X)
            {
                x1 = p2.X;
                x2 = p1.X;
                fchange = true;
            }
            if (p2.Y < p1.Y)
            {
                y1 = p2.Y;
                y2 = p1.Y;
                fchange = true;
            }
            if (fchange)
            {
                List<Point> rList = new List<Point>();
                rList.Add(new Point(x1, y1));
                rList.Add(new Point(x2, y2));
                return rList;
            }
            else return aList;
        }

        /*==========================================================================
        oCalcXYRot(RotAngle,xrot,yrot)
        ==========================================================================*/
        public static bool oCalcXYRot(
                    double RotAngle,
                    ref double xrot,
                    ref double yrot)
        {
            double gAng;

            xrot = yrot = 0.0;
            if (RotAngle == 0.0) return false;
            if (RotAngle == 90.0) { xrot = 0.0; yrot = 1.0; }
            else if (RotAngle == -90.0) { xrot = 0.0; yrot = -1.0; }
            else if (RotAngle == 180.0) { xrot = 1.0; yrot = 0.0; }
            else if (RotAngle == -180.0) { xrot = -1.0; yrot = 0.0; }
            else if (RotAngle == 270.0) { xrot = 0.0; yrot = -1.0; }
            else if (RotAngle == -270.0) { xrot = 0.0; yrot = 1.0; }
            else
            {
                gAng = RotAngle / 180.0 * Math.PI;
                xrot = Math.Cos(gAng);
                yrot = Math.Sin(gAng);
            }
            return true;
        }

        /*-----------------------------------------------------------------
        oIsPointsInRect(bx,by,ex,ey,tRect)
        -----------------------------------------------------------------*/
        public static bool oIsPointsInRect(
        double bx, double by, double ex, double ey,
        WRECT tRect)  /*---Assume in right order */
        {
            oXyCoordSwap(ref bx, ref by, ref ex, ref ey);
            if (ex < tRect.left || bx > tRect.right || ey < tRect.top || by > tRect.bottom) return false;
            else return true;
        }

        /*-----------------------------------------------------------------
        oIsPointsInRectX(bx,by,ex,ey,tRect)
        -----------------------------------------------------------------*/
        public static bool oIsPointsInRectX(
        double bx, double by, double ex, double ey,
        WRECT tRect)  /*---Assume in right order */
        {

            oXyCoordSwap(ref bx, ref by, ref ex, ref ey);

            if (bx < tRect.left || bx > tRect.right) return false;
            if (ex < tRect.left || ex > tRect.right) return false;
            if (by < tRect.top || by > tRect.bottom) return false;
            if (ey < tRect.top || ey > tRect.bottom) return false;
            return true;
        }

        public static bool IsPointInRect(double cx, double cy, double bx, double by, double ex, double ey)
        {
            oXyCoordSwap(ref bx, ref by, ref ex, ref ey);
            if (cx < bx || bx > ex) return false;
            if (cy < by || by > ey) return false;
            return true;
        }

        /*--------------------------------------------------------------------
        oSetRotAngle(RotAngle,iopt)
        --------------------------------------------------------------------*/
        public static void oSetRotAngle(
                            ref double RotAngle,
                            int iopt)  /* 0:Hori, 1:Vert */
        {
            int k, v, v2;

            k = (int)(RotAngle - 9000.0);
            if (k < 0) k = 0;
            if (iopt == 0)
            {
                if ((k & 2) != 0) v2 = 2;
                else v2 = 0;
                if ((k & 1) != 0) v = 0;
                else v = 1;
            }
            else
            {
                if ((k & 1) != 0) v2 = 1;
                else v2 = 0;
                if ((k & 2) != 0) v = 0;
                else v = 2;
            }
            if (v != 0 || v2 != 0) RotAngle = 9000.0 + v + v2;
            else RotAngle = 0.0;
        }

        //----------------------------------------------------------------------------------------
        public static List<Point> AdjustRect(List<Point> aPtList, int iSel, double dx, double dy)
        {
            double bx = aPtList[0].X;
            double by = aPtList[0].Y;
            double ex = aPtList[1].X;
            double ey = aPtList[1].Y;
            if (iSel == 0)
            {
                bx += dx;
                by += dy;
            }
            else if (iSel == 1)
            {
                by += dy;
            }
            else if (iSel == 2)
            {
                ex += dx;
                by += dy;
            }
            else if (iSel == 3)
            {
                ex += dx;
            }
            else if (iSel == 4)
            {
                ex += dx;
                ey += dy;
            }
            else if (iSel == 5)
            {
                ey += dy;
            }
            else if (iSel == 6)
            {
                bx += dx;
                ey += dy;
            }
            else if (iSel == 7)
            {
                bx += dx;
            }
            List<Point> aList = new List<Point>();
            aList.Add(new Point(bx, by));
            aList.Add(new Point(ex, ey));
            return aList;
        }

        //-----------------------------------------------------------------
        public static Point GetDXDY(List<Point> aPtList, int iSel, List<Point> aNewPtList)
        {
            double dx = 0;
            double dy = 0;
            if (iSel == 0)
            {
                dx = aNewPtList[0].X - aPtList[0].X;
                dy = aNewPtList[0].Y - aPtList[0].Y;
            }
            else if (iSel == 1)
            {
                dy = aNewPtList[0].Y - aPtList[0].Y;
            }
            else if (iSel == 2)
            {
                dx = aNewPtList[1].X - aPtList[1].X;
                dy = aNewPtList[0].Y - aPtList[0].Y;
            }
            else if (iSel == 3)
            {
                dx = aNewPtList[1].X - aPtList[1].X;
            }
            else if (iSel == 4)
            {
                dx = aNewPtList[1].X - aPtList[1].X;
                dy = aNewPtList[1].Y - aPtList[1].Y;
            }
            else if (iSel == 5)
            {
                dy = aNewPtList[1].Y - aPtList[1].Y;
            }
            else if (iSel == 6)
            {
                dx = aNewPtList[0].X - aPtList[0].X;
                dy = aNewPtList[1].Y - aPtList[1].Y;
            }
            else if (iSel == 7)
            {
                dx = aNewPtList[0].X - aPtList[0].X;
            }
            return new Point(dx, dy);
        }

        /*-----------------------------------------------------------------
        FUNCT: oChkInLine(cx,cy,x0,y0,x1,y1)
        -----------------------------------------------------------------*/
        public static bool oChkInLine(
         double cx, double cy,
         double x0, double y0, double x1, double y1)
        {
            double tx0, ty0, tx1, ty1;
            double iy;
            double slope;

            if (x0 >= x1 - gMARK && x0 <= x1 + gMARK)
            {
                oXyCoordSwap(ref x0, ref y0, ref x1, ref y1);
                if (cx >= x0 - 2 * gMARK && cx <= x0 + 2 * gMARK && cy >= y0 && cy <= y1) return true;
                else return false;
            }
            else if (y0 >= y1 - gMARK && y0 <= y1 + gMARK)
            {
                oXyCoordSwap(ref x0, ref y0, ref x1, ref y1);
                if (cx >= x0 && cx <= x1 && cy >= y0 - 2 * gMARK && cy <= y0 + 2 * gMARK) return true;
                else return false;
            }
            else
            {
                tx0 = x0; ty0 = y0; tx1 = x1; ty1 = y1;
                oXyCoordSwap(ref tx0, ref ty0, ref tx1, ref ty1);
                if (cx < tx0 - gMARK || cx > tx1 + gMARK || cy < ty0 - gMARK || cy > ty1 + gMARK) return false;
                if (x1 == x0 || y1 == y0) return true;
                slope = (double)(y1 - y0) / (double)(x1 - x0);
                iy = y0 + (double)(cx - x0) * slope;
                if (cy >= iy - gMARK && cy <= iy + gMARK) return true;
                else return false;
            }
        }

        /*-----------------------------------------------------------------
        FUNCT: oRectInRect(bRect,tRect)
        -----------------------------------------------------------------*/
        public static bool oRectInRect(
        WRECT bRect,
        WRECT tRect)
        {
            if (tRect.left >= bRect.left && tRect.left <= bRect.right &&
               tRect.right >= bRect.left && tRect.right <= bRect.right &&
               tRect.top >= bRect.top && tRect.top <= bRect.bottom &&
               tRect.bottom >= bRect.top && tRect.bottom <= bRect.bottom)
                return true;
            else return false;
        }
    }
}
