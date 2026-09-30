
//---------------------------------------------
//-- Expert System Rule 
//---------------------------------------------
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace OExpert
{
    public class XOption
    {
        public string option { get; set; }
        public bool visit { get; set; }

        //-----------------------------------------------------
        public XOption() { }

        //-----------------------------------------------------
        public XOption(string option, bool visit)
        {
            this.option = option;
            this.visit = visit;
        }

        //-----------------------------------------------------
        public static int Find(ObservableCollection<XOption> aList, string _option)
        {
            if (aList == null) return -1;
            for (int i = 0; i < aList.Count; i++)
            {
                if (aList[i].option == _option) return i;
            }
            return -1;
        }

        //-----------------------------------------------------
        public static ObservableCollection<XOption> FromOption(ObservableCollection<string> options, int iSelected)
        {
            if (options == null) return null;
            ObservableCollection<XOption> aList = new ObservableCollection<XOption>();
            for (int i = 0; i < options.Count; i++)
            {
                XOption o = new XOption();
                o.option = options[i];
                if (i == iSelected) o.visit = true;
                else o.visit = false;
                aList.Add(o);
            }
            return aList;
        }
    }

    //============================================================
    //--InputFact: for Facts to be input by users
    public class XInputFact
    {
        public int idx { get; set; }
        public string fact { get; set; }
        public string value { get; set; }
        public int ivalue { get; set; }  //--Selected index of the options
        public ObservableCollection<XOption> options { get; set; }
        public List<XRule> ruleList { get; set; }
        public int Level { get; set; }

        //---------------------------------------------------------------------------
        public XInputFact()
        {
        }

        //---------------------------------------------------------------------------
        public XInputFact(int _idx, InputFact _input)
        {
            idx = _input.idx;
            fact = _input.fact;
            value = _input.value;
            ivalue = _input.ivalue;  //--Selected index of the options
            options = XOption.FromOption(_input.options, ivalue);
            ruleList = _input.ruleList;
            Level = _input.Level;
        }

        //---------------------------------------------------------------------------
        public XInputFact(int _idx, string _fact, string _value, List<string> _options)
        {
            idx = _idx;
            fact = _fact;
            value = _value;
            ivalue = -1;
            if (_options == null) options = null;
            else
            {
                options = new ObservableCollection<XOption>();
                foreach (string o in _options)
                {
                    options.Add(new XOption(o, false));
                }
                ivalue = XOption.Find(options, value);
                if (ivalue >= 0) options[ivalue].visit = true;
            }
        }

        //---------------------------------------------------------------------------
        public XInputFact(int _idx, string _fact, string _value, ObservableCollection<XOption> _options)
        {
            idx = _idx;
            fact = _fact;
            value = _value;
            ivalue = -1;
            if (_options == null) options = null;
            else
            {
                options = new ObservableCollection<XOption>();
                foreach (XOption o in _options)
                {
                    options.Add(new XOption(o.option, o.visit));
                }
                ivalue = XOption.Find(options, value);
                if (ivalue >= 0) options[ivalue].visit = true;
            }
        }

        //---------------------------------------------------------------------------
        public XInputFact(int _idx, string _fact, string _value, List<string> _options, XRule _rule)
        {
            idx = _idx;
            fact = _fact;
            value = _value;
            ivalue = -1;
            if (_options == null) options = null;
            else
            {
                options = new ObservableCollection<XOption>();
                foreach (string o in _options)
                {
                    options.Add(new XOption(o, false));
                }
                ivalue = XOption.Find(options, value);
                if (ivalue >= 0) options[ivalue].visit = true;
            }
            if (_rule != null)
            {
                if (ruleList == null) ruleList = new List<XRule>();
                ruleList.Add(_rule);
            }
        }

        //---------------------------------------------------------------------------
        public static XInputFact Find(ObservableCollection<XInputFact> aList, string _fact)
        {
            if (aList == null) return null;
            foreach (XInputFact a in aList)
            {
                if (a.fact == _fact) return a;
            }
            return null;
        }

        //----------------------------------------------------------------------
        public static int SetNextWorkingOption(XInputFact a)
        {
            if (a == null || a.options == null) return -1;
            for (int i = 0; i < a.options.Count; i++)
            {
                if (a.options[i].visit == false)
                {
                    a.value = a.options[i].option;
                    a.ivalue = i;
                    return i;
                }
            }
            return -1;
        }

        //----------------------------------------------------------------------
        public static bool SetOptionVisit(XInputFact a, int iValue)
        {
            if (a == null || a.options == null) return false;
            if (iValue >= 0 && iValue < a.options.Count) { a.options[iValue].visit = true; return true; }
            return false;
        }


        //---------------------------------------------------------------------------
        //--Exclude the existing one
        public static ObservableCollection<XInputFact> FromXRulesX(List<XRule> aRules, ObservableCollection<XConclusion> aConList, ObservableCollection<XInputFact> aExistFacts)
        {
            if (aRules == null) return null;
            ObservableCollection<XInputFact> rList = new ObservableCollection<XInputFact>();
            int idx = 0;
            foreach (XRule aRule in aRules)
            {
                foreach (XCondition a in aRule.Conditions)
                {
                    if (XConclusion.Find(aConList, a.Condition) < 0 &&
                        XInputFact.Find(rList, a.Condition) == null)
                    {
                        if (aExistFacts == null || XInputFact.Find(aExistFacts, a.Condition) == null)
                        {
                            rList.Add(new XInputFact(idx, a.Condition, null, a.Options, aRule));
                            idx++;
                        }
                    }
                }
            }
            return rList;
        }

        //--------------------------------------------------------------
        public string toStr(string sInd)
        {
            string rstr = "Level: " + Level;
            rstr += "\r\n" + sInd + "idx: " + idx;
            rstr += "\r\n" + sInd + "fact: " + fact;
            if (options != null)
            {
                foreach (XOption o in options)
                {
                    rstr += String.Format("\r\n" + sInd + "  option: {0} ({1})", o.option, o.visit);
                }
            }
            rstr += "\r\n" + sInd + "value: " + value;
            rstr += "\r\n" + sInd + "ivalue: " + ivalue;
            return rstr;
        }

        //---------------------------------------------------------------------------
        public static bool SetXFactValues(ObservableCollection<XFact> xList, ObservableCollection<XInputFact> aList)
        {
            if (aList == null || xList == null) return false;
            foreach (XInputFact a in aList)
            {
                int ix = XFact.Find(xList, a.fact);
                if (ix >= 0) xList[ix].Value = a.value;
            }
            return true;
        }


        //---------------------------------------------------------------------------
        public static XInputFact Copy(XInputFact a)
        {
            if (a == null) return null;
            XInputFact x = new XInputFact();
            x.idx = a.idx;
            x.fact = a.fact;
            x.value = a.value;
            x.ivalue = a.ivalue;
            if (a.options == null) x.options = null;
            else
            {
                x.options = new ObservableCollection<XOption>();
                foreach (XOption o in a.options)
                {
                    x.options.Add(new XOption(o.option, o.visit));
                }
            }
            return x;
        }

        //---------------------------------------------------------------------------
        public static ObservableCollection<XInputFact> CopyList(ObservableCollection<XInputFact> aList)
        {
            if (aList == null) return null;
            ObservableCollection<XInputFact> rList = new ObservableCollection<XInputFact>();
            foreach (XInputFact a in aList)
            {
                rList.Add(XInputFact.Copy(a));
            }
            return rList;
        }

    }
}
