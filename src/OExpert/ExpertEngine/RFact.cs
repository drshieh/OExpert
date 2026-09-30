using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Controls;
//using System.Windows.Forms;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using System.Windows.Shapes;

namespace OExpert
{
    //========================================================================
    public class RFact
    {
        public string fact { get; set; }
        public double X { get; set; }
        public double Y { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }
        public string Value { get; set; }


        public static bool AddFact(List<RFact> aList, RCondition c, Rule r)
        {
            if (aList == null || r == null) return false;
            foreach (RFact a in aList)
            {
                if (a.fact == c.Condition) return true;
            }
            RFact x = new RFact();
            x.fact = c.Condition;
            x.Value = "";
            aList.Add(x);
            return true;
        }

        public static int Find(List<RFact> aList, string _fact)
        {
            if (aList == null) return -1;
            for (int i = 0; i < aList.Count; i++)
            {
                if (aList[i].fact == _fact) return i;
            }
            return -1;
        }

        public string toStr()
        {
            return fact + "\r\n" + Value;
        }

        //-------------------------------------------------
        public static List<RFact> CopyList(List<RFact> aList)
        {
            if (aList == null) return null;
            List<RFact> rList = new List<RFact>();
            foreach (RFact a in aList)
            {
                rList.Add(a);
            }
            return aList;
        }

        //-------------------------------------------------
        public static RFact Copy(RFact a)
        {
            RFact r = new RFact();
            r.fact = a.fact;
            r.Value = a.Value;
            r.X = a.X;
            r.Y = a.Y;
            r.Width = a.Width;
            r.Height = a.Height;
            return r;
        }

    }
}