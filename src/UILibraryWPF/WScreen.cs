using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Interop;

namespace OExpert.UILibraryWPF
{
    public class WScreen
    {
        public static void GetScreenSize(ref double width, ref double height)
        {
            width = System.Windows.SystemParameters.VirtualScreenWidth;
            height = System.Windows.SystemParameters.VirtualScreenHeight;
        }

        public static IEnumerable<WScreen> AllScreens()
        {
            foreach (Screen screen in System.Windows.Forms.Screen.AllScreens)
            {
                yield return new WScreen(screen);
            }
        }

        public static WScreen GetScreenFrom(Window window)
        {
            WindowInteropHelper windowInteropHelper = new WindowInteropHelper(window);
            Screen screen = System.Windows.Forms.Screen.FromHandle(windowInteropHelper.Handle);
            WScreen wpfScreen = new WScreen(screen);
            return wpfScreen;
        }

        public static WScreen GetScreenFrom(System.Windows.Point point)
        {
            int x = (int)Math.Round(point.X);
            int y = (int)Math.Round(point.Y);

            // are x,y device-independent-pixels ??
            System.Drawing.Point drawingPoint = new System.Drawing.Point(x, y);
            Screen screen = Screen.FromPoint(drawingPoint);
            WScreen wpfScreen = new WScreen(screen);

            return wpfScreen;
        }

        public static WScreen Primary
        {
            get { return new WScreen(System.Windows.Forms.Screen.PrimaryScreen); }
        }

        private readonly Screen screen;

        internal WScreen(System.Windows.Forms.Screen screen)
        {
            this.screen = screen;
        }

        public Rect DeviceBounds
        {
            get { return this.GetRect(this.screen.Bounds); }
        }

        public Rect WorkingArea
        {
            get { return this.GetRect(this.screen.WorkingArea); }
        }

        private Rect GetRect(Rectangle value)
        {
            // should x, y, width, height be device-independent-pixels ??
            return new Rect
            {
                X = value.X,
                Y = value.Y,
                Width = value.Width,
                Height = value.Height
            };
        }

        public bool IsPrimary
        {
            get { return this.screen.Primary; }
        }

        public string DeviceName
        {
            get { return this.screen.DeviceName; }
        }
    }
}
