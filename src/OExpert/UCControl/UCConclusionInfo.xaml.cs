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
    public partial class UCConclusionInfo : UserControl
    {
        public XConclusion aConclusion;
        public bool fReadOnly = false;

        public UCConclusionInfo()
        {
            InitializeComponent();
        }

        //-------------------------------------------------------
        public void SetObject(XConclusion _con, bool _readOnly)
        {
            fReadOnly = _readOnly;
            SetObject(_con);
        }

        //-------------------------------------------------------
        public void SetObject(XConclusion _con)
        {
            if (_con == null) aConclusion = new XConclusion();
            else aConclusion = _con;
            ShowData();
        }

        //-------------------------------------------------------
        public void ShowData()
        {
            if (aConclusion == null) return;
            tbConclusion.Text = aConclusion.conclusion;
            tbValue.Text = aConclusion.Value.ToString();
            tbX.Text = aConclusion.X.ToString();
            tbY.Text = aConclusion.Y.ToString();
            labelRules.Content = "Related Rules: " + ((aConclusion != null && aConclusion.ruleList != null) ? aConclusion.ruleList.Count : 0);
            dgRuleList.ItemsSource = aConclusion.ruleList;
        }

        //-------------------------------------------------------
        private void dgRuleList_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (dgRuleList.SelectedItem == null) return;
            XRule a = dgRuleList.SelectedItem as XRule;
            RuleInfoDlg dlg = new RuleInfoDlg(aConclusion.ruleList, a, true);
            dlg.ShowDialog();
        }
    }
}
