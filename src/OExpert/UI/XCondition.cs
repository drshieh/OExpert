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
    public class XCondition
    {
        public string Operator { get; set; }
        public string Condition { get; set; }
        public string WantValue { get; set; }
        public string Or { get; set; }
        public string InputValue { get; set; }
        public int EvaluatedValue { get; set; }
        public List<string> Options { get; set; }  //--Options to select from (comboxbox)

        public bool isConclusionType { get; set; }

        public List<XRule> useInRules { get; set; }

        //-------------------------------------------------------
        public XCondition()
        {
            Operator = null;
            Condition = null;
            WantValue = null;
            Or = "";
            Options = null;
            isConclusionType = false;
            EvaluatedValue = -1;
            InputValue = "";
            useInRules = null;
        }


        //-------------------------------------------------
        public static RCondition ToCondition(XCondition a)
        {
            RCondition r = new RCondition();
            r.Operator = a.Operator;
            r.Condition = a.Condition;
            r.WantValue = a.WantValue;
            r.Or = a.Or;
            r.Options = a.Options;
            return r;
        }

        //-------------------------------------------------
        public static List<RCondition> ToConditionList(List<XCondition> aList)
        {
            if (aList == null) return null;
            List<RCondition> rList = new List<RCondition>();
            foreach (XCondition a in aList)
            {
                rList.Add(XCondition.ToCondition(a));
            }
            return rList;
        }

        //-------------------------------------------------------
        public static List<XCondition> CopyList(List<XCondition> aList)
        {
            if (aList == null) return null;
            List<XCondition> rList = new List<XCondition>();
            foreach (XCondition a in aList)
            {
                rList.Add(XCondition.Copy(a));
            }
            return rList;
        }

        //-------------------------------------------------------
        public static XCondition Copy(XCondition a)
        {
            XCondition r = new XCondition();
            r.Operator = a.Operator;
            r.Condition = a.Condition;
            r.WantValue = a.WantValue;
            r.Or = a.Or;
            r.Options = a.Options;
            r.EvaluatedValue = a.EvaluatedValue;
            r.InputValue = a.InputValue;
            r.useInRules = a.useInRules;
            return r;
        }

        //-------------------------------------------------
        public static XCondition FromCondition(RCondition a)
        {
            XCondition r = new XCondition();
            r.Operator = a.Operator;
            r.Condition = a.Condition;
            r.WantValue = a.WantValue;
            r.Or = a.Or;
            r.Options = a.Options;
            r.EvaluatedValue = -1;
            r.InputValue = "";
            return r;
        }

        //-------------------------------------------------
        public static List<XCondition> FromConditionList(List<RCondition> aList)
        {
            if (aList == null) return null;
            List<XCondition> rList = new List<XCondition>();
            foreach (RCondition a in aList)
            {
                rList.Add(XCondition.FromCondition(a));
            }
            return rList;
        }

        //-------------------------------------------------------
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
            rstr += "\r\nsValue: " + InputValue;
            rstr += "\r\nValue: " + EvaluatedValue;
            return rstr;
        }

        //-------------------------------------------------------
        public static XCondition Find(List<XCondition> aList, string condition)
        {
            if (aList == null) return null;
            foreach (XCondition a in aList)
            {
                if (a.Condition == condition) return a;
            }
            return null;
        }

    }
}
