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
    /// <summary>
    /// Interaction logic for FactInputDlg.xaml
    /// </summary>
    public partial class FactInputOneDlg : Window
    {
        public ObservableCollection<InputFact> aInputFacts;
        public string Goal;
    //    List<string> options = null;

        public FactInputOneDlg()
        {
            InitializeComponent();
        }

        public FactInputOneDlg(ObservableCollection<InputFact> _facts, string _goal)
        {
            InitializeComponent();
            aInputFacts = _facts;
            Goal = _goal;
            dgFact.ItemsSource = aInputFacts;
            cbGoal.ItemsSource = MainWindow.aXRuleNet.aConclusionList.Select(a => a.conclusion);
            if (!String.IsNullOrEmpty(Goal))
            {
                WHelper.CBSetSelect(cbGoal, Goal);
            }
        }

        public void ShowData()
        {
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            if (btn == null) return;
            if(btn == btnOk)
            {
                Goal = WHelper.CBGetSelect(cbGoal);

                this.DialogResult = true;
                this.Close();
            }
            else if(btn == btnCancel)
            {
                this.DialogResult = false;
                this.Close();
            }
        }

        private void Options_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            InputFact fact = dgFact.SelectedItem as InputFact;
            ComboBox cb = sender as ComboBox;
            if (fact != null && cb != null)
            {
                fact.value = cb.SelectedItem.ToString();
                fact.ivalue = cb.SelectedIndex + 1;
            }
        }
    }
}
