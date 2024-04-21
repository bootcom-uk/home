using Models;
using MongoDB.Bson;
using System.Collections.Immutable;

namespace Services.DataServices
{
    public class BudgetsService
    {

        internal RealmService _realmService;

        internal readonly PaymentPeriodService _paymentPeriodService;

        public BudgetsService(RealmService realmService, PaymentPeriodService paymentPeriodService)
        {
            _realmService = realmService;
            _paymentPeriodService = paymentPeriodService;
        }

        public async Task UpdateBudget(BudgetCategories budgetCategories)
        {
            await _realmService.Realm.WriteAsync(() => {
                _realmService.Realm.Add(budgetCategories, true);
            });
        }

        public double? OutstandingBudgetsForPeriod(PaymentPeriod paymentPeriod)
        {

            Double? budgetTotal = 0;

            foreach (var budget in paymentPeriod.Budgets.Where(record => record.Budget > 0))
            {
                var currentBudgetTotal = (budget.Budget - budget.AmountSpent + budget.AmountReceived);
                if (currentBudgetTotal!.Value < 0) {
                    currentBudgetTotal = 0;
                }

                budgetTotal += currentBudgetTotal;
            }

            return budgetTotal;
        }


        public async Task<BudgetCategories?> GetBudgetById(ObjectId? id)
        {
            if (_realmService.Realm is null) await _realmService.InitializeAsync();

            return _realmService.Realm!.All<BudgetCategories>()
                .FirstOrDefault(record => record.Id ==  id);
        }

        public async Task<IQueryable<BudgetCategories>> CollectDefaultBudgets()
        {
            if (_realmService.Realm is null) await _realmService.InitializeAsync();

            return _realmService.Realm!.All<BudgetCategories>()
                .OrderBy(record => record.PaymentCategoryId!.Name);
        }

        public async Task FullPaymentPeriodBudgetResync(ObjectId paymentPeriodId)
        {

            var paymentPeriod = _realmService.Realm.All<PaymentPeriod>()
                .FirstOrDefault(record => record.Id == paymentPeriodId);

            if (paymentPeriod!.Budgets == null || paymentPeriod.Budgets.Count == 0)
            {

                await _realmService.Realm.WriteAsync(async () =>
                {
                    foreach (var budget in await CollectDefaultBudgets())
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
                });                
            }
            
            var payments = _realmService.Realm.All<Payments>()
                .Where(record => record.StartDate >= paymentPeriod.DateFrom && record.EndDate <= paymentPeriod.DateTo)
                .ToList();

            var paymentTypes = _realmService.Realm.All<PaymentType>()
                .ToList();

            await _realmService.Realm.WriteAsync(() =>
            {

                foreach (var budgetCategory in paymentPeriod.Budgets!)
                {
                    var filteredPaymentTypes = paymentTypes.Where(record => budgetCategory.BudgetCategoryId is not null && budgetCategory.BudgetCategoryId.PaymentCategoryId is not null && record.PaymentCategoryId!.Id == budgetCategory.BudgetCategoryId.PaymentCategoryId.Id)
                    .ToImmutableList();

                    var filteredPayments = Enumerable.Empty<Payments>(); 
                    
                    if(budgetCategory.BudgetCategoryId!.AssociatedResource is not null)
                    {
                        filteredPayments = payments.Where(record => record.PaymentTypeId is not null && filteredPaymentTypes.Contains(record.PaymentTypeId) && (budgetCategory.BudgetCategoryId!.AssociatedResource is not null && budgetCategory.BudgetCategoryId!.AssociatedResource.Id == record.AssociatedResource?.Id));
                    } else
                    {
                        filteredPayments = payments.Where(record => record.PaymentTypeId is not null && filteredPaymentTypes.Contains(record.PaymentTypeId));
                    }
                    

                    var amountPaid = filteredPayments.Where(record => record.AmountPaid > 0)
                        .Select(record => record.AmountPaid)
                        .Sum();
                    var amountReceived = filteredPayments.Where(record => record.AmountReceived > 0)
                        .Select(record => record.AmountReceived)
                        .Sum();

                    budgetCategory.AmountSpent = amountPaid ?? 0;
                    budgetCategory.AmountReceived = amountReceived ?? 0;
                    budgetCategory.BudgetRemaining = (budgetCategory.Budget ?? 0) - budgetCategory.AmountSpent.Value + budgetCategory.AmountReceived.Value;
                }
            
                _realmService.Realm.Add(paymentPeriod, true);
            });
            
        }

        public PaymentPeriod ConfigurePaymentPeriodBudgets(PaymentPeriod paymentPeriod)
        {
            var budgetCategories = _realmService.Realm.All<BudgetCategories>();

            foreach (var budgetCategory in budgetCategories)
            {
                paymentPeriod.Budgets.Add(new()
                {
                    Budget = budgetCategory.DefaultBudget,
                    AmountReceived = 0,
                    AmountSpent = 0,
                    BudgetCategoryId = budgetCategory
                });
            }
                
            return paymentPeriod;
        }

    }
}
