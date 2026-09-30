using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;

namespace OExpert
{
    /// <summary>
    /// Interaction logic for RuleInfoDlg.xaml
    /// </summary>
    public partial class UCRuleInfo : UserControl
    {
        public XRule aRule;  //--Working Rule
        public bool fReadOnly = false;
        public List<XRule> ruleList = new List<XRule>();

        public UCRuleInfo()
        {
            InitializeComponent();
        }

        //-------------------------------------------------------
        public void SetObject(XRule _rule, bool _readOnly)
        {
            fReadOnly = _readOnly;
            SetObject(_rule);
        }

        //-------------------------------------------------------
        public void SetObject(XRule _rule)
        {
            if (_rule == null) aRule = new XRule();
            else aRule = _rule;
            SetConditionButtons();
            ShowRule();
            if (!ruleList.Contains(aRule)) ruleList.Add(aRule);
        }

        //-------------------------------------------------------
        public void GoBack()
        {
            if (ruleList == null || ruleList.Count < 1) return;
            int i = ruleList.IndexOf(aRule);
            i--;
            if (i < 0) i = 0;
            SetObject(ruleList[i]);
        }

        //-------------------------------------------------------
        public void GoForward()
        {
            if (ruleList == null || ruleList.Count < 1) return;
            int i = ruleList.IndexOf(aRule);
            i++;
            if (i >= ruleList.Count) i = ruleList.Count - 1;
            SetObject(ruleList[i]);
        }

        //-------------------------------------------------------
        public void SetConditionButtons()
        {
            if (!fReadOnly)
            {
                btnAddCondition.Visibility = Visibility.Visible;
                btnDeleteCondition.Visibility = Visibility.Visible;
                btnEditCondition.Visibility = Visibility.Visible;
            }
            else
            {
                btnAddCondition.Visibility = Visibility.Collapsed;
                btnDeleteCondition.Visibility = Visibility.Collapsed;
                btnEditCondition.Visibility = Visibility.Collapsed;
            }
        }

        //-------------------------------------------------------
        public void ShowRule()
        {
            if (aRule == null) return;
            tbID.Text = aRule.ID.ToString();
            tbName.Text = aRule.Name;
            tbValue.Text = aRule.Value.ToString();
            tbConclusion.Text = aRule.Conclusion;
            tbProbability.Text = aRule.Probability.ToString();
            tbX.Text = aRule.X.ToString();
            tbY.Text = aRule.Y.ToString();
            dgConditions.ItemsSource = aRule.Conditions;
            dgChildRule.ItemsSource = aRule.aChildRules;
            dgFromRule.ItemsSource = aRule.aFromRules;
        }

        //-------------------------------------------------------
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            if (btn == null) return;
            else if (btn == btnGoBack)
            {
                GoBack();
            }
            else if (btn == btnGoForward)
            {
                GoForward();
            }
            else if (btn == btnEditCondition)
            {
                EditCondition();
            }
            else if (btn == btnAddCondition)
            {
                ConditionDataDlg dlg = new ConditionDataDlg();
                dlg.ShowDialog();
                if (dlg.DialogResult == true)
                {
                    dgConditions.ItemsSource = null;
                    aRule.Conditions.Add(dlg.aCondition);
                    dgConditions.ItemsSource = aRule.Conditions;
                }
            }
            else if (btn == btnDeleteCondition)
            {
                if (dgConditions.SelectedItem != null)
                {
                    RCondition c = dgConditions.SelectedItem as RCondition;
                    MessageBox.Show("Are you sure of deleting condition: " + c.toStr());
                }
            }
        }

        //-------------------------------------------------------
        private void dgConditions_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            EditCondition();
        }

        //---------------------------------------------
        public void EditCondition()
        {
            if (dgConditions.SelectedItem == null) return;
            ConditionDataDlg dlg = new ConditionDataDlg(XCondition.CopyList(aRule.Conditions), dgConditions.SelectedIndex, fReadOnly);
            dlg.ShowDialog();
            if (dlg.DialogResult == true)
            {
                dgConditions.ItemsSource = null;
                aRule.Conditions = dlg.aConditionList;
                dgConditions.ItemsSource = aRule.Conditions;
            }
        }

        //---------------------------------------------
        private void dgRule_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            DataGrid dg = sender as DataGrid;
            if (dg == dgFromRule)
            {
                if (aRule.aFromRules != null)
                {
                    int i = dg.SelectedIndex;
                    if (i >= 0 && i < aRule.aFromRules.Count)
                    {
                        SetObject(aRule.aFromRules[i]);
                    }
                }
            }
            else if (dg == dgChildRule)
            {
                if (aRule.aChildRules != null)
                {
                    int i = dg.SelectedIndex;
                    if (i >= 0 && i < aRule.aChildRules.Count)
                    {
                        SetObject(aRule.aChildRules[i]);
                    }
                }
            }
        }
    }
}
