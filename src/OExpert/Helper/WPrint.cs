using OExpert.UILibraryWPF;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Printing;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Xps.Packaging;
using PdfSharp;
using System.Windows.Xps;
using System.Windows;
using System.Diagnostics;

namespace OExpert
{
    public class WPrint
    {

        //--------------------------------------------------------------------------------
        public static void PrintVisual(DrawingVisual visual, string sObjType, string sObjName, WRECT upRect)
        {
            //            ImageSource aSource = WVisual2Image.RenderToPNGImageSource(visual, upRect);
            BitmapSource aSource = WVisual2Image.GetRenderTargetBitmapFromControl(visual, upRect, WVisual2Image.defaultDpi);
            Bitmap originalBitmap = BitmapFromSource(aSource);
            string sDir = WHelper.ValidateDir("TEMP", true);
            if (!Directory.Exists(sDir)) Directory.CreateDirectory(sDir);
            originalBitmap.Save(String.Format(sDir + "{0}.BMP", sObjName));

            double paperX = 8; //-inches: 8
            double paperY = 10.5;  //-inches: 10.5
            if (GlobalVarX.PrintOrientation == 1)  //-Landscape
            {
                double v = paperX;
                paperX = paperY;
                paperY = v;
            }
            double xWidth = paperX * WVisual2Image.defaultDpi;
            double xHeight = paperY * WVisual2Image.defaultDpi;
            //            double xWidth = GlobalVarX.PaperSizeWidth * WVisual2Image.defaultDpi;
            //            double xHeight = GlobalVarX.PaperSizeHeight * WVisual2Image.defaultDpi;
            int ix = (int)(originalBitmap.Width / xWidth);
            if (ix * xWidth < originalBitmap.Width) ix++;
            int iy = (int)(originalBitmap.Height / xHeight);
            if (iy * xHeight < originalBitmap.Height) iy++;

            /*
            if (ix <= 1 && iy <= 1)
            {
                xWidth = originalBitmap.Width;
                xHeight = originalBitmap.Height;
            }
            */

            //            List<Bitmap> aBmpList = SplitImage(aSource, ix, iy);
            List<XBitmap> aBmpList = SplitImage(aSource, xWidth, xHeight);
            for (int i = 0; i < aBmpList.Count; i++)
            {
                aBmpList[i].bitmap.Save(String.Format(sDir + "{0}-{1:00}-{2:00}.BMP", sObjName, aBmpList[i].row, aBmpList[i].col));
            }

            FlowDocument doc = new FlowDocument();
            //            doc.ColumnWidth = 2 * paperX * WVisual2Image.defaultDpi;
            doc.ColumnWidth = xWidth;
            doc.PageWidth = xWidth;
            doc.PageHeight = xHeight;
            int iPage = 1;
            string sTitle = String.Format("{0}: {1}", sObjType, sObjName);
            foreach (XBitmap aBmp in aBmpList)
            {
                Section section = new Section();
                if (iPage > 1) section.BreakPageBefore = true;
                string tstr = sTitle;
                if (ix > 1 || iy > 1) tstr += String.Format(" (page {0}-{1})", aBmp.row, aBmp.col);
                Paragraph p = new Paragraph(new Run(tstr));
                p.FontSize = 8;
                section.Blocks.Add(p);

                System.Windows.Controls.Image aImage = new System.Windows.Controls.Image();
                aImage.Source = BitmapToSource(aBmp.bitmap);
                aImage.Width = aBmp.bitmap.Width * 0.95;
                aImage.Height = aBmp.bitmap.Height * 0.9;
                aImage.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
                aImage.VerticalAlignment = System.Windows.VerticalAlignment.Top;
                BlockUIContainer aContainer = new BlockUIContainer(aImage);
                section.Blocks.Add(aContainer);

                doc.Blocks.Add(section);
                aBmp.bitmap.Dispose();
                iPage++;
            }

            doc.Name = "RuleNet";
            
            InvokePrint("RuleNet", doc);
        }

