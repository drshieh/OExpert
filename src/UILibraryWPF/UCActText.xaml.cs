using Com.DRS.ObjLogic;
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
    /// Interaction logic for UCActText.xaml
    /// </summary>
    public partial class UCActText : UserControl
    {
        public string Text = "aaa";
        public UCActText()
        {
            InitializeComponent();
            SetText(Text);
        }
        public UCActText(string str)
        {
            InitializeComponent();
            SetText(Text);
        }

        public void SetText(string str)
        {
            Text = str;
            btnAction.Content = str;
            btnAction.ToolTip = str;
        }

        public void SetColor(string TextColor, string BackColor)
        {
            if(!String.IsNullOrEmpty(TextColor)) btnAction.Foreground = WBrush.CreateSolidBrush(TextColor);
            if(!String.IsNullOrEmpty(BackColor)) btnAction.Background = WBrush.CreateSolidBrush(BackColor);
        }
    }
}
