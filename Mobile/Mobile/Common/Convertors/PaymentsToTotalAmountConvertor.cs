using Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mobile.Common.Convertors
{
    public class PaymentsToTotalAmountConvertor : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double result = 0;
            var items = value as IEnumerable<FuturePayment>;
            if (items != null)
            {
                if (items != null)
                {
                    foreach (var futurePayment in items)
                    {
                        result += futurePayment.PaymentExpectedAmount;
                    }
                }
            }
            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
