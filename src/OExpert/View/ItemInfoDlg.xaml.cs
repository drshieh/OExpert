using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
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
    /// Interaction logic for RuleInfoDlg.xaml
    /// </summary>
    public partial class ItemInfoDlg : Window
    {
        public ItemInfoDlg()
        {
            InitializeComponent();
            ucRuleInfo.Visibility = Visibility.Visible;
            ucFactInfo.Visibility = Visibility.Collapsed;
            ucConclusionInfo.Visibility = Visibility.Collapsed;
        }

        public ItemInfoDlg(object _item)
        {
            InitializeComponent();
            ShowItemData(_item);
        }

        //-------------------------------------------------------
        public void SetItem(object _item)
        {
            ShowItemData(_item);
        }

        //-------------------------------------------------------
        public void ShowItemData(object item)
        {
            if (item == null) return;
            MainWindow.aXRuleNet.aSelectedObject4Info = item;
            Type t = item.GetType();
            if (t == typeof(XRule))
            {
                tbType.Text = "Rule";
                ucRuleInfo.Visibility = Visibility.Visible;
                ucFactInfo.Visibility = Visibility.Collapsed;
                ucConclusionInfo.Visibility = Visibility.Collapsed;
                ucRuleInfo.fReadOnly = false;
                ucRuleInfo.SetObject(item as XRule);
                btnSave.Visibility = Visibility.Visible;
            }
            else if (t == typeof(XFact))
            {
                tbType.Text = "Fact";
                ucRuleInfo.Visibility = Visibility.Collapsed;
                ucFactInfo.Visibility = Visibility.Visible;
                ucConclusionInfo.Visibility = Visibility.Collapsed;
                ucFactInfo.fReadOnly = true;
                ucFactInfo.SetObject(item as XFact);
                btnSave.Visibility = Visibility.Collapsed;
            }
            else if (t == typeof(XConclusion))
            {
                tbType.Text = "Conclusion";
                ucRuleInfo.Visibility = Visibility.Collapsed;
                ucFactInfo.Visibility = Visibility.Collapsed;
                ucConclusionInfo.Visibility = Visibility.Visible;
                ucConclusionInfo.fReadOnly = true;
                ucConclusionInfo.SetObject(item as XConclusion);
                btnSave.Visibility = Visibility.Collapsed;
            }
            else return;
        }

        //-------------------------------------------------------
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            if (btn == null) return;
            if (btn == btnCancel)
            {
                this.Close();
            }
            else if(btn == btnSave)
            {
                SaveRule();
            }
        }

        //-------------------------------------------------------
        public void SaveRule()
        {
            XRule aRule = MainWindow.aXRuleNet.aSelectedObject4Info as XRule;
            if (aRule == null) return;
            aRule.Conclusion = ucRuleInfo.tbConclusion.Text;
            aRule.Probability = Double.Parse(ucRuleInfo.tbProbability.Text.Trim());
            aRule.X = Double.Parse(ucRuleInfo.tbX.Text.Trim());
            aRule.Y = Double.Parse(ucRuleInfo.tbY.Text.Trim());
            aRule.Conditions = new List<XCondition>();
            foreach (var a in ucRuleInfo.dgConditions.ItemsSource)
            {
                XCondition c = a as XCondition;
                if (c != null)
                {
                    aRule.Conditions.Add(c);
                }
            }
        }

        //-------------------------------------------------------
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (MainWindow.tp.wItemInfoDlg != null)
            {
                MainWindow.tp.wItemInfoDlg = null;
                MainWindow.aXRuleNet.aSelectedObject4Info = null;
            }

        }
    }
}
