using System.Globalization;

namespace Mobile.Common.Convertors
{
    public class PaymentReceiptOrPaymentColourConvertor : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var paymentValue = values[0] as double?;
            var receiptValue = values[1] as double?;

            if (paymentValue != null) return Color.FromArgb("#FF3131"); ;
            return Color.FromArgb("#3cd070");
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
