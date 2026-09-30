using OExpert.UILibraryWPF;
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
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace OExpert
{
    public class OptionValue
    {
        public string value { get; set; }

        public OptionValue()
        {

        }

        public OptionValue(string s)
        {
            value = s;
        }

        public static ObservableCollection<OptionValue> FromList(List<string> aList)
        {
            if (aList == null) return null;
            ObservableCollection<OptionValue> rList = new ObservableCollection<OptionValue>();
            foreach (string a in aList)
            {
                rList.Add(new OptionValue(a));
            }
            return rList;
        }

        public static List<string> ToList(ObservableCollection<OptionValue> aList)
        {
            if (aList == null) return null;
            List<string> rList = new List<string>();
            foreach (OptionValue a in aList)
            {
                rList.Add(a.value);
            }
            return rList;
        }
    }

    /// <summary>
    /// Interaction logic for ConditionDataDlg.xaml
    /// </summary>
    public partial class ConditionDataDlg : Window
    {
        public List<XCondition> aConditionList;
        public XCondition aCondition;
        public int iCondition = -1;
        public bool fReadOnly = false;

        public ObservableCollection<OptionValue> aOptions = new ObservableCollection<OptionValue>();
        
        public ConditionDataDlg()
        {
            InitializeComponent();
            SetNavigateButtons();
            aCondition = new XCondition();
        }

        //-------------------------------------------------------
        public ConditionDataDlg(List<XCondition> _condList, int icond, bool _readOnly)
        {
            InitializeComponent();
            aConditionList = _condList;
            iCondition = icond;
            fReadOnly = _readOnly;
            if (aConditionList != null)
            {
                if (icond >= 0 && icond < aConditionList.Count)
                {
                    aCondition = aConditionList[icond];
                }
            }
            if (aCondition == null) aCondition = new XCondition();
            ShowCondition(true);
            SetNavigateButtons();
        }

        //-------------------------------------------------------
        public void SetNavigateButtons()
        {
            Visibility v = Visibility.Visible;
            if (aConditionList == null || aConditionList.Count <= 1) v = Visibility.Collapsed;
            btnFirst.Visibility = v;
            btnPrevious.Visibility = v;
            btnNext.Visibility = v;
            btnLast.Visibility = v;
            labelCount.Visibility = v;
            if (fReadOnly)
            {
                btnSave.Visibility = Visibility.Collapsed;
            }
        }

        //-------------------------------------------------------
        public void ShowCondition(bool fOpt)
        {
            if (aCondition == null) return;
            int isel = dgOptions.SelectedIndex;
            dgOptions.ItemsSource = null;
            labelCount.Content = String.Format("{0} of {1}", iCondition, (aConditionList == null ? 0 : aConditionList.Count));
            cbOperator.ItemsSource = GConstants.aRuleOperList.Select(x=>x.Oper); 
            WHelper.CBSetSelect(cbOperator, aCondition.Operator);
            tbCondition.Text = aCondition.Condition;
            tbWantValue.Text = aCondition.WantValue.ToString();
            tbInputValue.Text = (aCondition.InputValue == null ? "" : aCondition.InputValue.ToString());
            tbEvaluatedValue.Text = aCondition.EvaluatedValue.ToString();
            if (fOpt) aOptions = OptionValue.FromList(aCondition.Options);
            dgOptions.ItemsSource = aOptions;
            if (isel >= aOptions.Count) isel = aOptions.Count - 1;
            dgOptions.SelectedIndex = isel;
            ckbOr.IsChecked = (aCondition.Or == "Or" ? true : false);
        }

        //-------------------------------------------------------
        public void SaveCondition()
        {
            if (aCondition == null) aCondition = new XCondition();
            aCondition.Operator = WHelper.CBGetSelect(cbOperator);
            aCondition.Condition = tbCondition.Text;
            aCondition.WantValue = tbWantValue.Text.Trim();
            aCondition.InputValue = tbInputValue.Text.Trim();
            aCondition.EvaluatedValue = WHelper.GetInt32(tbEvaluatedValue.Text.Trim());
            aCondition.Options = OptionValue.ToList(aOptions);
            aCondition.Or = (ckbOr.IsChecked == true ? "Or" : "");
            if (aConditionList != null)
            {
                aConditionList[iCondition] = aCondition;
            }
        }

        //-------------------------------------------------------
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            if (btn == null) return;
            if(btn == btnSave)
            {
                SaveCondition();
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
                iCondition = 0;
                aCondition = aConditionList[iCondition];
                ShowCondition(true);
            }
            else if (btn == btnPrevious)
            {
                iCondition--;
                if (iCondition < 0) iCondition = aConditionList.Count - 1;
                aCondition = aConditionList[iCondition];
                ShowCondition(true);
            }
            else if(btn == btnNext)
            {
                iCondition++;
                if (iCondition >= aConditionList.Count) iCondition = 0;
                aCondition = aConditionList[iCondition];
                ShowCondition(true);
            }
            else if (btn == btnLast)
            {
                iCondition = aConditionList.Count;
                aCondition = aConditionList[iCondition];
                ShowCondition(true);
            }
            else if (btn == btnMoveToTop)
            {
                int isel = dgOptions.SelectedIndex;
                if (isel >= 0 && isel < aOptions.Count)
                {
                    dgOptions.ItemsSource = null;
                    OptionValue sOpt = aOptions[isel];
                    aOptions.RemoveAt(isel);
                    aOptions.Insert(0, sOpt);
                    dgOptions.ItemsSource = aOptions;
                }
            }
            else if (btn == btnMoveUp)
            {
                int isel = dgOptions.SelectedIndex;
                if (isel > 0 && isel < aOptions.Count)
                {
                    dgOptions.ItemsSource = null;
                    OptionValue sOpt = aOptions[isel];
                    aOptions.RemoveAt(isel);
                    aOptions.Insert(isel-1, sOpt);
                    dgOptions.ItemsSource = aOptions;
                }
            }
            else if (btn == btnMoveDown)
            {
                int isel = dgOptions.SelectedIndex;
                if (isel >= 0 && isel < aOptions.Count-1)
                {
                    dgOptions.ItemsSource = null;
                    OptionValue sOpt = aOptions[isel];
                    aOptions.RemoveAt(isel);
                    aOptions.Insert(isel+1, sOpt);
                    dgOptions.ItemsSource = aOptions;
                }
            }
            else if (btn == btnMoveToBottom)
            {
                int isel = dgOptions.SelectedIndex;
                if (isel >= 0 && isel < aOptions.Count-1)
                {
                    dgOptions.ItemsSource = null;
                    OptionValue sOpt = aOptions[isel];
                    aOptions.RemoveAt(isel);
                    aOptions.Add(sOpt);
                    dgOptions.ItemsSource = aOptions;
                }
            }
            else if (btn == btnAddOption)
            {
                dgOptions.ItemsSource = null;
                if (aOptions == null) aOptions = new ObservableCollection<OptionValue>();
                aOptions.Add(new OptionValue("New Option"));
                dgOptions.ItemsSource = aOptions;
            }
            else if (btn == btnDeleteOption)
            {
                int isel = dgOptions.SelectedIndex;
                dgOptions.ItemsSource = null;

                if (aOptions != null && isel >= 0 && isel <= aOptions.Count)
                {
                    aOptions.RemoveAt(isel);
                    dgOptions.ItemsSource = aOptions;
                }
            }

        }
    }
}
