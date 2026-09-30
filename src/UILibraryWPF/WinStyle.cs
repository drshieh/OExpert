/*#####################################################################
                    WIN_TYPE.C
Matching the Window STYLE
#####################################################################*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
//using System.Windows.Forms;

namespace Com.DRS.UILibraryWPF
{
    public class WinStyle
    {
        public static MessageBoxButton oMB_GetButton(string sstr0)
        {
            string sstr = sstr0.ToUpper();
            if (sstr == "OK") return MessageBoxButton.OK;
            else if (sstr == "OKCANCEL") return MessageBoxButton.OKCancel;
            else if (sstr == "YESNO") return MessageBoxButton.YesNo;
            else if (sstr == "YESNOCANCEL") return MessageBoxButton.YesNoCancel;
            return MessageBoxButton.OK;
        }

        public static MessageBoxImage oMB_GetIcon(string sstr0)
        {
            string sstr = sstr0.ToUpper();
            if (sstr == "ASTERISK") return MessageBoxImage.Asterisk;
            else if (sstr == "EXCLAMATION") return MessageBoxImage.Exclamation;
            else if (sstr == "HAND") return MessageBoxImage.Hand;
            else if (sstr == "INFORMATION") return MessageBoxImage.Information;
            else if (sstr == "STOP") return MessageBoxImage.Stop;
            else if (sstr == "ERROR") return MessageBoxImage.Error;
            else if (sstr == "NONE") return MessageBoxImage.None;
            else if (sstr == "QUESTION") return MessageBoxImage.Question;
            else if (sstr == "WARNING") return MessageBoxImage.Warning;
            else return MessageBoxImage.None;
        }

        /*====================================================================
        oMB_GetRtnName(value,rstr)
         Get the MessageBox Return Value Name String
        ====================================================================*/
        public static string oMB_GetRtnName(int value)
        {
            return "OK";
            //if (value == (int)DialogResult.OK) return "OK";
            //else if (value == (int)DialogResult.Cancel) return "CANCEL";
            //else if (value == (int)DialogResult.Abort) return "ABORT";
            //else if (value == (int)DialogResult.Retry) return "RETRY";
            //else if (value == (int)DialogResult.Ignore) return "IGNORE";
            //else if (value == (int)DialogResult.Yes) return "YES";
            //else if (value == (int)DialogResult.No) return "NO";
            //else return "NONE";
        }
    }
}
