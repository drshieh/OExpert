using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using static System.Net.Mime.MediaTypeNames;

namespace OExpert.UILibraryWPF
{
    public class WDrawingVisual
    {

        //----------------------------------------------------------------------
        public static void WDrawLine(DrawingContext dc, Pen pen, double bx, double by, double ex, double ey)
        {
            if (pen == null) pen = new Pen(Brushes.Black, 1);
            dc.DrawLine(pen, new Point(bx, by), new Point(ex, ey));
        }

        //----------------------------------------------------------------------
        public static void WDrawLine(DrawingContext dc, Pen pen, double bx, double by, double ex, double ey, int arwCornFit)
        {
            if (pen == null) pen = new Pen(Brushes.Black, 1);
            dc.DrawLine(pen, new Point(bx, by), new Point(ex, ey));

            if (arwCornFit == GlobalVarX.gARROW_BOTH)
            {
                WDrawingVisual.WDrawArrowHead(dc, GlobalVarX.hDefBrush, pen, bx, by, ex, ey, 0, 0, pen.Thickness);  //--End
                WDrawingVisual.WDrawArrowHead(dc, GlobalVarX.hDefBrush, pen, ex, ey, bx, by, 0, 0, pen.Thickness);  //--Head
            }
            else if (arwCornFit == GlobalVarX.gARROW_HEAD)
            {
                WDrawingVisual.WDrawArrowHead(dc, GlobalVarX.hDefBrush, pen, ex, ey, bx, by, 0, 0, pen.Thickness);  //--Head
            }
            else if (arwCornFit == GlobalVarX.gARROW_END)
            {
                WDrawingVisual.WDrawArrowHead(dc, GlobalVarX.hDefBrush, pen, bx, by, ex, ey, 0, 0, pen.Thickness);  //--End
            }
        }

        //----------------------------------------------------------------------
        public static void WDrawRectangle(DrawingContext dc, Brush brush, Pen pen, double bx, double by, double ex, double ey)
        {
            //                if (pen == null) pen = new Pen(Brushes.Black, 3);
            //                if (brush == null) brush = Brushes.Wheat;
            FlipCoords(ref bx, ref by, ref ex, ref ey);
            if (brush == null)
            {
                dc.DrawRectangle(Brushes.Transparent, pen, new Rect(bx, by, ex - bx, ey - by));
            }
            else
            {
                dc.DrawRectangle(brush, pen, new Rect(bx, by, ex - bx, ey - by));
            }
        }

        //----------------------------------------------------------------------
        public static void WDrawRectangle(DrawingContext dc, LinearGradientBrush brush, Pen pen, double bx, double by, double ex, double ey)
        {
            FlipCoords(ref bx, ref by, ref ex, ref ey);
            if (brush == null)
            {
                dc.DrawRectangle(Brushes.Transparent, pen, new Rect(bx, by, ex - bx, ey - by));
            }
            else
            {
                dc.DrawRectangle(brush, pen, new Rect(bx, by, ex - bx, ey - by));
            }
        }

        //----------------------------------------------------------------------
        public static void WDrawRoundedRectangle(DrawingContext dc, Brush brush, Pen pen, double bx, double by, double ex, double ey, string mxyStr)
        {
            if (pen == null) pen = new Pen(Brushes.Black, 3);
            if (brush == null) brush = Brushes.Wheat;
            FlipCoords(ref bx, ref by, ref ex, ref ey);
            List<Point> aPointList = WDrawingVisual.GetCoords(mxyStr, 0, 0, 0, 0);  //--No bx,by adjustment
            double radiusX = 4;
            double radiusY = 4;
            if (aPointList != null && aPointList.Count > 0)
            {
                radiusX = aPointList[0].X;
                radiusY = aPointList[0].Y;
            }
            dc.DrawRoundedRectangle(brush, pen, new Rect(bx, by, ex - bx, ey - by), radiusX, radiusY);
        }

        //----------------------------------------------------------------------
        public static void WDrawRoundedRectangle(DrawingContext dc, Brush brush, Pen pen, double bx, double by, double ex, double ey, double radiusX, double radiusY)
        {
            if (pen == null) pen = new Pen(Brushes.Black, 3);
            if (brush == null) brush = Brushes.Wheat;
            FlipCoords(ref bx, ref by, ref ex, ref ey);
            dc.DrawRoundedRectangle(brush, pen, new Rect(bx, by, ex - bx, ey - by), radiusX, radiusY);
        }

        //----------------------------------------------------------------------
        public static void WDrawEllipse(DrawingContext dc, Brush brush, Pen pen, double bx, double by, double ex, double ey)
        {
            if (pen == null) pen = new Pen(Brushes.Black, 3);
            if (brush == null) brush = Brushes.Wheat;
            SwapCoord(ref bx, ref by, ref ex, ref ey);
            double CX = (bx + ex) / 2;
            double CY = (by + ey) / 2;
            double radiusX = (ex - bx) / 2;
            double radiusY = (ey - by) / 2;
            dc.DrawEllipse(brush, pen, new Point(CX, CY), radiusX, radiusY);
        }

        //----------------------------------------------------------------------
        public static List<Point> PointsBoundByAngle(double bx, double by, double ex, double ey, List<Point> aPointList)
        {
            if (bx == ex || by == ey) return aPointList;
            double radiusX = Math.Abs(ex - bx) / 2;
            double radiusY = Math.Abs(ey - by) / 2;
            double CX = bx + radiusX;
            double CY = by + radiusY;
            List<Point> aList = new List<Point>();
            foreach (Point p in aPointList)
            {
                int begQuad = 1;
                double begAng = GetAngle(p.X - bx, p.Y - by, radiusX, radiusY, ref begQuad);
                aList.Add(new Point(CX + radiusX * Math.Cos(begAng), CY + radiusY * Math.Sin(begAng)));
            }
            return aList;
        }

        //----------------------------------------------------------------------
        public static void PointBoundByAngle(double bx, double by, double ex, double ey, ref double x, ref double y)
        {
            if (bx == ex || by == ey) return;
            double radiusX = Math.Abs(ex - bx) / 2;
            double radiusY = Math.Abs(ey - by) / 2;
            double CX = bx + radiusX;
            double CY = by + radiusY;
            int begQuad = 1;
            double begAng = GetAngle(x - bx, y - by, radiusX, radiusY, ref begQuad);
            x = CX + radiusX * Math.Cos(begAng);
            y = CY + radiusY * Math.Sin(begAng);
        }

        //----------------------------------------------------------------------
        public static void WDrawArc(DrawingContext dc, Brush brush, Pen pen, double bx, double by, double ex, double ey, string mxyStr, double RotAngle)
        {
            if (bx == ex || by == ey) return;
            if (pen == null) pen = new Pen(Brushes.Transparent, 1);
            if (brush == null) brush = Brushes.Transparent;
            List<PathSegment> pSegList = new List<PathSegment>();
            double radiusX = Math.Abs(ex - bx) / 2;
            double radiusY = Math.Abs(ey - by) / 2;
            double CX = bx + radiusX;
            double CY = by + radiusY;
            List<Point> aPointList = WDrawingVisual.GetCoords(mxyStr, bx, by, ex, ey);
            int begQuad = 1;
            double begAng = GetAngle(aPointList[0].X - bx, aPointList[0].Y - by, radiusX, radiusY, ref begQuad);
            double begX = CX + radiusX * Math.Cos(begAng);
            double begY = CY + radiusY * Math.Sin(begAng);
            int endQuad = 1;
            double endAng = GetAngle(aPointList[1].X - bx, aPointList[1].Y - by, radiusX, radiusY, ref endQuad);
            double endX = CX + radiusX * Math.Cos(endAng);
            double endY = CY + radiusY * Math.Sin(endAng);
            Point ePoint = new Point(endX, endY);
            bool isLargeArc = CheckIsLargeArc(begQuad, endQuad, begAng, endAng);
            ArcSegment arc = new ArcSegment(ePoint, new Size(radiusX, radiusY), RotAngle, isLargeArc, SweepDirection.Clockwise, true);
            pSegList.Add(arc);
            Point bPoint = new Point(begX, begY);
            PathFigure f = new PathFigure(bPoint, pSegList, false);
            PathGeometry pathg = new PathGeometry();
            pathg.Figures.Add(f);
            dc.DrawGeometry(brush, pen, pathg);
        }

