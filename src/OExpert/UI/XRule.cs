//using System.Windows.Forms;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using System.Windows.Shapes;

namespace OExpert
{
    //============================================================
    public class XRule
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Message { get; set; }
        public List<XCondition> Conditions { get; set; }
        public string Conclusion { get; set; }
        public double Probability { get; set; }
        public double CalcProbability { get; set; }  //--Rule conclusion Probability during inference

        public int Value { get; set; }  //--RULE_TRUE, RULE_FALSE, RULE_UNKNOWN
        public double X { get; set; }
        public double Y { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }

        public int iLevel { get; set; }
        public List<XRule> aFromRules;  //--the conditions that has the conclusion of other rules
        public List<XRule> aChildRules;  //--the Conclusion is used in other rules
        public Color FColor { get; set; }
        public DrawingVisual aDrawVisualX { get; set; }

        //-------------------------------------------------
        public XRule()
        {
            ID = 0;
            Name = "";
            Message = "";
            Conditions = null;
            Conclusion = null;
            Probability = 0;
            CalcProbability = 0;
            X = 0;
            Y = 0;
            Width = 0;
            Height = 0;
            Value = -1;
            FColor = Colors.Yellow;
            iLevel = -1;
        }

        //-------------------------------------------------
        public static Rule ToRule(XRule a)
        {
            Rule r = new Rule();
            r.ID = a.ID;
            r.Name = a.Name;
            r.Message = a.Message;
            r.Conditions = XCondition.ToConditionList(a.Conditions);
            r.Conclusion = a.Conclusion;
            r.Probability = a.Probability;
            r.Value = a.Value;
            r.X = a.X;
            r.Y = a.Y;
            r.Width = a.Width;
            r.Height = a.Height;
            return r;
        }

        //-------------------------------------------------
        public static List<Rule> ToRuleList(ObservableCollection<XRule> aList)
        {
            if (aList == null) return null;
            List<Rule> rList = new List<Rule>();
            foreach (XRule a in aList)
            {
                rList.Add(XRule.ToRule(a));
            }
            return rList;
        }

        //-------------------------------------------------
        public static ObservableCollection<XRule> CopyList(ObservableCollection<XRule> aList)
        {
            if (aList == null) return null;
            ObservableCollection<XRule> rList = new ObservableCollection<XRule>();
            foreach (XRule a in aList)
            {
                rList.Add(XRule.Copy(a));
            }
            return rList;
        }

        //-------------------------------------------------
        public static XRule Copy(XRule a)
        {
            XRule r = new XRule();
            r.ID = a.ID;
            r.Name = a.Name;
            r.Message = a.Message;
            r.Conditions = XCondition.CopyList(a.Conditions);
            r.Conclusion = a.Conclusion;
            r.Probability = a.Probability;
            r.CalcProbability = a.CalcProbability;

            r.Value = a.Value;
            r.X = a.X;
            r.Y = a.Y;
            r.Width = a.Width;
            r.Height = a.Height;

            //?? r.aFromRules = a.aFromRules;
            //?? r.aChildRules = a.aChildRules;
            //?? r.FColor = a.FColor;
            //?? r.aDrawVisualX = a.aDrawVisualX;
            r.aFromRules = null;
            r.aChildRules = null;
            r.aDrawVisualX = null;
            return r;
        }

        //-------------------------------------------------
        public static void CopyContent(XRule a, XRule r)
        {
            r.ID = a.ID;
            r.Name = a.Name;
            r.Message = a.Message;
            r.Conditions = XCondition.CopyList(a.Conditions);
            r.Conclusion = a.Conclusion;
            r.Probability = a.Probability;
            r.CalcProbability = a.CalcProbability;

            r.Value = a.Value;
            r.X = a.X;
            r.Y = a.Y;
            r.Width = a.Width;
            r.Height = a.Height;

            r.aFromRules = a.aFromRules;
            r.aChildRules = a.aChildRules;
            r.FColor = a.FColor;
            r.aDrawVisualX = a.aDrawVisualX;
            r.iLevel = a.iLevel;
        }

