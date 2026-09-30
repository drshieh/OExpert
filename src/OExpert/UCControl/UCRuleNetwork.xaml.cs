using Microsoft.Win32;
using OExpert.Properties;
using OExpert.UILibraryWPF;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Printing;
using System.Runtime.ConstrainedExecution;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Xml.Schema;
using static ICSharpCode.AvalonEdit.Document.TextDocumentWeakEventManager;

namespace OExpert
{
    /// <summary>
    /// Interaction logic for RuleNetwork.xaml
    /// </summary>
    /// 
    public partial class UCRuleNetwork : UserControl
    {
        //------------------------------------------------------------
        public UCRuleNetwork()
        {
            InitializeComponent();
        }

        //------------------------------------------------------------------------
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            SetCKBShow();
        }

        //------------------------------------------------------------
        public void ShowRuleItemInfo(object o)
        {
            if (o == null) return;

            if (o.GetType() == typeof(XRule))
            {
                XRule aRule = o as XRule;
                MainWindow.tp.EditRule(aRule);
            }
            else if (o.GetType() == typeof(XFact))
            {
                XFact aFact = o as XFact;
                FactInfoDlg dlg = new FactInfoDlg(null, aFact, true);
                dlg.Owner = MainWindow.tp;
                dlg.ShowDialog();
            }
            else if (o.GetType() == typeof(XConclusion))
            {
                XConclusion aCon = o as XConclusion;
                ConclusionInfoDlg dlg = new ConclusionInfoDlg(null, aCon, true);
                dlg.Owner = MainWindow.tp;
                dlg.ShowDialog();
            }
        }