        //----------------------------------------------------------------------
        public static bool CheckIsLargeArc(int begQuad, int endQuad, double begAng, double endAng)
        {
            bool isLargeArc = false;
            if (endQuad == begQuad && begAng >= endAng) isLargeArc = true;
            else
            {
                if (begQuad == 1)
                {
                    if (endQuad == 2) isLargeArc = false;
                    if (endQuad == 3)
                    {
                        if ((endAng - begAng) >= Math.PI) isLargeArc = true;
                    }
                    if (endQuad == 4) isLargeArc = true;
                }
                else if (begQuad == 2)
                {
                    if (endQuad == 3) isLargeArc = false;
                    if (endQuad == 4)
                    {
                        if ((endAng - begAng) >= Math.PI) isLargeArc = true;
                    }
                    if (endQuad == 1) isLargeArc = true;
                }
                else if (begQuad == 3)
                {
                    if (endQuad == 4) isLargeArc = false;
                    if (endQuad == 1)
                    {
                        if ((endAng - begAng) >= Math.PI) isLargeArc = true;
                    }
                    if (endQuad == 2) isLargeArc = true;
                }
                else if (begQuad == 4)
                {
                    if (endQuad == 1) isLargeArc = false;
                    if (endQuad == 2)
                    {
                        if (Math.PI * 2 - Math.Abs(endAng - begAng) >= Math.PI) isLargeArc = true;
                    }
                    if (endQuad == 3) isLargeArc = true;
                }
            }
            return isLargeArc;
        }

        //----------------------------------------------------------------------
        public static double GetAngle(double x, double y, double radiusX, double radiusY, ref int iquadrant)
        {
            iquadrant = 1;
            if (x > radiusX && y > radiusY) iquadrant = 1; //-4
            else if (x < radiusX && y >= radiusY) iquadrant = 2; //--3
            else if (x < radiusX && y < radiusY) iquadrant = 3;  //--2
            else if (x >= radiusX && y <= radiusY) iquadrant = 4;  //--1
            double aAng = Math.Abs(Math.Atan((y - radiusY) / (x - radiusX)));
            if (iquadrant == 1) return aAng;
            else if (iquadrant == 2) return Math.PI - aAng;
            else if (iquadrant == 3) return Math.PI + aAng;
            else return 2 * Math.PI - aAng;
        }

        //----------------------------------------------------------------------
        public static void WDrawChord(DrawingContext dc, Brush brush, Pen pen, double bx, double by, double ex, double ey, string mxyStr, double RotAngle)
        {
            if (bx == ex || by == ey) return;
            List<Point> aPointList = WDrawingVisual.GetCoords(mxyStr, bx, by, ex, ey);
            WDrawChord(dc, brush, pen, bx, by, ex, ey, aPointList, RotAngle);
        }

        //----------------------------------------------------------------------
        public static void WDrawChord(DrawingContext dc, Brush brush, Pen pen, double bx, double by, double ex, double ey, List<Point> aPointList, double RotAngle)
        {
            if (bx == ex || by == ey) return;
            if (pen == null) pen = new Pen(Brushes.Transparent, 1);
            if (brush == null) brush = Brushes.Transparent;
            List<PathSegment> pSegList = new List<PathSegment>();
            double radiusX = Math.Abs(ex - bx) / 2;
            double radiusY = Math.Abs(ey - by) / 2;
            double CX = bx + radiusX;
            double CY = by + radiusY;
            int begQuad = 1;
            double begAng = GetAngle(aPointList[0].X - bx, aPointList[0].Y - by, radiusX, radiusY, ref begQuad);
            double begX = CX + radiusX * Math.Cos(begAng);
            double begY = CY + radiusY * Math.Sin(begAng);
            int endQuad = 1;
            double endAng = GetAngle(aPointList[1].X - bx, aPointList[1].Y - by, radiusX, radiusY, ref endQuad);
            double endX = CX + radiusX * Math.Cos(endAng);
            double endY = CY + radiusY * Math.Sin(endAng);
            Point ePoint = new Point(endX, endY);
            bool isLargeArc = CheckIsLargeArc(begQuad, endQuad, begAng, endAng);
            ArcSegment arc = new ArcSegment(ePoint, new Size(radiusX, radiusY), RotAngle, isLargeArc, SweepDirection.Clockwise, true);
            pSegList.Add(arc);
            Point bPoint = new Point(begX, begY);
            PathFigure f = new PathFigure(bPoint, pSegList, true);
            PathGeometry pathg = new PathGeometry();
            pathg.Figures.Add(f);
            dc.DrawGeometry(brush, pen, pathg);
        }

        //----------------------------------------------------------------------
        public static void WDrawPie(DrawingContext dc, Brush brush, Pen pen, double bx, double by, double ex, double ey, string mxyStr, double RotAngle)
        {
            if (bx == ex || by == ey) return;
            List<Point> aPointList = WDrawingVisual.GetCoords(mxyStr, bx, by, ex, ey);
            WDrawPie(dc, brush, pen, bx, by, ex, ey, aPointList, RotAngle);
        }

        //----------------------------------------------------------------------
        public static void WDrawPie(DrawingContext dc, Brush brush, Pen pen, double bx, double by, double ex, double ey, List<Point> aPointList, double RotAngle)
        {
            if (bx == ex || by == ey) return;
            if (pen == null) pen = new Pen(Brushes.Transparent, 1);
            if (brush == null) brush = Brushes.Transparent;
            List<PathSegment> pSegList = new List<PathSegment>();
            double radiusX = Math.Abs(ex - bx) / 2;
            double radiusY = Math.Abs(ey - by) / 2;
            double CX = bx + radiusX;
            double CY = by + radiusY;
            Point cPoint = new Point(CX, CY);
            int begQuad = 1;
            double begAng = GetAngle(aPointList[0].X - bx, aPointList[0].Y - by, radiusX, radiusY, ref begQuad);
            double begX = CX + radiusX * Math.Cos(begAng);
            double begY = CY + radiusY * Math.Sin(begAng);
            int endQuad = 1;
            double endAng = GetAngle(aPointList[1].X - bx, aPointList[1].Y - by, radiusX, radiusY, ref endQuad);
            double endX = CX + radiusX * Math.Cos(endAng);
            double endY = CY + radiusY * Math.Sin(endAng);
            Point ePoint = new Point(endX, endY);
            Point bPoint = new Point(begX, begY);
            LineSegment line1 = new LineSegment(bPoint, true);
            pSegList.Add(line1);
            bool isLargeArc = CheckIsLargeArc(begQuad, endQuad, begAng, endAng);
            ArcSegment arc = new ArcSegment(ePoint, new Size(radiusX, radiusY), RotAngle, isLargeArc, SweepDirection.Clockwise, true);
            pSegList.Add(arc);
            LineSegment line2 = new LineSegment(cPoint, true);
            pSegList.Add(line2);
            PathFigure f = new PathFigure(cPoint, pSegList, true);
            PathGeometry pathg = new PathGeometry();
            pathg.Figures.Add(f);
            dc.DrawGeometry(brush, pen, pathg);
        }

        //----------------------------------------------------------------------
        public static void WDrawPort(DrawingContext dc, Brush brush, Pen pen, double bx, double by, double ex, double ey)
        {
            if (bx == ex || by == ey) return;
            int iQuad = 0;
            double xAng = GetAngle(ex - bx, ey - by, 6, 6, ref iQuad);
            xAng *= 180 / Math.PI;  //in degree

            if (pen == null) pen = new Pen(Brushes.Transparent, 1);
            if (brush == null) brush = Brushes.Transparent;
            double CX = (bx + ex) / 2;
            double CY = (by + ey) / 2;

            dc.PushTransform(new RotateTransform(xAng, CX, CY));

            dc.DrawRectangle(brush, pen, new Rect(CX, by, ex - CX, ey - by));
            dc.DrawLine(pen, new Point(bx, CY), new Point(ex, CY));

            // Remove transformation for rotation as its no longer needed.
            dc.Pop();
        }

        //----------------------------------------------------------------------
        public static void WDrawPort(DrawingContext dc, Brush brush, Pen pen, double bx, double by, double CX, double CY, double RotAngle)
        {
            if (pen == null) pen = new Pen(Brushes.Transparent, 1);
            if (brush == null) brush = Brushes.Transparent;

            if (RotAngle != 0)
            {
                dc.PushTransform(new RotateTransform(RotAngle, CX, CY));
            }
            dc.DrawRectangle(brush, pen, new Rect(bx + CX, by + CY - 2, 3, 4));
            dc.DrawLine(pen, new Point(bx + CX - 3, by + CY), new Point(bx + CX + 3, by + CY));

            // Remove transformation for rotation as its no longer needed.
            if (RotAngle != 0) dc.Pop();
        }

        //----------------------------------------------------------------------
        public static void WDrawBezierCurve(DrawingContext dc, Brush brush, Pen pen, double bx, double by, double ex, double ey, string sXy)
        {
            List<Point> pointList = GetCoords(sXy, bx, by, ex, ey);
            WDrawBezierCurve(dc, brush, pen, pointList);
        }

