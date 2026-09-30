//---------------------------------------------
//--Forward and Backward chaining
//---------------------------------------------
using Newtonsoft.Json.Linq;
using OExpert.UILibraryWPF;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows;
using System.Windows.Media;

namespace OExpert
{
    public class EConclude
    {
        public string conclusion;
        public int value;
        public double probability;

        public EConclude()
        {
        }

        public EConclude(string _conclusion, int _value, double _probability)
        {
            this.conclusion = _conclusion;
            this.value = _value;
            this.probability = _probability;
        }

        public static EConclude Find(List<EConclude> list, string _conclusion)
        {
            if (list == null || list.Count == 0) return null;
            foreach (EConclude e in list)
            {
                if (e.conclusion == _conclusion) return e;
            }
            return null;
        }
    }

    public class OEngineFBC
    {
        //     public List<XRule> Rules { get; } = new List<XRule>();
        public XRuleNet aXRuleNet { get; set; }
        public List<string> aTraceList;
        public List<XTrace> aXTraceList;
        public List<XTrace2> aXTrace2List;

        //-------------------------------------------------------------------------------------
        public OEngineFBC()
        {
        }

        //-------------------------------------------------------------------------------------
        public OEngineFBC(XRuleNet _xruleNet)
        {
            if (_xruleNet == null) Console.WriteLine("No rules defined");
            aXRuleNet = _xruleNet;
        }

        //-------------------------------------------------------------------------------------
        public int EvaluateCondition(XCondition c, ObservableCollection<InputFact> facts)
        {
            int iRtn = -1;
            if (c != null)
            {
                InputFact x = InputFact.Find(facts, c.Condition);
                if (x != null)
                {
                    iRtn = EvaluateConditionD(c, x.value, x.ivalue);
                    c.EvaluatedValue = iRtn;
                    c.InputValue = x.value;
                    if (aTraceList != null) aTraceList.Add(String.Format(" condition: {0} {1} {2} : {3}, {4}", c.Operator, c.Condition, c.WantValue, c.InputValue, c.EvaluatedValue));
                }
                else
                {
                    c.EvaluatedValue = -1;
                    c.InputValue = "";
                    if (aTraceList != null) aTraceList.Add(String.Format(" *condition: {0} {1} {2} : {3}, {4}", c.Operator, c.Condition, c.WantValue, c.InputValue, c.EvaluatedValue));
                }
            }
            return iRtn;
        }

        //-------------------------------------------------------------------------------------
        public int EvaluateConditionX(XCondition c, ObservableCollection<XInputFact> facts)
        {
            int iRtn = -1;
            if (c != null)
            {
                XInputFact x = XInputFact.Find(facts, c.Condition);
                if (x != null)
                {
                    iRtn = EvaluateConditionD(c, x.value, x.ivalue);
                    c.EvaluatedValue = iRtn;
                    c.InputValue = x.value;
                    if (aTraceList != null) aTraceList.Add(String.Format(" condition: {0} {1} {2} : {3}, {4}", c.Operator, c.Condition, c.WantValue, c.InputValue, c.EvaluatedValue));
                }
                else
                {
                    c.EvaluatedValue = -1;
                    c.InputValue = "";
                    if (aTraceList != null) aTraceList.Add(String.Format(" *condition: {0} {1} {2} : {3}, {4}", c.Operator, c.Condition, c.WantValue, c.InputValue, c.EvaluatedValue));
                }
            }
            return iRtn;
        }