        //-------------------------------------------------
        public static XRule FromRule(Rule a)
        {
            XRule r = new XRule();
            r.ID = a.ID;
            r.Name = a.Name;
            r.Message = a.Message;
            r.Conditions = XCondition.FromConditionList(a.Conditions);
            r.Conclusion = a.Conclusion;
            r.Probability = a.Probability;
            r.CalcProbability = a.Probability;
            r.Value = a.Value;
            r.X = a.X;
            r.Y = a.Y;
            r.Width = a.Width;
            r.Height = a.Height;
            return r;
        }

        //-------------------------------------------------
        public static ObservableCollection<XRule> FromRuleList(List<Rule> aList)
        {
            if (aList == null) return null;
            ObservableCollection<XRule> rList = new ObservableCollection<XRule>();
            foreach (Rule a in aList)
            {
                rList.Add(XRule.FromRule(a));
            }
            return rList;
        }

        //------------------------------------------------------------------------------
        public static int Find(ObservableCollection<XRule> aList, int id)
        {
            if (aList == null || id < 1) return -1;
            for (int i = 0; i < aList.Count; i++)
            {
                if (aList[i].ID == id) return i;
            }
            return -1;
        }

        //------------------------------------------------------------------------------
        public static List<XRule> GetTopRules(ObservableCollection<XRule> aList)
        {
            if (aList == null) return null;
            List<XRule> rList = new List<XRule>();
            foreach (XRule a in aList)
            {
                if (a.aFromRules == null || a.aFromRules.Count <= 0) rList.Add(a);
            }
            return rList;
        }

        //------------------------------------------------------------------------------
        public static List<XRule> FindByConclusion(ObservableCollection<XRule> aList, string _conclusion)
        {
            if (aList == null || String.IsNullOrEmpty(_conclusion)) return null;
            List<XRule> childList = new List<XRule>();
            foreach (XRule a in aList)
            {
                if (a.Conclusion == _conclusion)
                {
                    if (!childList.Contains(a)) childList.Add(a);
                }
            }
            return childList;
        }

        //------------------------------------------------------------------------------
        public static ObservableCollection<XFact> GetXFacts(ObservableCollection<XRule> aList, ObservableCollection<XConclusion> aConclusionList)
        {
            if (aList == null) return null;
            ObservableCollection<XFact> aXFactList = new ObservableCollection<XFact>();
            foreach (XRule a in aList)
            {
                if (a.Conditions == null || a.Conditions.Count <= 0) continue;
                foreach (XCondition c in a.Conditions)
                {
                    if (XConclusion.Find(aConclusionList, c.Condition) < 0)  //--exclude the Conclusions
                    {
                        XFact.AddXFact(aXFactList, c, a);
                    }
                }
            }
            return aXFactList;
        }

        //------------------------------------------------------------------------------
        public static ObservableCollection<XConclusion> GetXConclusions(ObservableCollection<XRule> aList)
        {
            if (aList == null) return null;
            ObservableCollection<XConclusion> conclusionList = new ObservableCollection<XConclusion>();
            foreach (XRule a in aList)
            {
                XConclusion.AddXConclusion(conclusionList, a);
            }
            return conclusionList;
        }

        //----------------------------------------------------------------
        public static void ResolveRuleLink(ObservableCollection<XRule> aRuleList)
        {
            if (aRuleList == null) return;
            foreach (XRule a in aRuleList)
            {
                a.aFromRules = new List<XRule>();
                foreach (XCondition c in a.Conditions)
                {
                    List<XRule> fromRules = XRule.FindByConclusion(aRuleList, c.Condition);
                    if (fromRules != null && fromRules.Count > 0)
                    {
                        a.aFromRules.AddRange(fromRules);
                    }
                }
                if (a.aFromRules.Count > 0)
                {
                    foreach (XRule x in a.aFromRules)
                    {
                        if (x.aChildRules == null) x.aChildRules = new List<XRule>();
                        if (!x.aChildRules.Contains(a)) x.aChildRules.Add(a);
                    }
                }
            }
        }

