
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
    //============================================================
    //--InputFact: for Facts to be input by users
    public class InputFact
    {
        public int idx { get; set; }
        public string fact { get; set; }
        public string value { get; set; }
        public int ivalue { get; set; }  //--Selected index of the options
        public ObservableCollection<string> options { get; set; }
        public List<XRule> ruleList { get; set; }
        public int Level { get; set; }

        //---------------------------------------------------------------------------
        public InputFact()
        {
        }

        //---------------------------------------------------------------------------
        public InputFact(int _idx, string _fact, string _value)
        {
            idx = _idx;
            fact = _fact;
            value = _value;
            options = new ObservableCollection<string>() { "true", "false" };
            ivalue = options.IndexOf(value);
            ruleList = null;
        }

        //---------------------------------------------------------------------------
        public InputFact(int _idx, string _fact, string _value, List<string> _options)
        {
            idx = _idx;
            fact = _fact;
            value = _value;
            ivalue = -1;
            if (_options == null) options = null;
            else
            {
                options = new ObservableCollection<string>();
                foreach (string o in _options)
                {
                    options.Add(o);
                }
                ivalue = options.IndexOf(value);
            }
        }

        //---------------------------------------------------------------------------
        public InputFact(int _idx, string _fact, string _value, List<string> _options, XRule _rule)
        {
            idx = _idx;
            fact = _fact;
            value = _value;
            ivalue = -1;
            if (_options == null) options = null;
            else
            {
                options = new ObservableCollection<string>();
                foreach (string o in _options)
                {
                    options.Add(o);
                }
                ivalue = options.IndexOf(value);
            }
            if (_rule != null)
            {
                if (ruleList == null) ruleList = new List<XRule>();
                ruleList.Add(_rule);
            }
        }

        //---------------------------------------------------------------------------
        public InputFact(int _idx, string _fact, string _value, List<string> _options, List<XRule> _ruleList)
        {
            idx = _idx;
            fact = _fact;
            value = _value;
            ivalue = -1;
            if (_options == null) options = null;
            else
            {
                options = new ObservableCollection<string>();
                foreach (string o in _options)
                {
                    options.Add(o);
                }
                ivalue = options.IndexOf(value);
            }
            if (_ruleList != null)
            {
                if (ruleList == null) ruleList = new List<XRule>();
                foreach (XRule r in ruleList)
                {
                    if (!ruleList.Contains(r)) ruleList.Add(r);
                }
            }
        }

        //---------------------------------------------------------------------------
        public static InputFact Create(ObservableCollection<InputFact> aList, int _idx, string _fact, string _value, List<string> _options, XRule _rule)
        {
            InputFact x = Find(aList, _fact);
            if (x == null) x = new InputFact(_idx, _fact, _value, _options);
            if (_rule != null)
            {
                if (x.ruleList == null) x.ruleList = new List<XRule>();
                if (!x.ruleList.Contains(_rule)) x.ruleList.Add(_rule);
            }
            return x;
        }

        //---------------------------------------------------------------------------
        public static InputFact Create(ObservableCollection<InputFact> aList, int _idx, string _fact, string _value, List<string> _options, List<XRule> _ruleList)
        {
            InputFact x = Find(aList, _fact);
            if (x == null) x = new InputFact(_idx, _fact, _value, _options);
            if (x.ruleList == null) x.ruleList = new List<XRule>();

            if (_ruleList != null)
            {
                if (x.ruleList == null) x.ruleList = new List<XRule>();
                foreach (XRule r in _ruleList)
                {
                    if (!x.ruleList.Contains(r)) x.ruleList.Add(r);
                }
            }
            return x;
        }

        //---------------------------------------------------------------------------
        public static InputFact Find(ObservableCollection<InputFact> aList, string _fact)
        {
            if (aList == null) return null;
            foreach (InputFact a in aList)
            {
                if (a.fact == _fact) return a;
            }
            return null;
        }

        //---------------------------------------------------------------------------
        public static ObservableCollection<InputFact> FromXFacts(ObservableCollection<XFact> aFactList, ObservableCollection<XRule> aRuleList)
        {
            if (aFactList == null) return null;
            List<XCondition> condList = GetAllConditionOptions(aRuleList);
            ObservableCollection<InputFact> rList = new ObservableCollection<InputFact>();
            int idx = 0;
            foreach (XFact a in aFactList)
            {
                XCondition c = XCondition.Find(condList, a.fact);
                if (c == null) rList.Add(InputFact.Create(rList, idx, a.fact, a.Value, null, c.useInRules));
                else rList.Add(InputFact.Create(rList, idx, a.fact, a.Value, c.Options, c.useInRules));
                idx++;
            }
            return rList;
        }

        //---------------------------------------------------------------------------
        public static InputFact FromXFact(XFact a, ObservableCollection<XRule> aRuleList)
        {
            int idx = 0;
            List<XCondition> condList = GetAllConditionOptions(aRuleList);
            XCondition c = XCondition.Find(condList, a.fact);
            if (c == null) return new InputFact(idx, a.fact, a.Value, null, c.useInRules);
            else return new InputFact(idx, a.fact, a.Value, c.Options, c.useInRules);
        }

        //---------------------------------------------------------------------------
        public static ObservableCollection<InputFact> FromXRuleOne(XRule aRule, ObservableCollection<XConclusion> aConList)
        {
            if (aRule == null || aRule.Conditions == null) return null;
            ObservableCollection<InputFact> rList = new ObservableCollection<InputFact>();
            int idx = 0;
            foreach (XCondition a in aRule.Conditions)
            {
                if (XConclusion.Find(aConList, a.Condition) < 0)
                {
                    rList.Add(new InputFact(idx, a.Condition, null, a.Options, a.useInRules));
                    idx++;
                }
            }
            return rList;
        }

        //---------------------------------------------------------------------------
        public static ObservableCollection<InputFact> FromXRules(List<XRule> aRules, ObservableCollection<XConclusion> aConList)
        {
            if (aRules == null) return null;
            ObservableCollection<InputFact> rList = new ObservableCollection<InputFact>();
            int idx = 0;
            foreach (XRule aRule in aRules)
            {
                foreach (XCondition a in aRule.Conditions)
                {
                    if (XConclusion.Find(aConList, a.Condition) < 0 &&
                        InputFact.Find(rList, a.Condition) == null)
                    {
                        rList.Add(new InputFact(idx, a.Condition, null, a.Options, aRule));
                        idx++;
                    }
                }
            }
            return rList;
        }

        //---------------------------------------------------------------------------
        //--Exclude the existing one
        public static ObservableCollection<InputFact> FromXRulesX(List<XRule> aRules, ObservableCollection<XConclusion> aConList, ObservableCollection<InputFact> aExistFacts)
        {
            if (aRules == null) return null;
            ObservableCollection<InputFact> rList = new ObservableCollection<InputFact>();
            int idx = 0;
            foreach (XRule aRule in aRules)
            {
                foreach (XCondition a in aRule.Conditions)
                {
                    if (XConclusion.Find(aConList, a.Condition) < 0 &&
                        InputFact.Find(rList, a.Condition) == null)
                    {
                        if (aExistFacts == null || InputFact.Find(aExistFacts, a.Condition) == null)
                        {
                            rList.Add(new InputFact(idx, a.Condition, null, a.Options, aRule));
                            idx++;
                        }
                    }
                }
            }
            return rList;
        }

        //---------------------------------------------------------------------------
        public static List<XCondition> GetAllConditionOptions(ObservableCollection<XRule> aRuleList)
        {
            if (aRuleList == null) return null;
            List<XCondition> rList = new List<XCondition>();
            foreach (XRule a in aRuleList)
            {
                if (a.Conditions == null) continue;
                foreach (XCondition c in a.Conditions)
                {
                    if (XCondition.Find(rList, c.Condition) == null) rList.Add(c);
                }
            }
            return rList;
        }

        //---------------------------------------------------------------------------
        public static bool SetXFactValues(ObservableCollection<XFact> xList, ObservableCollection<InputFact> aList)
        {
            if (aList == null || xList == null) return false;
            foreach (InputFact a in aList)
            {
                int ix = XFact.Find(xList, a.fact);
                if (ix >= 0) xList[ix].Value = a.value;
            }
            return true;
        }

        //---------------------------------------------------------------------------
        public static ObservableCollection<InputFact> CopyList(ObservableCollection<InputFact> aList)
        {
            if (aList == null) return null;
            ObservableCollection<InputFact> rList = new ObservableCollection<InputFact>();
            foreach (InputFact a in aList)
            {
                rList.Add(new InputFact(a.idx, a.fact, a.value, a.options.ToList()));
            }
            return rList;
        }

        public string toStr(string sInd)
        {
            string rstr = "Level: " + Level;
            rstr += "\r\n" + sInd + "idx: " + idx;
            rstr += "\r\n" + sInd + "fact: " + fact;
            if (options != null)
            {
                foreach (string o in options)
                {
                    rstr += "\r\n" + sInd + "  option: " + o;
                }
            }
            rstr += "\r\n" + sInd + "value: " + value;
            rstr += "\r\n" + sInd + "ivalue: " + ivalue;
            return rstr;
        }
    }
}
