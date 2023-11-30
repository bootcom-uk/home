using MongoDB.Bson;
using System.Globalization;

namespace Mobile.Common.Convertors
{
    public class PaymentTypeSelectedConvertor : IMultiValueConverter
    {
        public object Convert(object[] value, Type targetType, object parameter, CultureInfo culture)
        {
            var currentId = (ObjectId?)value[0];
            var comparisonId = (ObjectId?)value[1];
            if(comparisonId is null ) { return false; }
            return currentId == comparisonId;
        }


        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
