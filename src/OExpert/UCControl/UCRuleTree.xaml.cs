//-------------------------------------------------------------
//-- UCRuleTree.xaml.cs : Client functions
//-------------------------------------------------------------
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Numerics;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;


namespace OExpert
{
    //===========================================================================
    public partial class UCRuleTree : UserControl
    {
        public BitmapImage tvBitmapImage = null;
        public BitmapImage tvBitmapImageP = null;  //--with children

        //------------------------------------------------------------
        public UCRuleTree()
        {
            InitializeComponent();
        }

        //------------------------------------------------------------
        private void UCRuleTree_Loaded(object sender, System.EventArgs e)
        {
            UpdateControl();
        }

        //------------------------------------------------------
        public void UpdateControl()
        {
            ucRuleInfo.aRule = null;
            MakeTreeView(tvRule);
            if (ucRuleInfo.aRule != null) Find(ucRuleInfo.aRule.Name, false);
            else
            {
                WHelper.TVExpandAll(tvRule, true);
                ucRuleInfo.SetObject(null, true);
            }
        }

        //------------------------------------------------------
        public BitmapImage CreateBitmapImage(string fileName)
        {
            string sPath = System.IO.Path.GetFullPath(fileName);
            Uri sUri = new Uri(sPath);
            BitmapImage bitmapImage = new BitmapImage();
            // BitmapImage.UriSource must be in a BeginInit/EndInit block
            bitmapImage.BeginInit();
            bitmapImage.UriSource = sUri;
            bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
            bitmapImage.EndInit();
            return bitmapImage;
        }

        //---------------------------------------------
        public void MakeTreeView(TreeView tv)
        {
            tv.Items.Clear();

            if (MainWindow.aXRuleNet == null || MainWindow.aXRuleNet.aRuleList == null) return;
            List<XRule> aTopList = new List<XRule>();
            foreach (XRule a in MainWindow.aXRuleNet.aRuleList)
            {
                if (a.aFromRules == null || a.aFromRules.Count <= 0) aTopList.Add(a);
            }

            if (tvBitmapImageP == null) tvBitmapImageP = CreateBitmapImage("ImagesX\\TiChi.png");
            if (tvBitmapImage == null) tvBitmapImage = CreateBitmapImage("ImagesX\\TiChi_B.png");

            foreach (XRule a in aTopList)
            {
                TreeViewItem tn = MakeTreeItem(a);
                tv.Items.Add(tn);
                if (a.aChildRules != null) MakeTreeViewChild(tn, a);
            }
        }

        //----------------------------------------------------------------
        public TreeViewItem MakeTreeItem(XRule a)
        {
            TreeViewItem tn = new TreeViewItem();
            DockPanel dp = new DockPanel();
            System.Windows.Controls.Image imgCtrl = new System.Windows.Controls.Image();
            //set image source
            if (a.aChildRules != null) imgCtrl.Source = tvBitmapImageP;
            else imgCtrl.Source = tvBitmapImage;
            imgCtrl.Width = 18;
            imgCtrl.Height = 18;

            TextBlock tb = new TextBlock();
            tb.Text = String.Format("{0}: {1} ({2})", a.ID, a.Name, a.Value);
            SetTreeItemColor(tb, a.Value);
            tb.Margin = new Thickness(2);
            dp.Children.Add(imgCtrl);
            dp.Children.Add(tb);

            tn.Header = dp;
            tn.Tag = a;

            if (a.Value > 0 && ucRuleInfo.aRule == null)
            {
                ucRuleInfo.aRule = a;
            }
            return tn;
        }

        //----------------------------------------------------------------
        private void MakeTreeViewChild(TreeViewItem pn, XRule ap)
        {
            if (ap == null || ap.aChildRules == null) return;
            foreach (XRule a in ap.aChildRules)
            {
                TreeViewItem tn = MakeTreeItem(a);
                pn.Items.Add(tn);
                if (a.aChildRules != null) MakeTreeViewChild(tn, a);
            }
        }

