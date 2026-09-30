using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OExpert.UILibraryWPF
{
    public class WRECT
    {

        public double left;
        public double top;
        public double right;
        public double bottom;

        public WRECT()
        {
        }

        public WRECT(double bx, double by, double ex, double ey)
        {
            left = bx;
            top = by;
            right = ex;
            bottom = ey;
        }

        public WRECT(WRECT r)
        {
            left = r.left;
            top = r.top;
            right = r.right;
            bottom = r.bottom;
        }

        public void Update(double bx, double by, double ex, double ey)
        {
            left = bx;
            top = by;
            right = ex;
            bottom = ey;
        }

    }

}
