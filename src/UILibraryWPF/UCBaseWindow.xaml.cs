using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Com.DRS.UILibraryWPF
{

    /// <summary>
    /// Interaction logic for UCBaseWindow.xaml
    /// </summary>
    public partial class UCBaseWindow : UserControl
    {
        public static double captionHeight = 24;

        public UCBaseWindow()
        {
            InitializeComponent();
        }

        public void SetCaption(string sCaption)
        {
            tbCaption.Text = sCaption;
        }
    }
}