        //----------------------------------------------------------------------
        public static void WDrawBezierCurve(DrawingContext dc, Brush brush, Pen pen, List<Point> pointList)
        {
            if (pointList == null || pointList.Count <= 1) return;
            if (pointList.Count == 2)
            {
                WDrawPolyLine(dc, brush, pen, pointList);
                return;
            }
            else if (pointList.Count == 3)
            {
                WDrawQuadraticBezierCurve(dc, brush, pen, pointList);
                return;
            }
            else if (pointList.Count > 4)
            {
                WDrawPolyBezier(dc, brush, pen, pointList);
                return;
            }
            if (pen == null) pen = new Pen(Brushes.Black, 3);
            GeometryGroup groupg = new GeometryGroup();
            List<PathSegment> pSegList = new List<PathSegment>();
            BezierSegment bezier = new BezierSegment(pointList[1], pointList[2], pointList[3], true);
            pSegList.Add(bezier);
            PathFigure f = new PathFigure(pointList[0], pSegList, false);
            PathGeometry pathg = new PathGeometry();
            pathg.Figures.Add(f);
            groupg.Children.Add(pathg);

            dc.DrawGeometry(brush, pen, groupg);
        }


        //----------------------------------------------------------------------
        public static void WDrawBezierCurve(DrawingContext dc, Brush brush, Pen pen, double bx, double by, double ex, double ey, string sXy, int arwCornFit)
        {
            List<Point> pointList = GetCoords(sXy, bx, by, ex, ey);
            WDrawBezierCurve(dc, brush, pen, pointList, arwCornFit);
        }

        //----------------------------------------------------------------------
        public static void WDrawBezierCurve(DrawingContext dc, Brush brush, Pen pen, List<Point> pointList, int arwCornFit)
        {
            if (pointList == null || pointList.Count <= 1) return;
            if (pointList.Count == 2)
            {
                WDrawPolyLine(dc, brush, pen, pointList, arwCornFit);
                return;
            }
            else if (pointList.Count == 3)
            {
                WDrawQuadraticBezierCurve(dc, brush, pen, pointList, arwCornFit);
                return;
            }
            else if (pointList.Count > 4)
            {
                WDrawPolyBezier(dc, brush, pen, pointList, arwCornFit);
                return;
            }
            if (pen == null) pen = new Pen(Brushes.Black, 3);
            GeometryGroup groupg = new GeometryGroup();
            List<PathSegment> pSegList = new List<PathSegment>();
            BezierSegment bezier = new BezierSegment(pointList[1], pointList[2], pointList[3], true);
            pSegList.Add(bezier);
            PathFigure f = new PathFigure(pointList[0], pSegList, false);
            PathGeometry pathg = new PathGeometry();
            pathg.Figures.Add(f);
            groupg.Children.Add(pathg);

            dc.DrawGeometry(brush, pen, groupg);
            int n = pointList.Count - 1;
            if (arwCornFit == GlobalVarX.gARROW_BOTH)
            {
                WDrawingVisual.WDrawArrowHead(dc, brush, pen, pointList[n - 1].X, pointList[n - 1].Y, pointList[n].X, pointList[n].Y, 0, 0, pen.Thickness);  //--End
                WDrawingVisual.WDrawArrowHead(dc, brush, pen, pointList[1].X, pointList[1].Y, pointList[0].X, pointList[0].Y, 0, 0, pen.Thickness);  //--Head
            }
            else if (arwCornFit == GlobalVarX.gARROW_HEAD)
            {
                WDrawingVisual.WDrawArrowHead(dc, brush, pen, pointList[1].X, pointList[1].Y, pointList[0].X, pointList[0].Y, 0, 0, pen.Thickness);  //--Head
            }
            else if (arwCornFit == GlobalVarX.gARROW_END)
            {
                WDrawingVisual.WDrawArrowHead(dc, brush, pen, pointList[n - 1].X, pointList[n - 1].Y, pointList[n].X, pointList[n].Y, 0, 0, pen.Thickness);  //--End
            }
        }

        //----------------------------------------------------------------------
        public static void WDrawQuadraticBezierCurve(DrawingContext dc, Brush brush, Pen pen, double bx, double by, double ex, double ey, string sXy)
        {
            List<Point> pointList = GetCoords(sXy, bx, by, ex, ey);
            WDrawQuadraticBezierCurve(dc, brush, pen, pointList);
        }

        //----------------------------------------------------------------------
        public static void WDrawQuadraticBezierCurve(DrawingContext dc, Brush brush, Pen pen, List<Point> pointList)
        {
            if (pointList == null || pointList.Count <= 0) return;
            if (pointList.Count < 3)
            {
                WDrawPolyLine(dc, brush, pen, pointList);
            }
            else if (pointList.Count > 3)
            {
                WDrawPolyQuadraticBezier(dc, brush, pen, pointList);
            }
            else
            {
                if (pen == null) pen = new Pen(Brushes.Black, 3);
                GeometryGroup groupg = new GeometryGroup();
                List<PathSegment> pSegList = new List<PathSegment>();
                QuadraticBezierSegment bezier = new QuadraticBezierSegment(pointList[1], pointList[2], true);
                pSegList.Add(bezier);
                PathFigure f = new PathFigure(pointList[0], pSegList, false);
                PathGeometry pathg = new PathGeometry();
                pathg.Figures.Add(f);
                groupg.Children.Add(pathg);

                dc.DrawGeometry(brush, pen, groupg);
            }
        }

        //----------------------------------------------------------------------
        public static void WDrawQuadraticBezierCurve(DrawingContext dc, Brush brush, Pen pen, double bx, double by, double ex, double ey, string sXy, int arwCornFit)
        {
            List<Point> pointList = GetCoords(sXy, bx, by, ex, ey);
            WDrawQuadraticBezierCurve(dc, brush, pen, pointList, arwCornFit);
        }

        //----------------------------------------------------------------------
        public static void WDrawQuadraticBezierCurve(DrawingContext dc, Brush brush, Pen pen, List<Point> pointList, int arwCornFit)
        {
            if (pointList == null || pointList.Count <= 0) return;
            if (pointList.Count < 3)
            {
                WDrawPolyLine(dc, brush, pen, pointList, arwCornFit);
            }
            else if (pointList.Count > 3)
            {
                WDrawPolyQuadraticBezier(dc, brush, pen, pointList, arwCornFit);
            }
            else
            {
                if (pen == null) pen = new Pen(Brushes.Black, 3);
                GeometryGroup groupg = new GeometryGroup();
                List<PathSegment> pSegList = new List<PathSegment>();
                QuadraticBezierSegment bezier = new QuadraticBezierSegment(pointList[1], pointList[2], true);
                pSegList.Add(bezier);
                PathFigure f = new PathFigure(pointList[0], pSegList, false);
                PathGeometry pathg = new PathGeometry();
                pathg.Figures.Add(f);
                groupg.Children.Add(pathg);

                dc.DrawGeometry(brush, pen, groupg);
                int n = pointList.Count - 1;
                if (arwCornFit == GlobalVarX.gARROW_BOTH)
                {
                    WDrawingVisual.WDrawArrowHead(dc, brush, pen, pointList[n - 1].X, pointList[n - 1].Y, pointList[n].X, pointList[n].Y, 0, 0, pen.Thickness);  //--End
                    WDrawingVisual.WDrawArrowHead(dc, brush, pen, pointList[1].X, pointList[1].Y, pointList[0].X, pointList[0].Y, 0, 0, pen.Thickness);  //--Head
                }
                else if (arwCornFit == GlobalVarX.gARROW_HEAD)
                {
                    WDrawingVisual.WDrawArrowHead(dc, brush, pen, pointList[1].X, pointList[1].Y, pointList[0].X, pointList[0].Y, 0, 0, pen.Thickness);  //--Head
                }
                else if (arwCornFit == GlobalVarX.gARROW_END)
                {
                    WDrawingVisual.WDrawArrowHead(dc, brush, pen, pointList[n - 1].X, pointList[n - 1].Y, pointList[n].X, pointList[n].Y, 0, 0, pen.Thickness);  //--End
                }
            }
        }

        //----------------------------------------------------------------------
        public static void WDrawPolyLine(DrawingContext dc, Brush brush, Pen pen, double bx, double by, double ex, double ey, string sXy)
        {
            List<Point> pointList = GetCoords(sXy, bx, by, ex, ey);
            WDrawPolyLine(dc, brush, pen, pointList);
        }

        //----------------------------------------------------------------------
        public static void WDrawPolyLine(DrawingContext dc, Brush brush, Pen pen, List<Point> pointList)
        {
            if (pointList == null || pointList.Count <= 0) return;
            if (pen == null) pen = new Pen(Brushes.Black, 3);
            GeometryGroup groupg = new GeometryGroup();

            List<PathSegment> pSegList = new List<PathSegment>();
            PolyLineSegment poly = new PolyLineSegment(pointList, true);
            pSegList.Add(poly);
            PathFigure f = new PathFigure(pointList[0], pSegList, false);
            PathGeometry pathg = new PathGeometry();
            pathg.Figures.Add(f);
            groupg.Children.Add(pathg);

            dc.DrawGeometry(brush, pen, groupg);
        }