        //-------------------------------------------------------------------------------------
        public int EvaluateConditionD(XCondition c, string factValue, int ifactValue)
        {
            if (c == null) return -1;
            int iRtn = -1;
            if (c.Operator == "Ask")  //--Ask for a condition, if no WantValue => return the ivalue, else compare
            {
                if (String.IsNullOrEmpty(c.WantValue)) iRtn = ifactValue + 1;  //--selected index (option) + 1
                else
                {
                    if (WHelper.IsBoolean(factValue) && WHelper.IsBoolean(c.WantValue))
                    {
                        if (WHelper.GetBoolean(factValue) == WHelper.GetBoolean(c.WantValue)) iRtn = 1;
                        else iRtn = -1;
                    }
                    else
                    {
                        if (factValue == c.WantValue) iRtn = 1;
                        else iRtn = -1;
                    }
                }
            }
            else if (c.Operator == "Is")
            {
                if (WHelper.IsBoolean(factValue) && WHelper.IsBoolean(c.WantValue))
                {
                    if (WHelper.GetBoolean(factValue) == WHelper.GetBoolean(c.WantValue)) iRtn = 1;
                    else iRtn = -1;
                }
                else
                {
                    if (factValue == c.WantValue) iRtn = 1;
                    else iRtn = -1;
                }
            }
            else if (c.Operator == "IsNot")
            {
                if (WHelper.IsBoolean(factValue) && WHelper.IsBoolean(c.WantValue))
                {
                    if (WHelper.GetBoolean(factValue) != WHelper.GetBoolean(c.WantValue)) iRtn = 1;
                    else iRtn = -1;
                }
                else
                {
                    if (factValue != c.WantValue) iRtn = 1;
                    else iRtn = -1;
                }
            }
            else if (c.Operator == "In")
            {
                if (c.WantValue.IndexOf(factValue) >= 0) iRtn = 1;
                else iRtn = -1;
            }
            else if (c.Operator == "NotIn")
            {
                if (c.WantValue.IndexOf(factValue) < 0) iRtn = 1;
                else iRtn = -1;
            }
            else if (c.Operator == "==")
            {
                if (factValue == c.WantValue) iRtn = 1;
                else return -1;
            }
            else if (c.Operator == "!=")
            {
                if (factValue != c.WantValue) iRtn = 1;
                else iRtn = -1;
            }
            else if (c.Operator == ">")
            {
                if (WHelper.GetDouble(factValue) > WHelper.GetDouble(c.WantValue)) iRtn = 1;
                else iRtn = -1;
            }
            else if (c.Operator == ">=")
            {
                if (WHelper.GetDouble(factValue) >= WHelper.GetDouble(c.WantValue)) iRtn = 1;
                else iRtn = -1;
            }
            else if (c.Operator == "<")
            {
                if (WHelper.GetDouble(factValue) < WHelper.GetDouble(c.WantValue)) iRtn = 1;
                else iRtn = -1;
            }
            else if (c.Operator == "<=")
            {
                if (WHelper.GetDouble(factValue) <= WHelper.GetDouble(c.WantValue)) return 1;
                else iRtn = -1;
            }
            Console.WriteLine("EvaluateConditionD: {0} {1} {2} => {3}", c.Operator, c.Condition, c.WantValue, iRtn);
            return iRtn;
        }


        //-------------------------------------------------------------------------------------
        public int EvaluateConclusion(XCondition c, List<EConclude> conclusions, ref double preConProb)
        {
            int iRtn = -1;
            if (c == null) return iRtn;
            int i = 0;
            foreach (EConclude a in conclusions)
            {
                if (aTraceList != null) aTraceList.Add(String.Format("   concluded: {0}> {1},  {2}, {3}", i, a.conclusion, a.value, a.probability));
                i++;
            }
            EConclude x = EConclude.Find(conclusions, c.Condition);
            if (x != null)
            {
                //     string sValue = "False";
                //     if (x.probability > 0) sValue = "True";
                string sValue = x.value.ToString();  //--**
                iRtn = EvaluateConclusionD(c, sValue);
                c.EvaluatedValue = iRtn;
                c.InputValue = sValue;
                if (aTraceList != null) aTraceList.Add(String.Format(" conclusion: {0} {1} {2} : {3}, {4}", c.Operator, c.Condition, c.WantValue, c.InputValue, c.EvaluatedValue));

                if (iRtn > 0) preConProb = x.probability;
                else preConProb = GlobalVarX.MinProbability;
            }
            else
            {
                c.EvaluatedValue = -1;
                c.InputValue = "";
                if (aTraceList != null) aTraceList.Add(String.Format(" *conclusion: {0} {1} {2} : {3}, {4}", c.Operator, c.Condition, c.WantValue, c.InputValue, c.EvaluatedValue));
            }
            return iRtn;
        }

