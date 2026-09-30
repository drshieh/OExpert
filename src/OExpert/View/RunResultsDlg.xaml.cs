using Microsoft.Win32;
using OExpert.UILibraryWPF;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Configuration;

namespace OExpert
{
    //---------------------------------------------------------------------------------
    public partial class RunResultsDlg : Window
    {

        //-----------------------------------------------------------
        public RunResultsDlg()
        {
            InitializeComponent();
        }

        //-----------------------------------------------------------
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            UpdateDisplayX();
        }

        //----------------------------------------------------------
        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            if (e.Key == Key.Escape)
            {
            }
        }

        //----------------------------------------------------------
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            if (btn == null) return;
            string sName = btn.Name;
            switch (sName)
            {

                case "btnExit":
                    this.Close();
                    break;
                default:
                    break;
            }
        }

        //------------------------------------------------------------------
        public void UpdateDisplayX()
        {
            ucResult.LoadResult();
        }

        //-------------------------------------------------------------------------------------------------
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            MainWindow.tp.wRunResultsDlg = null;
        }

    }

}