        //----------------------------------------------------------------
        public static void ResetRuleValues(ObservableCollection<XRule> aRuleList)
        {
            if (aRuleList == null) return;
            foreach (XRule a in aRuleList)
            {
                a.Value = -1;
                a.CalcProbability = 0;
            }
        }

        //---------------------------------------------------------
        public string toStr()
        {
            string rstr = "";
            string sRtn = "\r\n";
            rstr += "ID : " + ID;
            rstr += sRtn + "Name : " + Name;
            rstr += sRtn + "Message : " + Message;
            rstr += sRtn + "Conditions : ";
            if (Conditions != null)
            {
                foreach (XCondition c in Conditions)
                {
                    if(!String.IsNullOrEmpty(c.Or)) rstr += sRtn + String.Format("  {0} {1} {2} {3} => {4} ({5})", c.Or, c.Operator, c.Condition, c.WantValue, c.InputValue, c.EvaluatedValue);
                    else rstr += sRtn + String.Format("  {0} {1} {2} => {3} ({4})", c.Operator, c.Condition, c.WantValue, c.InputValue, c.EvaluatedValue);
                }
            }
            rstr += sRtn + "Conclusion: " + Conclusion;
            rstr += sRtn + "Probability: " + Probability;
            rstr += sRtn + "CalcProbability: " + CalcProbability;
            rstr += sRtn + "Value: " + Value;
            if (aChildRules != null && aChildRules.Count > 0)
            {
                rstr += sRtn + "-Child Rules:------------------------------";
                foreach (XRule r in aChildRules)
                {
                    rstr += sRtn + " Rule " + r.toStr2();
                }
            }
            if (aFromRules != null && aFromRules.Count > 0)
            {
                rstr += sRtn + "-From Rules:-------------------------------";
                foreach (XRule r in aFromRules)
                {
                    rstr += sRtn + " Rule " + r.toStr2();
                }
            }

            return rstr;
        }

        //---------------------------------------------------------
        public string toStr2()
        {
            string rstr = "";
            string sRtn = "\r\n";
            rstr += "ID: " + ID + " Name: " + Name + " Probability: " + Probability + 
                    " CalcP: " + CalcProbability + " Value: " + Value;

            rstr += sRtn + "  Conditions : ";
            if (Conditions != null)
            {
                foreach (XCondition c in Conditions)
                {
                    if (!String.IsNullOrEmpty(c.Or)) rstr += sRtn + String.Format("    {0} {1} {2} {3} => {4} ({5})", c.Or, c.Operator, c.Condition, c.WantValue, c.InputValue, c.EvaluatedValue);
                    else      rstr += sRtn + String.Format("    {0} {1} {2} => {3} ({4})", c.Operator, c.Condition, c.WantValue, c.InputValue, c.EvaluatedValue);
                }
            }
            rstr += sRtn + "  Conclusion: " + Conclusion;
            rstr += sRtn + "  Message : " + Message;
            return rstr;
        }

        //-------------------------------------------------
        public string GetRuleValue()
        {
            //   return string.Format("Rule {0} ({1})", ID, Value);
            return String.Format("{0}: {1} ({2})", ID, Name, Value);
        }

        //-------------------------------------------------------------------
        public string GetMessage(string sstr, string sInd)
        {
            if (String.IsNullOrEmpty(sstr)) return "";
            string s = sstr.Replace("/", "\\");
            string sMsg = "";
            if (s.IndexOf("\\") >= 0)
            {
                string[] lines = File.ReadAllLines(s);
                for(int i = 0; i < lines.Length; i++)
                {
                    sMsg += "\r\n" + sInd + lines[i];
                }
            }
            else sMsg = s;
            return sMsg;
        }

    }
}
