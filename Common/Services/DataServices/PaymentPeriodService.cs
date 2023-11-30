using Models;
using Realms;

namespace Services.DataServices
{
    public class PaymentPeriodService
    {

        internal RealmService _realmService;

        public PaymentPeriodService(RealmService realmService) { 
            _realmService = realmService;
        }

        public async Task<DateTime?> LastPeriodEnds()
        {
            if (_realmService.Realm is null) await _realmService.InitializeAsync();
            var maxDate = _realmService.Realm!.All<PaymentPeriod>()
                .OrderByDescending(record => record.DateTo)
                .FirstOrDefault()?.DateTo?.LocalDateTime;
            return maxDate;
        }

        public async Task<IQueryable<PaymentPeriod>> GetPaymentPeriods()
        {
            if (_realmService.Realm is null) await _realmService.InitializeAsync();

            return _realmService.Realm!.All<PaymentPeriod>()
                .OrderByDescending(record => record.DateFrom);
        }

        public PaymentPeriod? PaymentPeriodForDate(DateTimeOffset date)
        {
            if (_realmService.Realm is null) throw new NullReferenceException(nameof(PaymentPeriodForDate));
            return _realmService.Realm.All<PaymentPeriod>()
                .FirstOrDefault(record => record.DateFrom <= date && record.DateTo >= date.Date.Add(new TimeSpan(0, 23, 59, 59, 59)));
        }

        public PaymentPeriod? CurrentPaymentPeriod()
        {
            if (_realmService.Realm is null) throw new NullReferenceException(nameof(CurrentPaymentPeriod));

            var dateCheck = new DateTimeOffset(DateTime.Now.Date);

            return _realmService.Realm.All<PaymentPeriod>()
                .FirstOrDefault(record => record.DateFrom <= dateCheck && record.DateTo >= dateCheck);
        }

    }
}
