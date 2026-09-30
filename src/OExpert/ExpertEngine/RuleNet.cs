using Newtonsoft.Json;
using OExpert.UILibraryWPF;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace OExpert
{
    public class RuleNet
    {
        public string Name { get; set; }
        public List<Rule> aRuleList { get; set; }
        public List<RFact> aFactList { get; set; }
        public List<RConclusion> aConclusionList { get; set; }

        //---------------------------------------------------------
        public static RuleNet ReadFile(string sFile)
        {
            try
            {
                if (String.IsNullOrEmpty(sFile)) return null;
                if (!File.Exists(sFile)) return null;
                string sJson = File.ReadAllText(sFile);
                if (String.IsNullOrEmpty(sJson)) return null;
                RuleNet a = JsonConvert.DeserializeObject<RuleNet>(sJson);
                if (a == null) return null;
                if (a.aRuleList != null)
                {
                    a.aRuleList = a.aRuleList.OrderBy(x => x.Name).ToList();
                    if (a.aConclusionList == null) a.aConclusionList = GetConclusionList(a);  //-Conclusion needed for Fact
                    if (a.aFactList == null) a.aFactList = GetFactList(a);
                }
                return a;
            }
            catch (Exception e) {
                MessageBox.Show("ERROR: RuleNet.ReadFile: " + e.Message);
                return null; 
            }
        }

        //-------------------------------------------------
        public static List<RFact> GetFactList(RuleNet aNet)
        {
            if (aNet == null || aNet.aRuleList == null) return null;
            List<RFact> aFactList = new List<RFact>();
            foreach (Rule a in aNet.aRuleList)
            {
                if (a.Conditions == null || a.Conditions.Count <= 0) continue;
                foreach (RCondition c in a.Conditions)
                {
                    if (RConclusion.Find(aNet.aConclusionList, c.Condition) < 0)  //--exclude the Conclusions
                    {
                        RFact.AddFact(aFactList, c, a);
                    }
                }
            }
            return aFactList;
        }

        //------------------------------------------------------------------------------
        public static List<RConclusion> GetConclusionList(RuleNet aNet)
        {
            if (aNet == null || aNet.aRuleList == null) return null;
            List<RConclusion> conclusionList = new List<RConclusion>();
            foreach (Rule a in aNet.aRuleList)
            {
                RConclusion.AddConclusion(conclusionList, a);
            }
            return conclusionList;
        }

        //------------------------------------------------------------------------------
        public static void WriteFile(string sFile, RuleNet a)
        {
            if (String.IsNullOrEmpty(sFile) || a == null) return;
            StreamWriter sw = new StreamWriter(sFile);
            string sJson = JsonConvert.SerializeObject(a, Newtonsoft.Json.Formatting.Indented);
            sw.WriteLine(sJson);
            sw.Close();
        }

        //------------------------------------------------------------------------------
        public static string ToStr(RuleNet a)
        {
            if (a == null) return null;
            return JsonConvert.SerializeObject(a, Newtonsoft.Json.Formatting.Indented);
        }

        //---------------------------------------------
        public static bool SetFactSize(RFact aFact, Pen pen)
        {
            double bx = aFact.X;
            double ex = aFact.X + aFact.Width;
            double by = aFact.Y;
            double ey = aFact.Y + aFact.Height;
            string sfact = aFact.fact;
            bool fSizeChanged = false;
            WDrawingVisual.GetProperFormattedText(ref sfact, ref bx, ref by, ref ex, ref ey,
                 GlobalVarX.aRuleDefXFont, pen, ref fSizeChanged);
            if (fSizeChanged)
            {
                aFact.Width = ex - bx;
                aFact.Height = ey - by;
            }
            return fSizeChanged;
        }

        //---------------------------------------------
        public static bool SetRuleSize(Rule aRule, Pen pen)
        {
            double bx = aRule.X;
            double ex = aRule.X + aRule.Width;
            double by = aRule.Y;
            double ey = aRule.Y + aRule.Height;
            string srule = "Rule: " + aRule.ID;
            bool fSizeChanged = false;
            WDrawingVisual.GetProperFormattedText(ref srule, ref bx, ref by, ref ex, ref ey,
                 GlobalVarX.aRuleDefXFont, pen, ref fSizeChanged);
            if (fSizeChanged)
            {
                aRule.Width = ex - bx;
                aRule.Height = ey - by;
            }
            return fSizeChanged;
        }

        //---------------------------------------------
        public static bool SetConclusionSize(RConclusion aCon, Pen pen)
        {
            double bx = aCon.X;
            double ex = aCon.X + aCon.Width;
            double by = aCon.Y;
            double ey = aCon.Y + aCon.Height;
            string scon = aCon.conclusion;
            bool fSizeChanged = false;
            WDrawingVisual.GetProperFormattedText(ref scon, ref bx, ref by, ref ex, ref ey,
                 GlobalVarX.aRuleDefXFont, pen, ref fSizeChanged);
            if (fSizeChanged)
            {
                aCon.Width = ex - bx;
                aCon.Height = ey - by;
            }
            return fSizeChanged;
        }


        //---------------------------------------------------
        public void ResetValues()
        {
            if (aRuleList == null) return;
            foreach (Rule a in aRuleList)
            {
                a.Value = -1;
            }
            foreach (RFact a in aFactList)
            {
                a.Value = "";
            }
            foreach (RConclusion a in aConclusionList)
            {
                a.Value = -1;
            }
        }

        //-------------------------------------------------
        public void RemoveRuleItemList(List<object> aList)
        {
            if (aList == null) return;
            foreach (object a in aList)
            {
                RemoveRuleItem(a);
            }
        }

        //-----------------------------------------------------
        public void RemoveRuleItem(object o)
        {
            if (o.GetType() == typeof(Rule))
            {
                Rule x = o as Rule;
                if (aRuleList != null && aRuleList.Count > 0) aRuleList.Remove(x);
            }
            else if (o.GetType() == typeof(RFact))
            {
                RFact x = o as RFact;
                if (aFactList != null && aFactList.Count > 0) aFactList.Remove(x);
            }
            else if (o.GetType() == typeof(RConclusion))
            {
                RConclusion x = o as RConclusion;
                if (aConclusionList != null && aConclusionList.Count > 0) aConclusionList.Remove(x);
            }
        }
    }
}