        //------------------------------------------------------------
        private void mnuRuleNetwork_Click(object sender, RoutedEventArgs e)
        {
            MenuItem mi = sender as MenuItem;
            string sName = "";
            if (mi != null) sName = mi.Name;
            else
            {
                Button bn = sender as Button;
                if (bn != null) sName = bn.Name;
            }

            switch (sName)
            {
                case "Run":
                case "RunX":
                   // MainWindow.tp.RunEngine(MainWindow.tp);
                    MainWindow.tp.RunEngineAutoTestMix(MainWindow.tp);
                    break;
                case "RunMixMode":  //--Run with Start rule
                case "RunMixModeX":
                    MainWindow.tp.RunEngineMix(MainWindow.tp);
                    break;
                case "LastRunResult":
                case "LastRunResultX":
                    MainWindow.tp.ShowLastRunResult();
                    break;
                case "PrintRuleNetwork":
                case "PrintRuleNetworkX":
                    PrintRuleNetWork(ucDrawArea.aSetting.zoomFactorX, ucDrawArea.aSetting.zoomFactorY);
                    break;
                case "PrintRuleNetwork2PDF":
                    SaveFileDialog dlg1 = new SaveFileDialog();
                    dlg1.InitialDirectory = Directory.GetCurrentDirectory();
                    dlg1.Filter = "pdf files(*.pdf)|*.pdf|All files(*.*)|*.*";
                    if (dlg1.ShowDialog() == true && !String.IsNullOrEmpty(dlg1.FileName))
                    {
                        PrintRuleNetWork2PDF(ucDrawArea.aSetting.zoomFactorX, ucDrawArea.aSetting.zoomFactorY, dlg1.FileName);
                    }

                    break;
                case "PrintRuleNetworkData":
                case "PrintRuleNetworkDataX":
                    oPrintRuleNetworkData("_rule.txt");
                    break;
                case "CutRule":
                case "CutRuleX":
                    DeleteRuleItems();
                    break;
                case "AddRule":  //--Add new rule
                case "AddRuleX":
                    MainWindow.tp.AddRule();
                    break;
                case "CopyRule":  //--Copy Selected Rules (exclude Facts and Conclusions)
                case "CopyRuleX":
                    MainWindow.tp.CopyRules();
                    break;
                case "PasteRule":  //--Paste Copied Rules (exclude Facts and Conclusions)
                case "PasteRuleX":
                    MainWindow.tp.PasteRules();
                    break;

                case "Edit":  //--Edit rule
                case "EditX":
                    XRule aRule = null;
                    if (MainWindow.aXRuleNet.aSelectedObjectList != null && MainWindow.aXRuleNet.aSelectedObjectList.Count > 0)
                    {
                        aRule = MainWindow.aXRuleNet.aSelectedObjectList[0] as XRule;
                    }
                    if (aRule == null)
                    {
                        MessageBox.Show("Please select a Rule first!\r\nNot Fact or Conclusion.");
                    }
                    else MainWindow.tp.EditRule(aRule);
                    break;
                case "ShowInf":
                case "ShowInfX":
                    if (MainWindow.aXRuleNet.aSelectedObjectList != null && MainWindow.aXRuleNet.aSelectedObjectList.Count > 0)
                    {
                        ShowRuleItemInfo(MainWindow.aXRuleNet.aSelectedObjectList[0]);
                    }
                    break;
                case "SelectAll":
                case "SelectAllX":
                    MainWindow.aXRuleNet.aSelectedObjectList = new List<object>();
                    foreach (XRule r in MainWindow.aXRuleNet.aRuleList)
                    {
                        MainWindow.aXRuleNet.aSelectedObjectList.Add(r);
                    }
                    ucDrawArea.UpdateControl(null);
                    break;
                case "Reload":
                case "ReloadX":
                    if (!String.IsNullOrEmpty(MainWindow.tp.sRuleFile))
                    {
                        MainWindow.tp.LoadRuleFile(MainWindow.tp.sRuleFile);
                        ucDrawArea.UpdateControl(null);
                    }
                    break;
                case "ResetRule":
                case "ResetRuleX":
                    MainWindow.aXRuleNet.ResetValues();
                    UpdateControl();
                    break;
                case "Layout":
                case "LayoutX":
                    MainWindow.aXRuleNetBack = XRuleNet.Copy(MainWindow.aXRuleNet);
                    MainWindow.aXRuleNet.SetupNetwork(true);
                    UpdateControl();
                    break;
                case "ZoomIn":
                case "ZoomInX":
                    SetView('I', ucDrawArea);
                    break;
                case "ZoomOut":
                case "ZoomOutX":
                    SetView('O', ucDrawArea);
                    break;
                case "FullView":
                case "FullViewX":
                    SetView('N', ucDrawArea);
                    break;
                case "FitWindow":
                case "FitWindowX":
                    SetView('W', ucDrawArea);
                    break;
                case "btnFind":
                    FindInRuleNetwork(tbFind.Text);
                    break;
                //----------------------------------------Alignment
                case "AlignLeft":
                case "AlignLeftX":
                    MainWindow.aXRuleNetBack = XRuleNet.Copy(MainWindow.aXRuleNet);
                    WRule_Arrange.AlignItemList(MainWindow.aXRuleNet.aSelectedObjectList, AlignMode.Left);
                    break;
                case "AlignRight":
                case "AlignRightX":
                    MainWindow.aXRuleNetBack = XRuleNet.Copy(MainWindow.aXRuleNet);
                    WRule_Arrange.AlignItemList(MainWindow.aXRuleNet.aSelectedObjectList, AlignMode.Right);
                    break;
                case "AlignTop":
                case "AlignTopX":
                    MainWindow.aXRuleNetBack = XRuleNet.Copy(MainWindow.aXRuleNet);
                    WRule_Arrange.AlignItemList(MainWindow.aXRuleNet.aSelectedObjectList, AlignMode.Top);
                    break;
                case "AlignBottom":
                case "AlignBottomX":
                    MainWindow.aXRuleNetBack = XRuleNet.Copy(MainWindow.aXRuleNet);
                    WRule_Arrange.AlignItemList(MainWindow.aXRuleNet.aSelectedObjectList, AlignMode.Bottom);
                    break;
                case "AlignCenter":
                case "AlignCenterX":
                    MainWindow.aXRuleNetBack = XRuleNet.Copy(MainWindow.aXRuleNet);
                    WRule_Arrange.AlignItemList(MainWindow.aXRuleNet.aSelectedObjectList, AlignMode.Center);
                    break;
                case "AlignMiddle":
                case "AlignMiddleX":
                    MainWindow.aXRuleNetBack = XRuleNet.Copy(MainWindow.aXRuleNet);
                    WRule_Arrange.AlignItemList(MainWindow.aXRuleNet.aSelectedObjectList, AlignMode.Middle);
                    break;
                case "AlignSameWidth":
                case "AlignSameWidthX":
                    MainWindow.aXRuleNetBack = XRuleNet.Copy(MainWindow.aXRuleNet);
                    WRule_Arrange.AlignItemList(MainWindow.aXRuleNet.aSelectedObjectList, AlignMode.SameWidth);
                    break;
                case "AlignSameHeight":
                case "AlignSameHeightX":
                    MainWindow.aXRuleNetBack = XRuleNet.Copy(MainWindow.aXRuleNet);
                    WRule_Arrange.AlignItemList(MainWindow.aXRuleNet.aSelectedObjectList, AlignMode.SameHeight);
                    break;
                case "AlignEvenlyHorizontal":
                case "AlignEvenlyHorizontalX":
                    MainWindow.aXRuleNetBack = XRuleNet.Copy(MainWindow.aXRuleNet);
                    WRule_Arrange.AlignItemList(MainWindow.aXRuleNet.aSelectedObjectList, AlignMode.EvenlyHorizontal);
                    break;
                case "AlignEvenlyVertical":
                case "AlignEvenlyVerticalX":
                    MainWindow.aXRuleNetBack = XRuleNet.Copy(MainWindow.aXRuleNet);
                    WRule_Arrange.AlignItemList(MainWindow.aXRuleNet.aSelectedObjectList, AlignMode.EvenlyVertical);
                    break;
                case "Undo":
                    if(MainWindow.aXRuleNetBack != null)
                    {
                        XRuleNet xNet = XRuleNet.Copy(MainWindow.aXRuleNet);
                        MainWindow.aXRuleNet = XRuleNet.Copy(MainWindow.aXRuleNetBack);
                        MainWindow.aXRuleNetBack = xNet;
                        MainWindow.tp.UpdateDisplayX(false);
                    }
                    break;
                case "Redo":
                    if (MainWindow.aXRuleNetBack != null)
                    {
                        XRuleNet xNet = XRuleNet.Copy(MainWindow.aXRuleNet);
                        MainWindow.aXRuleNet = XRuleNet.Copy(MainWindow.aXRuleNetBack);
                        MainWindow.aXRuleNetBack = xNet;
                        MainWindow.tp.UpdateDisplayX(false);
                    }
                    break;
                default:
                    break;
            }
        }

