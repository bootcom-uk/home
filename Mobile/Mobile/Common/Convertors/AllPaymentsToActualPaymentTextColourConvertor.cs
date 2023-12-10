using System.Globalization;

namespace Mobile.Common.Convertors
{
    public class AllPaymentsToActualPaymentTextColourConvertor : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if ((double?)values[0] > 0)
            {
                return Color.FromArgb("#FF3131");
            }
            return Color.FromArgb("#3cd070");
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
