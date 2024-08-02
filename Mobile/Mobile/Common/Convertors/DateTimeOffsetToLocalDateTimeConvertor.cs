using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mobile.Common.Convertors
{
    public class DateTimeOffsetToLocalDateTimeConvertor : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var currentVal = (value as DateTimeOffset?);
            if(currentVal is not null)
            {
                return currentVal?.ToLocalTime();
            }
            return currentVal;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
