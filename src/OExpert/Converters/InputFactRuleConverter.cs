using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace OExpert
{
    [ValueConversion(typeof(int), typeof(string))]
    public class InputFactRuleConverter : IValueConverter
    {
        //--------------------------------------------
        //--value is true or false
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return "";
            InputFact x = value as InputFact;
            if (x == null || x.ruleList == null) return "";
            string rstr = "";
            foreach(XRule r in x.ruleList)
            {
                if (rstr == "") rstr = r.Name;
                else rstr += ", " + r.Name;
            }
            return rstr;
        }

        //--------------------------------------------
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
