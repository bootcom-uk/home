using Models;
using Models.Local;
using MongoDB.Bson;

namespace Services.DataServices
{
    public class PaymentPeriodService
    {

        internal RealmService _realmService { get; }

        public PaymentPeriodService(RealmService realmService) { 
            _realmService = realmService;
        }

        public async Task DeletePaymentPeriod(ObjectId id)
        {
            if (_realmService.Realm is null) await _realmService.InitializeAsync();

            await _realmService.Realm!.WriteAsync(async () =>
            {
                var paymentPeriod = await GetPaymentPeriodById(id);

                if (paymentPeriod is null) return;

                _realmService.Realm!.Remove(paymentPeriod);
            });
        }

        public async Task<ObjectId> SavePaymentPeriod(EditablePaymentPeriod editablePaymentPeriod, IEnumerable<BudgetCategories> budgets, bool isUpdate)
        {
            if (_realmService.Realm is null) await _realmService.InitializeAsync();


            // Updating a payment period
            if (isUpdate)
            {
                await _realmService.Realm!.Write(async () =>
                {
                    var paymentPeriod = await GetPaymentPeriodById(editablePaymentPeriod.Id!.Value);

                    if (paymentPeriod is null) return;

                    paymentPeriod.DateFrom = editablePaymentPeriod.DateFrom;
                    paymentPeriod.DateTo = editablePaymentPeriod.DateTo?.AddDays(1).AddMicroseconds(-1);

                    foreach (var budget in paymentPeriod.Budgets)
                    {
                        var modifiedBudget = editablePaymentPeriod.Budgets!.FirstOrDefault(record => record.BudgetCategoryId == budget.BudgetCategoryId!.Id);
                        if(modifiedBudget is null) continue;
                        budget.Budget = modifiedBudget.Budget;
                    }
                    _realmService.Realm.Add(paymentPeriod, isUpdate);

                });
                return editablePaymentPeriod.Id!.Value;
            }

            var id = ObjectId.GenerateNewId();

            // Creating a new payment period
            await _realmService.Realm!.WriteAsync(() =>
            {
                var paymentPeriod = new PaymentPeriod()
                {
                    Id = id,
                    DateFrom = editablePaymentPeriod.DateFrom,
                    DateTo = editablePaymentPeriod.DateTo?.AddDays(1).AddMicroseconds(-1),
                    OriginalId = null
                };

                foreach(var budget in budgets)
                {
                    paymentPeriod.Budgets!.Add(new()
                    {
                        AmountReceived = 0,
                        AmountSpent = 0,
                        Budget = budget.DefaultBudget,
                        BudgetCategoryId = budget,
                        BudgetRemaining = budget.DefaultBudget ?? 0
                    });
                }

                _realmService.Realm.Add(paymentPeriod, isUpdate);
                
            });

            return id;
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

        public async Task<PaymentPeriod?> GetPaymentPeriodById(ObjectId paymentPeriodId)
        {
            if (_realmService.Realm is null) await _realmService.InitializeAsync();

            return _realmService.Realm!.All<PaymentPeriod>().
                FirstOrDefault(record => record.Id == paymentPeriodId);
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