        //----------------------------------------------------------------------
        public static void WDrawPolyLine(DrawingContext dc, Brush brush, Pen pen, double bx, double by, double ex, double ey, string sXy, int arwCornFit)
        {
            List<Point> pointList = GetCoords(sXy, bx, by, ex, ey);
            WDrawPolyLine(dc, brush, pen, pointList, arwCornFit);
        }

        //----------------------------------------------------------------------
        public static void WDrawPolyLine(DrawingContext dc, Brush brush, Pen pen, List<Point> pointList, int Arrow)
        {
            if (pointList == null || pointList.Count <= 0) return;
            if (pen == null) pen = new Pen(Brushes.Black, 3);
            GeometryGroup groupg = new GeometryGroup();

            List<PathSegment> pSegList = new List<PathSegment>();
            PolyLineSegment poly = new PolyLineSegment(pointList, true);
            pSegList.Add(poly);
            PathFigure f = new PathFigure(pointList[0], pSegList, false);
            PathGeometry pathg = new PathGeometry();
            pathg.Figures.Add(f);
            groupg.Children.Add(pathg);

            dc.DrawGeometry(brush, pen, groupg);
            int n = pointList.Count - 1;
            if (Arrow == GlobalVarX.gARROW_BOTH)
            {
                WDrawingVisual.WDrawArrowHead(dc, brush, pen, pointList[n - 1].X, pointList[n - 1].Y, pointList[n].X, pointList[n].Y, 0, 0, pen.Thickness);  //--End
                WDrawingVisual.WDrawArrowHead(dc, brush, pen, pointList[1].X, pointList[1].Y, pointList[0].X, pointList[0].Y, 0, 0, pen.Thickness);  //--Head
            }
            else if (Arrow == GlobalVarX.gARROW_HEAD)
            {
                WDrawingVisual.WDrawArrowHead(dc, brush, pen, pointList[1].X, pointList[1].Y, pointList[0].X, pointList[0].Y, 0, 0, pen.Thickness);  //--Head
            }
            else if (Arrow == GlobalVarX.gARROW_END)
            {
                WDrawingVisual.WDrawArrowHead(dc, brush, pen, pointList[n - 1].X, pointList[n - 1].Y, pointList[n].X, pointList[n].Y, 0, 0, pen.Thickness);  //--End
            }
        }

        //----------------------------------------------------------------------
        public static void WDrawPolygon(DrawingContext dc, Brush brush, Pen pen, double bx, double by, double ex, double ey, string sXy)
        {
            List<Point> pointList = GetCoords(sXy, bx, by, ex, ey);
            WDrawPolygon(dc, brush, pen, pointList);
        }

        //----------------------------------------------------------------------
        public static void WDrawPolygon(DrawingContext dc, Brush brush, Pen pen, List<Point> pointList)
        {
            if (pointList == null || pointList.Count <= 0) return;
            if (pen == null) pen = new Pen(Brushes.Black, 2);
            if (brush == null) brush = Brushes.Wheat;
            GeometryGroup groupg = new GeometryGroup();

            List<PathSegment> pSegList = new List<PathSegment>();
            PolyLineSegment poly = new PolyLineSegment(pointList, true);
            pSegList.Add(poly);
            PathFigure f = new PathFigure(pointList[0], pSegList, true);
            PathGeometry pathg = new PathGeometry();
            pathg.Figures.Add(f);
            groupg.Children.Add(pathg);

            dc.DrawGeometry(brush, pen, groupg);
        }

        //----------------------------------------------------------------------
        public static void WDrawPolyBezier(DrawingContext dc, Brush brush, Pen pen, double bx, double by, double ex, double ey, string sXy)
        {
            List<Point> pointList = GetCoords(sXy, bx, by, ex, ey);
            WDrawPolyBezier(dc, brush, pen, pointList);
        }

        //----------------------------------------------------------------------
        public static void WDrawPolyBezier(DrawingContext dc, Brush brush, Pen pen, List<Point> pointList)
        {
            List<Point> pointListX = new List<Point>();
            List<Point> endPointList = null;
            int nend = (pointList.Count - 1) % 3;
            for (int i = 1; i < pointList.Count - nend; i++)
            {
                pointListX.Add(new Point(pointList[i].X, pointList[i].Y));
            }
            if (nend != 0)
            {
                int ib = pointList.Count - nend - 1;
                endPointList = new List<Point>();
                for (int i = ib; i < pointList.Count; i++)
                {
                    endPointList.Add(new Point(pointList[i].X, pointList[i].Y));
                }
            }
            if (pen == null) pen = new Pen(Brushes.Black, 3);
            if (brush == null) brush = Brushes.Wheat;
            GeometryGroup groupg = new GeometryGroup();
            Point bPoint = new Point(pointList[0].X, pointList[0].Y);
            List<PathSegment> pSegList = new List<PathSegment>();
            PolyBezierSegment polyBezier = new PolyBezierSegment(pointListX, true);
            pSegList.Add(polyBezier);
            PathFigure f = new PathFigure(bPoint, pSegList, false);
            PathGeometry pathg = new PathGeometry();
            pathg.Figures.Add(f);
            groupg.Children.Add(pathg);

            dc.DrawGeometry(brush, pen, groupg);
            if (nend == 1)
            {
                WDrawPolyLine(dc, brush, pen, endPointList);
            }
            else if (nend == 2)
            {
                WDrawQuadraticBezierCurve(dc, brush, pen, endPointList);
            }
        }

        //----------------------------------------------------------------------
        public static void WDrawPolyBezier(DrawingContext dc, Brush brush, Pen pen, double bx, double by, double ex, double ey, string sXy, int arwCornFit)
        {
            List<Point> pointList = GetCoords(sXy, bx, by, ex, ey);
            WDrawPolyBezier(dc, brush, pen, pointList, arwCornFit);
        }

        //----------------------------------------------------------------------
        public static void WDrawPolyBezier(DrawingContext dc, Brush brush, Pen pen, List<Point> pointList, int arwCornFit)
        {
            List<Point> pointListX = new List<Point>();
            List<Point> endPointList = null;
            int nend = (pointList.Count - 1) % 3;
            for (int i = 1; i < pointList.Count - nend; i++)
            {
                pointListX.Add(new Point(pointList[i].X, pointList[i].Y));
            }
            if (nend != 0)
            {
                int ib = pointList.Count - nend - 1;
                endPointList = new List<Point>();
                for (int i = ib; i < pointList.Count; i++)
                {
                    endPointList.Add(new Point(pointList[i].X, pointList[i].Y));
                }
            }
            if (pen == null) pen = new Pen(Brushes.Black, 3);
            if (brush == null) brush = Brushes.Wheat;
            GeometryGroup groupg = new GeometryGroup();
            Point bPoint = new Point(pointList[0].X, pointList[0].Y);
            List<PathSegment> pSegList = new List<PathSegment>();
            PolyBezierSegment polyBezier = new PolyBezierSegment(pointListX, true);
            pSegList.Add(polyBezier);
            PathFigure f = new PathFigure(bPoint, pSegList, false);
            PathGeometry pathg = new PathGeometry();
            pathg.Figures.Add(f);
            groupg.Children.Add(pathg);

            dc.DrawGeometry(brush, pen, groupg);
            if (nend == 1)
            {
                WDrawPolyLine(dc, brush, pen, endPointList);
            }
            else if (nend == 2)
            {
                WDrawQuadraticBezierCurve(dc, brush, pen, endPointList);
            }
            int n = pointList.Count - 1;
            if (arwCornFit == GlobalVarX.gARROW_BOTH)
            {
                WDrawingVisual.WDrawArrowHead(dc, brush, pen, pointList[n - 1].X, pointList[n - 1].Y, pointList[n].X, pointList[n].Y, 0, 0, pen.Thickness);  //--End
                WDrawingVisual.WDrawArrowHead(dc, brush, pen, pointList[1].X, pointList[1].Y, pointList[0].X, pointList[0].Y, 0, 0, pen.Thickness);  //--Head
            }
            else if (arwCornFit == GlobalVarX.gARROW_HEAD)
            {
                WDrawingVisual.WDrawArrowHead(dc, brush, pen, pointList[1].X, pointList[1].Y, pointList[0].X, pointList[0].Y, 0, 0, pen.Thickness);  //--Head
            }
            else if (arwCornFit == GlobalVarX.gARROW_END)
            {
                WDrawingVisual.WDrawArrowHead(dc, brush, pen, pointList[n - 1].X, pointList[n - 1].Y, pointList[n].X, pointList[n].Y, 0, 0, pen.Thickness);  //--End
            }
        }

        //----------------------------------------------------------------------
        public static void WDrawPolyQuadraticBezier(DrawingContext dc, Brush brush, Pen pen, double bx, double by, double ex, double ey, string sXy)
        {
            List<Point> pointList = GetCoords(sXy, bx, by, ex, ey);
            WDrawPolyQuadraticBezier(dc, brush, pen, pointList);
        }