        //-------------------------------------------------------------------------------------
        public int EvaluateConclusionD(XCondition c, string value)
        {
            if (c == null) return -1;
            int iRtn = -1;
            if (c.Operator == "Ask")
            {
                int iv = WHelper.FindInList(c.Options, value);
                iRtn = iv + 1;
            }
            else if (c.Operator == "Is")
            {
                if (WHelper.IsBoolean(value) && WHelper.IsBoolean(c.WantValue))
                {
                    if (WHelper.GetBoolean(value) == WHelper.GetBoolean(c.WantValue)) iRtn = 1;
                    else iRtn = -1;
                }
                else
                {
                    if (value == c.WantValue) iRtn = 1;
                    else iRtn = -1;
                }
            }
            else if (c.Operator == "IsNot")
            {
                if (WHelper.IsBoolean(value) && WHelper.IsBoolean(c.WantValue))
                {
                    if (WHelper.GetBoolean(value) != WHelper.GetBoolean(c.WantValue)) iRtn = 1;
                    else iRtn = -1;
                }
                else
                {
                    if (value != c.WantValue) iRtn = 1;
                    else iRtn = -1;
                }
            }
            else if (c.Operator == "In")
            {
                if (c.WantValue.IndexOf(value) >= 0) iRtn = 1;
                else iRtn = -1;
            }
            else if (c.Operator == "NotIn")
            {
                if (c.WantValue.IndexOf(value) < 0) iRtn = 1;
                else iRtn = -1;
            }
            else if (c.Operator == "==")
            {
                if (value == c.WantValue) return 1;
                else return -1;
            }
            else if (c.Operator == "!=")
            {
                if (value != c.WantValue) iRtn = 1;
                else iRtn = -1;
            }
            else if (c.Operator == ">")
            {
                if (WHelper.GetDouble(value) > WHelper.GetDouble(c.WantValue)) iRtn = 1;
                else iRtn = -1;
            }
            else if (c.Operator == ">=")
            {
                if (WHelper.GetDouble(value) >= WHelper.GetDouble(c.WantValue)) iRtn = 1;
                else iRtn = -1;
            }
            else if (c.Operator == "<")
            {
                if (WHelper.GetDouble(value) < WHelper.GetDouble(c.WantValue)) iRtn = 1;
                else iRtn = -1;
            }
            else if (c.Operator == "<=")
            {
                if (WHelper.GetDouble(value) <= WHelper.GetDouble(c.WantValue)) iRtn = 1;
                else iRtn = -1;
            }
            Console.WriteLine("EvaluateConclusionD: {0} {1} {2} => {3}", c.Operator, c.Condition, c.WantValue, iRtn);
            return iRtn;
        }

