using Models;

namespace Services.DataServices
{
    public class FuturePaymentsService
    {

        internal readonly RealmService _realmService;

        public FuturePaymentsService(RealmService realmService)
        {
            _realmService = realmService;
        }

        public async Task AddFuturePayment(FuturePayment futurePayment)
        {
            await _realmService.Realm.WriteAsync(() =>
            {
                _realmService.Realm.Add(futurePayment);
            });
        }

        public async Task UpdateFuturePayment(FuturePayment futurePayment)
        {
            await _realmService.Realm.WriteAsync(() =>
            {
                _realmService.Realm.Add(futurePayment, true);
            });
        }

        public IQueryable<FuturePayment> GetFuturePayments() {
            var futurePayments = _realmService.Realm.All<FuturePayment>()
                .OrderBy(record => record.PaymentRequiredDate);
            return futurePayments;
        }

        public async Task DeleteFuturePayment(FuturePayment futurePayment)
        {
            await _realmService.Realm.WriteAsync(() =>
            {
                _realmService.Realm.Remove(futurePayment);
            });
        }

        public double GetFuturePaymentsTotalForPeriod(PaymentPeriod paymentPeriod)
        {

            var futurePayments = _realmService.Realm.All<FuturePayment>()
                .Where(record => record.PaymentRequiredDate >= paymentPeriod.DateFrom && record.PaymentRequiredDate <= paymentPeriod.DateTo)
                .ToList();
             return futurePayments                
                .Select(record => record.PaymentExpectedAmount)
                .Sum();
        }

    }
}
