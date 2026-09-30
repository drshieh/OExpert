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
    public partial class UCResult : UserControl
    {
        //------------------------------------------------------------
        public UCResult()
        {
            InitializeComponent();
        }

        //------------------------------------------------------------
        private void UCResult_Loaded(object sender, System.EventArgs e)
        {
            LoadResult();
        }

        //------------------------------------------------------
        public void LoadResult()
        {
            lbResult.ItemsSource = null;
            if (String.IsNullOrEmpty(MainWindow.tp.sResultFile))
            {
                EmptyDisplay();
                return;
            }
            if (MainWindow.tp.aXResultList == null)
            {
                MainWindow.tp.aXResultList = XResult.ReadRunResults(MainWindow.tp.sResultFile);
            }
            lbResult.ItemsSource = MainWindow.tp.aXResultList;
            if (MainWindow.tp.aXResultList != null && MainWindow.tp.aXResultList.Count > 0)
            {
                ShowResult(MainWindow.tp.aXResultList[0]);
                lbResult.SelectedIndex = 0;
            }
            else EmptyDisplay();
        }

        //------------------------------------------------------
        public void UpdateControl()
        {
            LoadResult();
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
                    XResult r = lbResult.SelectedItem as XResult;
                    MainWindow.tp.ApplyToDisplay(r);
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
            MainWindow.tp.aXResultList.RemoveAt(isel);
            lbResult.ItemsSource = MainWindow.tp.aXResultList;
            isel--;
            if (isel >= 0 && isel < MainWindow.tp.aXResultList.Count) lbResult.SelectedIndex = isel;
            else lbResult.SelectedIndex = -1;
        }

        //----------------------------------------------------------------
        public void Save()
        {
            if (MainWindow.tp.aXResultList == null || MainWindow.tp.aXResultList.Count <= 0) return;
            StreamWriter sw = new StreamWriter(MainWindow.tp.sResultFile);
            foreach (XResult a in MainWindow.tp.aXResultList)
            {
                sw.WriteLine(a.ToStr());
            }
            sw.Close();
            MessageBox.Show("Completed Save Results to \r\n" + MainWindow.tp.sResultFile, "Information", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        //----------------------------------------------------------------
        public void Find()
        {
            if(!String.IsNullOrEmpty(tbSearch.Text.Trim()))
            {
                int isel = lbResult.SelectedIndex;
                string sSearch = tbSearch.Text.Trim();
                isel++;
                for(int i = isel; i < MainWindow.tp.aXResultList.Count; i++)
                {
                    if (!String.IsNullOrEmpty(MainWindow.tp.aXResultList[i].date) && MainWindow.tp.aXResultList[i].date.IndexOf(sSearch) >= 0)
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
                ShowResult(x as XResult);
            }
        }

        //------------------------------------------------
        public void ShowResult(XResult r)
        {
            if (r == null) EmptyDisplay();
            else
            {
                tbSummary.Text = r.sSummary;
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