        //----------------------------------------------------------------------
        public static void WDrawPolyQuadraticBezier(DrawingContext dc, Brush brush, Pen pen, List<Point> pointList)
        {
            List<Point> pointListX = new List<Point>();
            List<Point> endPointList = null;
            int nend = (pointList.Count - 1) % 2;
            for (int i = 1; i < pointList.Count - nend; i++)
            {
                pointListX.Add(new Point(pointList[i].X, pointList[i].Y));
            }
            if (nend != 0)
            {
                int ib = pointList.Count - nend - 1;
                endPointList = new List<Point>();
                for (int i = ib; i < pointList.Count; i++)
                {
                    endPointList.Add(new Point(pointList[i].X, pointList[i].Y));
                }
            }
            if (pen == null) pen = new Pen(Brushes.Black, 3);
            if (brush == null) brush = Brushes.Wheat;
            GeometryGroup groupg = new GeometryGroup();
            Point bPoint = new Point(pointList[0].X, pointList[0].Y);
            List<PathSegment> pSegList = new List<PathSegment>();
            PolyQuadraticBezierSegment polyBezier = new PolyQuadraticBezierSegment(pointListX, true);
            pSegList.Add(polyBezier);
            PathFigure f = new PathFigure(bPoint, pSegList, false);
            PathGeometry pathg = new PathGeometry();
            pathg.Figures.Add(f);
            groupg.Children.Add(pathg);
            dc.DrawGeometry(brush, pen, groupg);
            if (nend == 1)
            {
                WDrawPolyLine(dc, brush, pen, endPointList);
            }
        }

        //----------------------------------------------------------------------
        public static void WDrawPolyQuadraticBezier(DrawingContext dc, Brush brush, Pen pen, double bx, double by, double ex, double ey, string sXy, int arwCornFit)
        {
            List<Point> pointList = GetCoords(sXy, bx, by, ex, ey);
            WDrawPolyQuadraticBezier(dc, brush, pen, pointList, arwCornFit);
        }

        //----------------------------------------------------------------------
        public static void WDrawPolyQuadraticBezier(DrawingContext dc, Brush brush, Pen pen, List<Point> pointList, int arwCornFit)
        {
            List<Point> pointListX = new List<Point>();
            List<Point> endPointList = null;
            int nend = (pointList.Count - 1) % 2;
            for (int i = 1; i < pointList.Count - nend; i++)
            {
                pointListX.Add(new Point(pointList[i].X, pointList[i].Y));
            }
            if (nend != 0)
            {
                int ib = pointList.Count - nend - 1;
                endPointList = new List<Point>();
                for (int i = ib; i < pointList.Count; i++)
                {
                    endPointList.Add(new Point(pointList[i].X, pointList[i].Y));
                }
            }
            if (pen == null) pen = new Pen(Brushes.Black, 3);
            if (brush == null) brush = Brushes.Wheat;
            GeometryGroup groupg = new GeometryGroup();
            Point bPoint = new Point(pointList[0].X, pointList[0].Y);
            List<PathSegment> pSegList = new List<PathSegment>();
            PolyQuadraticBezierSegment polyBezier = new PolyQuadraticBezierSegment(pointListX, true);
            pSegList.Add(polyBezier);
            PathFigure f = new PathFigure(bPoint, pSegList, false);
            PathGeometry pathg = new PathGeometry();
            pathg.Figures.Add(f);
            groupg.Children.Add(pathg);
            dc.DrawGeometry(brush, pen, groupg);
            if (nend == 1)
            {
                WDrawPolyLine(dc, brush, pen, endPointList);
            }
            int n = pointList.Count - 1;
            if (arwCornFit == GlobalVarX.gARROW_BOTH)
            {
                WDrawingVisual.WDrawArrowHead(dc, brush, pen, pointList[n - 1].X, pointList[n - 1].Y, pointList[n].X, pointList[n].Y, 0, 0, pen.Thickness);  //--End
                WDrawingVisual.WDrawArrowHead(dc, brush, pen, pointList[1].X, pointList[1].Y, pointList[0].X, pointList[0].Y, 0, 0, pen.Thickness);  //--Head
            }
            else if (arwCornFit == GlobalVarX.gARROW_HEAD)
            {
                WDrawingVisual.WDrawArrowHead(dc, brush, pen, pointList[1].X, pointList[1].Y, pointList[0].X, pointList[0].Y, 0, 0, pen.Thickness);  //--Head
            }
            else if (arwCornFit == GlobalVarX.gARROW_END)
            {
                WDrawingVisual.WDrawArrowHead(dc, brush, pen, pointList[n - 1].X, pointList[n - 1].Y, pointList[n].X, pointList[n].Y, 0, 0, pen.Thickness);  //--End
            }

        }

        //----------------------------------------------------------------------
        // TYPE coordinates; TYPE coordinates
        // R 10,10 40,40; L 20,20 30,20
        public static void WDrawGeometry(DrawingContext dc, Brush brush, Pen pen, string sGeometry)
        {
            if (pen == null) pen = new Pen(Brushes.Black, 3);
            if (brush == null) brush = Brushes.Wheat;
            string[] SegList = sGeometry.Split(';');
            GeometryGroup groupg = new GeometryGroup();
            string sType = null;
            foreach (string seg in SegList)
            {
                List<Point> pointList = GetTypeAndCoords(seg, ref sType);
                if (sType == "L")
                {
                    LineGeometry lineg = new LineGeometry(pointList[0], pointList[1]);
                    groupg.Children.Add(lineg);
                }
                else if (sType == "R")
                {
                    RectangleGeometry rectg = new RectangleGeometry(new Rect(pointList[0], pointList[1]));
                    groupg.Children.Add(rectg);
                }
                else if (sType == "E")
                {
                    EllipseGeometry ellipseg = new EllipseGeometry(new Rect(pointList[0], pointList[1]));
                    groupg.Children.Add(ellipseg);
                }
                else if (sType == "A")  //-ARC
                {
                    List<PathSegment> pSegList = new List<PathSegment>();
                    ArcSegment arc = new ArcSegment(pointList[0], new Size(pointList[1].X - pointList[0].X, pointList[1].Y - pointList[0].Y), pointList[2].X, false, SweepDirection.Clockwise, true);
                    pSegList.Add(arc);
                    PathFigure f = new PathFigure(pointList[0], pSegList, true);
                    PathGeometry pathg = new PathGeometry();
                    pathg.Figures.Add(f);
                    groupg.Children.Add(pathg);
                }
                else if (sType == "A")  //-ARC
                {
                    List<PathSegment> pSegList = new List<PathSegment>();
                    ArcSegment arc = new ArcSegment(pointList[0], new Size(pointList[1].X - pointList[0].X, pointList[1].Y - pointList[0].Y), pointList[2].X, false, SweepDirection.Clockwise, true);
                    pSegList.Add(arc);
                    PathFigure f = new PathFigure(pointList[0], pSegList, true);
                    PathGeometry pathg = new PathGeometry();
                    pathg.Figures.Add(f);
                    groupg.Children.Add(pathg);
                }
                else if (sType == "B")  //-Bezier Curve
                {
                    List<PathSegment> pSegList = new List<PathSegment>();
                    BezierSegment bezier = new BezierSegment(pointList[0], pointList[1], pointList[2], true);
                    pSegList.Add(bezier);
                    PathFigure f = new PathFigure(pointList[0], pSegList, true);
                    PathGeometry pathg = new PathGeometry();
                    pathg.Figures.Add(f);
                    groupg.Children.Add(pathg);
                }
                else if (sType == "Q")  //-QuadraticBezier Curve
                {
                    List<PathSegment> pSegList = new List<PathSegment>();
                    QuadraticBezierSegment bezier = new QuadraticBezierSegment(pointList[0], pointList[1], true);
                    pSegList.Add(bezier);
                    PathFigure f = new PathFigure(pointList[0], pSegList, true);
                    PathGeometry pathg = new PathGeometry();
                    pathg.Figures.Add(f);
                    groupg.Children.Add(pathg);
                }
                else if (sType == "P")  //-PolyLine
                {
                    List<PathSegment> pSegList = new List<PathSegment>();
                    PolyLineSegment poly = new PolyLineSegment(pointList, true);
                    pSegList.Add(poly);
                    PathFigure f = new PathFigure(pointList[0], pSegList, true);
                    PathGeometry pathg = new PathGeometry();
                    pathg.Figures.Add(f);
                    groupg.Children.Add(pathg);
                }
                else if (sType == "PB")  //-Series of Bezier
                {
                    List<PathSegment> pSegList = new List<PathSegment>();
                    PolyBezierSegment polyBezier = new PolyBezierSegment(pointList, true);
                    pSegList.Add(polyBezier);
                    PathFigure f = new PathFigure(pointList[0], pSegList, true);
                    PathGeometry pathg = new PathGeometry();
                    pathg.Figures.Add(f);
                    groupg.Children.Add(pathg);
                }
                else if (sType == "PQ")  //-Series of Quadratic Bezier
                {
                    List<PathSegment> pSegList = new List<PathSegment>();
                    PolyQuadraticBezierSegment polyBezier = new PolyQuadraticBezierSegment(pointList, true);
                    pSegList.Add(polyBezier);
                    PathFigure f = new PathFigure(pointList[0], pSegList, true);
                    PathGeometry pathg = new PathGeometry();
                    pathg.Figures.Add(f);
                    groupg.Children.Add(pathg);
                }
            }
            dc.DrawGeometry(brush, pen, groupg);
        }

