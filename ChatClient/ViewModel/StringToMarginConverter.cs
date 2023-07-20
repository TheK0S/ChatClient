using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace ChatClient.ViewModel
{
    public class StringToMarginConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string name = (string)value;

            if(name == null)
            {
                return new Thickness(0, 0, 10, 0);
            }                
            else
            {
                if(name == MainWindow.UserName)
                    return new Thickness(160, 0, 0, 0);
                else
                    return new Thickness(0, 0, 10, 0);
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
