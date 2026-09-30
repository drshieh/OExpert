//---------------------------------------------
//-- Expert System Rule 
//---------------------------------------------
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows.Input;
using System.Windows.Media;

namespace OExpert
{

    public class RCondition
    {
        public string Operator { get; set; }
        public string Condition { get; set; }
        public string WantValue { get; set; }
        public string Or { get; set; }
        public List<string> Options { get; set; }  //--Options to select from (comboxbox)

        public RCondition()
        {
        }

        public RCondition(string _or, string _operator, string _condition, string _value, List<string> _options)
        {
            Or = _or;
            Operator = _operator;
            Condition = _condition;
            WantValue = _value;
            Options = _options;
        }

        public static RCondition Find(List<RCondition> aList, string condition)
        {
            if (aList == null) return null;
            foreach (RCondition a in aList)
            {
                if (a.Condition == condition) return a;
            }
            return null;
        }

        public static List<RCondition> CopyList(List<RCondition> aList)
        {
            if (aList == null) return null;
            List<RCondition> rList = new List<RCondition>();
            foreach (RCondition a in aList)
            {
                rList.Add(RCondition.Copy(a));
            }
            return rList;
        }

        public static RCondition Copy(RCondition a)
        {
            RCondition r = new RCondition();
            r.Or = a.Or;
            r.Operator = a.Operator;
            r.Condition = a.Condition;
            r.WantValue = a.WantValue;
            r.Options = a.Options;
            return r;
        }

        public string toStr()
        {
            string rstr = "";
            rstr += "Operator: " + Operator;
            rstr += "\r\nCondition: " + Condition;
            rstr += "\r\nWantValue: " + WantValue;
            rstr += "\r\nOr: " + Or;
            if (Options != null)
            {
                rstr += "\r\nOptions: ";
                foreach (string s in Options)
                {
                    rstr += "\r\n  " + s;
                }
            }
            return rstr;
        }
    }
}
