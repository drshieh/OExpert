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
    public enum AlignMode
    {
        Left,
        Right,
        Top,
        Bottom,
        Center,
        Middle,
        EvenlyHorizontal,
        EvenlyVertical,
        SameWidth,
        SameHeight
    }

    //===============================================================

    public class WRule_Arrange
    {
        //------------------------------------------------------------------------------------------------
        public static void AlignItemList(List<object> aSelectedList, AlignMode alignMode)
        {
            if (aSelectedList == null || aSelectedList.Count < 1) return;
            WRECT aFirstRect = new WRECT();  //--Bounding rect for the first Item
            WRECT aBoundRect = GetBoundRect(aSelectedList, ref aFirstRect);
            if (aBoundRect == null) return;
            switch (alignMode)
            {
                case AlignMode.Left:
                    AlignLeft(aSelectedList, aFirstRect.left);
                    break;
                case AlignMode.Right:
                    AlignRight(aSelectedList, aFirstRect.right);
                    break;
                case AlignMode.Top:
                    AlignTop(aSelectedList, aFirstRect.top);
                    break;
                case AlignMode.Bottom:
                    AlignBottom(aSelectedList, aFirstRect.bottom);
                    break;
                case AlignMode.Center:    //--Horizontal center
                    AlignHCenter(aSelectedList, (aBoundRect.left + aBoundRect.right) / 2);
                    break;
                case AlignMode.Middle:    //--Vertical Center
                    AlignVCenter(aSelectedList, (aBoundRect.top + aBoundRect.bottom) / 2);
                    break;
                case AlignMode.EvenlyHorizontal:
                    AlignEvenlyHorizontal(aSelectedList, aBoundRect);
                    break;
                case AlignMode.EvenlyVertical:
                    AlignEvenlyVertical(aSelectedList, aBoundRect);
                    break;
                case AlignMode.SameWidth:
                    AlignSameWidth(aSelectedList, aFirstRect.right - aFirstRect.left);
                    break;
                case AlignMode.SameHeight:
                    AlignSameHeight(aSelectedList, aFirstRect.bottom - aFirstRect.top);
                    break;
                default:
                    break;
            }
            MainWindow.tp.UpdateDisplayX(false);
        }

        //-----------------------------------------------------------------------------
        public static void AlignSameWidth(List<object> aSelectedList, double aWidth)
        {
            if (aSelectedList == null || aSelectedList.Count <= 1) return;
            foreach (object o in aSelectedList)
            {
                if (o.GetType() == typeof(XRule))
                {
                    XRule a = (XRule)o;
                    a.Width = aWidth;
                }
                else if (o.GetType() == typeof(XFact))
                {
                    XFact a = (XFact)o;
                    a.Width = aWidth;
                }
                else if (o.GetType() == typeof(XConclusion))
                {
                    XConclusion a = (XConclusion)o;
                    a.Width = aWidth;
                }
            }
        }

        //-----------------------------------------------------------------------------
        public static void AlignSameHeight(List<object> aSelectedList, double aHeight)
        {
            if (aSelectedList == null || aSelectedList.Count <= 1) return;
            foreach (object o in aSelectedList)
            {
                if(o.GetType() == typeof(XRule))
                {
                    XRule a = (XRule)o;
                    a.Height = aHeight;
                }
                else if (o.GetType() == typeof(XFact))
                {
                    XFact a = (XFact)o;
                    a.Height = aHeight;
                }
                else if (o.GetType() == typeof(XConclusion))
                {
                    XConclusion a = (XConclusion)o;
                    a.Height = aHeight;
                }
            }
        }

        //-----------------------------------------------------------------------------
        public static void AlignEvenlyHorizontal(List<object> aSelectedList, WRECT aRect)
        {
            if (aSelectedList == null || aSelectedList.Count <= 1) return;
            double totalWidth = 0;
            double bx = 0, by = 0, ex = 0, ey = 0;
            foreach (object o in aSelectedList)
            {
                if (!GetItemCoord(o, ref bx, ref by, ref ex, ref ey)) continue;
                double width = ex - bx;
                totalWidth += width;
            }
            double dx = (aRect.right - aRect.left - totalWidth) / (aSelectedList.Count - 1);

            bx = aRect.left;
            foreach (object o in aSelectedList)
            {
                if (o.GetType() == typeof(XRule))
                {
                    XRule a = (XRule)o;
                    a.X = bx;
                    bx += a.Width + dx;
                }
                else if (o.GetType() == typeof(XRule))
                {
                    XRule a = (XRule)o;
                    a.X = bx;
                    bx += a.Width + dx;
                }
                else if (o.GetType() == typeof(XConclusion))
                {
                    XConclusion a = (XConclusion)o;
                    a.X = bx;
                    bx += a.Width + dx;
                }
            }
        }

        //-----------------------------------------------------------------------------
        public static void AlignEvenlyVertical(List<object> aSelectedList, WRECT aRect)
        {
            if (aSelectedList == null || aSelectedList.Count <= 1) return;
            double totalHeight = 0;
            double bx = 0, by = 0, ex = 0, ey = 0;
            foreach (object o in aSelectedList)
            {
                if(!GetItemCoord(o,ref bx,ref by,ref ex,ref ey)) continue;
                double height = ey - by;
                totalHeight += height;
            }
            double dy = (aRect.bottom - aRect.top - totalHeight) / (aSelectedList.Count - 1);

            by = aRect.top;
            foreach (object o in aSelectedList)
            {
                if (o.GetType() == typeof(XRule))
                {
                    XRule a = (XRule)o;
                    a.Y = by;
                    by += a.Height + dy;
                }
                else if (o.GetType() == typeof(XFact))
                {
                    XFact a = (XFact)o;
                    a.Y = by;
                    by += a.Height + dy;
                }
                else if (o.GetType() == typeof(XConclusion))
                {
                    XConclusion a = (XConclusion)o;
                    a.Y = by;
                    by += a.Height + dy;
                }
            }
        }

        //-----------------------------------------------------------------------------
        public static void AlignLeft(List<object> aSelectedList, double x)
        {
            if (aSelectedList == null || aSelectedList.Count < 1) return;
            foreach (object o in aSelectedList)
            {
                if (o.GetType() == typeof(XRule))
                {
                    XRule a = o as XRule;
                    a.X = x;
                }
                else if (o.GetType() == typeof(XFact))
                {
                    XFact a = o as XFact;
                    a.X = x;
                }
                else if (o.GetType() == typeof(XConclusion))
                {
                    XConclusion a = o as XConclusion;
                    a.X = x;
                }
            }
        }

        //-----------------------------------------------------------------------------
        public static void AlignRight(List<object> aSelectedList, double x)
        {
            if (aSelectedList == null || aSelectedList.Count < 1) return;
            foreach (object o in aSelectedList)
            {
                if (o.GetType() == typeof(XRule))
                {
                    XRule a = o as XRule;
                    a.X = x - a.Width;
                }
                else if (o.GetType() == typeof(XFact))
                {
                    XFact a = o as XFact;
                    a.X = x - a.Width;
                }
                else if (o.GetType() == typeof(XConclusion))
                {
                    XConclusion a = o as XConclusion;
                    a.X = x - a.Width;
                }
            }
        }

        //-----------------------------------------------------------------------------
        public static void AlignTop(List<object> aSelectedList, double y)
        {
            if (aSelectedList == null || aSelectedList.Count < 1) return;
            foreach (object o in aSelectedList)
            {
                if (o.GetType() == typeof(XRule))
                {
                    XRule a = o as XRule;
                    a.Y = y;
                }
                else if (o.GetType() == typeof(XFact))
                {
                    XFact a = o as XFact;
                    a.Y = y;
                }
                else if (o.GetType() == typeof(XConclusion))
                {
                    XConclusion a = o as XConclusion;
                    a.Y = y;
                }
            }
        }

        //-----------------------------------------------------------------------------
        public static void AlignBottom(List<object> aSelectedList, double y)
        {
            if (aSelectedList == null || aSelectedList.Count < 1) return;
            foreach (object o in aSelectedList)
            {
                if (o.GetType() == typeof(XRule))
                {
                    XRule a = o as XRule;
                    a.Y = y - a.Height;
                }
                else if (o.GetType() == typeof(XFact))
                {
                    XFact a = o as XFact;
                    a.Y = y - a.Height;
                }
                else if (o.GetType() == typeof(XConclusion))
                {
                    XConclusion a = o as XConclusion;
                    a.Y = y - a.Height;
                }
            }
        }

        //-----------------------------------------------------------------------------
        public static void AlignHCenter(List<object> aSelectedList, double CX)
        {
            if (aSelectedList == null || aSelectedList.Count < 1) return;
            foreach (object o in aSelectedList)
            {
                if (o.GetType() == typeof(XRule))
                {
                    XRule a = o as XRule;
                    a.X = CX - a.Width / 2;
                }
                else if (o.GetType() == typeof(XFact))
                {
                    XFact a = o as XFact;
                    a.X = CX - a.Width / 2;
                }
                else if (o.GetType() == typeof(XConclusion))
                {
                    XConclusion a = o as XConclusion;
                    a.X = CX - a.Width / 2;
                }
            }
        }

        //-----------------------------------------------------------------------------
        public static void AlignVCenter(List<object> aSelectedList, double CY)
        {
            if (aSelectedList == null || aSelectedList.Count < 1) return;
            foreach (object o in aSelectedList)
            {
                if (o.GetType() == typeof(XRule))
                {
                    XRule a = o as XRule;
                    a.Y = CY - a.Height / 2;
                }
                else if (o.GetType() == typeof(XFact))
                {
                    XFact a = o as XFact;
                    a.Y = CY - a.Height / 2;
                }
                else if (o.GetType() == typeof(XConclusion))
                {
                    XConclusion a = o as XConclusion;
                    a.Y = CY - a.Height / 2;
                }
            }
        }

        //-----------------------------------------------------------------------------
        public static WRECT GetBoundRect(List<object> aSelectedList, ref WRECT aFirstRect)
        {
            if (aSelectedList == null || aSelectedList.Count < 1) return null;
            WRECT aRect = new WRECT();
            aRect.left = Double.MaxValue;
            aRect.top = Double.MaxValue;
            aRect.right = 0;
            aRect.bottom = 0;
            double bx = 0, by = 0, ex = 0, ey = 0;
            int i = 0;
            foreach (object o in aSelectedList)
            {
                if (!GetItemCoord(o, ref bx, ref by, ref ex, ref ey)) continue;
                if (i == 0)
                {
                    aFirstRect.left = bx;
                    aFirstRect.top = by;
                    aFirstRect.right = ex;
                    aFirstRect.bottom = ey;
                }
                WCoord.oChkMinMax2(bx, by, ex, ey, ref aRect.left, ref aRect.top, ref aRect.right, ref aRect.bottom);
                i++;
            }
            return aRect;
        }

        //---------------------------------------------------
        public static bool GetItemCoord(object o, ref double bx, ref double by, ref double ex, ref double ey)
        {
            if (o == null) return false;
            if (o.GetType() == typeof(XRule))
            {
                XRule a = (XRule)o;
                bx = a.X; by = a.Y;
                ex = bx + a.Width; ey = by + a.Height;
                return true;
            }
            else if (o.GetType() == typeof(XFact))
            {
                XFact a = (XFact)o;
                bx = a.X; by = a.Y;
                ex = bx + a.Width; ey = by + a.Height;
                return true;
            }
            else if (o.GetType() == typeof(XConclusion))
            {
                XConclusion a = (XConclusion)o;
                bx = a.X; by = a.Y;
                ex = bx + a.Width; ey = by + a.Height;
                return true;
            }
            return false;
        }
    }
}
