using Models.Local;
using System.Globalization;

namespace Mobile.Common.Convertors
{
    public class EditableFuturePaymentCanSaveConvertor : IMultiValueConverter
    {

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var dateTime = values[0] as DateTime?;
            var detail = values[1] as string;
            var amount = values[2] as double?;
            var isProcessing = values[3] as bool?;

            return dateTime > DateTime.Now && !string.IsNullOrWhiteSpace(detail) && (amount != null && amount > 0) && (isProcessing != null && isProcessing == false); 
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
