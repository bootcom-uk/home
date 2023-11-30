using Syncfusion.Maui.Calendar;
using System.Globalization;

namespace Mobile.Common.Convertors
{
    public class CalendarSelectionChangedEventArgsToDateConvertor : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var args = (value as CalendarSelectionChangedEventArgs);
            return args.NewValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
