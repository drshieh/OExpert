using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace OExpert
{
    [ValueConversion(typeof(int), typeof(string))]
    public class TrueFalseConverter : IValueConverter
    {
        //--------------------------------------------
        //--value is true or false
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool == false) return false;
            bool flag = (bool)value;
            return flag;
        }

        //--------------------------------------------
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value.ToString().ToUpper() == "TRUE")
            {
                return true;
            }

            return false;
        }
    }
}
