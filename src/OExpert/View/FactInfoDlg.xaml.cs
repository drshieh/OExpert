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
    /// Interaction logic for FactInfoDlg.xaml
    /// </summary>
    public partial class FactInfoDlg : Window
    {
        public ObservableCollection<XFact> aFactList;
        public XFact aFact;
        public int iFact = -1;
        public bool fReadOnly = false;

        public FactInfoDlg()
        {
            InitializeComponent();
            SetNavigateButtons();
        }

        //-------------------------------------------------------
        public FactInfoDlg(ObservableCollection<XFact> _alist, XFact _fact, bool _readOnly)
        {
            InitializeComponent();
            aFactList = _alist;
            aFact = _fact;
            fReadOnly = _readOnly;
            if (aFactList != null)
            {
                iFact = XFact.Find(aFactList, aFact.fact);
                if (iFact >= 0 && iFact < aFactList.Count)
                {
                    aFact = aFactList[iFact];
                }
            }
            if (aFact == null) aFact = new XFact();
            ShowData();
            SetNavigateButtons();
        }

        //-------------------------------------------------------
        public void SetNavigateButtons()
        {
            Visibility v = Visibility.Visible;
            if (aFactList == null || aFactList.Count <= 1) v = Visibility.Collapsed;
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
        public void ShowData()
        {
            if (aFact == null) return;
            ucFactInfo.SetObject(aFact, false);
            labelCount.Content = String.Format("{0} of {1}", iFact, (aFactList == null ? 0 : aFactList.Count));
        }

        //-------------------------------------------------------
        public void SaveFact()
        {
            if (aFact == null) aFact = new XFact();
            aFact.fact = ucFactInfo.tbFact.Text;
            aFact.X = Double.Parse(ucFactInfo.tbX.Text.Trim());
            aFact.Y = Double.Parse(ucFactInfo.tbY.Text.Trim());
        }

        //-------------------------------------------------------
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            if (btn == null) return;
            if(btn == btnSave)
            {
                if (aFactList == null)
                {
                    SaveFact();
                }

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
                iFact = 0;
                aFact = aFactList[iFact];
                ShowData();
            }
            else if (btn == btnPrevious)
            {
                iFact--;
                if (iFact < 0) iFact = aFactList.Count - 1;
                aFact = aFactList[iFact];
                ShowData();
            }
            else if(btn == btnNext)
            {
                iFact++;
                if (iFact >= aFactList.Count) iFact = 0;
                aFact = aFactList[iFact];
                ShowData();
            }
            else if (btn == btnLast)
            {
                iFact = aFactList.Count;
                aFact = aFactList[iFact];
                ShowData();
            }

        }

    }
}
