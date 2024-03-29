using Models;

namespace Services.DataServices
{
    public class FuturePaymentsService(RealmService realmService, PaymentPeriodService paymentPeriodService)
    {


        public async Task AddFuturePayment(FuturePayment futurePayment)
        {
            await realmService.Realm.WriteAsync(() =>
            {
                realmService.Realm.Add(futurePayment);
            });

            await ResyncPaymentPeriodsForFuturePayment();
        }

        public async Task UpdateFuturePayment(FuturePayment futurePayment)
        {
            await realmService.Realm.WriteAsync(() =>
            {
                realmService.Realm.Add(futurePayment, true);
            });

            await ResyncPaymentPeriodsForFuturePayment();
        }

        public IQueryable<FuturePayment> GetFuturePayments() {
            var futurePayments = realmService.Realm.All<FuturePayment>()
                .OrderBy(record => record.PaymentRequiredDate);
            return futurePayments;
        }

        public async Task DeleteFuturePayment(FuturePayment futurePayment)
        {
            await realmService.Realm.WriteAsync(() =>
            {
                realmService.Realm.Remove(futurePayment);
            });
        }

        public async Task ResyncPaymentPeriodsForFuturePayment()
        {

            await realmService.Realm.WriteAsync(async () =>
            {
                var paymentPeriods = await paymentPeriodService.GetPaymentPeriods();

                foreach (var futurePayment in realmService.Realm.All<FuturePayment>())
                {
                    var paymentPeriod = paymentPeriods.FirstOrDefault(record => record.DateFrom <= futurePayment.PaymentRequiredDate && record.DateTo >= futurePayment.PaymentRequiredDate.Date);

                    futurePayment.PaymentPeriodId = paymentPeriod;
                    realmService.Realm.Add(futurePayment, true);
                }
            });
        }

        public double GetFuturePaymentsTotalForPeriod(PaymentPeriod paymentPeriod)
        {

            var futurePayments = realmService.Realm.All<FuturePayment>()
                .Where(record => record.PaymentRequiredDate >= paymentPeriod.DateFrom && record.PaymentRequiredDate <= paymentPeriod.DateTo)
                .ToList();
             return futurePayments                
                .Select(record => record.PaymentExpectedAmount)
                .Sum();
        }

    }
}
