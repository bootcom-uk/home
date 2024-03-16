using Models;
using System.Globalization;

namespace Mobile.Common.Convertors
{
    public class PaymentsToAmountConvertor : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var payments = value as IEnumerable<Payments>;

            var budgets = payments.First().PaymentPeriod.Budgets;
            var paymentTotal = payments
                .Where(record => record.AmountPaid > 0)
                .Select(record => record.AmountPaid)
                .Sum();
            var receiptTotal = payments
                .Where(record => record.AmountReceived > 0)
                .Select(record => record.AmountReceived)
                .Sum();
            var selectedBudget = budgets.FirstOrDefault(record => record.BudgetCategoryId.PaymentCategoryId.Id == payments.First().PaymentTypeId.PaymentCategoryId.Id && (record.BudgetCategoryId.AssociatedResource is null || (record.BudgetCategoryId.AssociatedResource != null && record.BudgetCategoryId.AssociatedResource.Id == payments.First().AssociatedResource.Id)));

            
            return selectedBudget?.Budget + receiptTotal - paymentTotal;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
