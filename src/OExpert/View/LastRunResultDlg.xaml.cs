using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace OExpert
{
    /// <summary>
    /// Interaction logic for FactInputDlg.xaml
    /// </summary>
    public partial class LastRunResultDlg : Window
    {
        public XResult aResult;

        //--------------------------------------------------
        public LastRunResultDlg()
        {
            InitializeComponent();
        }

        //--------------------------------------------------
        public LastRunResultDlg(XResult _result)
        {
            InitializeComponent();
            aResult = _result;
            ShowRunResult(aResult);
        }

        //--------------------------------------------------
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            if (btn == null) return;
            if (btn == btnClose)
            {
                this.Close();
            }
            else if (btn == btnSetInDisplay)
            {
                MainWindow.tp.ApplyToDisplay(aResult);
            }
        }

        //-----------------------------------------
        public void ShowRunResult(XResult r)
        {
            if (r == null) return;
            tbSummary.Text = r.sSummary;
            dgFact.ItemsSource = r.aXFactList;
            dgRule.ItemsSource = r.aXRuleList;
            dgConclusion.ItemsSource = r.aXConclusionList;
              //--Show in the trace tab
            ucAvalonEditor.textEditor.Text = r.sTraceText;
        }

        //--------------------------------------------------------------
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            MainWindow.tp.wLastRunResultDlg = null;
        }
    }
}
