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
    /// Interaction logic for FactInfoDlg.xaml
    /// </summary>
    public partial class UCFactInfo : UserControl
    {
        public XFact aFact;
        public bool fReadOnly = false;

        public UCFactInfo()
        {
            InitializeComponent();
        }

        //-------------------------------------------------------
        public void SetObject(XFact _fact, bool _readOnly)
        {
            fReadOnly = _readOnly;
            SetObject(_fact);
        }

        //-------------------------------------------------------
        public void SetObject(XFact _fact)
        {
            if (_fact == null) aFact = new XFact();
            else aFact = _fact;
            ShowData();
        }

        //-------------------------------------------------------
        public void ShowData()
        {
            if (aFact == null) return;
            tbFact.Text = aFact.fact;
            tbValue.Text = (aFact.Value == null ? "" : aFact.Value.ToString());
            tbX.Text = aFact.X.ToString();
            tbY.Text = aFact.Y.ToString();
            labelRules.Content = "Related Rules: " + ((aFact != null && aFact.ruleList != null) ? aFact.ruleList.Count : 0);
            dgRuleList.ItemsSource = aFact.ruleList;
        }

        //-------------------------------------------------------
        private void dgRuleList_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (dgRuleList.SelectedItem == null) return;
            XRule a = dgRuleList.SelectedItem as XRule;
            RuleInfoDlg dlg = new RuleInfoDlg(aFact.ruleList, a, true);
            dlg.ShowDialog();
        }
    }
}