        //--------------------------------------------------------------------------------
        public static void PrintVisual2PDF(DrawingVisual visual, string sObjType, string sObjName, WRECT upRect, string pdfFile)
        {
            //            ImageSource aSource = WVisual2Image.RenderToPNGImageSource(visual, upRect);
            BitmapSource aSource = WVisual2Image.GetRenderTargetBitmapFromControl(visual, upRect, WVisual2Image.defaultDpi);
            Bitmap originalBitmap = BitmapFromSource(aSource);
            string sDir = WHelper.ValidateDir("TEMP", true);
            if (!Directory.Exists(sDir)) Directory.CreateDirectory(sDir);
            originalBitmap.Save(String.Format(sDir + "{0}.BMP", sObjName));

            double paperX = 8; //-inches: 8
            double paperY = 10.5;  //-inches: 10.5
            if (GlobalVarX.PrintOrientation == 1)  //-Landscape
            {
                double v = paperX;
                paperX = paperY;
                paperY = v;
            }
            double xWidth = paperX * WVisual2Image.defaultDpi;
            double xHeight = paperY * WVisual2Image.defaultDpi;
            //            double xWidth = GlobalVarX.PaperSizeWidth * WVisual2Image.defaultDpi;
            //            double xHeight = GlobalVarX.PaperSizeHeight * WVisual2Image.defaultDpi;
            int ix = (int)(originalBitmap.Width / xWidth);
            if (ix * xWidth < originalBitmap.Width) ix++;
            int iy = (int)(originalBitmap.Height / xHeight);
            if (iy * xHeight < originalBitmap.Height) iy++;

            List<XBitmap> aBmpList = SplitImage(aSource, xWidth, xHeight);

            FlowDocument doc = new FlowDocument();
            doc.ColumnWidth = xWidth;
            doc.PageWidth = xWidth;
            doc.PageHeight = xHeight;
            int iPage = 1;
            string sTitle = String.Format("{0}: {1}", sObjType, sObjName);
            foreach (XBitmap aBmp in aBmpList)
            {
                Section section = new Section();
                if (iPage > 1) section.BreakPageBefore = true;
                string tstr = sTitle;
                if (ix > 1 || iy > 1) tstr += String.Format(" (page {0}-{1})", aBmp.row, aBmp.col);
                Paragraph p = new Paragraph(new Run(tstr));
                p.FontSize = 8;
                section.Blocks.Add(p);

                System.Windows.Controls.Image aImage = new System.Windows.Controls.Image();
                aImage.Source = BitmapToSource(aBmp.bitmap);
                aImage.Width = aBmp.bitmap.Width * 0.95;
                aImage.Height = aBmp.bitmap.Height * 0.9;
                aImage.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
                aImage.VerticalAlignment = System.Windows.VerticalAlignment.Top;
                BlockUIContainer aContainer = new BlockUIContainer(aImage);
                section.Blocks.Add(aContainer);

                doc.Blocks.Add(section);
                aBmp.bitmap.Dispose();
                iPage++;
            }

            doc.Name = "RuleNet";

            var test = (IDocumentPaginatorSource)doc;
            string xpsFile = String.Format(sDir + "{0}.xps", sObjName);

            XpsDocument xpsDocument = new XpsDocument(xpsFile, FileAccess.ReadWrite);
            XpsDocumentWriter writer = XpsDocument.CreateXpsDocumentWriter(xpsDocument);
            writer.Write(test.DocumentPaginator);
            xpsDocument.Close();
            PdfSharp.Xps.XpsConverter.Convert(xpsFile, pdfFile, 0);
            File.Delete(xpsFile);
            string sMsg = "Rule Netork printed to PDF file:\r\n " + pdfFile + "\r\n\r\nWant to View it?";
            if(MessageBox.Show(sMsg, "Information", MessageBoxButton.YesNo, MessageBoxImage.Exclamation) ==
                               MessageBoxResult.Yes)
            {
                Process.Start(pdfFile);
            }
        }

