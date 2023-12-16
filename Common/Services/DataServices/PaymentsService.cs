using Models;
using Models.Local;

namespace Services.DataServices
{
    public class PaymentsService
    {

        internal readonly RealmService _realmService;

        internal readonly PaymentPeriodService _paymentPeriodService;

        internal readonly BudgetsService _budgetsService;

        public PaymentsService(RealmService realmService, PaymentPeriodService paymentPeriodService, BudgetsService budgetsService)
        {
            _realmService = realmService;
            _paymentPeriodService = paymentPeriodService;
            _budgetsService = budgetsService;
        }

        public List<DailySpendAndReceipts> DailySpendAndReceiptsForLast7Days()
        {
            var items = new List<DailySpendAndReceipts>();

            for (var i = 0; i < 7; i ++)
            {

                var dateToCheck = DateTime.Now.Date.AddDays(0 - i);
                var dateOffsetToCheck = new DateTimeOffset(dateToCheck);

                var item = new DailySpendAndReceipts()
                {
                    Date = dateToCheck,
                    AmountSpent = _realmService.Realm
                    .All<Payments>()
                    .Where(record => record.StartDate == dateOffsetToCheck && record.AmountPaid > 0)
                    .ToList()
                    .Select(record => record.AmountPaid)
                    .Sum() ?? 0,
                    AmountReceived = _realmService.Realm
                    .All<Payments>()
                    .Where(record => record.StartDate == dateOffsetToCheck && record.AmountReceived > 0)
                    .ToList()
                    .Select(record => record.AmountReceived)
                    .Sum() ?? 0
                };
                items.Add(item);
            }

            return items
                .OrderByDescending(record => record.Date)
                .ToList();
        }

        public async Task<IQueryable<Payments>?> GetPaymentsByDates(DateTimeOffset dateFrom, DateTimeOffset dateTo)
        {
            if (_realmService.Realm is null) await _realmService.InitializeAsync();
            var records = _realmService.Realm!.All<Payments>()
                .Where(record => record.StartDate >= dateFrom && record.EndDate <= dateTo);

            return records;
        }

        public async Task<IEnumerable<Payments>>? GetLast3DaysPaymentsForCurrentPeriod(PaymentPeriod paymentPeriod)
        {
            if (_realmService.Realm is null) await _realmService.InitializeAsync();

            DateTimeOffset? from = DateTimeOffset.Now.AddDays(-3);
            DateTimeOffset? to = paymentPeriod.DateTo;

            var records = _realmService.Realm!.All<Payments>()
                .Where(record => record.StartDate >= from && record.EndDate <= to)
                .OrderByDescending(record => record.DateAdded);

            return records ?? Enumerable.Empty<Payments>();
        }

        public async Task AddPayment(Payments payments)
        {
            await _realmService.Realm.WriteAsync(() => {
                _realmService.Realm.Add(payments);
            });
            
            await _budgetsService.BudgetUpdateFromPayment(payments);            
        }

        public async Task UpdatePayment(Payments payments)
        {
            await _realmService.Realm.WriteAsync(() => {
                _realmService.Realm.Add(payments, true);
            });
            var paymentPeriod = _paymentPeriodService.PaymentPeriodForDate(payments.StartDate!.Value);
            if(paymentPeriod != null)
            {
                await _budgetsService.FullPaymentPeriodBudgetResync(paymentPeriod.Id!.Value);
            }
        }

        public async Task DeletePayment(Payments payments)
        {
            await _realmService.Realm.WriteAsync(() => {
                _realmService.Realm.Remove(payments);
            });
            var paymentPeriod = _paymentPeriodService.PaymentPeriodForDate(payments.StartDate!.Value);
            if (paymentPeriod != null)
            {
                await _budgetsService.FullPaymentPeriodBudgetResync(paymentPeriod.Id!.Value);
            }
        }

    }
}
