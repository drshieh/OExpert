using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Markup;
using System.Windows.Shapes;

namespace OExpert
{
    public class XResult
    {
        public string date { get; set; }
        public string ruleFile { get; set; }
        public string sSummary { get; set; }
        public List<XFact> aXFactList { get; set; }
        public List<XRule> aXRuleList { get; set; }
        public List<XConclusion> aXConclusionList { get; set; }

        public string sTraceText { get; set; }

        //-----------------------------------------------------------------------
        public static List<XResult> ReadRunResults(string resultFile)
        {
            if (String.IsNullOrEmpty(resultFile) || !File.Exists(resultFile))
            {
                OEngine.sRunResult = "";
                return null;
            }
            ObservableCollection<XResult> aResultList = new ObservableCollection<XResult>();
            string[] lines = File.ReadAllLines(resultFile);
            string sResult = "";
            for (int i = 0; i < lines.Length; i++)
            {
                if (String.IsNullOrEmpty(lines[i])) continue;
                if (lines[i].StartsWith("---Inference Report"))
                {
                    if (!String.IsNullOrEmpty(sResult))
                    {
                        XResult r = ParseXResult(sResult);
                        if (r != null) aResultList.Add(r);
                    }
                    sResult = lines[i];
                }
                else sResult += "\r\n" + lines[i];
            }
            if (!String.IsNullOrEmpty(sResult))
            {
                XResult r = ParseXResult(sResult);
                if (r != null) aResultList.Add(r);
            }
            var aList = aResultList.OrderByDescending(o => o.date);
            return aList.ToList();
        }

        //-----------------------------------------
        public static XResult ParseXResult(string _runResult)
        {
            if (String.IsNullOrEmpty(_runResult)) return null;
            XResult r = new XResult();
            string[] sSep = { "\r\n" };
            string[] lines = _runResult.Split(sSep, StringSplitOptions.None);
            int iXFact = -1;
            int iXRule = -1;
            int iXConclusion = -1;
            int iTrace = -1;
            int ik = -1;
            for (int i = 0; i < lines.Length; i++)
            {
                if (lines[i].IndexOf("Inference Report") >= 0)
                {
                    ik = lines[i].IndexOf("Inference Report") + "Inference Report".Length;
                    string str = lines[i].Substring(ik).Replace("-", "").Trim();
                    r.date = str;
                }
                else if (lines[i].IndexOf("Rule Network File:") >= 0)
                {
                    ik = lines[i].IndexOf("Rule Network File:") + "Rule Network File:".Length;
                    r.ruleFile = lines[i].Substring(ik).Trim();
                }
                else if (lines[i].StartsWith("* XFacts:")) iXFact = i;
                else if (lines[i].StartsWith("* XRules:")) iXRule = i;
                else if (lines[i].StartsWith("* XConclusions:")) iXConclusion = i;
                else if (lines[i].StartsWith("* Trace:")) iTrace = i;
            }
            r.sSummary = "";
            for (int i = 0; i < iXFact; i++)
            {
                r.sSummary += lines[i] + "\r\n";
            }

            string[] subx;
            string[] subxx;

            r.aXFactList = new List<XFact>();
            for (int i = iXFact + 1; i < iXRule; i++)
            {
                if (String.IsNullOrEmpty(lines[i])) continue;
                subx = lines[i].Trim().Replace("- ", "").Split(':');
                if (subx.Length == 2) r.aXFactList.Add(new XFact(subx[0].Trim(), subx[1].Trim()));
                else if (subx.Length == 3) r.aXFactList.Add(new XFact(subx[0].Trim(), subx[1].Trim(), subx[2].Trim()));
            }
            List<string> aRuleStrList = new List<string>();
            for (int i = iXRule + 1; i < iXConclusion; i++)
            {
                if (String.IsNullOrEmpty(lines[i])) continue;
                aRuleStrList.Add(lines[i]);
            }
            r.aXRuleList = new List<XRule>();
            XRule aRule = null;

            foreach (string s0 in aRuleStrList)
            {
                string s = s0.Trim();
                if (s.StartsWith("-"))
                {
                    subx = s.Split(':');
                    subxx = subx[0].Split(' ');
                    aRule = new XRule();
                    aRule.ID = WHelper.GetInt32(subxx[1].Trim());
                    subxx = subx[1].Split('|');
                    aRule.Name = subxx[0].Trim();
                    if (subxx.Length > 1) aRule.Conclusion = subxx[1].Trim();
                    if (subxx.Length > 2) aRule.Value = WHelper.GetInt32(subxx[2].Trim());
                    if (subxx.Length > 3) aRule.CalcProbability = WHelper.GetDouble(subxx[3].Trim());
                    aRule.Conditions = new List<XCondition>();
                    r.aXRuleList.Add(aRule);
                }
                else
                {
                    subx = s.Split(':');
                    subxx = subx[0].Split('|');
                    XCondition c = new XCondition();
                    c.Or = subxx[0].Trim();
                    c.Operator = subxx[1].Trim();
                    if (subxx.Length > 2) c.Condition = subxx[2].Trim();
                    if (subxx.Length > 3) c.WantValue = subxx[3].Trim();
                    subxx = subx[1].Split('|');
                    c.InputValue = subxx[0].Trim();
                    if (subxx.Length > 1) c.EvaluatedValue = WHelper.GetInt32(subxx[1].Trim());
                    aRule.Conditions.Add(c);
                }
            }
            int n = iTrace;
            if (iTrace < 0) n = lines.Length;
            r.aXConclusionList = new List<XConclusion>();
            for (int i = iXConclusion + 1; i < n; i++)
            {
                if (String.IsNullOrEmpty(lines[i])) continue;
                subx = lines[i].Split(':');
                if (subx.Length >= 2)
                {
                    XConclusion c = new XConclusion();
                    c.conclusion = subx[0].Replace("-", "").Trim();
                    subxx = subx[1].Split('|');
                    c.Value = WHelper.GetInt32(subxx[0].Trim());
                    if (subxx.Length > 1) c.CalcProbability = WHelper.GetDouble(subxx[1].Trim());
                    r.aXConclusionList.Add(c);
                }
            }
            if (iTrace >= 0)
            {
                r.sTraceText = "";
                for (int i = iTrace + 1; i < lines.Length; i++)
                {
                    if (String.IsNullOrEmpty(lines[i])) continue;
                    r.sTraceText += lines[i] + "\r\n";
                }
            }
            else
            {
                r.sTraceText = "";
                if (File.Exists(MainWindow.tp.sResultFile + ".trace"))
                {
                    r.sTraceText = File.ReadAllText(MainWindow.tp.sResultFile + ".trace");
                }
            }
            return r;
        }

