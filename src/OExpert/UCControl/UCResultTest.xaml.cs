//-------------------------------------------------------------
//-- UCResult.xaml.cs : Client functions
//-------------------------------------------------------------
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;


namespace OExpert
{
    public partial class UCResultTest : UserControl
    {
        //------------------------------------------------------------
        public UCResultTest()
        {
            InitializeComponent();
        }

        //------------------------------------------------------------
        private void UCResult_Loaded(object sender, System.EventArgs e)
        {
            SetActionTextAction();
            LoadTestResult();
        }

        //----------------------------------------------------------
        public void SetActionTextAction()
        {
            this.tbSummary.pActionLeftMouseUp = MainWindow.tp.ActionTextAction;
        }

        //------------------------------------------------------
        public void LoadTestResult()
        {
            lbResult.ItemsSource = null;
            if (String.IsNullOrEmpty(MainWindow.tp.sResultFile)){
                EmptyDisplay();
                return;
            }
            if (MainWindow.tp.aXResultTestList == null)
            {
                MainWindow.tp.aXResultTestList = XResultTest.ReadRunResultTests(MainWindow.tp.sResultFile + ".T");
            }
            lbResult.ItemsSource = MainWindow.tp.aXResultTestList;
            if (MainWindow.tp.aXResultTestList != null && MainWindow.tp.aXResultTestList.Count > 0)
            {
                ShowResult(MainWindow.tp.aXResultTestList[0]);
                lbResult.SelectedIndex = 0;
            }
            else EmptyDisplay();
        }

        //------------------------------------------------------
        public void UpdateControl()
        {
            LoadTestResult();
        }

        //--------------------------------------------------------------------------
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            if (btn == btnFind) Find();
            else if (btn == btnCut) Cut();
            else if (btn == btnSave) Save();
            else if (btn == btnSetInDisplay)
            {
                if (lbResult.SelectedItem != null)
                {
                    XResultTest r = lbResult.SelectedItem as XResultTest;
                    MainWindow.tp.ApplyTestToDisplay(r);
                }
            }
            else
            {
                CheckBox ckb = sender as CheckBox;
                if(ckb == ckbAutoDisplay)
                {
                    if(ckbAutoDisplay.IsChecked == true)
                    {
                        if (lbResult.SelectedItem != null)
                        {
                            XResultTest r = lbResult.SelectedItem as XResultTest;
                            MainWindow.tp.ApplyTestToDisplay(r);
                        }
                    }
                }
            }
        }

        //----------------------------------------------------------------
        private void tbSearch_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                Find();
            }
        }

        //----------------------------------------------------------------
        public void Cut()
        {
            int isel = lbResult.SelectedIndex;
            if (isel < 0) return;
            lbResult.ItemsSource = null;
            MainWindow.tp.aXResultTestList.RemoveAt(isel);
            lbResult.ItemsSource = MainWindow.tp.aXResultTestList;
            isel--;
            if (isel >= 0 && isel < MainWindow.tp.aXResultTestList.Count) lbResult.SelectedIndex = isel;
            else lbResult.SelectedIndex = -1;
        }

        //----------------------------------------------------------------
        public void Save()
        {
            if (MainWindow.tp.aXResultTestList == null || MainWindow.tp.aXResultTestList.Count <= 0) return;
            StreamWriter sw = new StreamWriter(MainWindow.tp.sResultFile + ".T");
            foreach (XResultTest a in MainWindow.tp.aXResultTestList)
            {
                sw.WriteLine(a.ToStr());
            }
            sw.Close();
            MessageBox.Show("Completed Save Result Tests to \r\n" + MainWindow.tp.sResultFile + ".T", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        //----------------------------------------------------------------
        public void Find()
        {
            if(!String.IsNullOrEmpty(tbSearch.Text.Trim()))
            {
                int isel = lbResult.SelectedIndex;
                string sSearch = tbSearch.Text.Trim();
                isel++;
                for(int i = isel; i < MainWindow.tp.aXResultTestList.Count; i++)
                {
                    if (!String.IsNullOrEmpty(MainWindow.tp.aXResultTestList[i].Name) && MainWindow.tp.aXResultTestList[i].Name.IndexOf(sSearch) >= 0)
                    {
                        lbResult.SelectedIndex = i;
                        break;
                    }
                }
                lbResult.SelectedIndex = -1;
            }
        }

        //------------------------------------------------
        private void lbResult_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            object x = lbResult.SelectedItem;
            if(x != null)
            {
                ShowResult(x as XResultTest);
            }
        }

        //------------------------------------------------
        public void ShowResult(XResultTest r)
        {
            if (r == null) EmptyDisplay();
            else
            {
                if (ckbAutoDisplay.IsChecked == true)
                {
                    MainWindow.tp.ApplyTestToDisplay(r);
                }

                tbSummary.Text = r.sSummary;
                //??tbSummary.Text = WHelper.SimpleText(r.sSummary);
                dgFact.ItemsSource = r.aXFactList;
                dgRule.ItemsSource = r.aXRuleList;
                dgConclusion.ItemsSource = r.aXConclusionList;
                //--Show in the trace tab
                ucTraceAvalonEditor.textEditor.Text = r.sTraceText;
            }
        }

        //------------------------------------------------
        public void EmptyDisplay()
        {
            tbSummary.Text = "";
            dgFact.ItemsSource = null;
            dgRule.ItemsSource = null;
            dgConclusion.ItemsSource = null;
            //--Show in the trace tab
            ucTraceAvalonEditor.textEditor.Text = "";
        }

    }
}
