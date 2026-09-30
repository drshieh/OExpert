using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Media;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Media.Imaging;
using System.IO;

namespace OExpert.UILibraryWPF
{
    public static class WVisual2Image
    {
        public const double defaultDpi = 96.0;

        //----------------------------------------------------------------------------
        public static ImageSource RenderToPNGImageSource(Visual targetControl, WRECT bound)
        {
            var renderTargetBitmap = GetRenderTargetBitmapFromControl(targetControl, bound);

            var encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(renderTargetBitmap));

            var result = new BitmapImage();

            using (var memoryStream = new MemoryStream())
            {
                encoder.Save(memoryStream);
                memoryStream.Seek(0, SeekOrigin.Begin);

                result.BeginInit();
                result.CacheOption = BitmapCacheOption.OnLoad;
                result.StreamSource = memoryStream;
                result.EndInit();
            }

            return result;
        }

        //----------------------------------------------------------------------------
        public static BitmapSource GetRenderTargetBitmapFromControl(Visual targetControl, double dpi = defaultDpi)
        {
            if (targetControl == null) return null;
            Rect bounds = VisualTreeHelper.GetDescendantBounds(targetControl);
            if (bounds == null) bounds = new Rect(0, 0, 800, 600);
            var renderTargetBitmap = new RenderTargetBitmap((int)(bounds.Width * dpi / 96.0),
                                                            (int)(bounds.Height * dpi / 96.0),
                                                            dpi,
                                                            dpi,
                                                            PixelFormats.Pbgra32);

            var drawingVisual = new DrawingVisual();

            using (var drawingContext = drawingVisual.RenderOpen())
            {
                var visualBrush = new VisualBrush(targetControl);
                drawingContext.DrawRectangle(visualBrush, null, new Rect(new Point(), bounds.Size));
            }

            renderTargetBitmap.Render(drawingVisual);
            return renderTargetBitmap;
        }


        //----------------------------------------------------------------------------
        public static BitmapSource GetRenderTargetBitmapFromControl(Visual targetControl, WRECT bounds, double dpi = defaultDpi)
        {
            if (targetControl == null) return null;

            var renderTargetBitmap = new RenderTargetBitmap((int)(bounds.right * dpi / 96.0),
                                                            (int)(bounds.bottom * dpi / 96.0),
                                                            dpi,
                                                            dpi,
                                                            PixelFormats.Pbgra32);

            var drawingVisual = new DrawingVisual();

            using (var drawingContext = drawingVisual.RenderOpen())
            {
                var visualBrush = new VisualBrush(targetControl);
                Rect tRect = new Rect(bounds.left, bounds.top, bounds.right - bounds.left, bounds.bottom - bounds.top);
                drawingContext.DrawRectangle(visualBrush, null, tRect);
            }

            renderTargetBitmap.Render(drawingVisual);
            return renderTargetBitmap;
        }
    }
}
