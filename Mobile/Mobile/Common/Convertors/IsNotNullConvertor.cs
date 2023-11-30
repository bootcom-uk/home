using MongoDB.Bson;
using System.Globalization;

namespace Mobile.Common.Convertors
{
    public class IsNotNullConvertor : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var isNotNull = ((ObjectId?) value) is not null;
            return isNotNull;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
