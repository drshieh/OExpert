//======================================================================================
// XGraphics - Handles the drawing of the graphics
//======================================================================================
using System;
using System.Windows.Controls;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using System.Reflection;
using Com.DRS.Helper;
using System.Windows;
using Com.DRS.ObjLogic;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Com.DRS.UILibraryWPF
{
    public class XGraphics
    {
        public static Canvas aCanvas = null;

        //---------------------------------------------------------------------------------------
        //-  Clears the entire drawing surface and fills it with the specified background color.  
        public static bool Clear(Color c)  
        {
            if (aCanvas != null) aCanvas.Background = new SolidColorBrush(c);
            return true;
        }

        //---------------------------------------------------------------------------------------
        //- Performs a bit-block transfer of color data, corresponding to a rectangle of pixels, from the screen to the drawing surface of the Graphics.  
        public static bool CopyFromScreen(int sx, int sy, int tx, int ty, Size sz, string copyPixelOperation)  
        {
            if (aCanvas == null) return false;
            MessageBox.Show("CopyFromScreen: TBD");
            /*
            if (!String.IsNullOrEmpty(copyPixelOperation)) aCanvas.CopyFromScreen(sx, sy, tx, ty, sz, GetCopyPixelOperation(copyPixelOperation));
            else aCanvas.CopyFromScreen(sx, sy, tx, ty, sz);
            */
            return true;
        }

        //---------------------------------------------------------------------------------------
        public static CopyPixelOperation GetCopyPixelOperation(string s)
        {
            string[] olist = Enum.GetNames(typeof(CopyPixelOperation));
            for (int i = 0; i < olist.Length; i++)
            {
                if (olist[i] == s) return (CopyPixelOperation)Enum.GetValues(typeof(CopyPixelOperation)).GetValue(i);
            }
            return 0;
        }

        //---------------------------------------------------------------------------------------
        //- Draws an arc representing a portion of an ellipse specified by a pair of coordinates, a width, and a height. 
        public static bool DrawArc(Pen p, double bx, double by, double ex, double ey, double width, double height)   
        {
            if (aCanvas == null) return false;
            aCanvas.DrawArc(p, (int)bx, (int)by, (int)bx, (int)ey, (int)width, (int)height);

            DrawingVisual dv = new DrawingVisual();
            using (DrawingContext dc = dv.RenderOpen())
            {
                Random rand = new Random();

                dc.drawa
                for (int i = 0; i < 200; i++)
                    dc.DrawRectangle(Brushes.Red, null, new Rect(rand.NextDouble() * 200, rand.NextDouble() * 200, 1, 1));

                dc.Close();
            }
            RenderTargetBitmap rtb = new RenderTargetBitmap(200, 200, 96, 96, PixelFormats.Pbgra32);
            rtb.Render(dv);
            Image img = new Image();
            img.Source = rtb;
            aCanvas.Children.Add(img);


            return true;
        }

        //---------------------------------------------------------------------------------------
        //- Draws a series of Bézier splines from an array of Point structures.  
        public static bool DrawBeziers(Pen p, Point[] pts)   
        {
            if (aCanvas == null) return false;
            aCanvas.DrawBeziers(p, pts);
            return true;
        }

        //---------------------------------------------------------------------------------------
        //- Draws a series of Bézier splines from an array of Point structures.  
        public static bool DrawBeziers(Pen p, List<WPOINT> pts)
        {
            if (aCanvas == null) return false;
            DrawBeziers(p, WPOINT2Point(pts));
            return true;
        }

        //---------------------------------------------------------------------------------------
        public static Point[] WPOINT2Point(List<WPOINT> ptList)
        {
            List<Point> aList = new List<Point>();
            foreach (WPOINT xp in ptList)
            {
                aList.Add(new Point((int)xp.X, (int)xp.Y));
            }
            return aList.ToArray();
        }

        //---------------------------------------------------------------------------------------
        //- Draws a closed cardinal spline defined by an array of Point structures.  tension: Value greater than or equal to 0.0F that specifies the tension of the curve
        public static bool DrawClosedCurve(Pen p, Point[] pts, float tension, string fillMode)   
        {
            if (aCanvas == null) return false;
            if (!String.IsNullOrEmpty(fillMode)) aCanvas.DrawClosedCurve(p, pts, tension, GetFillMode(fillMode));
            else aCanvas.DrawClosedCurve(p, pts);
            return true;
        }

        //---------------------------------------------------------------------------------------
        public static FillMode GetFillMode(string s)
        {
            string[] olist = Enum.GetNames(typeof(FillMode));
            for (int i = 0; i < olist.Length; i++)
            {
                if (olist[i] == s) return (FillMode)Enum.GetValues(typeof(FillMode)).GetValue(i);
            }
            return 0;
        }

        //---------------------------------------------------------------------------------------
        //- Draws a cardinal spline through a specified array of Point structures using a specified tension. 
        // pts: array of Point structures that define the spline. 
        // offet: Offset from the first element in the array of the points parameter to the starting point in the curve. 
        // nSeg: Number of segments after the starting point to include in the curve. 
        // tension: Value greater than or equal to 0.0F that specifies the tension of the curve. 

        public static bool DrawCurve(Pen p, Point[] pts, int offset, int nSeg, float tension)
        {
            if (aCanvas == null) return false;
            if (offset < 0)
            {
                if (tension >= 0.0) aCanvas.DrawCurve(p, pts, tension);
                else aCanvas.DrawCurve(p, pts);
            }
            else
            {
                aCanvas.DrawCurve(p, pts, offset, nSeg, tension);
            }
            return true;
        }

        //---------------------------------------------------------------------------------------
        //- Draws an ellipse specified by a bounding Rectangle structure. 
        public static bool DrawEllipse(Pen p, double bx, double by, double width, double height)    
        {
            if (aCanvas == null) return false;
            aCanvas.DrawEllipse(p, (int)bx, (int)by, (int)width, (int)height);
            return true;
        }

        //---------------------------------------------------------------------------------------
        //- Draws the image represented by the specified Icon within the area specified by a Rectangle structure. 
        public static bool DrawIcon(string iconFile, double bx, double by, double width, double height, bool fStretch)   
        {
            if (aCanvas == null) return false;
            WRECT r = new WRECT(bx, by, bx + width, by + height);
            DrawImage(iconFile, r, true);
            return true;
        }

        //---------------------------------------------------------------------------------------
        public static bool DrawImage(string imageFile, WRECT desRect, bool fClip)
        {
            if (aCanvas == null) return false;
            string sName = imageFile.Replace("\\\\", "/");
            string FileName = "../../../Data/" + sName;
            if (sName.IndexOf(":") >= 0) FileName = sName;
            // Create the image element.
            Image myImage = new Image();
            myImage.Width = desRect.right - desRect.left;
            myImage.Height = desRect.bottom - desRect.top;
            if (myImage.Width > aCanvas.ActualWidth) myImage.Width = aCanvas.ActualWidth;
            if (myImage.Height > aCanvas.ActualHeight) myImage.Height = aCanvas.ActualHeight;

            // Open a Uri and decode a BMP image
            if (FileName.ToUpper().EndsWith(".BMP"))
            {
                Uri myUri = new Uri(FileName, UriKind.RelativeOrAbsolute);
                BmpBitmapDecoder decoder = new BmpBitmapDecoder(myUri, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.Default);
                BitmapSource bitmapSource = decoder.Frames[0];
                myImage.Source = bitmapSource;
            }
            else
            {
                // Create source
                BitmapImage myBitmapImage = new BitmapImage();

                // BitmapImage.UriSource must be in a BeginInit/EndInit block
                myBitmapImage.BeginInit();
                myBitmapImage.UriSource = new Uri(FileName);

                // To save significant application memory, set the DecodePixelWidth or  
                // DecodePixelHeight of the BitmapImage value of the image source to the desired 
                // height or width of the rendered image. If you don't do this, the application will 
                // cache the image as though it were rendered as its normal size rather then just 
                // the size that is displayed.
                // Note: In order to preserve aspect ratio, set DecodePixelWidth
                // or DecodePixelHeight but not both.
                myBitmapImage.DecodePixelWidth = (int)(desRect.right - desRect.left);
                myBitmapImage.EndInit();
                //set image source
                myImage.Source = myBitmapImage;
            }
            // Draw the Image
            myImage.Stretch = Stretch.Fill;
            myImage.Margin = new Thickness(2);

            aCanvas.Children.Add(myImage);
            Canvas.SetLeft(myImage, desRect.left);
            Canvas.SetTop(myImage, desRect.top);
            return true;
        }

        //---------------------------------------------------------------------------------------
        //- Draws a line connecting two Point structures.
        public static bool DrawLine(Pen p, double bx, double by, double ex, double ey)    
        {
            if (aCanvas == null) return false;
            Line line = new Line();
            line.Stroke = p.Brush;
            line.StrokeThickness = p.Thickness;
            line.X1 = bx;
            line.Y1 = by;
            line.X2 = ex;
            line.Y2 = ey;
            Canvas.SetLeft(line, 0);
            Canvas.SetTop(line, 0);
            aCanvas.Children.Add(line);
            return true;
        }

        //---------------------------------------------------------------------------------------
        //-  Draws a GraphicsPath. 
        public static bool DrawPath(Pen p, string sPath)  
        {
            if (aCanvas == null) return false;
            aCanvas.Children.Add(CreateGraphicsPath(p, sPath));
            return true;
        }

        //------------------------------------------------------------------------
        //--Path spec:
        //   elementType:x1,y1,x2,y2,x3,y3...;elementType::x1,y1,x2,y2,x3,y3...;...
        public static Path CreateGraphicsPath(Pen p, string sPath)
        {
            Path aPath = new Path();
            aPath.Stroke = p.Brush;
            aPath.StrokeThickness = p.Thickness;
            GeometryGroup aGroup = new GeometryGroup();
            aPath.Data = aGroup;
            string[] substr = sPath.Split(';');
            foreach (string s in substr)
            {
                string[] substrx = s.Trim().Split(':');
                string sElement = substrx[0].Trim();
                if (sElement == "String")
                {
                    string pstr = substrx[1].Trim();
                    int i = 0;
                    string fFamily = "";
                    string fStyle = "";
                    double fSize = 12;
                    double bx = 0, by = 0, width = 0, height = 0;
                    string fFormat = "";
                    string sText = "";
                    string sToken = "";
                    while (pstr != null)
                    {
                        pstr = Token.oGetTOKENbyCH(pstr, (byte)',', ref sToken);
                        if (i == 0) fFamily = sToken;
                        else if (i == 1) fStyle = sToken;
                        else if (i == 2) Double.TryParse(sToken, out fSize);
                        else if (i == 3) Double.TryParse(sToken, out bx);
                        else if (i == 4) Double.TryParse(sToken, out by);
                        else if (i == 5) Double.TryParse(sToken, out width);
                        else if (i == 6) Double.TryParse(sToken, out height);
                        else if (i == 7) fFormat = sToken;
                        else if (i == 8) sText = sToken;
                    }
                    if (width <= 0 || height <= 0)
                    {
                        TextBlock textBlock = new TextBlock();
                        textBlock.FontFamily = WFont.GetFontFamily(fFamily);
                        textBlock.FontSize = fSize;
                        textBlock.Text = sText;
                        textBlock.Foreground = new SolidColorBrush(Colors.Black);
                        Canvas.SetLeft(textBlock, bx);
                        Canvas.SetTop(textBlock, by);
                        aCanvas.Children.Add(textBlock);
                    }
                    else
                    {
                        WFont.GetFitFont(width, height, fFamily, ref fSize, 0, sText);
                        TextBlock textBlock = new TextBlock();
                        textBlock.FontFamily = WFont.GetFontFamily(fFamily);
                        textBlock.FontSize = fSize;
                        textBlock.Text = sText;
                        textBlock.Foreground = new SolidColorBrush(Colors.Black);
                        Canvas.SetLeft(textBlock, bx);
                        Canvas.SetTop(textBlock, by);
                        aCanvas.Children.Add(textBlock);
                    }
                }
                else
                {
                    string[] sValues = substrx[1].Trim().Split(',');
                    List<double> fvs = MakeFloatValues(sValues);
                    int np = -1;
                    switch (sElement)
                    {
                        case "Arc":
                            ArcSegment aArc = new ArcSegment(new Point(fvs[0], fvs[1]), new Size(fvs[2], fvs[3]), fvs[4], false, SweepDirection.Clockwise, true);
                            aGroup.Children.Add(aArc);
                            break;
                        case "Bezier":
                            BezierSegment aBezier = new BezierSegment(new Point(fvs[0], fvs[1]), new Point(fvs[2], fvs[3]), new Point(fvs[4], fvs[5]), true);
                            aGroup.Children.Add(aBezier);
                            break;
                        case "ClosedCurve":
                            np = -1;
                            if ((fvs.Count % 2) > 0)
                            {
                                np = fvs.Count - 1;
                                aPath.AddClosedCurve(MakePoints(fvs, np).ToArray(), fvs[np]);
                            }
                            else aPath.AddClosedCurve(MakePoints(fvs, np).ToArray());
                            break;
                        case "Curve":
                            np = -1;
                            if ((fvs.Count % 2) > 0)
                            {
                                np = fvs.Count - 1;
                                aPath.AddCurve(MakePoints(fvs, np).ToArray(), fvs[np]);
                            }
                            else aPath.AddCurve(MakePoints(fvs, np).ToArray());
                            break;
                        case "Ellipse":
                            EllipseGeometry aEllipse = new EllipseGeometry();
                            aEllipse.Center = new Point(fvs[0], fvs[1]);
                            aEllipse.RadiusX = fvs[2];
                            aEllipse.RadiusY = fvs[3];
                            aGroup.Children.Add(aEllipse);
                            break;
                        case "Line":
                            LineGeometry aLine = new LineGeometry(new Point(fvs[0], fvs[1]), new Point(fvs[2], fvs[3]));
                            aGroup.Children.Add(aLine);
                            break;
                        case "Lines":
                            PolyLineSegment a = new PolyLineSegment(MakePoints(fvs, np), true);
                            aGroup.Children.Add(a);
                            break;
                        case "Pie":
                            aPath.AddPie(fvs[0], fvs[1], fvs[2], fvs[3], fvs[4], fvs[5]);
                            break;
                        case "Polygon":
                            aPath.AddPolygon(MakePoints(fvs, -1).ToArray());
                            break;
                        case "Rectangle":
                            aPath.AddRectangle(new RectangleF(fvs[0], fvs[1], fvs[2], fvs[3]));
                            break;
                    }
                }
            }
            return aPath;
        }

        //------------------------------------------------------------------------
        public static List<double> MakeFloatValues(string[] sValues)
        {
            float v = 0;
            List<double> rList = new List<double>();
            foreach (string s in sValues)
            {
                if (Double.TryParse(s.Trim(), out v)) rList.Add(v);
            }
            return rList;
        }

        //------------------------------------------------------------------------
        public static List<Point> MakePoints(List<double> fvs, int nv)
        {
            if (nv <= 0) nv = fvs.Count();
            List<Point> rList = new List<Point>();
            int npoints = nv / 2;
            int ip = 0;
            for (int i = 0; i < npoints; i++)
            {
                ip = i * 2;
                rList.Add(new Point(fvs[ip], fvs[ip + 1]));
            }
            return rList;
        }

        //------------------------------------------------------------------------
        public static List<Point> MakePointFs(string[] sPoints)
        {
            float x = 0;
            float y = 0;
            List<Point> rList = new List<Point>();
            foreach (string s in sPoints)
            {
                string[] substr = s.Split(',');
                Double.TryParse(substr[0].Trim(), out x);
                Double.TryParse(substr[1].Trim(), out y);
                rList.Add(new Point(x, y));
            }
            return rList;
        }

        //------------------------------------------------------------------------
        ////- Draws a pie shape defined by an ellipse specified by a coordinate pair, a width, a height, and two radial lines
        public static bool DrawPie(Pen p, double bx, double by, double ex, double ey, double width, double height)
        {
            if (aCanvas == null) return false;
            aCanvas.DrawArc(p, (int)bx, (int)by, (int)ex, (int)ey, (int)width, (int)height);
            return true;
        }

        //---------------------------------------------------------------------------------------
        // Draws a polygon defined by an array of Point structures. 
        public static bool DrawPolygon(Pen p, Point[] pts)    
        {
            if (aCanvas == null) return false;
            aCanvas.DrawPolygon(p, pts);
            return true;
        }

        //---------------------------------------------------------------------------------------
        // Draws a polygon defined by an array of Point structures. 
        public static bool DrawPolygon(Pen p, List<WPOINT> pts)
        {
            if (aCanvas == null) return false;
            List<Point> aList = new List<Point>();
            foreach (WPOINT xp in pts)
            {
                aList.Add(new Point((int)xp.X, (int)xp.Y));
            }
            aCanvas.DrawPolygon(p, aList.ToArray());
            return true;
        }

        //---------------------------------------------------------------------------------------
        //- Draws a rectangle specified by a coordinate pair, a width, and a height. 
        public static bool DrawRectangle(Pen p, double bx, double by, double width, double height)    
        {
            if (aCanvas == null) return false;
            aCanvas.DrawRectangle(p, (int)bx, (int)by, (int)width, (int)height);
            return true;
        }

        //---------------------------------------------------------------------------------------
        //- Draws a series of rectangles specified by Rectangle structures. 
        public static bool DrawRectangles(Pen p, Rectangle[] rects)    
        {
            if (aCanvas == null) return false;
            aCanvas.DrawRectangles(p, rects);
            return true;
        }

        //---------------------------------------------------------------------------------------
        //- Draws the specified text string at the specified location with the specified Brush and Font objects.  
        public static bool DrawString(String str, string fontName, int fontSize, string fontStyle, string brushColor, RectangleF rect, string sfmt)   
        {
            if (aCanvas == null) return false;
            Font f = WFont.SetFont(fontName, fontSize, fontStyle);
            return DrawString(str, f, brushColor, rect, sfmt);
        }

        //---------------------------------------------------------------------------------------
        //- Draws the specified text string at the specified location with the specified Brush and Font objects.  
        public static bool DrawString(String str, string fontName, int fontSize, int ifontStyle, string brushColor, RectangleF rect, string sfmt)
        {
            if (aCanvas == null) return false;
            Font f = WFont.SetFont(fontName, fontSize, ifontStyle);
            return DrawString(str, f, brushColor, rect, sfmt);
        }

        //---------------------------------------------------------------------------------------
        //- Draws the specified text string at the specified location with the specified Brush and Font objects.  
        public static bool DrawString(String str, int iFont, int fontSize, string fontStyle, string brushColor, RectangleF rect, string sfmt)
        {
            if (aCanvas == null) return false;
            Font f = WFont.SetFont(iFont, fontSize, fontStyle);
            return DrawString(str, f, brushColor, rect, sfmt);
        }

        //---------------------------------------------------------------------------------------
        //- Draws the specified text string at the specified location with the specified Brush and Font objects.  
        public static bool DrawString(String str, int iFont, int fontSize, int ifontStyle, string brushColor, RectangleF rect, string sfmt)
        {
            if (aCanvas == null) return false;
            Font f = WFont.SetFont(iFont, fontSize,ifontStyle);
            return DrawString(str, f, brushColor, rect, sfmt);
        }

        //---------------------------------------------------------------------------------------
        //- Draws the specified text string at the specified location with the specified Brush and Font objects.  
        public static bool DrawString(String str, Font hFont, string brushColor, RectangleF rect, string sfmt)
        {
            if (aCanvas == null) return false;
            Brush b = XBrush.CreateSolidBrush(brushColor);
            if (!String.IsNullOrEmpty(sfmt))
            {
                if (rect.Width == rect.Height) aCanvas.DrawString(str, hFont, b, rect, WFONT.GetStringFormat(sfmt));
                else aCanvas.DrawString(str, hFont, b, rect.Left, rect.Top, WFONT.GetStringFormat(sfmt));
            }
            else
            {
                if (rect.Width == rect.Height) aCanvas.DrawString(str, hFont, b, rect.Left, rect.Top);
                else aCanvas.DrawString(str, hFont, b, rect);
            }
            return true;
        }

        //---------------------------------------------------------------------------------------
        //- Fills the interior of a closed cardinal spline curve defined by an array of Point structures using the specified fill mode and tension.  
        public static bool FillClosedCurve(DBrush bd, Point[] pts, string fillMode, float tension)
        {
            if (aCanvas == null) return false;
            Brush b = XBrush.CreateBrush(bd);
            if (b == null) return false;
            FillClosedCurve(b, pts, fillMode, tension);
            return true;
        }

        //---------------------------------------------------------------------------------------
        //- Fills the interior of a closed cardinal spline curve defined by an array of Point structures using the specified fill mode and tension.  
        public static bool FillClosedCurve(Brush b, Point[] pts, string fillMode, float tension)   
        {
            if (aCanvas == null) return false;
            if (!String.IsNullOrEmpty(fillMode))
            {
                if (tension > 0) aCanvas.FillClosedCurve(b, pts, GetFillMode(fillMode), tension);
                else aCanvas.FillClosedCurve(b, pts, GetFillMode(fillMode));
            }
            else
            {
                aCanvas.FillClosedCurve(b, pts);
            }
            return true;
        }

        //---------------------------------------------------------------------------------------
        //- Fills the interior of an ellipse defined by a bounding rectangle specified by a pair of coordinates, a width, and a height.  
        public static bool FillEllipse(DBrush bd, double bx, double by, double width, double height)   
        {
            if (aCanvas == null) return false;
            Brush b = XBrush.CreateBrush(bd);
            if (b == null) return false;
            aCanvas.FillEllipse(b, (int)bx, (int)by, (int)width, (int)height);
            return true;
        }

        //---------------------------------------------------------------------------------------
        //- Fills the interior of an ellipse defined by a bounding rectangle specified by a pair of coordinates, a width, and a height.  
        public static bool FillEllipse(Brush b, double bx, double by, double width, double height)
        {
            if (aCanvas == null) return false;
            aCanvas.FillEllipse(b, (int)bx, (int)by, (int)width, (int)height);
            return true;
        }

        //---------------------------------------------------------------------------------------
        //-Fills the interior of a GraphicsPath.  
        public static bool FillPath(DBrush bd, string sPath) 
        {
            if (aCanvas == null) return false;
            Brush b = XBrush.CreateBrush(bd);
            if (b == null) return false;
            aCanvas.FillPath(b, CreateGraphicsPath(sPath));
            return true;
        }

        //---------------------------------------------------------------------------------------
        //-Fills the interior of a GraphicsPath.  
        public static bool FillPath(Brush b, string sPath)
        {
            if (aCanvas == null) return false;
            aCanvas.FillPath(b, CreateGraphicsPath(sPath));
            return true;
        }

        //---------------------------------------------------------------------------------------
        //- Fills the interior of a pie section defined by an ellipse specified by a pair of coordinates, a width, a height, and two radial lines.  
        public static bool FillPie(DBrush bd, int bx, int by, int width, int height, int startAngle, int sweepAngle)   
        {
            if (aCanvas == null) return false;
            Brush b = XBrush.CreateBrush(bd);
            if (b == null) return false;
            aCanvas.FillPie(b, bx, by, width, height, startAngle, sweepAngle);
            return true;
        }

        //---------------------------------------------------------------------------------------
        //- Fills the interior of a pie section defined by an ellipse specified by a pair of coordinates, a width, a height, and two radial lines.  
        public static bool FillPie(Brush b, int bx, int by, int width, int height, int startAngle, int sweepAngle)
        {
            if (aCanvas == null) return false;
            aCanvas.FillPie(b, bx, by, width, height, startAngle, sweepAngle);
            return true;
        }

        //---------------------------------------------------------------------------------------
        //- Fills the interior of a polygon defined by an array of points specified by Point structures.  
        public static bool FillPolygon(DBrush bd, Point[] pts, string fillMode)   
        {
            if (aCanvas == null) return false;
            Brush b = XBrush.CreateBrush(bd);
            if (b == null) return false;
            if (String.IsNullOrEmpty(fillMode)) aCanvas.FillPolygon(b, pts);
            else aCanvas.FillPolygon(b, pts, GetFillMode(fillMode));
            return true;
        }

        //---------------------------------------------------------------------------------------
        //- Fills the interior of a polygon defined by an array of points specified by Point structures.  
        public static bool FillPolygon(Brush b, Point[] pts, string fillMode)
        {
            if (aCanvas == null) return false;
            if (String.IsNullOrEmpty(fillMode)) aCanvas.FillPolygon(b, pts);
            else aCanvas.FillPolygon(b, pts, GetFillMode(fillMode));
            return true;
        }

        //---------------------------------------------------------------------------------------
        //- Fills the interior of a rectangle specified by a Rectangle structure. 
        public static bool FillRectangle(DBrush bd, double bx, double by, double width, double height)    
        {
            if (aCanvas == null) return false;
            Brush b = XBrush.CreateBrush(bd);
            if (b == null) return false;
            aCanvas.FillRectangle(b, (int)bx, (int)by, (int)width, (int)height);
            return true;
        }

        //---------------------------------------------------------------------------------------
        //- Fills the interior of a rectangle specified by a Rectangle structure. 
        public static bool FillRectangle(Brush b, double bx, double by, double width, double height)
        {
            if (aCanvas == null) return false;
            aCanvas.FillRectangle(b, (int)bx, (int)by, (int)width, (int)height);
            return true;
        }

        //---------------------------------------------------------------------------------------
        //- Fills the interiors of a series of rectangles specified by Rectangle structures. 
        public static bool FillRectangles(DBrush bd, Rectangle[] rects)    
        {
            if (aCanvas == null) return false;
            Brush b = XBrush.CreateBrush(bd);
            if (b == null) return false;
            aCanvas.FillRectangles(b, rects);
            return true;
        }

        //---------------------------------------------------------------------------------------
        //- Fills the interiors of a series of rectangles specified by Rectangle structures. 
        public static bool FillRectangles(Brush b, Rectangle[] rects)
        {
            if (aCanvas == null) return false;
            aCanvas.FillRectangles(b, rects);
            return true;
        }

        //---------------------------------------------------------------------------------------
        //- Measures the specified string when drawn with the specified Font within the specified layout area.  
        public static SizeF MeasureString(String str, string fontName, double fontSize, string fontStyle, double bx, double by, string stringFormat)   
        {
            if (aCanvas == null || String.IsNullOrEmpty(str)) return new SizeF(0, 0);
            Font f = WFONT.SetFont(fontName, (float)fontSize, fontStyle);
            SizeF siz;
            if (String.IsNullOrEmpty(stringFormat))
            {
                if (bx < 0 || by < 0) siz = aCanvas.MeasureString(str, f);
                else siz = aCanvas.MeasureString(str, f, new SizeF((float)bx, (float)by));
            }
            else siz = aCanvas.MeasureString(str, f, new PointF((int)bx, (int)by), WFONT.GetStringFormat(stringFormat));
            return siz;
        }

        //---------------------------------------------------------------------------------------
        //- Measures the specified string when drawn with the specified Font within the specified layout area.  
        public static SizeF MeasureString(String str, int iFont, double fontSize, int fStyle, double bx, double by, string stringFormat)
        {
            if (aCanvas == null || String.IsNullOrEmpty(str)) return new SizeF(0, 0);
            Font f = WFONT.SetFont(WFONT.FontFamilyList[iFont], (float)fontSize, WFONT.GetFontStyleStr(fStyle));
            SizeF siz;
            if (String.IsNullOrEmpty(stringFormat))
            {
                if (bx < 0 || by < 0) siz = aCanvas.MeasureString(str, f);
                else
                {
                    SizeF bsiz = new SizeF((float)bx, (float)by);
                    if(bsiz != null) siz = aCanvas.MeasureString(str, f, bsiz);
                    else siz = aCanvas.MeasureString(str, f);
                }
            }
            else
            {
                siz = aCanvas.MeasureString(str, f, new PointF((float)bx, (float)by), WFONT.GetStringFormat(stringFormat));
            }
            return siz;
        }

        //---------------------------------------------------------------------------------------
        //- Measures the specified string when drawn with the specified Font within the specified layout area.  
        public static SizeF MeasureString(String str, Font hFont, double bx, double by, string stringFormat)
        {
            if (aCanvas == null || String.IsNullOrEmpty(str)) return new SizeF(0, 0);
            SizeF siz;
            if (String.IsNullOrEmpty(stringFormat))
            {
                if (bx < 0 || by < 0) siz = aCanvas.MeasureString(str, hFont);
                else siz = aCanvas.MeasureString(str, hFont, new SizeF((float)bx, (float)by));
            }
            else siz = aCanvas.MeasureString(str, hFont, new PointF((float)bx, (float)by), WFONT.GetStringFormat(stringFormat));
            return siz;
        }

    }
}
