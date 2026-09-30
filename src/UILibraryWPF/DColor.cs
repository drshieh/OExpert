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
using System.Windows.Shapes;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace OExpert.UILibraryWPF
{
    public class DColor : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        //--------------------------------------------------------------------
        protected virtual void OnPropertyChanged(string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        //--------------------------------------------------------------------
        public DColor()
        { }

        //--------------------------------------------------------------------
        public DColor(string scolor, Brush acolor)
        {
            ColorName = scolor;
            aColor = acolor;
        }

        //--------------------------------------------------------------------
        public DColor(string scolor)
        {
            ColorName = scolor;
            aColor = new SolidColorBrush(WColor.GetColor(scolor));
        }

        //--------------------------------------------------------------------
        private string _ColorName;
        public string ColorName
        {
            get
            {
                return _ColorName;
            }

            set
            {
                if (_ColorName == value)
                    return;
                _ColorName = value;
                OnPropertyChanged();
            }
        }

        //--------------------------------------------------------------------
        private Brush _aColor;
        public Brush aColor
        {
            get
            {
                return _aColor;
            }

            set
            {
                if (_aColor == value)
                    return;
                _aColor = value;
                OnPropertyChanged();
            }
        }

        //--------------------------------------------------------------------
        public ObservableCollection<DColor> GetDColorList()
        {
            ObservableCollection<DColor> aDColorList = new ObservableCollection<DColor>();
            foreach (string c in WColor.sysColorNameList)
            {
                aDColorList.Add(new DColor(c));
            }
            return aDColorList;
        }
    }

}
