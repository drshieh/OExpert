using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reflection.Emit;
using System.Windows;
using System.Windows.Controls;
//using System.Windows.Forms;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using System.Windows.Shapes;

namespace OExpert
{
    //========================================================================
    public class XFact
    {
        public string fact { get; set; }
        public ObservableCollection<XRule> ruleList { get; set; }
        public double X { get; set; }
        public double Y { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }

        public Color FColor = Colors.LightGreen;
        public string Value { get; set; }
        public DrawingVisual aDrawVisualX { get; set; }
        public int Level { get; set; }  //--Process level, (dynamic)

        //-------------------------------------------------------------------------
        public XFact()
        {

        }

        //-------------------------------------------------------------------------
        public XFact(string _fact, string _value)
        {
            fact = _fact;
            Value = _value;
        }

        //-------------------------------------------------------------------------
        public XFact(string level, string _fact, string _value)
        {
            Level = WHelper.GetInt32(level);
            fact = _fact;
            Value = _value;
        }

        //-------------------------------------------------------------------------
        public static bool AddXFact(ObservableCollection<XFact> aList, XCondition c, XRule r)
        {
            if (aList == null || r == null) return false;
            foreach (XFact a in aList)
            {
                if (a.fact == c.Condition)
                {
                    if (a.ruleList == null) a.ruleList = new ObservableCollection<XRule>();
                    a.ruleList.Add(r);
                    return true;
                }
            }
            XFact x = new XFact();
            x.fact = c.Condition;
            x.Value = "";
            x.ruleList = new ObservableCollection<XRule>();
            x.ruleList.Add(r);
            aList.Add(x);
            return true;
        }

        //-------------------------------------------------------------------------
        public static int Find(ObservableCollection<XFact> aList, string _fact)
        {
            if (aList == null) return -1;
            for (int i = 0; i < aList.Count; i++)
            {
                if (aList[i].fact == _fact) return i;
            }
            return -1;
        }

        //-------------------------------------------------------------------------
        public string toStr()
        {
            string rstr = fact + "\r\n in # Rules: " + (ruleList == null ? 0 : ruleList.Count) + "---------------";
            if(ruleList != null)
            {
                foreach(XRule r in ruleList)
                {
                    rstr += "\r\nRule " + r.toStr2();
                }
            }
            return rstr;
        }

        //-------------------------------------------------
        public string GetFactValue()
        {
            return string.Format("{0} ({1})", fact, Value);
        }

        //-------------------------------------------------
        public static RFact ToRFact(XFact a)
        {
            if (a == null) return null;
            RFact r = new RFact();
            r.fact = a.fact;
            r.X = a.X;
            r.Y = a.Y;
            r.Width = a.Width;
            r.Height = a.Height;
            r.Value = a.Value;
            return r;
        }

        //-------------------------------------------------
        public static List<RFact> ToRFactList(ObservableCollection<XFact> aList)
        {
            if (aList == null) return null;
            List<RFact> rList = new List<RFact>();
            foreach (XFact a in aList)
            {
                rList.Add(XFact.ToRFact(a));
            }
            return rList;
        }

        //-------------------------------------------------
        public static XFact FromRFact(RFact a)
        {
            if (a == null) return null;
            XFact r = new XFact();
            r.fact = a.fact;
            r.X = a.X;
            r.Y = a.Y;
            r.Width = a.Width;
            r.Height = a.Height;
            r.Value = a.Value;
            return r;
        }

        //-------------------------------------------------
        public static ObservableCollection<XFact> FromRFactList(List<RFact> aList)
        {
            if (aList == null) return null;
            ObservableCollection<XFact> rList = new ObservableCollection<XFact>();
            foreach (RFact a in aList)
            {
                rList.Add(XFact.FromRFact(a));
            }
            return rList;
        }

        //-------------------------------------------------
        public static ObservableCollection<XFact> CopyList(ObservableCollection<XFact> aList)
        {
            if (aList == null) return null;
            ObservableCollection<XFact> rList = new ObservableCollection<XFact>();
            foreach (XFact a in aList)
            {
                rList.Add(XFact.Copy(a));
            }
            return rList;
        }

        //-------------------------------------------------
        public static XFact Copy(XFact a)
        {
            XFact r = new XFact();
            r.fact = a.fact;
            r.Value = a.Value;
            r.X = a.X;
            r.Y = a.Y;
            r.Width = a.Width;
            r.Height = a.Height;

            //??     r.ruleList = a.ruleList;
            //??     r.FColor = a.FColor;
            //??     r.aDrawVisualX = a.aDrawVisualX;
            r.ruleList = null;
            r.aDrawVisualX = null;
            return r;
        }

    }
}