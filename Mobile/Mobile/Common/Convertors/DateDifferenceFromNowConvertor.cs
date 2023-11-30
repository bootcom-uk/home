using System.Globalization;

namespace Mobile.Common.Convertors
{
    public class DateDifferenceFromNowConvertor : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var dto = (DateTimeOffset) value ;
            var dt = dto.LocalDateTime;

            return dt.Date.Subtract(DateTime.Now.Date).Days;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