        //-------------------------------------------------------------------------------------
        // Mix chaining implementation
        public List<EConclude> MixChain(out ObservableCollection<InputFact> factsX, ObservableCollection<InputFact> previousFacts)
        {
            var conclusions = new List<EConclude>();
            bool updated;
            ObservableCollection<InputFact> facts = new ObservableCollection<InputFact>();  //--Overall facts (InputFact)
            List<XRule> aWorkRules = XRule.GetTopRules(aXRuleNet.aRuleList);
            aTraceList = new List<string>();
            aXTraceList = new List<XTrace>();
            int iLevel = 0;
            do
            {
                Console.WriteLine("-----------------------------------LEVEL: " + iLevel);
                aTraceList.Add("-----------------------------------LEVEL: " + iLevel);
                iLevel++;
                updated = false;
                //--Input Facts
                ObservableCollection<InputFact> factsONE = InputFact.FromXRulesX(aWorkRules, aXRuleNet.aConclusionList, facts);
                if (factsONE != null && factsONE.Count > 0)
                {
                    foreach (InputFact a in factsONE)
                    {
                        InputFact x = InputFact.Find(previousFacts, a.fact);
                        if (x != null) { a.value = x.value; a.ivalue = x.ivalue; }
                    }
                    FactInputDlg dlg = new FactInputDlg(factsONE, null, false);
                    dlg.Owner = MainWindow.tp;
                    dlg.ShowDialog();
                    if (dlg.DialogResult == false)
                    {
                        factsX = null;
                        return null;
                    }
                    aXTraceList.Add(new XTrace(factsONE.ToList()));
                    foreach (InputFact f in factsONE)
                    {
                        aTraceList.Add(String.Format(" input: {0}, {1}", f.fact, f.value));
                        f.Level = iLevel;
                        facts.Add(f);
                    }
                }

                //--Evaluate Rules
                foreach (XRule rule in aWorkRules)
                {
                    int iRValue = -1;
                    double fromConProb = 0;
                    if (rule.Conditions != null && rule.Conditions.Count > 0)
                    {
                        iRValue = 1;
                        List<int> condValues = new List<int>();
                        bool fOr = false;
                        foreach (XCondition c in rule.Conditions)
                        {
                            if (c.Or == "Or") fOr = true;
                            int v = -1;
                            if (c.isConclusionType)
                            {
                                v = EvaluateConclusion(c, conclusions, ref fromConProb);
                            }
                            else
                            {
                                v = EvaluateCondition(c, facts);
                            }
                            condValues.Add(v);
                        }

                        //--final Value
                        if (fOr)
                        {
                            iRValue = -1;
                            foreach (int v in condValues)
                            {
                                if (v > iRValue) iRValue = v;
                            }
                        }
                        else
                        {
                            iRValue = 1;
                            foreach (int v in condValues)
                            {
                                if (v < 0) iRValue = 0;
                                else if (iRValue > 0 && v > iRValue) iRValue = v;
                            }
                        }
                    }
                    rule.Value = iRValue;

                    double prob = 0;

                    WDrawRule.DrawRuleXC(MainWindow.tp.ucRuleNetwork.ucDrawArea.DACanvas, rule, 0, 0, false, 1, 1, false);
                    UCRuleNetwork.SetItemIntoView(rule);

                    EConclude x = EConclude.Find(conclusions, rule.Conclusion);
                    if (x == null)
                    {
                        prob = rule.Probability;
                        if (fromConProb > 0) prob = fromConProb + prob * (1 - fromConProb);
                        x = new EConclude(rule.Conclusion, rule.Value, prob);
                        conclusions.Add(x);
                        updated = true;
                    }
                    else if (x.probability < rule.Probability)
                    {
                        prob = rule.Probability;
                        if (fromConProb > 0) prob = fromConProb + prob * (1 - fromConProb);
                        x.probability = prob;
                        updated = true;
                    }

                    rule.CalcProbability = prob;
                    aTraceList.Add(String.Format("==> RULE: {0} {1} {2} => {3} ({4})", rule.ID, rule.Name, rule.Conclusion, rule.Value, rule.CalcProbability));
                    Console.WriteLine("==> RULE: {0} {1} {2} => {3} ({4})", rule.ID, rule.Name, rule.Conclusion, rule.Value, rule.CalcProbability);
                }
                //--Working on the child rules
                List<XRule> aNewRules = new List<XRule>();
                foreach (XRule rule in aWorkRules)
                {
                    if (rule.CalcProbability <= GlobalVarX.MinProbability) continue;
                    //               if (rule.Value <= 0) continue;  //--only the True Rules, value > 0 (cover Ask)
                    if (rule.aChildRules == null || rule.aChildRules.Count <= 0) continue;
                    List<XRule> aList = PreConcludeEvaluateChildRules(conclusions, rule.aChildRules);
                    //aNewRules.AddRange(rule.aChildRules);
                    if (aList != null && aList.Count > 0) aNewRules.AddRange(aList);
                }
                if (aNewRules.Count > 0)
                {
                    aWorkRules = aNewRules;
                }
                else updated = false;
            } while (updated);

            factsX = facts;

            //--Write Trace file
            string sOutFile = MainWindow.tp.sResultFile + ".trace";
            StreamWriter sw = new StreamWriter(sOutFile);
            foreach (string s in aTraceList)
            {
                sw.WriteLine("{0}", s);
            }
            sw.Close();

            return conclusions;
        }


