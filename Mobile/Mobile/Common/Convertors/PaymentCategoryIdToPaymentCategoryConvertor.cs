using Microsoft.Extensions.Logging.Abstractions;
using Models;
using MongoDB.Bson;
using Syncfusion.Maui.Inputs;
using System.Globalization;

namespace Mobile.Common.Convertors
{
    public class PaymentCategoryIdToPaymentCategoryConvertor : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var dataSource = ((parameter as SfComboBox).ItemsSource as IQueryable<PaymentCategory>);
            var id = (value as ObjectId?);
            if(id is null)
            {
                return null;
            }
            return dataSource.FirstOrDefault(record => record.Id == id);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var paymentCategory = (value as PaymentCategory);
            if(paymentCategory is null)
            {
                return null;
            }
            return paymentCategory.Id;
        }
    }
}
