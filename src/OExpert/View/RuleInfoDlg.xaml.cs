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
    public partial class RuleInfoDlg : Window
    {
        public ObservableCollection<XRule> aRuleList;
        public XRule aRule;  //--Working Rule
        public int iRule = -1;
        public bool fReadOnly = false;

        public RuleInfoDlg()
        {
            InitializeComponent();
            SetNavigateButtons();
        }

        public RuleInfoDlg(XRule _rule, bool _readOnly)
        {
            InitializeComponent();
            if (_rule == null) aRule = new XRule();
            else aRule = XRule.Copy(_rule);
            aRuleList = null;
            iRule = -1;
            fReadOnly = _readOnly;
            ShowRule();
            SetNavigateButtons();
        }

        //-------------------------------------------------------
        public RuleInfoDlg(ObservableCollection<XRule> _aList, XRule _rule, bool _readOnly)
        {
            InitializeComponent();
            if (_rule == null) aRule = new XRule();
            else aRule = XRule.Copy(_rule);
            aRuleList = _aList;
            if (aRule == null) iRule = -1;
            else if(aRuleList != null){
                iRule = XRule.Find(aRuleList, aRule.ID);
                if (iRule >= 0 && iRule < aRuleList.Count)
                {
                    aRule = aRuleList[iRule];
                }
            }
            if (aRule == null) aRule = new XRule();
            fReadOnly = _readOnly;
            ShowRule();
            SetNavigateButtons();
        }

        //-------------------------------------------------------
        public void SetNavigateButtons()
        {
            Visibility v = Visibility.Visible;
            if (aRuleList == null || aRuleList.Count <= 1) v = Visibility.Collapsed;
            btnFirst.Visibility = v;
            btnPrevious.Visibility = v;
            btnNext.Visibility = v;
            btnLast.Visibility = v;
            labelCount.Visibility = v;
        }

        //-------------------------------------------------------
        public void ShowRule()
        {
            if (aRule == null) return;
            ucRuleInfo.SetObject(aRule, fReadOnly);
            labelCount.Content = String.Format("{0} of {1}", iRule, (aRuleList == null ? 0 : aRuleList.Count));
        }

        //-------------------------------------------------------
        public void SaveRule()
        {
            if (aRule == null) aRule = new XRule();
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
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            if (btn == null) return;
            if(btn == btnSave)
            {
                SaveRule();

                this.DialogResult = true;
                this.Close();
            }
            else if(btn == btnCancel)
            {
                this.DialogResult = false;
                this.Close();
            }
            else if (btn == btnFirst)
            {
                iRule = 0;
                aRule = aRuleList[iRule];
                ShowRule();
            }
            else if (btn == btnPrevious)
            {
                iRule--;
                if (iRule < 0) iRule = aRuleList.Count - 1;
                aRule = aRuleList[iRule];
                ShowRule();
            }
            else if(btn == btnNext)
            {
                iRule++;
                if (iRule >= aRuleList.Count) iRule = 0;
                aRule = aRuleList[iRule];
                ShowRule();
            }
            else if (btn == btnLast)
            {
                iRule = aRuleList.Count;
                aRule = aRuleList[iRule];
                ShowRule();
            }
        }
    }
}
