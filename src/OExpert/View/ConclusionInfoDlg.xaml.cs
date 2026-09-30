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
    /// Interaction logic for RuleInfoDlg.xaml
    /// </summary>
    public partial class ConclusionInfoDlg : Window
    {
        public ObservableCollection<XConclusion> aConclusionList;
        public XConclusion aConclusion;
        public int iCon = -1;
        public bool fReadOnly = false;

        public ConclusionInfoDlg()
        {
            InitializeComponent();
            SetNavigateButtons();
        }

        //-------------------------------------------------------
        public ConclusionInfoDlg(ObservableCollection<XConclusion> _alist, XConclusion _con, bool _readOnly)
        {
            InitializeComponent();
            aConclusionList = _alist;
            aConclusion = _con;
            fReadOnly = _readOnly;
            if (aConclusionList != null)
            {
                iCon = XConclusion.Find(aConclusionList, aConclusion.conclusion);
                if (iCon >= 0 && iCon < aConclusionList.Count)
                {
                    aConclusion = aConclusionList[iCon];
                }
            }
            if (aConclusion == null) aConclusion = new XConclusion();
            ShowData(); 
            SetNavigateButtons();
        }

        //-------------------------------------------------------
        public void SetNavigateButtons()
        {
            Visibility v = Visibility.Visible;
            if (aConclusionList == null || aConclusionList.Count <= 1) v = Visibility.Collapsed;
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
            if (aConclusion == null) return;
            ucConclusionInfo.SetObject(aConclusion, false);
            labelCount.Content = String.Format("{0} of {1}", iCon, (aConclusionList == null ? 0 : aConclusionList.Count));
        }

        //-------------------------------------------------------
        public void SaveConclusion()
        {
            if (aConclusion == null) aConclusion = new XConclusion();
            aConclusion.conclusion = ucConclusionInfo.tbConclusion.Text;
            aConclusion.X = Double.Parse(ucConclusionInfo.tbX.Text.Trim());
            aConclusion.Y = Double.Parse(ucConclusionInfo.tbY.Text.Trim());
        }

        //-------------------------------------------------------
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            if (btn == null) return;
            if(btn == btnSave)
            {
                if (aConclusionList == null)
                {
                    SaveConclusion();
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
                iCon = 0;
                aConclusion = aConclusionList[iCon];
                ShowData();
            }
            else if (btn == btnPrevious)
            {
                iCon--;
                if (iCon < 0) iCon = aConclusionList.Count - 1;
                aConclusion = aConclusionList[iCon];
                ShowData();
            }
            else if(btn == btnNext)
            {
                iCon++;
                if (iCon >= aConclusionList.Count) iCon = 0;
                aConclusion = aConclusionList[iCon];
                ShowData();
            }
            else if (btn == btnLast)
            {
                iCon = aConclusionList.Count;
                aConclusion = aConclusionList[iCon];
                ShowData();
            }

        }
    }
}
