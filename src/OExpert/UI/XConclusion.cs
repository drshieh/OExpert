//using System.Windows.Forms;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using System.Windows.Shapes;

namespace OExpert
{
    //========================================================================
    public class XConclusion
    {
        public string conclusion { get; set; }
        public ObservableCollection<XRule> ruleList { get; set; }
        public double X { get; set; }
        public double Y { get; set; }

        public double Width { get; set; }
        public double Height { get; set; }
        public Color FColor = Colors.LightBlue;
        public int Value { get; set; }  //--- -1:unknown, 0:false, 1:true
        public double CalcProbability { get; set; }  //--- calculated probability
        public DrawingVisual aDrawVisualX { get; set; }

        //-------------------------------------------------
        public static bool AddXConclusion(ObservableCollection<XConclusion> aList, XRule r)
        {
            foreach (XConclusion a in aList)
            {
                if (a.conclusion == r.Conclusion)
                {
                    if (a.ruleList == null) a.ruleList = new ObservableCollection<XRule>();
                    a.ruleList.Add(r);
                    return true;
                }
            }
            XConclusion x = new XConclusion();
            x.conclusion = r.Conclusion;
            x.ruleList = new ObservableCollection<XRule>();
            x.ruleList.Add(r);
            aList.Add(x);
            return true;
        }

        //-------------------------------------------------
        public static int Find(ObservableCollection<XConclusion> aList, string sConclusion)
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
            string rstr = conclusion + " (calcP: " + CalcProbability + ")\r\n in # Rules: " + (ruleList == null ? 0 : ruleList.Count);
            if (ruleList != null)
            {
                foreach (XRule r in ruleList)
                {
                    rstr += "\r\nRule " + r.toStr2();
                }
            }
            return rstr;
        }

        //-------------------------------------------------
        public string toStr2()
        {
            string sRtn = "\r\n";
            string rstr = "Conclusion: " + conclusion;
            rstr += sRtn + " CalcProbability: " + CalcProbability;
            rstr += sRtn + " Value: " + Value;
            rstr += sRtn + string.Format(" X:{0} Y:{1} Width:{2} Height:{3}", X, Y, Width, Height);
            if (ruleList != null)
            {
                rstr += "\r\n In Rules #: " + (ruleList == null ? 0 : ruleList.Count) + "--------------------";
                foreach (XRule a in ruleList)
                {
                    rstr += sRtn + string.Format("  ID:{0} Name:{1} Conclusion:{2}", a.ID, a.Name, a.Conclusion);
                }
            }
            return rstr;
        }

        //-------------------------------------------------
        public string GetConclusionValue()
        {
            return string.Format("{0} ({1})", conclusion, Value);
        }

        //-------------------------------------------------
        public static XConclusion FromRConclusion(RConclusion a)
        {
            if (a == null) return null;
            XConclusion r = new XConclusion();
            r.conclusion = a.conclusion;
            r.X = a.X;
            r.Y = a.Y;
            r.Width = a.Width;
            r.Height = a.Height;
            r.Value = a.Value;
            return r;
        }

        //-------------------------------------------------
        public static ObservableCollection<XConclusion> FromRConclusionList(List<RConclusion> aList)
        {
            if (aList == null) return null;
            ObservableCollection<XConclusion> rList = new ObservableCollection<XConclusion>();
            foreach (RConclusion a in aList)
            {
                rList.Add(XConclusion.FromRConclusion(a));
            }
            return rList;
        }

        //-------------------------------------------------
        public static RConclusion ToRConclusion(XConclusion a)
        {
            if (a == null) return null;
            RConclusion r = new RConclusion();
            r.conclusion = a.conclusion;
            r.X = a.X;
            r.Y = a.Y;
            r.Width = a.Width;
            r.Height = a.Height;
            r.Value = a.Value;
            return r;
        }

        //-------------------------------------------------
        public static List<RConclusion> ToRConclusionList(ObservableCollection<XConclusion> aList)
        {
            if (aList == null) return null;
            List<RConclusion> rList = new List<RConclusion>();
            foreach (XConclusion a in aList)
            {
                rList.Add(XConclusion.ToRConclusion(a));
            }
            return rList;
        }

        //-------------------------------------------------
        public static ObservableCollection<XConclusion> CopyList(ObservableCollection<XConclusion> aList)
        {
            if (aList == null) return null;
            ObservableCollection<XConclusion> rList = new ObservableCollection<XConclusion>();
            foreach (XConclusion a in aList)
            {
                rList.Add(XConclusion.Copy(a));
            }
            return rList;
        }

        //-------------------------------------------------
        public static XConclusion Copy(XConclusion a)
        {
            XConclusion r = new XConclusion();
            r.conclusion = a.conclusion;
            r.Value = a.Value;
            r.CalcProbability = a.CalcProbability;
            r.X = a.X;
            r.Y = a.Y;
            r.Width = a.Width;
            r.Height = a.Height;

            //??   r.ruleList = a.ruleList;
            //??   r.FColor = a.FColor;
            //??   r.aDrawVisualX = a.aDrawVisualX;
            r.ruleList = null;
            r.aDrawVisualX = null;
            return r;
        }

    }

}