using OExpert.UILibraryWPF;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace OExpert
{
    public class WSettings
    {
        public double zoomFactorX = 1;
        public double zoomFactorY = 1;
        public bool IsFit = false;

        public bool fMultiSelect = false;
        public WRECT SelRect = new WRECT(0, 0, 0, 0);

        //       public List<object> aSelectedObjectList = new List<object>();
        //       public List<object> aPreSelectedObjectList = null;

        public OperMode iOperMode = OperMode.None;
        public int iSelPos = -1;

        public DrawingVisual aWorkVisual;

        public bool iLeftButtonDown = false;
        public bool iRightButtonDown = false;
        public Point prevCursorPos = new Point(0, 0);
        public Point cursorPos = new Point(0, 0);
        public List<Point> aPointList = null;
        public List<Point> aRubPointList = null;
    }
}