        //----------------------------------------------------------------------
        //--return if bx,by,ex,ey has changed
        public static bool WDrawTextBox(DrawingContext dc, Brush brushR, Pen pen, string stext,
                  double bx, double by, double ex, double ey, XFont aXFont, int iTextAlign, double fRotAngle)
        {
            bool fSizeChanged = false;
            FormattedText formattedText = GetProperFormattedText(ref stext,
                      ref bx, ref by, ref ex, ref ey, aXFont, pen, ref fSizeChanged);
            WDrawingVisual.WDrawRectangle(dc, brushR, pen, bx, by, ex, ey);
            WDrawingVisual.WDrawText(dc, formattedText, stext, bx, by, ex, ey,
                       GlobalVarX.aRuleDefXFont, GConstants.DT_CENTER | GConstants.DT_VCENTER, fRotAngle);
            return fSizeChanged;
        }

        //----------------------------------------------------------------------
        //--iFontStyle: TypefaceStyle, TextDecoration, FontWeight, FontStretch
        public static void WDrawText(DrawingContext dc, FormattedText formattedText, string stext, 
            double bx, double by, double ex, double ey, XFont aXFont, int iTextAlign, double fRotAngle)
        {
            FontWeight aFontWeight = DFontEnum.GetValueDFontWeight(aXFont.FontWeight);
            FontStretch aFontStretch = DFontEnum.GetValueDFontStretch(aXFont.FontStretch);
            TextDecorationCollection aTextDecorations = DFontEnum.GetValueDTextDecoration(aXFont.FontTextDecoration);
            FontStyle aTypefaceStyle = DFontEnum.GetValueDTypefaceStyle(aXFont.FontTypeFace);

            formattedText.SetFontWeight(aFontWeight);
            if (aTextDecorations != null) formattedText.SetTextDecorations(aTextDecorations);
            formattedText.SetFontStretch(aFontStretch);
            formattedText.SetFontStyle(aTypefaceStyle);

            if ((iTextAlign & GConstants.DT_RIGHT) != 0)
            {
                formattedText.TextAlignment = TextAlignment.Right;
            }
            else if ((iTextAlign & GConstants.DT_CENTER) != 0)
            {
                formattedText.TextAlignment = TextAlignment.Center;
            }
            else if ((iTextAlign & GConstants.DT_JUSTIFY) != 0)
            {
                formattedText.TextAlignment = TextAlignment.Justify;
            }
            else formattedText.TextAlignment = TextAlignment.Left;

//            formattedText.FlowDirection = FlowDirection.LeftToRight;
            //            formattedText.TextAlignment = DFontEnum.GetValueDTextAlignment(iTextAlign);


            if (formattedText.Width > Math.Abs(ex - bx)) ex = bx + formattedText.Width;
            if (formattedText.Height > Math.Abs(ey - by)) ey = by + formattedText.Height;

            //    WDrawRectangle(dc, brush, null, bx, by, ex, ey);  //-Draw background   

            AdjustTextVLoc(iTextAlign, formattedText.Height, ref by, ey);

            double tx = bx;
            double ty = by;
            if(fRotAngle != 0) dc.PushTransform(new RotateTransform(fRotAngle, bx, by));

            if ((iTextAlign & GConstants.DT_CENTER) != 0) tx = (bx + ex) / 2;
            else if ((iTextAlign & GConstants.DT_JUSTIFY) != 0) tx = (bx + ex) / 2;
            else if ((iTextAlign & GConstants.DT_RIGHT) != 0) tx = ex;

            dc.DrawText(formattedText, new Point(tx, ty));
            if(fRotAngle != 0) dc.Pop();
        }

        //----------------------------------------------------------------------
        public static FormattedText GetProperFormattedText(ref string stext,
            ref double bx, ref double by, ref double ex, ref double ey, XFont aXFont, Pen pen, ref bool fSizeChanged)
        {
            if (aXFont.FontSize == double.NaN || aXFont.FontSize <= 0) aXFont.FontSize = 2;

            FontFamily aFontFamily = WFont.GetFontFamily("Arial");
            if (!String.IsNullOrEmpty(aXFont.FontName) && aXFont.FontName != "Arial") aFontFamily = WFont.GetFontFamily(aXFont.FontName);

            int iFontTypeFace = DFontEnum.GetIndexDTypefaceStyle(aXFont.FontTypeFace);
            List<Typeface> tfList = aFontFamily.GetTypefaces().ToList<Typeface>();

            FormattedText formattedText = new FormattedText(stext,
                                                System.Globalization.CultureInfo.CurrentCulture,
                                                //        CultureInfo.GetCultureInfo("en-us"),
                                                FlowDirection.LeftToRight,
                                                //                                                new Typeface(aXFont.FontName),
                                                tfList[iFontTypeFace],
                                                aXFont.FontSize,
                                                pen.Brush);
            double width = Math.Abs(ex - bx);
            double height = Math.Abs(ey - by);
            if (formattedText.Width > width)
            {
                string[] subx = stext.Split(' ');
                string newStr = "";
                double w = 0;
                for (int i = 0; i < subx.Length; i++)
                {
                    FormattedText f = new FormattedText(subx[i] + " ",
                                                    System.Globalization.CultureInfo.CurrentCulture,
                                                    //        CultureInfo.GetCultureInfo("en-us"),
                                                    FlowDirection.LeftToRight,
                                                    // new Typeface(aXFont.FontName),
                                                    tfList[iFontTypeFace],
                                                    aXFont.FontSize,
                                                    pen.Brush);
                    w += f.Width;
                    if(w >= ex - bx)
                    {
                        newStr += "\r\n"+ subx[i] + " ";
                        w = f.Width;
                    }
                    else
                    {
                        newStr += subx[i] + " ";
                    }
                }
                string[] sSep = { "\r\n" };
                string[] subxx = newStr.Split(sSep, StringSplitOptions.None);
                newStr = "";
                for(int i=0;i<subxx.Length;i++)
                {
                    if (newStr == "") newStr = subxx[i].Trim();
                    else newStr += "\r\n" + subxx[i].Trim();
                }
                formattedText = new FormattedText(newStr,
                                System.Globalization.CultureInfo.CurrentCulture,
                                //        CultureInfo.GetCultureInfo("en-us"),
                                FlowDirection.LeftToRight,
                                // new Typeface(aXFont.FontName),
                                tfList[iFontTypeFace],
                                aXFont.FontSize,
                                pen.Brush);

            }
            fSizeChanged = false;
            if (formattedText.Width >= width) { ex = bx + formattedText.Width; fSizeChanged = true; }
            if (formattedText.Height >= height){ ey = by + formattedText.Height; fSizeChanged = true; }
            return formattedText;
        }

        //----------------------------------------------------------------------
        public static void AdjustTextVLoc(int iTextAlign, double textHeight, ref double by, double ey)
        {
            double vertMargin = (ey - by) - textHeight;
            if ((iTextAlign & GConstants.DT_VCENTER) != 0)
            {
                by += vertMargin / 2;
            }
            else if ((iTextAlign & GConstants.DT_BOTTOM) != 0)
            {
                by += vertMargin;
            }
        }

        //----------------------------------------------------------------------
        public static void AdjustTextHLoc(int iTextAlign, double textWidth, ref double bx, double ex)
        {
            double horiMargin = (ex - bx) - textWidth;
            if ((iTextAlign & GConstants.DT_RIGHT) != 0)
            {
                bx += horiMargin;
            }
            else if ((iTextAlign & GConstants.DT_CENTER) != 0)
            {
                bx += horiMargin / 2;
            }
        }

        //----------------------------------------------------------------------
        public static void AdjustTextLoc(int iTextAlign, double textWidth, double textHeight, ref double bx, ref double by, double ex, double ey)
        {
            double horiMargin = (ex - bx) - textWidth;
            double vertMargin = (ey - by) - textHeight;
            if ((iTextAlign & GConstants.DT_RIGHT) != 0)
            {
                bx += horiMargin;
            }
            else if ((iTextAlign & GConstants.DT_CENTER) != 0)
            {
                bx += horiMargin / 2;
            }

            if ((iTextAlign & GConstants.DT_VCENTER) != 0)
            {
                by += vertMargin / 2;
            }
            else if ((iTextAlign & GConstants.DT_BOTTOM) != 0)
            {
                by += vertMargin;
            }
        }