        //-------------------------------------------------------------------------------------
        // Auto Test MixChain implementation
        public List<EConclude> AutoTestMixChain(StreamWriter sw, out ObservableCollection<XInputFact> factsX, ObservableCollection<XInputFact> previousFacts)
        {
            var conclusions = new List<EConclude>();
            bool updated;
            ObservableCollection<XInputFact> facts = new ObservableCollection<XInputFact>();  //--Overall facts (InputFact)
            List<XRule> aWorkRules = XRule.GetTopRules(aXRuleNet.aRuleList);
            aTraceList = new List<string>();
            aXTrace2List = new List<XTrace2>();
            int iLevel = 0;
            do
            {
                Console.WriteLine("-----------------------------------LEVEL: " + iLevel);
                aTraceList.Add("-----------------------------------LEVEL: " + iLevel);
                iLevel++;
                updated = false;
                //--Input Facts
                ObservableCollection<XInputFact> factsONE = XInputFact.FromXRulesX(aWorkRules, aXRuleNet.aConclusionList, facts);
                if (factsONE != null && factsONE.Count > 0)
                {
                    for (int i=0;i< factsONE.Count;i++)
                    {
                        XInputFact x = XInputFact.Find(previousFacts, factsONE[i].fact);
                        if (x != null) factsONE[i] = x;
                        else factsONE[i].ivalue = 0;

                        if (factsONE[i].options != null)
                        {
                            int k = factsONE[i].ivalue;
                            if (k >= 0 && k < factsONE[i].options.Count)
                            {
                                factsONE[i].value = factsONE[i].options[k].option;
                                factsONE[i].options[k].visit = true;
                            }
                        }
                    }

                    foreach (XInputFact f in factsONE)
                    {
                        aTraceList.Add(String.Format(" input: {0}, {1}", f.fact, f.value));
                        f.Level = iLevel;
                        facts.Add(f);
                    }
                    aXTrace2List.Add(new XTrace2(factsONE.ToList()));
                }

                //--Evaluate Rules
                foreach (XRule rule in aWorkRules)
                {
                    int iRValue = -1;
                    double fromConProb = 0;
                    if (rule.Conditions != null && rule.Conditions.Count > 0)
                    {
                        iRValue = 1;
                        List<int> condValues = new List<int>();
                        bool fOr = false;
                        foreach (XCondition c in rule.Conditions)
                        {
                            if (c.Or == "Or") fOr = true;
                            int v = -1;
                            if (c.isConclusionType)
                            {
                                v = EvaluateConclusion(c, conclusions, ref fromConProb);
                            }
                            else
                            {
                                v = EvaluateConditionX(c, facts);
                            }
                            condValues.Add(v);
                        }

                        //--final Value
                        if (fOr)
                        {
                            iRValue = -1;
                            foreach (int v in condValues)
                            {
                                if (v > iRValue) iRValue = v;
                            }
                        }
                        else
                        {
                            iRValue = 1;
                            foreach (int v in condValues)
                            {
                                if (v < 0) iRValue = 0;
                                else if (iRValue > 0 && v > iRValue) iRValue = v;
                            }
                        }
                    }
                    rule.Value = iRValue;

                    double prob = 0;

            //??        WDrawRule.DrawRuleXC(MainWindow.tp.ucRuleNetwork.ucDrawArea.DACanvas, rule, 0, 0, false, 1, 1, false);
            //??        UCRuleNetwork.SetItemIntoView(rule);

                    EConclude x = EConclude.Find(conclusions, rule.Conclusion);
                    if (x == null)
                    {
                        prob = rule.Probability;
                        if (fromConProb > 0) prob = fromConProb + prob * (1 - fromConProb);
                        x = new EConclude(rule.Conclusion, rule.Value, prob);
                        conclusions.Add(x);
                        updated = true;
                    }
                    else if (x.probability < rule.Probability)
                    {
                        prob = rule.Probability;
                        if (fromConProb > 0) prob = fromConProb + prob * (1 - fromConProb);
                        x.probability = prob;
                        updated = true;
                    }

                    rule.CalcProbability = prob;
                    aTraceList.Add(String.Format("==> RULE: {0} {1} {2} => {3} ({4})", rule.ID, rule.Name, rule.Conclusion, rule.Value, rule.CalcProbability));
                    Console.WriteLine("==> RULE: {0} {1} {2} => {3} ({4})", rule.ID, rule.Name, rule.Conclusion, rule.Value, rule.CalcProbability);
                }
                //--Working on the child rules
                List<XRule> aNewRules = new List<XRule>();
                foreach (XRule rule in aWorkRules)
                {
                    if (rule.CalcProbability <= GlobalVarX.MinProbability) continue;
                    //               if (rule.Value <= 0) continue;  //--only the True Rules, value > 0 (cover Ask)
                    if (rule.aChildRules == null || rule.aChildRules.Count <= 0) continue;
                    List<XRule> aList = PreConcludeEvaluateChildRules(conclusions, rule.aChildRules);
                    //aNewRules.AddRange(rule.aChildRules);
                    if (aList != null && aList.Count > 0) aNewRules.AddRange(aList);
                }
                if (aNewRules.Count > 0)
                {
                    aWorkRules = aNewRules;
                }
                else updated = false;
            } while (updated);

            factsX = facts;

            //--Write XTrace file
            if (sw != null)
            {
                foreach (XTrace2 s in aXTrace2List)
                {
                    sw.WriteLine("{0}", s.toStr());
                }
            }

            return conclusions;
        }

