//using System.Windows.Forms;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using System.Windows.Shapes;

namespace OExpert
{
    //========================================================================
    public class RConclusion
    {
        public string conclusion { get; set; }
        public double X { get; set; }
        public double Y { get; set; }

        public double Width { get; set; }
        public double Height { get; set; }
        public int Value { get; set; }  //--- -1:unknown, 0:false, 1:true

        //-------------------------------------------------
        public static bool AddConclusion(List<RConclusion> aList, Rule r)
        {
            foreach (RConclusion a in aList)
            {
                if (a.conclusion == r.Conclusion) return false;
            }
            RConclusion x = new RConclusion();
            x.conclusion = r.Conclusion;
            aList.Add(x);
            return true;
        }

        //-------------------------------------------------
        public static int Find(List<RConclusion> aList, string sConclusion)
        {
            if (aList == null) return -1;
            for(int i=0;i<aList.Count;i++)
            {
                if (aList[i].conclusion == sConclusion) return i;
            }
            return -1;
        }

        //-------------------------------------------------
        public string toStr()
        {
            return conclusion + "\r\n" + Value;
        }

        //-------------------------------------------------
        public static List<RConclusion> CopyList(List<RConclusion> aList)
        {
            if (aList == null) return null;
            List<RConclusion> rList = new List<RConclusion>();
            foreach (RConclusion a in aList)
            {
                rList.Add(a);
            }
            return aList;
        }

        //-------------------------------------------------
        public static RConclusion Copy(RConclusion a)
        {
            RConclusion r = new RConclusion();
            r.conclusion = a.conclusion;
            r.Value = a.Value;
            r.X = a.X;
            r.Y = a.Y;
            r.Width = a.Width;
            r.Height = a.Height;

            return r;
        }

    }

}