        //----------------------------------------------------------------------
        //--iFontStyle: TypefaceStyle, TextDecoration, FontWeight, FontStretch
        public static void WMeasureString(Pen pen, string stext, XFont aXFont, int iTextAlign, ref double width, ref double height)
        {

            if (aXFont.FontSize < 2) aXFont.FontSize = 2;
            FormattedText formattedText = new FormattedText(stext,
                                                            CultureInfo.GetCultureInfo("en-us"),
                                                            FlowDirection.LeftToRight,
                                                            new Typeface(aXFont.FontName),
                                                            aXFont.FontSize,
                                                            pen.Brush);
            formattedText.TextAlignment = DFontEnum.GetValueDTextAlignment(iTextAlign);
            FontWeight aFontWeight = FontWeights.Normal;
            FontStretch aFontStretch = FontStretches.Normal;
            TextDecorationCollection aTextDecorations = null;
            FontStyle aTypefaceStyle = FontStyles.Normal;  //-TypefaceStyle
            WFont.GetFontStyle(aXFont, ref aTypefaceStyle, ref aFontWeight, ref aFontStretch, ref aTextDecorations);
            formattedText.SetFontWeight(aFontWeight);
            if (aTextDecorations != null) formattedText.SetTextDecorations(aTextDecorations);
            formattedText.SetFontStretch(aFontStretch);
            formattedText.SetFontStyle(aTypefaceStyle);

            width = formattedText.Width;
            height = formattedText.Height;
        }

        //----------------------------------------------------------------------
        public static Size WMeasureString(string textstr, XFont aXFont, double lx, double ly)
        {
            if (aXFont.FontSize < 2) aXFont.FontSize = 2;
            FontFamily aFontFamily = WFont.GetFontFamily(aXFont.FontName);

            SolidColorBrush brush = new SolidColorBrush(Colors.Black);
            var culture = System.Globalization.CultureInfo.CurrentCulture;
            List<Typeface> tfList = aFontFamily.GetTypefaces().ToList<Typeface>();
            int iTypeface = DFontEnum.GetIndexDTypefaceStyle(aXFont.FontTypeFace);
            FormattedText x = new FormattedText(textstr, culture, FlowDirection.LeftToRight, tfList[iTypeface], aXFont.FontSize, brush);
            return new Size(x.Width, x.Height);
        }
        //----------------------------------------------------------------------
        public static Size WMeasureString(string textstr, string sFontName, double iFontSize, int fStyle, double lx, double ly)
        {
            if (iFontSize < 2) iFontSize = 2;
            if (fStyle < 0) fStyle = 0;
            FontFamily aFontFamily = WFont.GetFontFamily(sFontName);

            SolidColorBrush brush = new SolidColorBrush(Colors.Black);
            var culture = System.Globalization.CultureInfo.CurrentCulture;
            List<Typeface> tfList = aFontFamily.GetTypefaces().ToList<Typeface>();
            FormattedText x = new FormattedText(textstr, culture, FlowDirection.LeftToRight, tfList[fStyle], iFontSize, brush);
            return new Size(x.Width, x.Height);
        }

        //----------------------------------------------------------------------
        public static void WDrawImage(DrawingContext dc, Brush brush, string imageFile, double bx, double by, double ex, double ey, string sFontName, double iFontSize)
        {
            WCoord.oXyCoordSwap(ref bx, ref by, ref ex, ref ey);
            if (bx == ex || by == ey)
            {
                Console.WriteLine("ERROR: WDrawImage: {0} Bad size: W:{1}, H:{2}", imageFile, ex - bx, ey - by);
                return;
            }
            if (!File.Exists(imageFile))
            {
                Console.WriteLine("ERROR: WDrawImage: {0} doesnot exist!", imageFile);
                return;
            }
            // Open a Uri and decode a BMP image
            if (imageFile.ToUpper().EndsWith(".BMP"))
            {
                Uri myUri = WHelper.GetUri(imageFile);
                BmpBitmapDecoder decoder = new BmpBitmapDecoder(myUri, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.Default);
                BitmapSource bitmapSource = decoder.Frames[0];
                dc.DrawImage(bitmapSource, new Rect(bx, by, ex - bx, ey - by));
            }
            else
            {
                // Create source
                BitmapImage myBitmapImage = new BitmapImage();

                // BitmapImage.UriSource must be in a BeginInit/EndInit block
                myBitmapImage.BeginInit();
                myBitmapImage.UriSource = WHelper.GetUri(imageFile);

                // To save significant application memory, set the DecodePixelWidth or  
                // DecodePixelHeight of the BitmapImage value of the image source to the desired 
                // height or width of the rendered image. If you don't do this, the application will 
                // cache the image as though it were rendered as its normal size rather then just 
                // the size that is displayed.
                // Note: In order to preserve aspect ratio, set DecodePixelWidth
                // or DecodePixelHeight but not both.
                myBitmapImage.EndInit();
                //set image source
                dc.DrawImage(myBitmapImage, new Rect(bx, by, ex - bx, ey - by));
            }
        }

        //----------------------------------------------------------------------
        public static void WDrawVideo(DrawingContext dc, Brush brush, string videoFile, double bx, double by, double ex, double ey, string sFontName, double iFontSize)
        {
            if (!File.Exists(videoFile))
            {
                Console.WriteLine("ERROR: WDrawImage: {0} doesnot exist!", videoFile);
                return;
            }
            MediaPlayer player = new MediaPlayer();
            player.Open(WHelper.GetUri(videoFile));
            player.Play();
            dc.DrawVideo(player, new Rect(bx, by, ex, ey));
        }

        //----------------------------------------------------------------------
        public static BitmapSource GetImage(string imageFile)
        {

            // Open a Uri and decode a BMP image
            if (imageFile.ToUpper().EndsWith(".BMP"))
            {
                Uri myUri = WHelper.GetUri(imageFile);
                BmpBitmapDecoder decoder = new BmpBitmapDecoder(myUri, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.Default);
                BitmapSource bitmapSource = decoder.Frames[0];
                return bitmapSource;
            }
            else
            {
                // Create source
                BitmapImage myBitmapImage = new BitmapImage();

                // BitmapImage.UriSource must be in a BeginInit/EndInit block
                myBitmapImage.BeginInit();
                myBitmapImage.UriSource = WHelper.GetUri(imageFile);

                // To save significant application memory, set the DecodePixelWidth or  
                // DecodePixelHeight of the BitmapImage value of the image source to the desired 
                // height or width of the rendered image. If you don't do this, the application will 
                // cache the image as though it were rendered as its normal size rather then just 
                // the size that is displayed.
                // Note: In order to preserve aspect ratio, set DecodePixelWidth
                // or DecodePixelHeight but not both.
                myBitmapImage.EndInit();
                //set image source
                return myBitmapImage;
            }
        }

