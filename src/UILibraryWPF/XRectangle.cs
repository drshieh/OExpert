using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace OExpert.UILibraryWPF
{
    public class XRectangle
    {
        public Rectangle rect;
        
        public void Add(WDrawingCanvas aCanvas, double bx, double by, double width, double height)
        {
            rect = new Rectangle();
            rect.Stroke = Brushes.LightBlue;
            rect.Fill = Brushes.Yellow;
            rect.StrokeThickness = 2;
            rect.Width = width;
            rect.Height = height;
            Canvas.SetLeft(rect, bx);
            Canvas.SetTop(rect, by);
            aCanvas.Children.Add(rect);
        }
    }
}