        //----------------------------------------------------------------
        public string ToStr()
        {
            string rstr = "";
            //            rstr += String.Format("---Inference Report-------{0}--------------------", date);
            //            rstr += String.Format("\r\n-- Rule Network File: {0}", ruleFile);
            rstr += String.Format("\r\n{0}", sSummary);
            rstr += String.Format("\r\n* XFacts:");
            foreach (XFact a in aXFactList)
            {
                rstr += String.Format("\r\n  - {0}: {1}: {2}", a.Level, a.fact, a.Value);
            }
            rstr += String.Format("\r\n* XRules:");
            foreach (XRule a in aXRuleList)
            {
                rstr += String.Format("\r\n  - {0}:{1} | {2} | {3} | {4}", a.ID, a.Name, a.Conclusion, a.Value, a.Probability);
                if (a.Conditions != null)
                {
                    foreach (XCondition c in a.Conditions)
                    {
                        rstr += String.Format("\r\n    {0} | {1} | {2} | {3} : {4} | {5}", c.Or, c.Operator, c.Condition, c.WantValue, c.InputValue, c.EvaluatedValue);
                    }
                }
            }
            rstr += String.Format("\r\n* XConclusions:");
            foreach (XConclusion a in aXConclusionList)
            {
                rstr += String.Format("\r\n  - {0}: {1} | {2}", a.conclusion, a.Value, a.CalcProbability);
            }
            rstr += String.Format("\r\n* Trace:");
            rstr += String.Format("\r\n" + sTraceText);
            rstr += "\r\n";
            return rstr;
        }

        //----------------------------------------------------------------------
        public static string ToStrX(List<XResult> aList)
        {
            string rstr = "";
            foreach (XResult a in aList)
            {
                rstr += "Date: " + a.date + "\r\n";
                rstr += "RuleFile: " + a.ruleFile + "\r\n";
                rstr += "Summary: " + a.sSummary + "\r\n";
            }
            return rstr;
        }
    }
}