        /*-----------------------------------------------------------------
        WDrawArrowHead(bx,by,ex,ey,adjx,adjy,xrat,yrat,inCX,inCY)
         Draw the Arrow head at the end of the line which is coords at ex,ey
         The hDC has the correct info
                              0
                            / | \
                          3---2---1
                              |
                              |
        -----------------------------------------------------------------*/
        public static void WDrawArrowHead(DrawingContext dc, Brush brush, Pen hPen,
                                          double bx, double by, double ex, double ey,  /* line coords: Arrow head at ex,ey */
                                          double adjx, double adjy,
                                          double lineWidth)
        {
            double fslope, angle;
            double xrot, yrot;
            List<WPOINT> pt = new List<WPOINT>();

            Pen hPenA = new Pen(hPen.Brush, 1);
            Brush brushA = hPen.Brush.Clone();
            brushA.Opacity = 1;
            bx += adjx; by += adjy; ex += adjx; ey += adjy;
            pt.Add(new WPOINT(ex, ey));
            pt.Add(new WPOINT(ex, ey));
            pt.Add(new WPOINT(ex, ey));
            pt.Add(new WPOINT(ex, ey));
            double dw = lineWidth - 1;
            if (dw < 0) dw = 0;
            if (bx == ex)
            {             /* | */
                if (ey < by)
                {
                    pt[0].X = ex;
                    pt[0].Y = ey - 1.4;
                    pt[1].X = ex + ((GlobalVarX.gARW_Width + dw));
                    pt[1].Y = ey + ((GlobalVarX.gARW_Length + dw));
                    pt[2].X = ex;
                    pt[2].Y = pt[1].Y - ((GlobalVarX.gARW_Dent + dw));
                }
                else
                {
                    pt[0].X = ex;
                    pt[0].Y = ey + 1.4;
                    pt[1].X = ex + ((GlobalVarX.gARW_Width + dw));
                    pt[1].Y = ey - ((GlobalVarX.gARW_Length + dw));
                    pt[2].X = ex;
                    pt[2].Y = pt[1].Y + ((GlobalVarX.gARW_Dent + dw));
                }
                pt[3].X = ex - ((GlobalVarX.gARW_Width + dw));
                pt[3].Y = pt[1].Y;
            }
            else if (by == ey)
            {    /* -- */
                if (ex < bx)
                {
                    pt[0].X = ex - 1.4;
                    pt[0].Y = ey;
                    pt[1].X = ex + ((GlobalVarX.gARW_Length + dw));
                    pt[1].Y = ey + ((GlobalVarX.gARW_Width + dw));
                    pt[2].X = pt[1].X - ((GlobalVarX.gARW_Dent + dw));
                    pt[2].Y = ey;
                }
                else
                {
                    pt[0].X = ex + 1.4;
                    pt[0].Y = ey;
                    pt[1].X = ex - ((GlobalVarX.gARW_Length + dw));
                    pt[1].Y = ey + ((GlobalVarX.gARW_Width + dw));
                    pt[2].X = pt[1].X + ((GlobalVarX.gARW_Dent + dw));
                    pt[2].Y = ey;
                }
                pt[3].X = pt[1].X;
                pt[3].Y = ey - ((GlobalVarX.gARW_Width + dw));
            }
            else
            {    /* ---------irregular---*/
                fslope = (bx - ex) / (by - ey);
                angle = Math.Atan(fslope);
                xrot = Math.Cos(angle);
                yrot = Math.Sin(angle);
                //if (inCX > 1 || inCY > 1)
                //{  
                //    WExtendLineEnd(bx, by, ex, ey, inCX, inCY, fslope, ref pt[0].X, ref pt[0].Y);
                //    ex = pt[0].X; ey = pt[0].Y;
                //}
                pt[0].X = ex;
                pt[0].Y = ey;
                if (ey < by)
                {
                    pt[1].X = ex - ((GlobalVarX.gARW_Width + dw));
                    pt[1].Y = ey + ((GlobalVarX.gARW_Length + dw) + 1.6);
                    pt[2].X = ex;
                    pt[2].Y = pt[1].Y - ((GlobalVarX.gARW_Dent + dw));
                    pt[3].X = ex + ((GlobalVarX.gARW_Width + dw));
                    pt[3].Y = pt[1].Y;
                }
                else
                {
                    //pt[1].X = ex - (arwWidth * xrat);
                    //pt[1].Y = ey - (arwLength * yrat - 1.6);
                    //pt[2].X = ex;
                    //pt[2].Y = pt[1].Y + (arwDent * yrat);
                    pt[1].X = ex + ((GlobalVarX.gARW_Width + dw));
                    pt[1].Y = ey - ((GlobalVarX.gARW_Length + dw) - 1.6);
                    pt[2].X = ex;
                    pt[2].Y = pt[1].Y + ((GlobalVarX.gARW_Dent + dw));
                    pt[3].X = ex - ((GlobalVarX.gARW_Width + dw));
                    pt[3].Y = pt[1].Y;
                }
                WCoord.oRotateXYCoords(ref pt[1].X, ref pt[1].Y, pt[0].X, pt[0].Y, xrot, yrot);
                WCoord.oRotateXYCoords(ref pt[2].X, ref pt[2].Y, pt[0].X, pt[0].Y, xrot, yrot);
                WCoord.oRotateXYCoords(ref pt[3].X, ref pt[3].Y, pt[0].X, pt[0].Y, xrot, yrot);
            }
            WDrawingVisual.WDrawPolygon(dc, brushA, hPenA, WPOINT.toPoint(pt));
        }

        //-----------------------------------------------------------------
        public static void WExtendLineEnd(double bx, double by, double ex, double ey,
                                          double inCX, double inCY,
                                          double fslope,       /*  dx / dy */
                                          ref double rx,
                                          ref double ry)
        {
            double M, K, R;

            K = 1.4 * (double)inCY;
            R = Math.Sqrt((Math.Pow((double)(ex - bx), 2.0) + Math.Pow((double)(ey - by), 2.0)));
            M = Math.Pow((K + R), 2.0);
            K = fslope * fslope + 1.0;
            R = Math.Sqrt(M / K);
            if (ey > by) ry = by + R;
            else ry = by - R;
            rx = bx + ((ry - by) * fslope);
        }

        //-------------------------------------------------------------------------
        public static List<Point> GetTypeAndCoords(string str, ref string sType)
        {
            if (String.IsNullOrEmpty(str)) return null;
            str = str.Replace('|', ' ');
            string[] subx = str.Split(' ');
            sType = subx[0];
            List<Point> pointList = new List<Point>();
            double x;
            double y;
            for (int i = 1; i < subx.Length; i++)
            {
                string[] subxx = subx[i].Split(',');
                if (subxx.Length >= 2)
                {
                    x = Double.Parse(subxx[0].Trim());
                    y = Double.Parse(subxx[1].Trim());
                    pointList.Add(new Point(x, y));
                }
            }
            return pointList;
        }

        //-------------------------------------------------------------------------
        public static List<Point> GetCoords(string str, double bx, double by, double ex, double ey)
        {
            if (String.IsNullOrEmpty(str)) return null;
            str = str.Replace('|', ' ');
            char[] cSep = { ' ' };
            string[] subx = str.Trim().Split(cSep, StringSplitOptions.RemoveEmptyEntries);
            List<Point> pointList = new List<Point>();
            double x = 0;
            double y = 0;
            for (int i = 0; i < subx.Length; i++)
            {
                string[] subxx = subx[i].Split(',');
                if (subxx.Length >= 2 && Double.TryParse(subxx[0].Trim(), out x) &&
                    Double.TryParse(subxx[1].Trim(), out y))
                {
                    pointList.Add(new Point(bx + x, by));
                }
            }
            return pointList;
        }

        //-------------------------------------------------------------------------
        //--No adjustment
        public static List<Point> GetCoords(string str)
        {
            if (String.IsNullOrEmpty(str)) return null;
            str = str.Replace('|', ' ');
            char[] cSep = { ' ' };
            string[] subx = str.Trim().Split(cSep, StringSplitOptions.RemoveEmptyEntries);
            List<Point> pointList = new List<Point>();
            double x = 0;
            double y = 0;
            for (int i = 0; i < subx.Length; i++)
            {
                string[] subxx = subx[i].Split(',');
                if (subxx.Length >= 2 && Double.TryParse(subxx[0].Trim(), out x) &&
                    Double.TryParse(subxx[1].Trim(), out y))
                {
                    pointList.Add(new Point(x, y));
                }
            }
            return pointList;
        }

        //-------------------------------------------------------------------------
        //--No adjustment
        public static List<Point> AdjustCoords(List<Point> ptList, double adjx, double adjy)
        {
            List<Point> pointList = new List<Point>();
            foreach (Point p in ptList)
            {
                pointList.Add(new Point(adjx + p.X, adjy + p.Y));
            }
            return pointList;
        }

        //-------------------------------------------------------------------------
        public static void GetCoordsMinMax(List<Point> pointList, ref double bx, ref double by, ref double ex, ref double ey)
        {
            bx = by = 100000000;
            ex = ey = 0;
            foreach (Point p in pointList)
            {
                if (p.X > ex) ex = p.X;
                if (p.X < bx) bx = p.X;
                if (p.Y > ey) ey = p.Y;
                if (p.Y < by) by = p.Y;
            }
        }

        //-------------------------------------------------------------------------
        public static void FlipCoords(ref double bx, ref double by, ref double ex, ref double ey)
        {
            double v = 0;
            if (ex < bx)
            {
                v = bx;
                bx = ex;
                ex = v;
            }
            if (ey < by)
            {
                v = by;
                by = ey;
                ey = v;
            }
        }

        //-------------------------------------------------------------------------
        public static List<Point> GetRelativeCoords(List<Point> pointList, double bx, double by)
        {
            List<Point> rList = new List<Point>();
            foreach (Point p in pointList)
            {
                rList.Add(new Point(p.X - bx, p.Y - by));
            }
            return rList;
        }

        //-------------------------------------------------------------------------
        public static string GetCoordsStr(List<Point> pointList)
        {
            string rstr = "";
            for (int i=0;i<pointList.Count;i++)
            {
                if (i == 0) rstr += pointList[i].X + "," + pointList[i].Y;
                else       rstr += "|" + pointList[i].X + "," + pointList[i].Y;
            }
            return rstr;
        }

        //-------------------------------------------------------------------------
        public static void SwapCoord(ref double bx, ref double by, ref double ex, ref double ey)
        {
            double fvalue = 0;
            if (ex < bx)
            {
                fvalue = bx;
                bx = ex;
                ex = fvalue;
            }
            if (ey < by)
            {
                fvalue = by;
                by = ey;
                ey = fvalue;
            }
        }
    }
}