        //-------------------------------------------------------------------------------------
        public List<XRule> PreConcludeEvaluateChildRules(List<EConclude> conclusions, List<XRule> childRules)
        {
            if (childRules == null || childRules.Count <= 0) return null;
            List<XRule> aList = new List<XRule>();
            double fromConProb = 0;
            foreach (XRule rule in childRules)
            {
                if (rule.Conditions == null || rule.Conditions.Count <= 0) continue;
                int iRValue = -1;
                foreach (XCondition c in rule.Conditions)
                {
                    if (!c.isConclusionType) continue;
                    if (EvaluateConclusion(c, conclusions, ref fromConProb) > 0)
                    {
                        iRValue = 1;
                    }
                }
                if (iRValue > 0) aList.Add(rule);
            }
            return aList;
        }

        //-------------------------------------------------------------------------------------
        // Forward chaining implementation
        public Dictionary<string, double> ForwardChain(ObservableCollection<InputFact> facts)
        {
            var conclusions = new Dictionary<string, double>();
            bool updated;

            do
            {
                updated = false;
                foreach (var rule in aXRuleNet.aRuleList)
                {
                    int iRValue = -1;
                    if (rule.Conditions != null && rule.Conditions.Count > 0)
                    {
                        foreach (XCondition c in rule.Conditions)
                        {
                            if (EvaluateCondition(c, facts) < 0)
                            {
                                iRValue = 0;
                                break;
                            }
                            else iRValue = 1;
                        }
                    }
                    rule.Value = iRValue;
                    if (rule.Value >= 1)
                    {
                        if (!conclusions.ContainsKey(rule.Conclusion) ||
                            conclusions[rule.Conclusion] < rule.Probability)
                        {
                            conclusions[rule.Conclusion] = rule.Probability;
                            updated = true;
                        }
                    }
                }
            } while (updated);

            return conclusions;
        }

