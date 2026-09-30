//---------------------------------------------
//--Forward and Backward chaining
//---------------------------------------------
using OExpert.UILibraryWPF;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Xml.Linq;

namespace OExpert
{
    public class OEngine
    {
        public static string sRunErrorMsg = "";
        public static string sRunResult = "";
        public static ObservableCollection<InputFact> aPreviousInputFacts = null;
        public static ObservableCollection<XInputFact> aXPreviousInputFacts = null;

        //-------------------------------------------------------------------
        //--Start from the top rules 
        public bool RunMixMode(Window pw, ObservableCollection<XRule> aRules)
        {
            sRunErrorMsg = "";
            if (aRules == null)
            {
                sRunErrorMsg = "Inference Engine Failed!\r\n - No Rules specified";
                return false;
            }

            MainWindow.aXRuleNet.ResetValues();
            MainWindow.tp.UpdateDisplayX(false);

            var expertSystem = new OEngineFBC(MainWindow.aXRuleNet);

            sRunResult = String.Format("\r\n\r\n---Inference Report-------{0}--------------------", DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss"));
            sRunResult += "\r\n-- Rule Network File: " + MainWindow.tp.sRuleFile;
            ObservableCollection<InputFact> inputFacts = new ObservableCollection<InputFact>();
            var inferResults = expertSystem.MixChain(out inputFacts, aPreviousInputFacts);
            if (inferResults == null)
            {
                sRunErrorMsg = "Inference cancelled!";
                return false;
            }
            sRunResult += "\r\n\r\n* Facts:";
            foreach (InputFact a in inputFacts)
            {
                int ifact = XFact.Find(MainWindow.aXRuleNet.aFactList, a.fact);
                if (ifact >= 0)
                {
                    MainWindow.aXRuleNet.aFactList[ifact].Value = a.value;
                    MainWindow.aXRuleNet.aFactList[ifact].Level = a.Level;
                }
                sRunResult += String.Format("\r\n   {0} : {1} : {2}", a.Level, a.fact, a.value);
            }
            sRunResult += "\r\n=> Inference Results:";

            var aResultList = inferResults.OrderByDescending(kv => kv.probability);
            foreach (var result in aResultList)
            {
                int iCon = XConclusion.Find(MainWindow.aXRuleNet.aConclusionList, result.conclusion);
                if (iCon >= 0)
                {
                    MainWindow.aXRuleNet.aConclusionList[iCon].Value = result.value;
                    MainWindow.aXRuleNet.aConclusionList[iCon].CalcProbability = result.probability;
                }
                List<XRule> rList = XRule.FindByConclusion(MainWindow.aXRuleNet.aRuleList, result.conclusion);
                string sMessage = "";
                if (rList != null && rList.Count > 0)
                {
                    foreach (XRule r in rList)
                    {
                        r.Value = result.value;
                        r.CalcProbability = result.probability;
                        if (r.CalcProbability < GlobalVarX.MinProbability) r.Value = 0;
                        else if (!String.IsNullOrEmpty(r.Message))
                        {
                            sMessage += String.Format("\r\n    R: {0} : {1}",r.Name,r.Message);
                            sMessage += String.Format("\r\n{0}", r.GetMessage(r.Message,"      "));
                        }
                    }
                }
                Console.WriteLine($"- {result.conclusion}:  {result.value}, {result.probability:P0}");
                if (result.value > 0)
                {
                    sRunResult += "\r\n" + String.Format($"  - {result.conclusion} : {result.value}, {result.probability:P0}");
                    if(!String.IsNullOrEmpty(sMessage)) sRunResult += sMessage;
                }
            }

            sRunResult += "\r\n\r\n";
            sRunResult += "\r\n* XFacts:";
            foreach (XFact a in MainWindow.aXRuleNet.aFactList)
            {
                sRunResult += "\r\n  " + String.Format("- {0}: {1}: {2}", a.Level, a.fact, a.Value);
            }
            sRunResult += "\r\n* XRules:";
            foreach (XRule a in MainWindow.aXRuleNet.aRuleList)
            {
                sRunResult += "\r\n  " + String.Format("- {0}:{1} | {2} | {3} | {4}", a.ID, a.Name, a.Conclusion, a.Value, a.CalcProbability);
                foreach (XCondition c in a.Conditions)
                {
                    sRunResult += "\r\n    " + String.Format("  {0} | {1} | {2} | {3} : {4} | {5} ", c.Or, c.Operator, c.Condition, c.WantValue, c.InputValue, c.EvaluatedValue);
                }
            }
            sRunResult += "\r\n* XConclusions:";
            foreach (XConclusion a in MainWindow.aXRuleNet.aConclusionList)
            {
                sRunResult += "\r\n  " + String.Format("- {0}: {1} | {2}", a.conclusion, a.Value, a.CalcProbability);
            }

            sRunResult += "\r\n* Trace:";
            sRunResult += "\r\n -- " + DateTime.Now;
            foreach(string a in expertSystem.aTraceList)
            {
                sRunResult += "\r\n" + a;
            }
            WriteResult2File(sRunResult);

            InputFact.SetXFactValues(MainWindow.aXRuleNet.aFactList, inputFacts);
            aPreviousInputFacts = InputFact.CopyList(inputFacts);
            return true;
        }

        //-------------------------------------------------------------------
        //--Start from the top rules 
        public bool AutoTestMixMode(Window pw, ObservableCollection<XRule> aRules)
        {
            sRunErrorMsg = "";
            if (aRules == null)
            {
                sRunErrorMsg = "Inference Engine Failed!\r\n - No Rules specified";
                return false;
            }
            MainWindow.tp.Cursor = Cursors.Wait;
            string sRunResult = "*Rule Network File: " + MainWindow.tp.sRuleFile;
            sRunResult += String.Format("\r\n*Date: {0}", DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss"));

            ObservableCollection<XInputFact> inputFacts = new ObservableCollection<XInputFact>();

            string sOutFileX = MainWindow.tp.sResultFile + ".tracex";
            StreamWriter sw = new StreamWriter(sOutFileX);
            StreamWriter sw2 = new StreamWriter(MainWindow.tp.sResultFile + ".T");

            bool fMore = true;

            var expertSystem = new OEngineFBC(MainWindow.aXRuleNet);

            int iPath = 0;
            while (fMore)
            {
                iPath++;
                MainWindow.aXRuleNet.ResetValues();
 //               MainWindow.tp.UpdateDisplayX(false);
                sw.WriteLine("****Path: " + iPath + " ----------------------------------------------");
                sRunResult = String.Format("\r\n-Path {0:000}", iPath);
                var inferResults = expertSystem.AutoTestMixChain(sw, out inputFacts, aXPreviousInputFacts);
                if (inferResults == null)
                {
                    sRunErrorMsg = "Inference cancelled!";
                    break;
                }
                sRunResult += "\r\n* Facts:";
                foreach (XInputFact a in inputFacts)
                {
                    int ifact = XFact.Find(MainWindow.aXRuleNet.aFactList, a.fact);
                    if (ifact >= 0) MainWindow.aXRuleNet.aFactList[ifact].Value = a.value;
                    sRunResult += String.Format("\r\n   {0} : {1}", a.fact, a.value);
                }
                sRunResult += "\r\n=> Inference Results:";

                var aResultList = inferResults.OrderByDescending(kv => kv.probability);
                foreach (var result in aResultList)
                {
                    int iCon = XConclusion.Find(MainWindow.aXRuleNet.aConclusionList, result.conclusion);
                    if (iCon >= 0)
                    {
                        MainWindow.aXRuleNet.aConclusionList[iCon].Value = result.value;
                        MainWindow.aXRuleNet.aConclusionList[iCon].CalcProbability = result.probability;
                    }
                    List<XRule> rList = XRule.FindByConclusion(MainWindow.aXRuleNet.aRuleList, result.conclusion);
                    string sMessage = "";
                    if (rList != null && rList.Count > 0)
                    {
                        foreach (XRule r in rList)
                        {
                            r.Value = result.value;
                            r.CalcProbability = result.probability;
                            if (r.CalcProbability < GlobalVarX.MinProbability) r.Value = 0;
                            else if (!String.IsNullOrEmpty(r.Message))
                            {
                                sMessage += String.Format("\r\n    R: {0} : {1}", r.Name, r.Message);
                                sMessage += String.Format("\r\n{0}", r.GetMessage(r.Message, "      "));
                            }
                        }
                    }
                    Console.WriteLine($"- {result.conclusion}:  {result.value}, {result.probability:P0}");
                    if (result.value > 0)
                    {
                        sRunResult += "\r\n" + String.Format($"  - {result.conclusion} : {result.value}, {result.probability:P0}");
                        if (!String.IsNullOrEmpty(sMessage)) sRunResult += sMessage;
                    }
                }

                sRunResult += "\r\n\r\n";
                sRunResult += "\r\n* XFacts:";
                foreach (XFact a in MainWindow.aXRuleNet.aFactList)
                {
                    sRunResult += "\r\n  " + String.Format("- {0}: {1}", a.fact, a.Value);
                }
                sRunResult += "\r\n* XRules:";
                foreach (XRule a in MainWindow.aXRuleNet.aRuleList)
                {
                    sRunResult += "\r\n  " + String.Format("- {0}:{1} | {2} | {3} | {4}", a.ID, a.Name, a.Conclusion, a.Value, a.CalcProbability);
                    foreach (XCondition c in a.Conditions)
                    {
                        sRunResult += "\r\n    " + String.Format("  {0} | {1} | {2} | {3} : {4} | {5} ", c.Or, c.Operator, c.Condition, c.WantValue, c.InputValue, c.EvaluatedValue);
                    }
                }
                sRunResult += "\r\n* XConclusions:";
                foreach (XConclusion a in MainWindow.aXRuleNet.aConclusionList)
                {
                    sRunResult += "\r\n  " + String.Format("- {0}: {1} | {2}", a.conclusion, a.Value, a.CalcProbability);
                }

                sRunResult += "\r\n* Trace:";
                sRunResult += String.Format("\r\n Path {0} -- {1}",iPath, DateTime.Now);
                foreach (string a in expertSystem.aTraceList)
                {
                    sRunResult += "\r\n" + a;
                }
                sw2.WriteLine(sRunResult);

                XInputFact.SetXFactValues(MainWindow.aXRuleNet.aFactList, inputFacts);
                  //--prepared inputfact for next run
                aXPreviousInputFacts = XInputFact.CopyList(inputFacts);
                if (aXPreviousInputFacts.Count > 0)
                {
                    fMore = false;
                    for (int i = aXPreviousInputFacts.Count - 1; i >= 0; i--)
                    {
                        int ix = XInputFact.SetNextWorkingOption(aXPreviousInputFacts[i]);
                        if (ix >= 0)
                        {
                            fMore = true;
                            break;
                        }
                    }
                }
            }
            sw.Close();
            sw2.Close();
            string sMsg = "Total paths explored: " + iPath;
            MessageBox.Show(sMsg, "Completed", MessageBoxButton.OK, MessageBoxImage.Information);
            MainWindow.tp.Cursor = Cursors.Arrow;
            MainWindow.tp.UpdateDisplay(false);
            return true;
        }

        //-------------------------------------------------------------------
        public bool Run(Window pw, ObservableCollection<XRule> aRules, ObservableCollection<InputFact> aInputFacts, string aGoal, bool fUseInputDlg)
        {
            sRunErrorMsg = "";
            if (aRules == null)
            {
                sRunErrorMsg = "Inference Engine Failed!\r\n - No Rules specified";
                return false;
            }

            if (aPreviousInputFacts == null)
            {
                aPreviousInputFacts = InputFact.FromXFacts(MainWindow.aXRuleNet.aFactList, MainWindow.aXRuleNet.aRuleList);
            }
            if (aInputFacts == null)
            {
                aInputFacts = aPreviousInputFacts;
            }
            string goal = aGoal;
            if (String.IsNullOrEmpty(aGoal)) goal = "Flu";

            if (fUseInputDlg)
            {
                FactInputDlg dlg = new FactInputDlg(aInputFacts, goal, true);
                dlg.Owner = pw;
                dlg.ShowDialog();
                if (dlg.DialogResult == false)
                {
                    sRunErrorMsg = "Inference Engine Failed!\r\n - Fact Inputs cancelled!";
                    return false;
                }
                goal = dlg.Goal;
            }

            MainWindow.aXRuleNet.ResetValues();

            var expertSystem = new OEngineFBC(MainWindow.aXRuleNet);

            sRunResult = String.Format("---Inference Report-------{0}--------------------", DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss"));
            sRunResult += "\r\n-- RuleNetwork File: " + MainWindow.tp.sRuleFile;
            sRunResult += "\r\n\r\n* Facts:";
            foreach (InputFact a in aInputFacts)
            {
                int ifact = XFact.Find(MainWindow.aXRuleNet.aFactList, a.fact);
                if (ifact >= 0) MainWindow.aXRuleNet.aFactList[ifact].Value = a.value;
                sRunResult += String.Format("\r\n   {0} : {1}", a.fact, a.value);
            }
            sRunResult += "\r\n=> Forward Chaining Results:";

            var forwardResults = expertSystem.ForwardChain(aInputFacts);
            foreach (var result in forwardResults.OrderByDescending(kv => kv.Value))
            {
                int icon = XConclusion.Find(MainWindow.aXRuleNet.aConclusionList, result.Key);
                if (icon >= 0) MainWindow.aXRuleNet.aConclusionList[icon].Value = (result.Value > 0 ? 1 : 0);
                List<XRule> rList = XRule.FindByConclusion(MainWindow.aXRuleNet.aRuleList, result.Key);
                if (rList != null && rList.Count > 0)
                {
                    foreach (XRule r in rList)
                    {
                        r.Value = 1;
                    }
                }
                Console.WriteLine($"- {result.Key}: {result.Value:P0}");
                sRunResult += "\r\n  " + String.Format($"- {result.Key}: {result.Value:P0}");
            }

            Console.WriteLine("\r\n\r\n=> Backward Chaining for '{0}':", goal);
            sRunResult += String.Format("\r\n=> Backward Chaining for '{0}':", goal);

            var (backwardResult, probability) = expertSystem.BackwardChain(goal, aInputFacts);
            List<XRule> aGoalRules = XRule.FindByConclusion(MainWindow.aXRuleNet.aRuleList, goal);
            foreach (XRule a in aGoalRules)
            {
                if (probability > 0) a.Value = 1;
                else a.Value = 0;
            }

            int iCon = XConclusion.Find(MainWindow.aXRuleNet.aConclusionList, goal);
            if (iCon >= 0)
            {
                if (probability > 0) MainWindow.aXRuleNet.aConclusionList[iCon].Value = 1;
                else MainWindow.aXRuleNet.aConclusionList[iCon].Value = 0;
            }

            Console.WriteLine($"- {goal}: {(backwardResult ? probability.ToString("P0") : "Not concluded")}");
            sRunResult += "\r\n  " + String.Format($"- {goal}: {(backwardResult ? probability.ToString("P0") : "Not concluded")}");
            sRunResult += "\r\n\r\n";
            sRunResult += "\r\n* XFacts:";
            foreach (XFact a in MainWindow.aXRuleNet.aFactList)
            {
                sRunResult += "\r\n  " + String.Format("- {0}: {1}", a.fact, a.Value);
            }
            sRunResult += "\r\n* XRules:";
            foreach (XRule a in MainWindow.aXRuleNet.aRuleList)
            {
                sRunResult += "\r\n  " + String.Format("- Rule {0}: {1}", a.ID, a.Value);
            }
            sRunResult += "\r\n* XConclusions:";
            foreach (XConclusion a in MainWindow.aXRuleNet.aConclusionList)
            {
                sRunResult += "\r\n  " + String.Format("- {0}: {1}", a.conclusion, a.Value);
            }
            WriteResult2File(sRunResult);
            InputFact.SetXFactValues(MainWindow.aXRuleNet.aFactList, aInputFacts);
            aPreviousInputFacts = InputFact.CopyList(aInputFacts);
            return true;
        }

        //-------------------------------------------------------------------
        public void WriteResult2File(string sResult)
        {
            if (String.IsNullOrEmpty(MainWindow.tp.sResultFile)) return;
            RunHistoryFile.OpenResultFile(MainWindow.tp.sResultFile, true);
            RunHistoryFile.WriteLine(sResult);
            RunHistoryFile.CloseResultFile();
        }

        //-------------------------------------------------------------------
        public void Run0(XRuleNet aRuleNet, List<InputFact> aInputFacts, string goal)
        {
            if (aRuleNet == null || aInputFacts == null) return;

            var expertSystem = new OEngineFBC(aRuleNet);

            Dictionary<string, string> facts = aInputFacts.ToDictionary(x => x.fact, x => x.value);

            Console.WriteLine("Forward Chaining Results:");
            var forwardResults = expertSystem.ForwardChain0(facts);
            foreach (var result in forwardResults.OrderByDescending(kv => kv.Value))
            {
                Console.WriteLine($"- {result.Key}: {result.Value:P0}");
            }

            Console.WriteLine("\nBackward Chaining for '{0}':", goal);
            var (backwardResult, probability) = expertSystem.BackwardChain0(goal, facts);
            Console.WriteLine($"- {goal}: {(backwardResult ? probability.ToString("P0") : "Not concluded")}");
        }
    }
}
