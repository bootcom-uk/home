using System.Globalization;

namespace Mobile.Common.Convertors
{
    public class PaymentReceiptOrPaymentTextConvertor : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var paymentValue = values[0] as double?;
            var receiptValue = values[1] as double?;

            if(paymentValue != null && paymentValue > 0) return paymentValue;
            return receiptValue;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
