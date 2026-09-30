using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace OExpert
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            string sRuleFile = "";
            foreach (string arg in e.Args)
            {
                sRuleFile = arg;
            }
            base.OnStartup(e);
            MainWindow mainWindow = new MainWindow(sRuleFile);
            mainWindow.Show();
        }
    }
}