        //----------------------------------------------------------------
        public void SetTreeItemColor(TextBlock tb, int value)
        {
            tb.FontSize = 12;
            tb.FontWeight = FontWeights.Normal;

            if (value == 0) tb.Foreground = new SolidColorBrush(Colors.Red);
            else if (value == 1)
            {
                tb.Foreground = new SolidColorBrush(Colors.Green);
                tb.FontSize = 14;
                tb.FontWeight = FontWeights.Bold;
            }
            else tb.Foreground = new SolidColorBrush(Colors.Black);
        }

        //---------------------------------------------
        public void MakeTreeView0(TreeView tv)
        {
            tv.Items.Clear();
            if (MainWindow.aXRuleNet == null || MainWindow.aXRuleNet.aRuleList == null) return;
            List<XRule> aTopList = new List<XRule>();
            foreach (XRule a in MainWindow.aXRuleNet.aRuleList)
            {
                if (a.aFromRules == null || a.aFromRules.Count <= 0) aTopList.Add(a);
            }

            foreach (XRule a in aTopList)
            {
                TreeViewItem tn = new TreeViewItem();
                SetTreeItemColor0(tn, a.Value);
                tn.Header = String.Format("{0}: {1} ({2})", a.ID, a.Name, a.Value);
                tn.Tag = a;
                tv.Items.Add(tn);
                if(a.aChildRules != null) MakeTreeViewChild0(tn, a);
            }
        }

        //----------------------------------------------------------------
        private void MakeTreeViewChild0(TreeViewItem pn, XRule ap)
        {
            if (ap == null || ap.aChildRules == null) return;
            foreach (XRule a in ap.aChildRules)
            {
                TreeViewItem tn = new TreeViewItem();
                SetTreeItemColor0(tn, a.Value);
                tn.Header = String.Format("{0}: {1} ({2})", a.ID, a.Name, a.Value);
                tn.Tag = a;
                pn.Items.Add(tn);
                if (a.aChildRules != null) MakeTreeViewChild0(tn, a);
            }
        }

        //----------------------------------------------------------------
        public void SetTreeItemColor0(TreeViewItem tn, int value)
        {
            tn.FontSize = 12;
            tn.FontWeight = FontWeights.Normal;

            if (value == 0) tn.Foreground = new SolidColorBrush(Colors.Red);
            else if (value == 1)
            {
                tn.Foreground = new SolidColorBrush(Colors.Green);
                tn.FontSize = 14;
                tn.FontWeight = FontWeights.Bold;
            }
            else tn.Foreground = new SolidColorBrush(Colors.Black);
        }

        //--------------------------------------------------------------------------
        private void btnFind_Click(object sender, RoutedEventArgs e)
        {
            Find(tbSearch.Text, true);
        }

        //--------------------------------------------------------------
        private void btnCollapse_Click(object sender, RoutedEventArgs e)
        {
            WHelper.TVExpandAll(tvRule, false);
        }

        //--------------------------------------------------------------
        private void btnExpand_Click(object sender, RoutedEventArgs e)
        {
            WHelper.TVExpandAll(tvRule, true);
        }

        //--------------------------------------------------------------------------
        private void tvRule_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (tvRule.SelectedItem == null) return;

            TreeViewItem vi = (TreeViewItem)tvRule.SelectedItem;

            ShowTreeItem(vi);
        }

        //----------------------------------------------------------------
        public void ShowTreeItem(TreeViewItem vi)
        {
            TreeViewItem ti = (TreeViewItem)tvRule.SelectedItem;
            if(ti == null) return;
            XRule aRule = ti.Tag as XRule;

            ucRuleInfo.SetObject(aRule, true);
        }


        //----------------------------------------------------------------
        private void tbSearch_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                Find(tbSearch.Text, true);
            }
        }

        //----------------------------------------------------------------
        public void Find(string sFilter, bool fMsg)
        {
            WHelper.TVExpandAll(tvRule, true);
            TreeViewItem t = WHelper.FindInTree2(tvRule, sFilter);
            if (t == null)
            {
                if(fMsg) MessageBox.Show("End of List!  Cannot find: " + sFilter);
            }
            else
            {
                if (tvRule.SelectedItem != null) (tvRule.SelectedItem as TreeViewItem).IsSelected = false;
                t.IsSelected = true;
                t.BringIntoView();
                ShowTreeItem(t);
            }
        }

    }
}
