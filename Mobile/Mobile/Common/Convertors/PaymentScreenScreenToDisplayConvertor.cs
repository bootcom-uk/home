using System.Globalization;

namespace Mobile.Common.Convertors
{
    public class PaymentScreenScreenToDisplayConvertor : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var currentScreenName = parameter as string;
            var selectedScreenName = value as string;   
            return (currentScreenName == selectedScreenName);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