        //-----------------------------------------------------------------
        public static int InvokePrint(string sName, FlowDocument doc)
        {
            // Create the print dialog object and set options
            PrintDialog pDialog = new PrintDialog();
            pDialog.PageRangeSelection = PageRangeSelection.AllPages;
            pDialog.UserPageRangeEnabled = true;

            // Display the dialog. This returns true if the user presses the Print button.
            Nullable<Boolean> fRtn = pDialog.ShowDialog();
            if (fRtn == true)
            {
                IDocumentPaginatorSource idpSource = doc;
                if (GlobalVarX.PrintOrientation == 1) pDialog.PrintTicket.PageOrientation = System.Printing.PageOrientation.Landscape;
                else if (GlobalVarX.PrintOrientation == 1) pDialog.PrintTicket.PageOrientation = System.Printing.PageOrientation.Portrait;

                pDialog.PrintDocument(idpSource.DocumentPaginator, sName);
                return 1;
            }
            return 0;
        }

        //------------------------------------------------------------------------
        public static List<XBitmap> SplitImage(BitmapSource aSource, double width, double height)
        {
            List<XBitmap> aBmpList = new List<XBitmap>();
            // Original image
            Bitmap Original_Image = BitmapFromSource(aSource);
            // Original image pixel format
            System.Drawing.Imaging.PixelFormat Pixel_Format = Original_Image.PixelFormat;

            // Set new Tile
            int nRow = (int)(Original_Image.Height / height);
            if (nRow * height < Original_Image.Height) nRow++;
            int nColumn = (int)(Original_Image.Width / width);
            if (nColumn * width < Original_Image.Width) nColumn++;

            RectangleF tRect = new RectangleF();

            for (int iRow = 0; iRow < nRow; iRow++)  //--row
            {
                tRect.Y = (float)(iRow * height);
                if (iRow == nRow - 1) tRect.Height = (float)(Original_Image.Height - iRow * height);
                else tRect.Height = (float)height;
                for (int iColumn = 0; iColumn < nColumn; iColumn++)  //--Column
                {
                    tRect.X = (float)(iColumn * width);
                    if (iColumn == nColumn - 1) tRect.Width = (float)(Original_Image.Width - iColumn * width);
                    else tRect.Width = (float)width;
                    aBmpList.Add(new XBitmap(iRow, iColumn, Original_Image.Clone(tRect, Pixel_Format)));
                }
            }
            Original_Image.Dispose();
            return aBmpList;
        }

        //-----------------------------------------------------------------
        public static BitmapSource BitmapToSource(System.Drawing.Bitmap bitmap)
        {
            var bitmapData = bitmap.LockBits(
                new System.Drawing.Rectangle(0, 0, bitmap.Width, bitmap.Height),
                System.Drawing.Imaging.ImageLockMode.ReadOnly, bitmap.PixelFormat);

            var bitmapSource = BitmapSource.Create(
                bitmapData.Width, bitmapData.Height,
                bitmap.HorizontalResolution, bitmap.VerticalResolution,
                PixelFormats.Bgra32, null,
                bitmapData.Scan0, bitmapData.Stride * bitmapData.Height, bitmapData.Stride);

            bitmap.UnlockBits(bitmapData);
            return bitmapSource;
        }

