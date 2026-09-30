/* ###################################################################
    M_WRAP.C
 The routines that handles all the drawing of the outer wraping of text
 #####################################################################*/
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace OExpert.UILibraryWPF
{
    public class WDrawTextWrap
    {
        /*-------------------------------------------------------------------
        oDrawWrapElmt(hDC,tRect,lpPen,logBrush,fGrad,iWrap,dxw,dyw)
        --------------------------------------------------------------------*/
        public static int oDrawWrapElmt(
                DrawingContext hDC,
                WRECT tRect,
                Pen pen,
                Brush brush,
                bool fGrad,
                int iWrap,  /* wrapping type */
                int dxw,
                int dyw)
        {
            if (fGrad && iWrap <= 0)
            {
                WDrawingVisual.WDrawRectangle(hDC, brush, pen, tRect.left, tRect.top, tRect.right, tRect.bottom);
            }
            else if (iWrap > 0)
            {
                List<WPOINT> pt = oGetWrapPT(iWrap, tRect);
                oDCDrawWrap(hDC, pen, brush, iWrap, tRect.left, tRect.top, tRect.right, tRect.bottom, pt);
            }
            else return 0;
            return 1;
        }

        /*-----------------------------------------------------------------
        oDCDrawWrap(hDC,iWrap,x0,y0,x1,y1,pt,n)
        -----------------------------------------------------------------*/
        public static void oDCDrawWrap(
        DrawingContext hDC,
        Pen pen,
        Brush brush,
        int iWrap,
        double x0,
        double y0,
        double x1,
        double y1,
        List<WPOINT> pt)
        {
            switch (iWrap)
            {
                case 1:   // rectangle
                case 26:  //Process
                    WDrawingVisual.WDrawRectangle(hDC, brush, pen, x0, y0, x1, y1);
                    break;
                case 2: // ELLIPSE
                case 38:  //Connector
                    WDrawingVisual.WDrawEllipse(hDC, brush, pen, x0, y0, x1, y1);
                    break;
                case 3: // ROUNDRECT
                    double dx = Math.Abs(x1 - x0) / 5;
                    double dy = Math.Abs(y1 - y0) / 5;
                    if (dy < dx) dx = dy;
                    WDrawingVisual.WDrawRoundedRectangle(hDC, brush, pen, x0, y0, x1, y1, dx, dx);
                    break;
                case 4: // CHORD
                    WDrawingVisual.WDrawChord(hDC, brush, pen, x0, y0, x1, y1, WPOINT.toPoint(pt), 0);
                    break;
                case 5:  // PIE
                    WDrawingVisual.WDrawPie(hDC, brush, pen, x0, y0, x1, y1, WPOINT.toPoint(pt), 0);
                    break;
                case 6:  //ParRect
                case 7:  //Trapezoid
                case 8:  //Diamond
                case 9:  //Octagon
                case 10:  //IsosTri
                case 11:  //RightTri
                case 12:  //Hexagon
                case 13:  //Cross
                case 14:  //Pentagon
                case 22:  //Lighting
                case 28:  //Decision
                case 29:  //Data
                case 34:  //Terminator
                case 35:  //Preparation
                case 36:  //ManualInput
                case 37:  //ManualOper
                case 39:  //OffPageConn
                case 40:  //Card
                case 44:  //Collate
                case 45:  //Sort
                case 46:  //Extract
                case 47:  //Merge
                case 123:  //LeftArrow
                case 124:  //RightArrow
                case 125:  //BothArrow
                    WDrawingVisual.WDrawPolygon(hDC, brush, pen, WPOINT.toPoint(pt));
                    break;

                //-----Special treatment------
                case 15:  //Can
                case 51:  //MagDisk
                    WDrawingVisual.WDrawBezierCurve(hDC, brush, pen, WPOINT.toPoint(pt));
                    List<WPOINT> xpt = new List<WPOINT>();
                    xpt.Add(new UILibraryWPF.WPOINT(pt[0].X, pt[0].Y));
                    xpt.Add(new UILibraryWPF.WPOINT(pt[9].X, pt[9].Y));
                    xpt.Add(new UILibraryWPF.WPOINT(pt[8].X, pt[8].Y));
                    xpt.Add(new UILibraryWPF.WPOINT(pt[3].X, pt[3].Y));
                    WDrawingVisual.WDrawBezierCurve(hDC, brush, pen, WPOINT.toPoint(xpt));
                    break;
                case 16:  //Cube
                    WDrawingVisual.WDrawPolygon(hDC, brush, pen, WPOINT.toPoint(pt));
                    WDrawingVisual.WDrawLine(hDC, pen, pt[1].X, pt[1].Y, pt[6].X, pt[6].Y);
                    WDrawingVisual.WDrawLine(hDC, pen, pt[3].X, pt[3].Y, pt[6].X, pt[6].Y);
                    WDrawingVisual.WDrawLine(hDC, pen, pt[5].X, pt[5].Y, pt[6].X, pt[6].Y);
                    break;
                case 17:  //Bevel
                    WDrawingVisual.WDrawRectangle(hDC, brush, pen, pt[0].X, pt[0].Y, pt[1].X, pt[1].Y);
                    WDrawingVisual.WDrawRectangle(hDC, brush, pen, pt[2].X, pt[2].Y, pt[3].X, pt[3].Y);
                    break;
                case 18:  //FoldCorner
                    WDrawingVisual.WDrawPolygon(hDC, brush, pen, WPOINT.toPoint(pt));
                    WDrawingVisual.WDrawLine(hDC, pen, pt[5].X, pt[5].Y, pt[3].X, pt[3].Y);
                    WDrawingVisual.WDrawLine(hDC, pen, pt[5].X, pt[5].Y, pt[4].X, pt[4].Y);
                    break;
                case 19:  //Donut
                    WDrawingVisual.WDrawEllipse(hDC, brush, pen, pt[0].X, pt[0].Y, pt[1].X, pt[1].Y);
                    WDrawingVisual.WDrawEllipse(hDC, brush, pen, pt[2].X, pt[2].Y, pt[3].X, pt[3].Y);
                    break;
                case 20:  //NoSym
                    WDrawingVisual.WDrawEllipse(hDC, brush, pen, pt[0].X, pt[0].Y, pt[1].X, pt[1].Y);
                    WDrawingVisual.WDrawChord(hDC, brush, pen, pt[2].X, pt[2].Y, pt[3].X, pt[3].Y, WPOINT.toPoint(pt), 0);
                    WDrawingVisual.WDrawChord(hDC, brush, pen, pt[2].X, pt[2].Y, pt[3].X, pt[3].Y, WPOINT.toPoint(pt), 0);
                    break;
                case 21:  //Heart
                    WDrawingVisual.WDrawBezierCurve(hDC, brush, pen, WPOINT.toPoint(pt));
                    break;

                case 27:  //AltPorc
                    WDrawingVisual.WDrawRoundedRectangle(hDC, brush, pen, pt[0].X, pt[0].Y, pt[1].X, pt[1].Y, pt[2].X, pt[2].Y);
                    break;
                case 30:  //PreDefProc
                case 31:  //IntStor
                    WDrawingVisual.WDrawRectangle(hDC, brush, pen, pt[0].X, pt[0].Y, pt[1].X, pt[1].Y);
                    WDrawingVisual.WDrawLine(hDC, pen, pt[2].X, pt[2].Y, pt[3].X, pt[3].Y);
                    WDrawingVisual.WDrawLine(hDC, pen, pt[4].X, pt[4].Y, pt[5].X, pt[5].Y);
                    break;
                case 32:  //Docu
                    WDrawingVisual.WDrawBezierCurve(hDC, brush, pen, WPOINT.toPoint(pt));
                    break;
                case 33:  //MultiDocu
                    WDrawingVisual.WDrawBezierCurve(hDC, brush, pen, WPOINT.toPoint(pt));
                    int np = pt.Count;
                    WDrawingVisual.WDrawLine(hDC, pen, pt[1].X, pt[1].Y, pt[np - 1].X, pt[np - 1].Y);
                    WDrawingVisual.WDrawLine(hDC, pen, pt[np - 1].X, pt[np - 1].Y, pt[9].X, pt[9].Y);
                    WDrawingVisual.WDrawLine(hDC, pen, pt[3].X, pt[3].Y, pt[np - 2].X, pt[np - 2].Y);
                    WDrawingVisual.WDrawLine(hDC, pen, pt[np - 2].X, pt[np - 2].Y, pt[7].X, pt[7].Y);
                    break;
                case 41:  //PunchTape
                    WDrawingVisual.WDrawBezierCurve(hDC, brush, pen, WPOINT.toPoint(pt));
                    break;
                case 42:  //SumJunction
                case 43:  //Or
                    WDrawingVisual.WDrawEllipse(hDC, brush, pen, pt[0].X, pt[0].Y, pt[1].X, pt[1].Y);
                    WDrawingVisual.WDrawLine(hDC, pen, pt[2].X, pt[2].Y, pt[3].X, pt[3].Y);
                    WDrawingVisual.WDrawLine(hDC, pen, pt[4].X, pt[4].Y, pt[5].X, pt[5].Y);
                    break;
                case 48:  //StoredData
                    WDrawingVisual.WDrawBezierCurve(hDC, brush, pen, WPOINT.toPoint(pt));
                    break;
                case 49:  //Delay
                    WDrawingVisual.WDrawBezierCurve(hDC, brush, pen, WPOINT.toPoint(pt));
                    break;
                case 50:  //SeqAccStor
                    WDrawingVisual.WDrawPolygon(hDC, brush, pen, WPOINT.toPoint(pt));
                    int n = pt.Count;
                    WDrawingVisual.WDrawEllipse(hDC, brush, pen, pt[n - 2].X, pt[n - 2].Y, pt[n - 1].X, pt[n - 1].Y);
                    break;
                case 52:  //DirAccStor
                    WDrawingVisual.WDrawBezierCurve(hDC, brush, pen, WPOINT.toPoint(pt));
                    List<WPOINT> xpt1 = new List<WPOINT>();
                    xpt1.Add(new UILibraryWPF.WPOINT(pt[7].X, pt[7].Y));
                    xpt1.Add(new UILibraryWPF.WPOINT(pt[8].X, pt[8].Y));
                    xpt1.Add(new UILibraryWPF.WPOINT(pt[9].X, pt[9].Y));
                    xpt1.Add(new UILibraryWPF.WPOINT(pt[4].X, pt[4].Y));
                    WDrawingVisual.WDrawBezierCurve(hDC, brush, pen, WPOINT.toPoint(xpt1));
                    break;
                case 53:  //Display
                    WDrawingVisual.WDrawBezierCurve(hDC, brush, pen, WPOINT.toPoint(pt));
                    break;
            }
        }

        /*-----------------------------------------------------------------
        oGetWrapPT(iWrap,pt)
        -----------------------------------------------------------------*/
        public static List<WPOINT> oGetWrapPT(
        int iWrap,
        WRECT tRect)
        {
            List<WPOINT> pt = new List<WPOINT>();
            switch (iWrap)
            {
                /*------Basic shape ----------*/
                case 0:  //NONE
                case 1:  //RECT
                case 2:  //ELLIPSE
                case 3:  //ROUNDRECT
                case 4:  //CHORD
                case 5:  //PIE
                    break;
                case 6:  //ParRect
                    pt.Add(new UILibraryWPF.WPOINT(3, 0));
                    pt.Add(new UILibraryWPF.WPOINT(10, 0));
                    pt.Add(new UILibraryWPF.WPOINT(7, 5));
                    pt.Add(new UILibraryWPF.WPOINT(3, 5));
                    break;
                case 7:  //Trapezoid
                    pt.Add(new UILibraryWPF.WPOINT(0, 0));
                    pt.Add(new UILibraryWPF.WPOINT(10, 0));
                    pt.Add(new UILibraryWPF.WPOINT(8, 6));
                    pt.Add(new UILibraryWPF.WPOINT(2, 6));
                    break;
                case 8:  //Diamond
                    pt.Add(new UILibraryWPF.WPOINT(0, 5));
                    pt.Add(new UILibraryWPF.WPOINT(3, 0));
                    pt.Add(new UILibraryWPF.WPOINT(6, 5));
                    pt.Add(new UILibraryWPF.WPOINT(3, 10));
                    break;
                case 9:  //Octagon
                    pt.Add(new UILibraryWPF.WPOINT(0, 7));
                    pt.Add(new UILibraryWPF.WPOINT(0, 3));
                    pt.Add(new UILibraryWPF.WPOINT(3, 0));
                    pt.Add(new UILibraryWPF.WPOINT(7, 0));
                    pt.Add(new UILibraryWPF.WPOINT(10, 3));
                    pt.Add(new UILibraryWPF.WPOINT(10, 7));
                    pt.Add(new UILibraryWPF.WPOINT(7, 10));
                    pt.Add(new UILibraryWPF.WPOINT(3, 10));
                    break;
                case 10:  //IsosTri
                    pt.Add(new UILibraryWPF.WPOINT(0, 7));
                    pt.Add(new UILibraryWPF.WPOINT(4, 0));
                    pt.Add(new UILibraryWPF.WPOINT(8, 7));
                    break;
                case 11:  //RightTri
                    pt.Add(new UILibraryWPF.WPOINT(0, 8));
                    pt.Add(new UILibraryWPF.WPOINT(0, 0));
                    pt.Add(new UILibraryWPF.WPOINT(8, 8));
                    break;
                case 12:  //Hexagon
                    pt.Add(new UILibraryWPF.WPOINT(0, 3));
                    pt.Add(new UILibraryWPF.WPOINT(3, 0));
                    pt.Add(new UILibraryWPF.WPOINT(7, 0));
                    pt.Add(new UILibraryWPF.WPOINT(10, 3));
                    pt.Add(new UILibraryWPF.WPOINT(7, 6));
                    pt.Add(new UILibraryWPF.WPOINT(3, 6));

                    break;
                case 13:  //Cross
                    pt.Add(new UILibraryWPF.WPOINT(0, 7));
                    pt.Add(new UILibraryWPF.WPOINT(0, 3));
                    pt.Add(new UILibraryWPF.WPOINT(3, 3));
                    pt.Add(new UILibraryWPF.WPOINT(3, 0));
                    pt.Add(new UILibraryWPF.WPOINT(7, 0));
                    pt.Add(new UILibraryWPF.WPOINT(7, 3));
                    pt.Add(new UILibraryWPF.WPOINT(10, 3));
                    pt.Add(new UILibraryWPF.WPOINT(10, 7));
                    pt.Add(new UILibraryWPF.WPOINT(7, 7));
                    pt.Add(new UILibraryWPF.WPOINT(7, 10));
                    pt.Add(new UILibraryWPF.WPOINT(3, 10));
                    pt.Add(new UILibraryWPF.WPOINT(3, 7));
                    break;
                case 14:  //Pentagon
                    pt.Add(new UILibraryWPF.WPOINT(0, 3));
                    pt.Add(new UILibraryWPF.WPOINT(4, 0));
                    pt.Add(new UILibraryWPF.WPOINT(8, 3));
                    pt.Add(new UILibraryWPF.WPOINT(6, 7));
                    pt.Add(new UILibraryWPF.WPOINT(2, 7));
                    break;
                case 15:  //Can
                case 51:  //MagDisk
                    pt.Add(new UILibraryWPF.WPOINT(0, 2));
                    pt.Add(new UILibraryWPF.WPOINT(2, 0));
                    pt.Add(new UILibraryWPF.WPOINT(6, 0));
                    pt.Add(new UILibraryWPF.WPOINT(8, 2));
                    pt.Add(new UILibraryWPF.WPOINT(8, 8));
                    pt.Add(new UILibraryWPF.WPOINT(6, 10));
                    pt.Add(new UILibraryWPF.WPOINT(2, 10));
                    pt.Add(new UILibraryWPF.WPOINT(0, 8));
                    pt.Add(new UILibraryWPF.WPOINT(6, 4));
                    pt.Add(new UILibraryWPF.WPOINT(2, 4));
                    break;
                case 16:  //Cube
                    pt.Add(new UILibraryWPF.WPOINT(0, 6));
                    pt.Add(new UILibraryWPF.WPOINT(0, 2));
                    pt.Add(new UILibraryWPF.WPOINT(3, 0));
                    pt.Add(new UILibraryWPF.WPOINT(10, 0));
                    pt.Add(new UILibraryWPF.WPOINT(10, 4));
                    pt.Add(new UILibraryWPF.WPOINT(7, 6));
                    pt.Add(new UILibraryWPF.WPOINT(7, 2));
                    break;
                case 17:  //Bevel
                    pt.Add(new UILibraryWPF.WPOINT(0, 0));
                    pt.Add(new UILibraryWPF.WPOINT(10, 10));
                    pt.Add(new UILibraryWPF.WPOINT(2, 2));
                    pt.Add(new UILibraryWPF.WPOINT(8, 8));
                    break;
                case 18:  //FoldCorner
                    pt.Add(new UILibraryWPF.WPOINT(0, 8));
                    pt.Add(new UILibraryWPF.WPOINT(0, 0));
                    pt.Add(new UILibraryWPF.WPOINT(10, 0));
                    pt.Add(new UILibraryWPF.WPOINT(10, 6));
                    pt.Add(new UILibraryWPF.WPOINT(8, 8));
                    pt.Add(new UILibraryWPF.WPOINT(8, 6));
                    break;
                case 19:  //Donut
                    pt.Add(new UILibraryWPF.WPOINT(0, 0));
                    pt.Add(new UILibraryWPF.WPOINT(10, 10));
                    pt.Add(new UILibraryWPF.WPOINT(2, 2));
                    pt.Add(new UILibraryWPF.WPOINT(8, 8));
                    break;
                case 20:  //NoSym
                    pt.Add(new UILibraryWPF.WPOINT(0, 0));
                    pt.Add(new UILibraryWPF.WPOINT(10, 10));
                    pt.Add(new UILibraryWPF.WPOINT(2, 2));
                    pt.Add(new UILibraryWPF.WPOINT(8, 8));
                    pt.Add(new UILibraryWPF.WPOINT(2, 4));
                    pt.Add(new UILibraryWPF.WPOINT(6, 8));
                    pt.Add(new UILibraryWPF.WPOINT(4, 2));
                    pt.Add(new UILibraryWPF.WPOINT(8, 6));
                    break;
                case 21:  //Heart
                    pt.Add(new UILibraryWPF.WPOINT(0, 3));
                    pt.Add(new UILibraryWPF.WPOINT(0, 0));
                    pt.Add(new UILibraryWPF.WPOINT(4, 0));
                    pt.Add(new UILibraryWPF.WPOINT(5, 2));
                    pt.Add(new UILibraryWPF.WPOINT(6, 0));
                    pt.Add(new UILibraryWPF.WPOINT(10, 0));
                    pt.Add(new UILibraryWPF.WPOINT(10, 3));
                    pt.Add(new UILibraryWPF.WPOINT(10, 6));
                    pt.Add(new UILibraryWPF.WPOINT(8, 8));
                    pt.Add(new UILibraryWPF.WPOINT(5, 9));
                    pt.Add(new UILibraryWPF.WPOINT(2, 8));
                    pt.Add(new UILibraryWPF.WPOINT(0, 6));
                    pt.Add(new UILibraryWPF.WPOINT(0, 3));
                    break;
                case 123:  //Left Arrow
                    pt.Add(new UILibraryWPF.WPOINT(0, 4));
                    pt.Add(new UILibraryWPF.WPOINT(3, 8));
                    pt.Add(new UILibraryWPF.WPOINT(3, 6));
                    pt.Add(new UILibraryWPF.WPOINT(6, 6));
                    pt.Add(new UILibraryWPF.WPOINT(6, 2));
                    pt.Add(new UILibraryWPF.WPOINT(3, 2));
                    pt.Add(new UILibraryWPF.WPOINT(3, 0));
                    pt.Add(new UILibraryWPF.WPOINT(0, 4));
                    break;
                case 124:  //Right Arrow
                    pt.Add(new UILibraryWPF.WPOINT(6, 4));
                    pt.Add(new UILibraryWPF.WPOINT(3, 0));
                    pt.Add(new UILibraryWPF.WPOINT(3, 2));
                    pt.Add(new UILibraryWPF.WPOINT(0, 2));
                    pt.Add(new UILibraryWPF.WPOINT(0, 6));
                    pt.Add(new UILibraryWPF.WPOINT(3, 6));
                    pt.Add(new UILibraryWPF.WPOINT(3, 8));
                    pt.Add(new UILibraryWPF.WPOINT(6, 4));
                    break;
                case 125:  //Both Arrow
                    pt.Add(new UILibraryWPF.WPOINT(0, 4));
                    pt.Add(new UILibraryWPF.WPOINT(3, 8));
                    pt.Add(new UILibraryWPF.WPOINT(3, 6));
                    pt.Add(new UILibraryWPF.WPOINT(9, 6));
                    pt.Add(new UILibraryWPF.WPOINT(9, 8));
                    pt.Add(new UILibraryWPF.WPOINT(12, 4));
                    pt.Add(new UILibraryWPF.WPOINT(9, 0));
                    pt.Add(new UILibraryWPF.WPOINT(9, 2));
                    pt.Add(new UILibraryWPF.WPOINT(3, 2));
                    pt.Add(new UILibraryWPF.WPOINT(3, 0));
                    pt.Add(new UILibraryWPF.WPOINT(0, 4));
                    break;
                case 22:  //Lighting
                    pt.Add(new UILibraryWPF.WPOINT(0, 3));
                    pt.Add(new UILibraryWPF.WPOINT(1, 0));
                    pt.Add(new UILibraryWPF.WPOINT(4, 1));
                    pt.Add(new UILibraryWPF.WPOINT(3, 3));
                    pt.Add(new UILibraryWPF.WPOINT(4, 4));
                    pt.Add(new UILibraryWPF.WPOINT(3, 6));
                    pt.Add(new UILibraryWPF.WPOINT(4, 7));
                    pt.Add(new UILibraryWPF.WPOINT(0, 10));
                    pt.Add(new UILibraryWPF.WPOINT(1, 7));
                    pt.Add(new UILibraryWPF.WPOINT(0, 6));
                    pt.Add(new UILibraryWPF.WPOINT(1, 4));
                    break;
                case 23:  //Sun
                case 24:  //Moon
                    Console.WriteLine("iwrap: {0} TBD???", iWrap);
                    break;
                /*------Flow Chart------------*/
                case 26:  //Process
                    break;
                case 27:  //AltProc
                    pt.Add(new UILibraryWPF.WPOINT(0, 0));
                    pt.Add(new UILibraryWPF.WPOINT(10, 8));
                    pt.Add(new UILibraryWPF.WPOINT(2, 2));
                    break;
                case 28:  //Decision
                    pt.Add(new UILibraryWPF.WPOINT(0, 2));
                    pt.Add(new UILibraryWPF.WPOINT(4, 0));
                    pt.Add(new UILibraryWPF.WPOINT(8, 2));
                    pt.Add(new UILibraryWPF.WPOINT(4, 4));
                    break;
                case 29:  //Data
                    pt.Add(new UILibraryWPF.WPOINT(0, 5));
                    pt.Add(new UILibraryWPF.WPOINT(2, 0));
                    pt.Add(new UILibraryWPF.WPOINT(10, 0));
                    pt.Add(new UILibraryWPF.WPOINT(8, 5));
                    break;
                case 30:  //PreDefProc
                    pt.Add(new UILibraryWPF.WPOINT(0, 0));
                    pt.Add(new UILibraryWPF.WPOINT(10, 8));
                    pt.Add(new UILibraryWPF.WPOINT(2, 0));
                    pt.Add(new UILibraryWPF.WPOINT(2, 8));
                    pt.Add(new UILibraryWPF.WPOINT(8, 0));
                    pt.Add(new UILibraryWPF.WPOINT(8, 8));
                    break;
                case 31:  //IntStor
                    pt.Add(new UILibraryWPF.WPOINT(0, 0));
                    pt.Add(new UILibraryWPF.WPOINT(10, 8));
                    pt.Add(new UILibraryWPF.WPOINT(0, 2));
                    pt.Add(new UILibraryWPF.WPOINT(10, 2));
                    pt.Add(new UILibraryWPF.WPOINT(2, 0));
                    pt.Add(new UILibraryWPF.WPOINT(2, 8));
                    break;
                case 32:  //Docu
                    pt.Add(new UILibraryWPF.WPOINT(0, 0));
                    pt.Add(new UILibraryWPF.WPOINT(10, 0));
                    pt.Add(new UILibraryWPF.WPOINT(10, 5));
                    pt.Add(new UILibraryWPF.WPOINT(8, 5));
                    pt.Add(new UILibraryWPF.WPOINT(3, 8));
                    pt.Add(new UILibraryWPF.WPOINT(0, 7));
                    break;
                case 33:  //MultiDocu
                    pt.Add(new UILibraryWPF.WPOINT(0, 2));
                    pt.Add(new UILibraryWPF.WPOINT(1, 2));
                    pt.Add(new UILibraryWPF.WPOINT(1, 1));
                    pt.Add(new UILibraryWPF.WPOINT(2, 1));
                    pt.Add(new UILibraryWPF.WPOINT(2, 0));
                    pt.Add(new UILibraryWPF.WPOINT(9, 0));
                    pt.Add(new UILibraryWPF.WPOINT(9, 4));
                    pt.Add(new UILibraryWPF.WPOINT(8, 4));
                    pt.Add(new UILibraryWPF.WPOINT(8, 5));
                    pt.Add(new UILibraryWPF.WPOINT(7, 5));
                    pt.Add(new UILibraryWPF.WPOINT(7, 6));
                    pt.Add(new UILibraryWPF.WPOINT(4, 7));
                    pt.Add(new UILibraryWPF.WPOINT(2, 8));
                    pt.Add(new UILibraryWPF.WPOINT(0, 7));
                    pt.Add(new UILibraryWPF.WPOINT(8, 1));
                    pt.Add(new UILibraryWPF.WPOINT(7, 2));
                    break;
                case 34:  //Terminator
                    pt.Add(new UILibraryWPF.WPOINT(0, 2));
                    pt.Add(new UILibraryWPF.WPOINT(2, 0));
                    pt.Add(new UILibraryWPF.WPOINT(8, 0));
                    pt.Add(new UILibraryWPF.WPOINT(10, 2));
                    pt.Add(new UILibraryWPF.WPOINT(8, 4));
                    pt.Add(new UILibraryWPF.WPOINT(2, 4));
                    break;
                case 35:  //Preparation
                    pt.Add(new UILibraryWPF.WPOINT(0, 3));
                    pt.Add(new UILibraryWPF.WPOINT(3, 0));
                    pt.Add(new UILibraryWPF.WPOINT(7, 0));
                    pt.Add(new UILibraryWPF.WPOINT(10, 3));
                    pt.Add(new UILibraryWPF.WPOINT(7, 6));
                    pt.Add(new UILibraryWPF.WPOINT(3, 6));
                    break;
                case 36:  //ManualInput
                    pt.Add(new UILibraryWPF.WPOINT(0, 8));
                    pt.Add(new UILibraryWPF.WPOINT(0, 3));
                    pt.Add(new UILibraryWPF.WPOINT(10, 0));
                    pt.Add(new UILibraryWPF.WPOINT(10, 8));
                    break;
                case 37:  //ManualOper
                    pt.Add(new UILibraryWPF.WPOINT(0, 0));
                    pt.Add(new UILibraryWPF.WPOINT(10, 0));
                    pt.Add(new UILibraryWPF.WPOINT(8, 6));
                    pt.Add(new UILibraryWPF.WPOINT(2, 6));
                    break;
                case 38:  //Connector
                    break;
                case 39:  //OffPageConn
                    pt.Add(new UILibraryWPF.WPOINT(0, 5));
                    pt.Add(new UILibraryWPF.WPOINT(0, 0));
                    pt.Add(new UILibraryWPF.WPOINT(6, 0));
                    pt.Add(new UILibraryWPF.WPOINT(6, 5));
                    pt.Add(new UILibraryWPF.WPOINT(3, 8));
                    break;
                case 40:  //Card
                    pt.Add(new UILibraryWPF.WPOINT(0, 6));
                    pt.Add(new UILibraryWPF.WPOINT(0, 2));
                    pt.Add(new UILibraryWPF.WPOINT(2, 0));
                    pt.Add(new UILibraryWPF.WPOINT(10, 0));
                    pt.Add(new UILibraryWPF.WPOINT(10, 6));
                    break;
                case 41:  //PunchTape
                    pt.Add(new UILibraryWPF.WPOINT(0, 1));
                    pt.Add(new UILibraryWPF.WPOINT(3, 2));
                    pt.Add(new UILibraryWPF.WPOINT(7, 0));
                    pt.Add(new UILibraryWPF.WPOINT(10, 0));
                    pt.Add(new UILibraryWPF.WPOINT(10, 5));
                    pt.Add(new UILibraryWPF.WPOINT(7, 5));
                    pt.Add(new UILibraryWPF.WPOINT(3, 7));
                    pt.Add(new UILibraryWPF.WPOINT(0, 6));
                    break;
                case 42:  //SumJunction
                    pt.Add(new UILibraryWPF.WPOINT(0, 0));
                    pt.Add(new UILibraryWPF.WPOINT(6, 6));
                    pt.Add(new UILibraryWPF.WPOINT(1, 1));
                    pt.Add(new UILibraryWPF.WPOINT(5, 5));
                    pt.Add(new UILibraryWPF.WPOINT(5, 1));
                    pt.Add(new UILibraryWPF.WPOINT(1, 5));
                    break;
                case 43:  //Or
                    pt.Add(new UILibraryWPF.WPOINT(0, 0));
                    pt.Add(new UILibraryWPF.WPOINT(6, 6));
                    pt.Add(new UILibraryWPF.WPOINT(0, 3));
                    pt.Add(new UILibraryWPF.WPOINT(6, 3));
                    pt.Add(new UILibraryWPF.WPOINT(3, 0));
                    pt.Add(new UILibraryWPF.WPOINT(3, 6));
                    break;
                case 44:  //Collate
                    pt.Add(new UILibraryWPF.WPOINT(0, 0));
                    pt.Add(new UILibraryWPF.WPOINT(6, 0));
                    pt.Add(new UILibraryWPF.WPOINT(0, 8));
                    pt.Add(new UILibraryWPF.WPOINT(6, 8));
                    break;
                case 45:  //Sort
                    pt.Add(new UILibraryWPF.WPOINT(0, 5));
                    pt.Add(new UILibraryWPF.WPOINT(3, 0));
                    pt.Add(new UILibraryWPF.WPOINT(6, 5));
                    pt.Add(new UILibraryWPF.WPOINT(3, 10));
                    break;
                case 46:  //Extract
                    pt.Add(new UILibraryWPF.WPOINT(0, 6));
                    pt.Add(new UILibraryWPF.WPOINT(3, 0));
                    pt.Add(new UILibraryWPF.WPOINT(6, 6));
                    break;
                case 47:  //Merge
                    pt.Add(new UILibraryWPF.WPOINT(0, 0));
                    pt.Add(new UILibraryWPF.WPOINT(6, 0));
                    pt.Add(new UILibraryWPF.WPOINT(3, 6));
                    break;
                case 48:  //StoredData
                    pt.Add(new UILibraryWPF.WPOINT(3, 6));
                    pt.Add(new UILibraryWPF.WPOINT(0, 5));
                    pt.Add(new UILibraryWPF.WPOINT(0, 1));
                    pt.Add(new UILibraryWPF.WPOINT(3, 0));
                    pt.Add(new UILibraryWPF.WPOINT(10, 0));
                    pt.Add(new UILibraryWPF.WPOINT(7, 1));
                    pt.Add(new UILibraryWPF.WPOINT(7, 5));
                    pt.Add(new UILibraryWPF.WPOINT(10, 6));
                    break;
                case 49:  //Delay
                    pt.Add(new UILibraryWPF.WPOINT(0, 0));
                    pt.Add(new UILibraryWPF.WPOINT(8, 2));
                    pt.Add(new UILibraryWPF.WPOINT(8, 8));
                    pt.Add(new UILibraryWPF.WPOINT(0, 10));
                    break;
                case 50:  //SeqAccStor
                    pt.Add(new UILibraryWPF.WPOINT(7, 7));
                    pt.Add(new UILibraryWPF.WPOINT(9, 7));
                    pt.Add(new UILibraryWPF.WPOINT(9, 8));
                    pt.Add(new UILibraryWPF.WPOINT(4, 8));
                    pt.Add(new UILibraryWPF.WPOINT(0, 0));
                    pt.Add(new UILibraryWPF.WPOINT(8, 8));
                    break;
                case 52:  //DirAccStor
                    pt.Add(new UILibraryWPF.WPOINT(2, 8));
                    pt.Add(new UILibraryWPF.WPOINT(0, 6));
                    pt.Add(new UILibraryWPF.WPOINT(0, 2));
                    pt.Add(new UILibraryWPF.WPOINT(2, 0));
                    pt.Add(new UILibraryWPF.WPOINT(8, 0));
                    pt.Add(new UILibraryWPF.WPOINT(10, 2));
                    pt.Add(new UILibraryWPF.WPOINT(10, 6));
                    pt.Add(new UILibraryWPF.WPOINT(8, 8));
                    pt.Add(new UILibraryWPF.WPOINT(6, 6));
                    pt.Add(new UILibraryWPF.WPOINT(6, 2));
                    break;
                case 53:  //Display
                    pt.Add(new UILibraryWPF.WPOINT(0, 3));
                    pt.Add(new UILibraryWPF.WPOINT(3, 0));
                    pt.Add(new UILibraryWPF.WPOINT(8, 0));
                    pt.Add(new UILibraryWPF.WPOINT(10, 2));
                    pt.Add(new UILibraryWPF.WPOINT(10, 4));
                    pt.Add(new UILibraryWPF.WPOINT(8, 6));
                    pt.Add(new UILibraryWPF.WPOINT(3, 6));
                    break;
                case 54:  //HorScroll
                    break;
                case 55:  //VerScroll
                    break;
            }

            if (pt.Count <= 0) return pt;
            double xrat, yrat;
            double maxx = 0, maxy = 0;

            for (int i = 0; i < pt.Count; i++)
            {
                if (pt[i].X > maxx) maxx = pt[i].X;
                if (pt[i].Y > maxy) maxy = pt[i].Y;
            }
            if (maxx > 0) xrat = (tRect.right - tRect.left) / maxx;
            else xrat = 1.0;
            if (maxy > 0) yrat = (tRect.bottom - tRect.top) / maxy;
            else yrat = 1.0;
            for (int i = 0; i < pt.Count; i++)
            {
                if (i == pt.Count - 1 && iWrap == 27)
                {
                    pt[i].X = pt[i].X * xrat;
                    pt[i].Y = pt[i].Y * yrat;
                }
                else
                {
                    pt[i].X = tRect.left + pt[i].X * xrat;
                    pt[i].Y = tRect.top + pt[i].Y * yrat;
                }
            }
            return pt;
        }
    }
}
