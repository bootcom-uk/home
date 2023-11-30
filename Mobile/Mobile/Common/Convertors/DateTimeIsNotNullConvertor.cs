using System.Globalization;

namespace Mobile.Common.Convertors
{
    public class DateTimeIsNotNullConvertor : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var isNotNull = ((DateTime?)value) is not null;
            return isNotNull;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