        //------------------------------------------------------------------------
        public static Bitmap BitmapFromSource(BitmapSource bitmapsource)
        {
            //convert image format
            var src = new System.Windows.Media.Imaging.FormatConvertedBitmap();
            src.BeginInit();
            src.Source = bitmapsource;
            //            src.DestinationFormat = System.Windows.Media.PixelFormats.Bgra32;
            src.DestinationFormat = PixelFormats.Bgra32;
            src.EndInit();

            //copy to bitmap
            Bitmap bitmap = new Bitmap(src.PixelWidth, src.PixelHeight, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            var data = bitmap.LockBits(new Rectangle(System.Drawing.Point.Empty, bitmap.Size), System.Drawing.Imaging.ImageLockMode.WriteOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            src.CopyPixels(System.Windows.Int32Rect.Empty, data.Scan0, data.Height * data.Stride, data.Stride);
            bitmap.UnlockBits(data);
            return bitmap;
        }

        //============================PrinterASCIIFile================================
        public static Font verdana10Font = new Font("Verdana", 6);
        public static StreamReader srPrint = null;
        public static int iPrintPage = 0;
        public static string sPrintFile;

        //---------------------------------------------------------
        public static void PrinterASCIIFile(string sFileName)
        {
            //double paperX = 8; //-inches
            //double paperY = 10.5;  //-inches
            //if (GlobalVarX.PrintOrientation == 1)  //-Landscape
            //{
            //    double v = paperX;
            //    paperX = paperY;
            //    paperY = v;
            //}
            //double xWidth = paperX * WVisual2Image.defaultDpi;
            //double xHeight = paperY * WVisual2Image.defaultDpi;

            sPrintFile = sFileName;
            PrintDialog printDialog = new PrintDialog();
            if ((bool)printDialog.ShowDialog().GetValueOrDefault())
            {
                iPrintPage = 1;
                srPrint = new StreamReader(sFileName);
                //                FlowDocument doc = new FlowDocument();
                //                doc.ColumnWidth = xWidth;
                //                doc.PageWidth = xWidth;
                //                doc.PageHeight = xHeight;
                PrintDocument pd = new PrintDocument();
                pd.PrintPage += new PrintPageEventHandler(PrintTextFileHandler);
                pd.Print();
                //Close the reader  
                if (srPrint != null)
                    srPrint.Close();
                //while (!srPrint.EndOfStream)
                //{
                //    string sLine = srPrint.ReadLine();
                //    Paragraph myParagraph = new Paragraph();
                //    myParagraph.Margin = new Thickness(0);
                //    myParagraph.Inlines.Add(new Run(sLine));
                //    doc.Blocks.Add(myParagraph);
                //}
                //srPrint.Close();
                //DocumentPaginator paginator = ((IDocumentPaginatorSource)doc).DocumentPaginator;
                //printDialog.PrintDocument(paginator, sTitle);
            }
        }

        //---------------------------------------------------
        private static void PrintTextFileHandler(object sender, PrintPageEventArgs ppeArgs)
        {
            //Get the Graphics object  
            Graphics g = ppeArgs.Graphics;
            float linesPerPage = 0;
            float yPos = 0;
            int count = 0;
            //Read margins from PrintPageEventArgs  
            float leftMargin = ppeArgs.MarginBounds.Left;
            float topMargin = ppeArgs.MarginBounds.Top;

            g.DrawString("--" + sPrintFile + "--", verdana10Font, System.Drawing.Brushes.Black, (float)(leftMargin + ppeArgs.MarginBounds.Width) / 2 - 30, (float)(topMargin / 2), new StringFormat());
            g.DrawString("-" + iPrintPage + "-", verdana10Font, System.Drawing.Brushes.Black, (float)(leftMargin + ppeArgs.MarginBounds.Width), (float)(topMargin / 2), new StringFormat());
            iPrintPage++;

            string sLine = null;
            //Calculate the lines per page on the basis of the height of the page and the height of the font  
            linesPerPage = ppeArgs.MarginBounds.Height / verdana10Font.GetHeight(g);
            //Now read lines one by one, using StreamReader  
            while (count < linesPerPage && ((sLine = srPrint.ReadLine()) != null))
            {
                if (sLine.IndexOf("<PAGEBREAK>") >= 0) break;
                //Calculate the starting position  
                yPos = topMargin + (count * verdana10Font.GetHeight(g));
                //Draw text  
                g.DrawString(sLine, verdana10Font, System.Drawing.Brushes.Black, leftMargin, yPos, new StringFormat());
                //Move to next line  
                count++;
            }
            //If PrintPageEventArgs has more pages to print  
            if (sLine != null)
            {
                ppeArgs.HasMorePages = true;
            }
            else
            {
                ppeArgs.HasMorePages = false;
            }
        }
    }

    //====================================================
    public class XBitmap
    {
        public int row;
        public int col;
        public Bitmap bitmap;

        public XBitmap()
        {

        }
        public XBitmap(int _row, int _col, Bitmap _bitmap)
        {
            row = _row; 
            col = _col; 
            bitmap = _bitmap;
        }
    }
}
