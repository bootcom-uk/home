using System.Globalization;

namespace Mobile.Common.Convertors
{
    public class AllPaymentsToActualPaymentConvertor : IMultiValueConverter
    {
        

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
         
            if ((double?) values[0] > 0)
            {
                return values[0];
            }
            return values[1];
        }


        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