        //-------------------------------------------------------------------------------------
        // Backward chaining implementation with probability
        public (bool result, double probability) BackwardChain(string goal, ObservableCollection<InputFact> facts)
        {
            var applicableRules = aXRuleNet.aRuleList.Where(r => r.Conclusion == goal).ToList();
            if (!applicableRules.Any()) return (false, 0.0);

            double maxProbability = 0.0;

            foreach (var rule in applicableRules)
            {
                double ruleProbability = rule.Probability;
                bool allConditionsMet = true;
                if (rule.Conditions != null && rule.Conditions.Count > 0)
                {
                    foreach (XCondition c in rule.Conditions)
                    {
                        InputFact x = InputFact.Find(facts, c.Condition);
                        if (x != null)
                        {
                            if (EvaluateConditionD(c, x.value, x.ivalue) < 0)
                            {
                                allConditionsMet = false;
                                break;
                            }
                        }
                        else
                        {
                            var (subResult, subProbability) = BackwardChain(c.Condition, facts);
                            if (subResult)
                            {
                                ruleProbability = ruleProbability + subProbability * (1- ruleProbability);
                            }
                            else
                            {
                                allConditionsMet = false;
                                break;
                            }
                        }
                    }
                }
                if (allConditionsMet && ruleProbability > maxProbability)
                {
                    maxProbability = ruleProbability;
                }
            }

            return (maxProbability > 0, maxProbability);
        }

        //-------------------------------------------------------------------------------------
        // Forward chaining implementation
        public Dictionary<string, double> ForwardChain0(Dictionary<string, string> facts)
        {
            var conclusions = new Dictionary<string, double>();
            bool updated;

            do
            {
                updated = false;
                foreach (var rule in aXRuleNet.aRuleList)
                {
                    var x = rule.Conditions.All(cond => facts.ContainsKey(cond.Condition));
                    var x2 = rule.Conditions.All(cond => facts.ContainsKey(cond.Condition) && facts[cond.Condition] == cond.WantValue);
                    if (rule.Conditions.All(cond =>
                        facts.ContainsKey(cond.Condition) && facts[cond.Condition] == cond.WantValue))
                    {
                        if (!conclusions.ContainsKey(rule.Conclusion) ||
                            conclusions[rule.Conclusion] < rule.Probability)
                        {
                            conclusions[rule.Conclusion] = rule.Probability;
                            updated = true;
                        }
                    }
                }
            } while (updated);

            return conclusions;
        }

        //-------------------------------------------------------------------------------------
        // Backward chaining implementation with probability
        public (bool result, double probability) BackwardChain0(string goal, Dictionary<string, string> facts)
        {
            var applicableRules = aXRuleNet.aRuleList.Where(r => r.Conclusion == goal).ToList();
            if (!applicableRules.Any()) return (false, 0.0);

            double maxProbability = 0.0;

            foreach (var rule in applicableRules)
            {
                double ruleProbability = rule.Probability;
                bool allConditionsMet = true;
                foreach (var condition in rule.Conditions)
                {
                    if (facts.ContainsKey(condition.Condition))
                    {
                        if (facts[condition.Condition] != condition.WantValue)
                        {
                            allConditionsMet = false;
                            break;
                        }
                    }
                    else
                    {
                        var (subResult, subProbability) = BackwardChain0(condition.Condition, facts);
                        if (subResult)
                        {
                            ruleProbability = ruleProbability + subProbability * (1 - ruleProbability);
                        }
                        else
                        {
                            allConditionsMet = false;
                            break;
                        }
                    }
                }

                if (allConditionsMet && ruleProbability > maxProbability)
                {
                    maxProbability = ruleProbability;
                }
            }

            return (maxProbability > 0, maxProbability);
        }
    }
}
