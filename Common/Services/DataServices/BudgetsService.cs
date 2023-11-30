using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public double? OutstandingBudgetsForPeriod(PaymentPeriod paymentPeriod)
        {

            Double? budgetTotal = 0;

            foreach (var budget in paymentPeriod.Budgets.Where(record => record.Budget > 0))
            {
                budgetTotal += (budget.Budget - budget.AmountSpent + budget.AmountReceived);
            }

            return budgetTotal;
        }

        public async Task BudgetUpdateFromPayment(Payments payments)
        {
            if (payments.PaymentTypeId is null) return;
            if (payments.StartDate is null) return;

            var paymentPeriod = _paymentPeriodService.PaymentPeriodForDate(payments.StartDate.Value);

            if(paymentPeriod is null) return;

            var budgets = paymentPeriod.Budgets.Where(record => record.BudgetCategoryId is not null && record.BudgetCategoryId.PaymentCategoryId == payments.PaymentTypeId.PaymentCategoryId);

            PaymentPeriod_Budgets? budgetToUse = null;

            switch (budgets.Count()) {
                case > 1:
                    budgetToUse = budgets.FirstOrDefault(record => record.BudgetCategoryId!.AssociatedResource == payments.AssociatedResource);
                    break;
                case 1:
                    budgetToUse = budgets.First();
                    break;
            }
                
            if(budgetToUse is null) return;

            budgetToUse.AmountReceived += payments.AmountReceived;
            budgetToUse.AmountSpent += payments.AmountPaid;
            budgetToUse.BudgetRemaining = budgetToUse.Budget ?? 0 + payments.AmountReceived ?? 0 - payments.AmountPaid ?? 0;

           await _realmService.Realm.WriteAsync(() => {
                _realmService.Realm.Add(paymentPeriod, true);
            });
            
        }

        public async Task FullPaymentPeriodBudgetResync(PaymentPeriod paymentPeriod)
        {
            if (paymentPeriod.Budgets == null || paymentPeriod.Budgets.Count == 0) return;
            
            var payments = _realmService.Realm.All<Payments>()
                .Where(record => record.StartDate >= paymentPeriod.DateFrom && record.EndDate <= paymentPeriod.DateTo)
                .ToList();

            var paymentTypes = _realmService.Realm.All<PaymentType>()
                .ToList();


            foreach(var budgetCategory in paymentPeriod.Budgets)
            {
                var filteredPaymentTypes = paymentTypes.Where(record => budgetCategory.BudgetCategoryId is not null && record.PaymentCategoryId == budgetCategory.BudgetCategoryId.PaymentCategoryId);

                var filteredPayments = payments.Where(record => filteredPaymentTypes.Contains(record.PaymentTypeId));

                var amountPaid = filteredPayments.Where(record => record.AmountPaid > 0)
                    .Select(record => record.AmountPaid)
                    .Sum();
                var amountReceived = filteredPayments.Where(record => record.AmountReceived > 0)
                    .Select(record => record.AmountReceived)
                    .Sum();

                budgetCategory.AmountSpent = amountPaid;
                budgetCategory.AmountReceived = amountReceived;                
            }

            await _realmService.Realm.WriteAsync(() =>
            {
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