        //--------------------------------------------------------------------
        public static int PrintRuleNetWork(double xrat, double yrat)
        {
            if (MainWindow.aXRuleNet == null || MainWindow.aXRuleNet.aRuleList == null)
            {
                MessageBox.Show("No Rule Network specified!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return 0;
            }
            DrawingVisual visual = new DrawingVisual();
            XRuleNet.GetRuleNetSize(MainWindow.aXRuleNet);
            WRECT tRect = new WRECT(0, 0, MainWindow.aXRuleNet.xMax * xrat, MainWindow.aXRuleNet.yMax * yrat);
            WRECT upRect = new WRECT(0, 0, MainWindow.aXRuleNet.xMax * xrat, MainWindow.aXRuleNet.yMax * yrat);
            using (DrawingContext dc = visual.RenderOpen())
            {
                //               MdlMgr.oDrawModel(dc, aMdlHead, null, 0, 0, xrat, yrat, GConstants.WIN_NORMAL, ref upRect);
                WDrawRule.DrawRuleNetInRect(dc, MainWindow.aXRuleNet, tRect,
                            'R',
                            0, // 1:box only, 2:box+title, 0:none 
                            1000,  // If has draw icon flag: 1000:NO, other yes 
                            1.0);  // icon title ratio 
            }
            WPrint.PrintVisual(visual, "RuleNet", Path.GetFileName(MainWindow.tp.sRuleFile), tRect);

            return (1);
        }

        //--------------------------------------------------------------------
        public static int PrintRuleNetWork2PDF(double xrat, double yrat, string pdfFile)
        {
            if (MainWindow.aXRuleNet == null || MainWindow.aXRuleNet.aRuleList == null)
            {
                MessageBox.Show("No Rule Network specified!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return 0;
            }
            DrawingVisual visual = new DrawingVisual();
            XRuleNet.GetRuleNetSize(MainWindow.aXRuleNet);
            WRECT tRect = new WRECT(0, 0, MainWindow.aXRuleNet.xMax * xrat, MainWindow.aXRuleNet.yMax * yrat);
            WRECT upRect = new WRECT(0, 0, MainWindow.aXRuleNet.xMax * xrat, MainWindow.aXRuleNet.yMax * yrat);
            using (DrawingContext dc = visual.RenderOpen())
            {
                //               MdlMgr.oDrawModel(dc, aMdlHead, null, 0, 0, xrat, yrat, GConstants.WIN_NORMAL, ref upRect);
                WDrawRule.DrawRuleNetInRect(dc, MainWindow.aXRuleNet, tRect,
                            'R',
                            0, // 1:box only, 2:box+title, 0:none 
                            1000,  // If has draw icon flag: 1000:NO, other yes 
                            1.0);  // icon title ratio 
            }
            WPrint.PrintVisual2PDF(visual, "RuleNet", Path.GetFileName(MainWindow.tp.sRuleFile), tRect, pdfFile);

            return (1);
        }

        //--<<=================================

        //--------------------------------------------------------------------
        public void DeleteRuleItems()
        {
            if (MainWindow.aXRuleNet.aSelectedObjectList == null || MainWindow.aXRuleNet.aSelectedObjectList.Count <= 0)
            {
                MessageBox.Show("Please select an Item first", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            else
            {
                string sMsg = String.Format("Are you sure of deleting the RuleNetwork Items\r\n  # selected: {0}\r\n  Including: {1}",
                              MainWindow.aXRuleNet.aSelectedObjectList.Count, MainWindow.aXRuleNet.GetRuleItemStr(MainWindow.aXRuleNet.aSelectedObjectList[0]));
                if (MessageBox.Show(sMsg, "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    MainWindow.aXRuleNetBack = XRuleNet.Copy(MainWindow.aXRuleNet);
                    MainWindow.aXRuleNet.RemoveRuleItemList(MainWindow.aXRuleNet.aSelectedObjectList);
                    XRuleNet.Resolve(MainWindow.aXRuleNet);
                    MainWindow.tp.dgRule.ItemsSource = MainWindow.aXRuleNet.aRuleList;
                    MainWindow.tp.ucRuleNetwork.UpdateControl();
                }
            }
        }

        //------------------------------------------------------------
        public void SetView(char cView, UCDrawArea da)
        {
            if (cView == 'I')  //--ZoomIn
            {
                da.aSetting.zoomFactorX /= 0.8;
                da.aSetting.zoomFactorY /= 0.8;
            }
            else if (cView == 'O')  //--ZoomOut
            {
                da.aSetting.zoomFactorX *= 0.8;
                da.aSetting.zoomFactorY *= 0.8;
            }
            else if (cView == 'W')  //--Fit
            {
                da.aSetting.zoomFactorX = 1;
                da.aSetting.zoomFactorY = 1;
                XRuleNet.GetRuleNetSize(MainWindow.aXRuleNet);

                if (MainWindow.aXRuleNet.xMax > 0) da.aSetting.zoomFactorX = (da.ActualWidth - 20) / MainWindow.aXRuleNet.xMax;
                if (MainWindow.aXRuleNet.yMax > 0) da.aSetting.zoomFactorY = (da.ActualHeight - 20) / MainWindow.aXRuleNet.yMax;
                if (da.aSetting.zoomFactorX > da.aSetting.zoomFactorY) da.aSetting.zoomFactorX = da.aSetting.zoomFactorY;
                else da.aSetting.zoomFactorY = da.aSetting.zoomFactorX;
            }
            else  //--Full
            {
                da.aSetting.zoomFactorX = 1;
                da.aSetting.zoomFactorY = 1;
            }
            var st = new ScaleTransform();
            ucDrawArea.DACanvas.RenderTransform = st;
            st.ScaleX = da.aSetting.zoomFactorX;
            st.ScaleY = da.aSetting.zoomFactorY;
            da.UpdateControl(null);
        }

        //-------------------------------------------------------------------
        public void UpdateControl()
        {
            ucDrawArea.UpdateControl(null);
        }

        //-----------------------------------------------------------------------
        public static void oPrintRuleNetworkData(string fileName)
        {
            RuleNet a = XRuleNet.ToRuleNet(MainWindow.aXRuleNet);
            if (a == null) return;
            try
            {
                StreamWriter sw = new StreamWriter(fileName);
                if (sw != null)
                {
                    string str = RuleNet.ToStr(a);
                    sw.WriteLine(str);
                    sw.Close();
                    WPrint.PrinterASCIIFile(fileName);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("ERROR: PrintRuleNetwork Data\r\n" + ex.ToString());
            }
        }

        //----------------------------------------------------------------------
        private void tbFind_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Return)
            {
                FindInRuleNetwork(tbFind.Text);
            }
        }

        //----------------------------------------------------------------------
        public void FindInRuleNetwork(string stext)
        {
            if (MainWindow.aXRuleNet == null) return;
            string stextU = stext.ToUpper();
            MainWindow.aXRuleNet.aSelectedObjectList.Clear();
            if (MainWindow.tp.fShowRule)
            {
                foreach (XRule a in MainWindow.aXRuleNet.aRuleList)
                {
                    if (a.Name.ToUpper().IndexOf(stextU) >= 0)
                    {
                        MainWindow.aXRuleNet.aSelectedObjectList.Add(a);
                    }
                }
            }
            if (MainWindow.tp.fShowFact)
            {
                foreach (XFact a in MainWindow.aXRuleNet.aFactList)
                {
                    if (a.fact.ToUpper().IndexOf(stextU) >= 0)
                    {
                        MainWindow.aXRuleNet.aSelectedObjectList.Add(a);
                    }
                }
            }
            if (MainWindow.tp.fShowConclusion)
            {
                foreach (XConclusion a in MainWindow.aXRuleNet.aConclusionList)
                {
                    if (a.conclusion.ToUpper().IndexOf(stextU) >= 0)
                    {
                        MainWindow.aXRuleNet.aSelectedObjectList.Add(a);
                    }
                }
            }
            MainWindow.tp.UpdateDisplayX(false);
            if (MainWindow.aXRuleNet.aSelectedObjectList != null && MainWindow.aXRuleNet.aSelectedObjectList.Count > 0)
            {
                SetItemIntoView(MainWindow.aXRuleNet.aSelectedObjectList[0]);
            }
        }

        //----------------------------------------------------------------------
        public static void SetItemIntoView(object aCmp)
        {
            if (aCmp == null) return;
            double offsetX = 0;
            double offsetY = 0;
            GetItemXY(aCmp, ref offsetX, ref offsetY);
            //      offsetX /= 2;
            //      offsetY /= 2;
            double minX = MainWindow.tp.ucRuleNetwork.ucDrawArea.DAScrollViewer.HorizontalOffset;
            double maxX = minX + MainWindow.tp.ucRuleNetwork.ucDrawArea.DAScrollViewer.ActualWidth;
            double minY = MainWindow.tp.ucRuleNetwork.ucDrawArea.DAScrollViewer.VerticalOffset;
            double maxY = minY + MainWindow.tp.ucRuleNetwork.ucDrawArea.DAScrollViewer.ActualHeight;
            double dx = MainWindow.tp.ucRuleNetwork.ucDrawArea.DAScrollViewer.ActualWidth / 3;
            double dy = MainWindow.tp.ucRuleNetwork.ucDrawArea.DAScrollViewer.ActualHeight / 3;
            if (offsetX < minX || offsetX > maxX) MainWindow.tp.ucRuleNetwork.ucDrawArea.DAScrollViewer.ScrollToHorizontalOffset(offsetX-dx);
            if (offsetY < minY || offsetY > maxY) MainWindow.tp.ucRuleNetwork.ucDrawArea.DAScrollViewer.ScrollToVerticalOffset(offsetY-dy);
        }

        //----------------------------------------------------------------------
        public static void GetItemXY(object aCmp, ref double x, ref double y)
        {
            x = 0;
            y = 0;
            if (aCmp.GetType() == typeof(XRule)) { x = ((XRule)aCmp).X; y = ((XRule)aCmp).Y; }
            else if (aCmp.GetType() == typeof(XFact)) { x = ((XFact)aCmp).X; y = ((XFact)aCmp).Y; }
            else if (aCmp.GetType() == typeof(XConclusion)) { x = ((XConclusion)aCmp).X; y = ((XConclusion)aCmp).Y; }
        }

        //----------------------------------------------------------------------
        private void ckbShowRules_Click(object sender, RoutedEventArgs e)
        {
            CheckBox ckb = sender as CheckBox;
            if (ckb == ckbShowTooltip)
            {
                MainWindow.tp.fShowToolTip = (bool)ckb.IsChecked;
                if(ucDrawArea.CustomTooltip.IsOpen == true && MainWindow.tp.fShowToolTip != true) ucDrawArea.CustomTooltip.IsOpen = false;
            }
            else if (ckb == ckbShowRule) MainWindow.tp.fShowRule = (bool)ckb.IsChecked;
            else if (ckb == ckbShowFact) MainWindow.tp.fShowFact = (bool)ckb.IsChecked;
            else if (ckb == ckbShowConclusion) MainWindow.tp.fShowConclusion = (bool)ckb.IsChecked;
            else if (ckb == ckbShowRuleLink) MainWindow.tp.fShowRuleLink = (bool)ckb.IsChecked;
            else if (ckb == ckbShowFactLink) MainWindow.tp.fShowFactLink = (bool)ckb.IsChecked;
            else if (ckb == ckbShowConclusionLink) MainWindow.tp.fShowConclusionLink = (bool)ckb.IsChecked;
            else
            {
                Button btn = sender as Button;
                if (btn == btnShowRuleOnly)
                {
                    MainWindow.tp.fShowRule = true;
                    MainWindow.tp.fShowFact = false;
                    MainWindow.tp.fShowConclusion = false;
                    MainWindow.tp.fShowRuleLink = true;
                    MainWindow.tp.fShowFactLink = false;
                    MainWindow.tp.fShowConclusionLink = false;
                    SetCKBShow();
                }
                else if (btn == btnShowFactOnly)
                {
                    MainWindow.tp.fShowRule = false;
                    MainWindow.tp.fShowFact = true;
                    MainWindow.tp.fShowConclusion = false;
                    MainWindow.tp.fShowRuleLink = false;
                    MainWindow.tp.fShowFactLink = false;
                    MainWindow.tp.fShowConclusionLink = false;
                    SetCKBShow();
                }
                else if (btn == btnShowConclusionOnly)
                {
                    MainWindow.tp.fShowRule = false;
                    MainWindow.tp.fShowFact = false;
                    MainWindow.tp.fShowConclusion = true;
                    MainWindow.tp.fShowRuleLink = false;
                    MainWindow.tp.fShowFactLink = false;
                    MainWindow.tp.fShowConclusionLink = false;
                    SetCKBShow();
                }
                else if (btn == btnShowAll)
                {
                    MainWindow.tp.fShowRule = true;
                    MainWindow.tp.fShowFact = true;
                    MainWindow.tp.fShowConclusion = true;
                    MainWindow.tp.fShowRuleLink = true;
                    MainWindow.tp.fShowFactLink = true;
                    MainWindow.tp.fShowConclusionLink = true;
                    SetCKBShow();
                }
                else if (btn == btnClearAll)
                {
                    MainWindow.tp.fShowRule = false;
                    MainWindow.tp.fShowFact = false;
                    MainWindow.tp.fShowConclusion = false;
                    MainWindow.tp.fShowRuleLink = false;
                    MainWindow.tp.fShowFactLink = false;
                    MainWindow.tp.fShowConclusionLink = false;
                    SetCKBShow();
                }
            }

            MainWindow.tp.UpdateDisplayX(false);
        }

        //-----------------------------------------------------
        public void SetCKBShow()
        {
            ckbShowRule.IsChecked = MainWindow.tp.fShowRule;
            ckbShowFact.IsChecked = MainWindow.tp.fShowFact;
            ckbShowConclusion.IsChecked = MainWindow.tp.fShowConclusion;
            ckbShowRuleLink.IsChecked = MainWindow.tp.fShowRuleLink;
            ckbShowFactLink.IsChecked = MainWindow.tp.fShowFactLink;
            ckbShowConclusionLink.IsChecked = MainWindow.tp.fShowConclusionLink;

            ckbShowTooltip.IsChecked = MainWindow.tp.fShowToolTip;
        }
    }
}
