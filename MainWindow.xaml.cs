using Microsoft.Win32;
using Newtonsoft.Json;
using OExpert.UILibraryWPF;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Data.Sql;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace OExpert
{
    public enum OperMode
    {
        None,
        Move,
        Resize,
        Individual,
        Connect,
        Run
    }

    //---------------------------------------------------------------------------------
    public partial class MainWindow : Window
    {
        public static MainWindow tp;

        public string sRuleFile;

        public static XRuleNet aXRuleNet = new XRuleNet();
        public static XRuleNet aXRuleNetBack = null;

        public string sAppDir = Directory.GetCurrentDirectory();
        public string sLogDir = "Logs";
        public string sLogFile;
        public string sResultDir = "Results";  //--run result directory
        public string sResultFile;

        public List<XRule> aCopyRuleList = new List<XRule>();
        public List<XResult> aXResultList = null;
        public List<XResultTest> aXResultTestList = null;

        public bool fChanged = false;

        public bool fShowRule = true;
        public bool fShowFact = true;
        public bool fShowConclusion = true;
        public bool fShowRuleLink = true;
        public bool fShowFactLink = true;
        public bool fShowConclusionLink = true;

        public bool fShowToolTip = true;

        public ItemInfoDlg wItemInfoDlg = null;
        public LastRunResultDlg wLastRunResultDlg = null;
        public RunResultsDlg wRunResultsDlg = null;
        public ResultTestDlg wResultTestDlg = null;

        public XTcpServer aTcpServer = null;
        public int iTcpServerPort = 8080;
        public int iTcpChineseMDPort = 8888;
        public XTcpClient aTcpClient = new XTcpClient();

        //-----------------------------------------------------------
        public MainWindow()
        {
            InitializeComponent();
        }

        //-----------------------------------------------------------
        public MainWindow(string _ruleFile)
        {
            InitializeComponent();
            sRuleFile = _ruleFile;
        }

        //-----------------------------------------------------------
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            tp = this;
            Console.WriteLine("1");
            if (ConfigurationManager.AppSettings["TcpServerPort"] != null)
                iTcpServerPort = Int32.Parse(ConfigurationManager.AppSettings["TcpServerPort"].ToString());
            if (ConfigurationManager.AppSettings["TcpChineseMDPort"] != null)
                iTcpChineseMDPort = Int32.Parse(ConfigurationManager.AppSettings["TcpChineseMDPort"].ToString());

            if (!String.IsNullOrEmpty(ConfigurationManager.AppSettings["ResultDir"].ToString())) sResultDir = ConfigurationManager.AppSettings["ResultDir"].ToString();
            if (!String.IsNullOrEmpty(ConfigurationManager.AppSettings["LogDir"].ToString())) sLogDir = ConfigurationManager.AppSettings["LogDir"].ToString();
            sResultDir = WHelper.ValidateDir(sResultDir, true);
            sLogDir = WHelper.ValidateDir(sLogDir, true);
            sAppDir = Directory.GetCurrentDirectory();
            sLogDir = sAppDir + "\\Logs";
            sLogFile = sLogDir + "\\OExpert_" + DateTime.Now.ToString("yyyyMMdd-HHmmss") + ".log";
            LogFile.OpenLogFile(sLogFile, false, false);
            LogFile.pWriteListener = null;
            LogFile.aListener = null;
            LogFile.aStepLB = null;
            LogFile.aListenerMessage = null;

            if (!String.IsNullOrEmpty(sRuleFile))
            {
                LogFile.WriteLine("Open Rule File: " + sRuleFile); 
                bool rtn = LoadRuleFile(sRuleFile);
                if (!rtn) LogFile.WriteLine("Read Rule File failed!");
                else LogFile.WriteLine("Read Rule File successful!");
            }
            aTcpServer = new XTcpServer();
            aTcpServer.StartServer(iTcpServerPort);
        }

        //----------------------------------------------------------
        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            if (e.Key == Key.Escape)
            {
            }
        }

        //----------------------------------------------------------
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            string sName = "";
            if (btn != null) sName = btn.Name;
            else
            {
                MenuItem mi = sender as MenuItem;
                if (mi != null) sName = mi.Name;
            }
            switch (sName)
            {
                case "btnSave":
                case "mnuSave":
                    if (String.IsNullOrEmpty(sRuleFile)) SaveAsRuleFile();
                    else SaveRuleFile();
                    break;
                case "btnSaveAs":
                case "mnuSaveAs":
                    SaveAsRuleFile();
                    break;

                case "mnuOpenFile":
                case "btnOpenFile":
                    OpenFileDialog dlg0 = new OpenFileDialog();
                    //                    if (!String.IsNullOrEmpty(sRuleFile)) dlg0.InitialDirectory = System.IO.Path.GetDirectoryName(sRuleFile);
                    //                    else dlg0.InitialDirectory = Directory.GetCurrentDirectory();
                    dlg0.Filter = "Json files(*.json)|*.json|Rule Network files(*.rn)|*.rn|All files(*.*)|*.*";
                    if (dlg0.ShowDialog() == true)
                    {
                        LoadRuleFile(dlg0.FileName);
                    }
                    break;

                case "mnuNew":
                case "btnNew":
                    if (TrySave() < 0) break;  //--Cancel
                    fChanged = false;
                    sRuleFile = "";
                    labelFile.Content = sRuleFile;
                    aXRuleNet = new XRuleNet();
                    sResultFile = "";
                    UpdateDisplayX(true);
                    break;
                case "btnEdit":
                    if (dgRule.SelectedItem != null)
                    {
                        EditRule(dgRule.SelectedItem as XRule);
                    }
                    break;
                case "btnCopy":
                    if (aXRuleNet.aSelectedObjectList != null) aXRuleNet.aSelectedObjectList.Clear();

                    if (dgRule.SelectedItems != null)
                    {
                        if (aCopyRuleList == null) aCopyRuleList = new List<XRule>();
                        aCopyRuleList.Clear();
                        foreach (XRule a in dgRule.SelectedItems)
                        {
                            aXRuleNet.aSelectedObjectList.Add(a);
                        }
                        CopyRules();
                    }
                    break;
                case "btnPaste":
                    PasteRules();
                    break;
                case "btnAdd":
                    AddRule();
                    break;
                case "btnDelete":
                    if (dgRule.SelectedItem != null)
                    {
                        XRule a = dgRule.SelectedItem as XRule;
                        if (MessageBox.Show("Are you sure of Deleting Selected Rule:\r\n" + a.toStr(), "Confirmation",
                            MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                        {
                            aXRuleNetBack = XRuleNet.Copy(aXRuleNet);
                            aXRuleNet.aRuleList.RemoveAt(dgRule.SelectedIndex);
                            UpdateDisplay(false);
                        }
                    }
                    break;
                case "btnRun":
                case "mnuAutoTests":
                    //                 RunEngine(this);
                    RunEngineAutoTestMix(this);
                    break;
                case "mnuResultTest":
                case "btnResultTest":
                    ShowTestResults();
                    break;
                case "btnRunMix":
                case "mnuRunMix":
                    RunEngineMix(this);
                    break;
                case "mnuLastRunResult":
                case "btnLastRunResult":
                    ShowLastRunResult();
                    break;
                case "mnuRunResult":
                case "btnRunResult":
                    ShowRunResults();
                    break;
                case "mnuExit":
                case "btnExit":
                    this.Close();
                    break;
                case "mnuViewResultNotepad":
                case "btnResult":
                    if (String.IsNullOrEmpty(sResultFile))
                    {
                        MessageBox.Show("No Inferencing Results!\r\n Need to load rule network and run first!");
                    }
                    else
                    {
                        (new XWinExec()).RunExternal(null, "Notepad.exe", sResultFile, "Notepad", "Notepad", 0, false, null);
                    }
                    break;
                default:
                    break;
            }
        }

        //------------------------------------------------------------------
        public void UpdateDisplay(bool fSetCoord)
        {
            aXRuleNet.SetupNetwork(fSetCoord);
            UpdateDisplayX(true);
        }

        //------------------------------------------------------------------
        public void UpdateDisplayX(bool fResult)
        {
            if (aXRuleNet == null) return;
            labelTotal.Content = String.Format("# of Rules: {0}", (aXRuleNet.aRuleList == null ? 0 : aXRuleNet.aRuleList.Count));
            dgRule.ItemsSource = null;
            ucRuleNetwork.UpdateControl();
            dgRule.ItemsSource = aXRuleNet.aRuleList;
            ucRuleTree.UpdateControl();
            aXResultList = XResult.ReadRunResults(sResultFile);
            aXResultTestList = XResultTest.ReadRunResultTests(sResultFile + ".T");

            if (fResult)
            {
                if (wRunResultsDlg != null) wRunResultsDlg.ucResult.LoadResult();
            }
        }

        //------------------------------------------------------------------
        public void RunEngine(Window pw)
        {
            if (aXRuleNet.aRuleList == null || aXRuleNet.aRuleList.Count <= 0) return;
            OEngine ex = (new OEngine());
            if (ex.Run(pw, aXRuleNet.aRuleList, null, null, true))
            {
                ucRuleNetwork.UpdateControl();
                ucRuleTree.UpdateControl();
                ShowLastRunResult();
            }
            else
            {
                MessageBox.Show(OEngine.sRunErrorMsg, "Information", MessageBoxButton.OK, MessageBoxImage.Stop);
            }
        }

        //------------------------------------------------------------------
        //--Start from top then go through the linked
        public void RunEngineMix(Window pw)
        {
            if (aXRuleNet.aRuleList == null || aXRuleNet.aRuleList.Count <= 0) return;
            OEngine ex = (new OEngine());
            if (ex.RunMixMode(pw, aXRuleNet.aRuleList))
            {
                ucRuleNetwork.UpdateControl();
                ucRuleTree.UpdateControl();
                ShowLastRunResult();
            }
            else
            {
                MessageBox.Show(OEngine.sRunErrorMsg, "Information", MessageBoxButton.OK, MessageBoxImage.Stop);
            }
        }

        //------------------------------------------------------------------
        //--Start from top then go through the linked
        public void RunEngineAutoTestMix(Window pw)
        {
            if (aXRuleNet.aRuleList == null || aXRuleNet.aRuleList.Count <= 0) return;
            OEngine ex = (new OEngine());
            if (ex.AutoTestMixMode(pw, aXRuleNet.aRuleList))
            {
                ucRuleNetwork.UpdateControl();
                ucRuleTree.UpdateControl();
            }
            else
            {
                MessageBox.Show(OEngine.sRunErrorMsg, "Information", MessageBoxButton.OK, MessageBoxImage.Stop);
            }
        }

        //------------------------------------------------------------------
        public void ShowLastRunResult()
        {
            aXResultList = XResult.ReadRunResults(sResultFile);

            if (MainWindow.tp.aXResultList == null || MainWindow.tp.aXResultList.Count < 1) return;
            if (wLastRunResultDlg == null)
            {
                wLastRunResultDlg = new LastRunResultDlg(MainWindow.tp.aXResultList[0]);
                wLastRunResultDlg.Owner = this;
                wLastRunResultDlg.Show();
            }
            else
            {
                wLastRunResultDlg.ShowRunResult(MainWindow.tp.aXResultList[0]);
                wLastRunResultDlg.Activate();
            }
        }

        //------------------------------------------------------------------
        public void ShowTestResults()
        {
            if (wResultTestDlg == null)
            {
                wResultTestDlg = new ResultTestDlg();
                wResultTestDlg.Owner = this;
                wResultTestDlg.Show();
            }
            else
            {
                wResultTestDlg.UpdateDisplayX();
                wResultTestDlg.Activate();
            }
        }

        //------------------------------------------------------------------
        public void ShowRunResults()
        {
            aXResultList = XResult.ReadRunResults(sResultFile);
            if (wRunResultsDlg == null)
            {
                wRunResultsDlg = new RunResultsDlg();
                wRunResultsDlg.Owner = this;
                wRunResultsDlg.Show();
            }
            else
            {
                wRunResultsDlg.UpdateDisplayX();
                wRunResultsDlg.Activate();
            }
        }

        //-------------------------------------------------------
        public void ApplyToDisplay(XResult r)
        {
            if (r == null) return;
            if (r.aXFactList == null) return;
            if (OEngine.aPreviousInputFacts == null) OEngine.aPreviousInputFacts = new ObservableCollection<InputFact>();
            else OEngine.aPreviousInputFacts.Clear();
            foreach (XFact a in r.aXFactList)
            {
                int ix = XFact.Find(MainWindow.aXRuleNet.aFactList, a.fact);
                if (ix >= 0)
                {
                    MainWindow.aXRuleNet.aFactList[ix].Value = a.Value;
                    OEngine.aPreviousInputFacts.Add(InputFact.FromXFact(MainWindow.aXRuleNet.aFactList[ix], MainWindow.aXRuleNet.aRuleList));
                }
            }
            foreach (XRule a in r.aXRuleList)
            {
                int ix = XRule.Find(MainWindow.aXRuleNet.aRuleList, a.ID);
                if (ix >= 0)
                {
                    MainWindow.aXRuleNet.aRuleList[ix].Value = a.Value;
                    MainWindow.aXRuleNet.aRuleList[ix].CalcProbability = a.CalcProbability;
                    if (a.Conditions != null)
                    {
                        foreach (XCondition c in a.Conditions)
                        {
                            XCondition cx = XCondition.Find(MainWindow.aXRuleNet.aRuleList[ix].Conditions, c.Condition);
                            if (cx != null)
                            {
                                cx.InputValue = c.InputValue;
                                cx.EvaluatedValue = c.EvaluatedValue;
                            }
                        }
                    }
                }
            }
            foreach (XConclusion a in r.aXConclusionList)
            {
                int ix = XConclusion.Find(MainWindow.aXRuleNet.aConclusionList, a.conclusion);
                if (ix >= 0)
                {
                    MainWindow.aXRuleNet.aConclusionList[ix].Value = a.Value;
                    MainWindow.aXRuleNet.aConclusionList[ix].CalcProbability = a.CalcProbability;
                }
            }
            MainWindow.tp.UpdateDisplayX(false);
            MessageBox.Show("Last run result applied to display", "Completed", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        //-------------------------------------------------------
        public void ApplyTestToDisplay(XResultTest r)
        {
            if (r == null) return;
            if (r.aXFactList == null) return;
            if (OEngine.aPreviousInputFacts == null) OEngine.aPreviousInputFacts = new ObservableCollection<InputFact>();
            else OEngine.aPreviousInputFacts.Clear();
            foreach (XFact a in r.aXFactList)
            {
                int ix = XFact.Find(MainWindow.aXRuleNet.aFactList, a.fact);
                if (ix >= 0)
                {
                    MainWindow.aXRuleNet.aFactList[ix].Value = a.Value;
                    OEngine.aPreviousInputFacts.Add(InputFact.FromXFact(MainWindow.aXRuleNet.aFactList[ix], MainWindow.aXRuleNet.aRuleList));
                }
            }
            foreach (XRule a in r.aXRuleList)
            {
                int ix = XRule.Find(MainWindow.aXRuleNet.aRuleList, a.ID);
                if (ix >= 0)
                {
                    MainWindow.aXRuleNet.aRuleList[ix].Value = a.Value;
                    MainWindow.aXRuleNet.aRuleList[ix].CalcProbability = a.CalcProbability;
                    if (a.Conditions != null)
                    {
                        foreach (XCondition c in a.Conditions)
                        {
                            XCondition cx = XCondition.Find(MainWindow.aXRuleNet.aRuleList[ix].Conditions, c.Condition);
                            if (cx != null)
                            {
                                cx.InputValue = c.InputValue;
                                cx.EvaluatedValue = c.EvaluatedValue;
                            }
                        }
                    }
                }
            }
            foreach (XConclusion a in r.aXConclusionList)
            {
                int ix = XConclusion.Find(MainWindow.aXRuleNet.aConclusionList, a.conclusion);
                if (ix >= 0)
                {
                    MainWindow.aXRuleNet.aConclusionList[ix].Value = a.Value;
                    MainWindow.aXRuleNet.aConclusionList[ix].CalcProbability = a.CalcProbability;
                }
            }
            MainWindow.tp.UpdateDisplayX(false);
            //           MessageBox.Show("Run result applied to display", "Completed", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        //------------------------------------------------------------------
        public void SaveAsRuleFile()
        {
            SaveFileDialog dlg1 = new SaveFileDialog();
            dlg1.InitialDirectory = Directory.GetCurrentDirectory();
            dlg1.Filter = "Json files(*.json)|*.json|Rule files(*.rn)|*.rn|All files(*.*)|*.*";
            if (dlg1.ShowDialog() == true)
            {
                fChanged = false;
                sRuleFile = dlg1.FileName;
                SaveRuleFile();
            }
        }

        //------------------------------------------------------------------
        public void SaveRuleFile()
        {
            if (String.IsNullOrEmpty(sRuleFile)) return;
            RuleNet x = XRuleNet.ToRuleNet(aXRuleNet);

            RuleNet.WriteFile(sRuleFile, x);
            fChanged = false;
            MessageBox.Show("Successfully Save Rule Network to \r\n" + sRuleFile, "Information", MessageBoxButton.OK, MessageBoxImage.Stop);
        }

        //------------------------------------------------------------------
        public bool LoadRuleFile(string sFile)
        {
            if (String.IsNullOrEmpty(sFile)) return false;
            if (!File.Exists(sFile))
            {
                MessageBox.Show("Rule Network does not exist! \r\n" + sRuleFile, "ERROR", MessageBoxButton.OK, MessageBoxImage.Stop);
                return false;
            }
            sRuleFile = sFile;
            labelFile.Content = sRuleFile;
            sResultFile = sResultDir + System.IO.Path.GetFileName(sFile) + ".result";

            /*
            if (File.Exists(sResultFile))
            {
                ucResult.LoadResult(sResultFile);
                ucTestResult.LoadResult(sResultFile+".T");
            }
            else
            {
                OEngine.sRunResult = "";
            }
            */
            aXRuleNet = new XRuleNet();
            sRuleFile = sFile;
            aXRuleNet.Name = sFile;
            fChanged = false;

            RuleNet x = RuleNet.ReadFile(sFile);
            aXRuleNet = XRuleNet.FromRuleNet(x);

            OEngine.sRunErrorMsg = "";
            OEngine.aPreviousInputFacts = null;
            OEngine.sRunResult = "";

            if (wLastRunResultDlg != null) wLastRunResultDlg.Close();
            if (wItemInfoDlg != null) wItemInfoDlg.Close();

            labelTotal.Content = String.Format("# of Rules: {0}", (aXRuleNet == null || aXRuleNet.aRuleList == null ? 0 : aXRuleNet.aRuleList.Count));
            UpdateDisplayX(true);
            return true;
        }

        //------------------------------------------------------------------
        public XRule AddRule()
        {
            RuleInfoDlg dlg = new RuleInfoDlg();
            dlg.Owner = this;
            dlg.ShowDialog();
            if (dlg.DialogResult == true)
            {
                aXRuleNetBack = XRuleNet.Copy(aXRuleNet);

                dgRule.ItemsSource = null;
                if (aXRuleNet.aRuleList == null) aXRuleNet.aRuleList = new ObservableCollection<XRule>();
                aXRuleNet.aRuleList.Add(dlg.aRule);
                XRuleNet.Resolve(aXRuleNet);
                fChanged = true;
                UpdateDisplay(false);
                return dlg.aRule;
            }
            return null;
        }

        //------------------------------------------------------------------
        public void CopyRules()
        {
            aCopyRuleList = new List<XRule>();
            if (aXRuleNet != null && aXRuleNet.aSelectedObjectList != null)
            {
                foreach (object a in aXRuleNet.aSelectedObjectList)
                {
                    if (a.GetType() == typeof(XRule)) aCopyRuleList.Add((XRule)a);
                }
            }
        }

        //------------------------------------------------------------------
        public void PasteRules()
        {
            if (aCopyRuleList == null || aCopyRuleList.Count <= 0)
            {
                MessageBox.Show("Please make copy of rules first!\r\nThe selected may not be Rules", "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            aXRuleNetBack = XRuleNet.Copy(aXRuleNet);

            if (aXRuleNet.aSelectedObjectList != null) aXRuleNet.aSelectedObjectList.Clear();
            foreach (XRule aRule in aCopyRuleList)
            {
                XRule x = XRule.Copy(aRule);
                x.ID = aXRuleNet.GetNextRuleID();
                x.X += 12;
                x.Y += 12;
                if (aXRuleNet.aRuleList == null) aXRuleNet.aRuleList = new ObservableCollection<XRule>();
                aXRuleNet.aRuleList.Add(x);
                aXRuleNet.aSelectedObjectList.Add(x);
            }
            XRuleNet.Resolve(aXRuleNet);
            fChanged = true;
            UpdateDisplay(false);
        }

        //------------------------------------------------------------------
        public void EditRule(XRule aRule)
        {
            RuleInfoDlg dlg = new RuleInfoDlg(null, aRule, false);
            dlg.Owner = this;
            dlg.ShowDialog();
            if (dlg.DialogResult == true)
            {
                aXRuleNetBack = XRuleNet.Copy(aXRuleNet);
                int isel = XRule.Find(aXRuleNet.aRuleList, aRule.ID);
                if (isel >= 0)
                {
                    dgRule.ItemsSource = null;
                    aXRuleNet.aRuleList[isel] = dlg.aRule;
                }
                else
                {
                    dlg.aRule.ID = aXRuleNet.GetNextRuleID();
                    aXRuleNet.aRuleList.Add(dlg.aRule);
                }
                XRuleNet.Resolve(aXRuleNet);
                fChanged = true;
                UpdateDisplay(false);
            }
        }

        //-------------------------------------------------------------------------------------------------
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (TrySave() < 0) return;
            LogFile.CloseLogFile();
        }

        //-------------------------------------------------------------------------------------------------
        public int TrySave()
        {
            if (!fChanged) return 0;
            MessageBoxResult x = MessageBox.Show("Rule Network changed!\r\nDo you want to save before exit?", "WARNING", MessageBoxButton.YesNoCancel);
            if (x == MessageBoxResult.Yes)
            {
                if (String.IsNullOrEmpty(sRuleFile)) SaveAsRuleFile();
                else SaveRuleFile();
                return 1;
            }
            else if (x == MessageBoxResult.Cancel)  //--Cancel Closing
            {
                return -1;
            }
            else return 0;  //--No save, continue;
        }

        //-------------------------------------------------------------------------------------------------
        private void dgRule_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (dgRule.SelectedItem == null) return;
            EditRule(dgRule.SelectedItem as XRule);
        }

        //-------------------------------------------------------------------------------------------------
        private void tbFind_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                FindRuleInList();
            }
        }

        //-------------------------------------------------------------------------------------------------
        public void FindRuleInList()
        {
            if (String.IsNullOrEmpty(tbFind.Text)) return;
            int isel = dgRule.SelectedIndex;
            dgRule.SelectedIndex = -1;
            if (isel < 0) isel = 0;
            else isel++;
            string sFindU = tbFind.Text.ToUpper();
            for (int i = isel; i < aXRuleNet.aRuleList.Count; i++)
            {
                if (aXRuleNet.aRuleList[i].Name.ToUpper().IndexOf(sFindU) >= 0 ||
                    aXRuleNet.aRuleList[i].Conclusion.ToUpper().IndexOf(sFindU) >= 0)
                {
                    dgRule.SelectedIndex = i;
                    return;
                }
                if (aXRuleNet.aRuleList[i].Conditions != null && aXRuleNet.aRuleList[i].Conditions.Count > 0)
                {
                    foreach (XCondition c in aXRuleNet.aRuleList[i].Conditions)
                    {
                        if (c.Operator.ToUpper().IndexOf(sFindU) >= 0 ||
                            c.Condition.ToUpper().IndexOf(sFindU) >= 0)
                        {
                            dgRule.SelectedIndex = i;
                            return;
                        }
                    }
                }
            }
            dgRule.SelectedIndex = -1;
        }

        //--------------------------------------------------------------------------
        //--Action of the TextAction Control by bring up the associated conrol page
        //--The target parent is the TextBlock of the clicked text
        //--Main->XContent is the parent that can bring the dialog window properly
        //--------------------------------------------------------------------------
        public void ActionTextAction(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            try
            {
                TextBlock tb = sender as TextBlock;
                SendTcpMessage(tb.Tag.ToString());
            }
            catch (Exception ex)
            {
                MessageBox.Show("ERROR: " + ex.ToString());
            }
        }

        //--------------------------------------------------------------------------
        public async void SendTcpMessage(string sMsg)
        {
            XTcpClient _client = new XTcpClient();
            string response = await _client.SendMessage("localhost", iTcpChineseMDPort, sMsg);
        }

        //--------------------------------------------------------------------------
        //--Action of the TextAction Control by bring up the associated conrol page
        //--The target parent is the TextBlock of the clicked text
        //--Main->XContent is the parent that can bring the dialog window properly
        //--------------------------------------------------------------------------
        public void ActionTextActionX(object sender)
        {
            try
            {
                TextBlock tb = sender as TextBlock;
                string[] substr = tb.Tag.ToString().Split(':');
                string sObj = substr[0].Trim();
                int iIdx = Int32.Parse(substr[1].Trim());
                if (sObj.Equals("X"))
                {
                    /*
                    if (aPopupDataDlg != null && aPopupDataDlg.IsOpen == true && aAcupunctureDataPage != null)
                    {
                    }
                    else
                     */
                    /*
                    {
                        string smatch = "(" + iIdx + ")";
                        string wType = this.xContent.Child.GetType().ToString();
                        string sstr = "";
                        if (wType.IndexOf("Tree") >= 0)
                        {
                            AcupunctureTree_Page ax = this.xContent.Child as AcupunctureTree_Page;
                            sstr = ax.AcupunctureData.AcuPathEachData.tbJixFootNote.Text;
                        }
                        else if (wType.IndexOf("Query") >= 0)
                        {
                            AcupunctureQuery_Page ax = this.xContent.Child as AcupunctureQuery_Page;
                            sstr = ax.AcupunctureDataCtrl.AcuPathEachData.tbJixFootNote.Text;
                        }
                        else
                        {
                            AcupunctureData_Page ax = this.xContent.Child as AcupunctureData_Page;
                            sstr = ax.AcupunctureDataCtrl.AcuPathEachData.tbJixFootNote.Text;
                        }
                        int ib = sstr.IndexOf(smatch);
                        int ie = sstr.IndexOf('(', ib + smatch.Length);
                        if (ie < 0) ie = sstr.Length;
                        string sFootNote = XStr.RemoveDBTag(sstr.Substring(ib, ie - ib).Replace("\r\n", ""));

                        WHelper.ShowFootNoteDlg(sender, -1, -1, 400, 120, RS_ChineseMDS.RS_Explain, sFootNote);
                    }
                    */
                }
                else if (sObj.Equals("X2"))
                {
                    /*
                    if (aPopupDataDlg != null && aPopupDataDlg.IsOpen == true && aAcupunctureDataPage != null)
                    {
                    }
                    else
                    */
                    /*
                    {
                        string smatch = "(" + iIdx + ")";
                        string wType = this.xContent.Child.GetType().ToString();
                        string sstr = "";
                        if (wType.IndexOf("Tree") >= 0)
                        {
                            AcupunctureTree_Page ax = this.xContent.Child as AcupunctureTree_Page;
                            sstr = ax.AcupunctureData.AcuPathEachData.tbJixFootNote2.Text;
                        }
                        else if (wType.IndexOf("Query") >= 0)
                        {
                            AcupunctureQuery_Page ax = this.xContent.Child as AcupunctureQuery_Page;
                            sstr = ax.AcupunctureDataCtrl.AcuPathEachData.tbJixFootNote2.Text;
                        }
                        else
                        {
                            AcupunctureData_Page ax = this.xContent.Child as AcupunctureData_Page;
                            sstr = ax.AcupunctureDataCtrl.AcuPathEachData.tbJixFootNote2.Text;
                        }
                        int ib = sstr.IndexOf(smatch);
                        int ie = sstr.IndexOf('(', ib + smatch.Length);
                        if (ie < 0) ie = sstr.Length;
                        string sFootNote = XStr.RemoveDBTag(sstr.Substring(ib, ie - ib).Replace("\r\n", ""));

                        WHelper.ShowFootNoteDlg(sender, -1, -1, 400, 120, RS_ChineseMDS.RS_Explain2, sFootNote);
                    }
                    }
                    else
                    {
                    ShowDataDialog(sObj, iIdx);
                    }
                    */
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("ERROR: " + ex.ToString());
            }
        }

        //------------------------------------------------------------
        public string RunServerAction(string sRequest)
        {
            Console.WriteLine("RUN: " + sRequest);
            string[] subx = sRequest.Split('|');
            string sCmd = subx[0].Trim();
            string sFile = "";
            if (subx.Length > 1) sFile = subx[1].Trim();
            string sResponse = "";
            switch (sCmd)
            {
                case "Load":
                    LoadRuleFile(sFile);
                    sResponse = sFile + " Loaded";
                    break;
                case "Run":
                    if (!String.IsNullOrEmpty(sFile)) LoadRuleFile(sFile);
                    RunEngineMix(this);
                    ShowLastRunResult();
                    aXResultList = XResult.ReadRunResults(sResultFile);
                    if (aXResultList == null || aXResultList.Count < 1) sResponse = "No data received";
                    sResponse = JsonConvert.SerializeObject(aXResultList[0].sSummary, Newtonsoft.Json.Formatting.Indented);
                    break;
                case "RunTests":
                    if (!String.IsNullOrEmpty(sFile)) LoadRuleFile(sFile);
                    RunEngineAutoTestMix(this);
                    if (aXResultTestList != null) sResponse = XResultTest.ToStrX(aXResultTestList);
                    else sResponse = "Test cases is Empty";
                    break;
                case "ShowLastResult":
                    if (!String.IsNullOrEmpty(sFile)) LoadRuleFile(sFile);
                    ShowLastRunResult();
                    aXResultList = XResult.ReadRunResults(sResultFile);
                    if (aXResultList == null || aXResultList.Count < 1) sResponse = "No data received";
                    sResponse = aXResultList[0].sSummary;
                    break;
                case "ShowTestsResult":
                    if (!String.IsNullOrEmpty(sFile)) LoadRuleFile(sFile);
                    ShowTestResults();
                    if (aXResultTestList != null) sResponse = XResultTest.ToStrX(aXResultTestList);
                    else sResponse = "Test cases is Empty";
                    break;
                default:
                    break;

            }
            return sResponse;
        }
    }

}
