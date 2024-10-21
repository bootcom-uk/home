
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Configuration;
using Mobile.LocalModels;
using Models;
using Models.Local;
using Services;
using Services.DataServices;
using System.Collections.ObjectModel;

namespace Mobile.ViewModels
{
    public partial class MainPageViewModel : ViewModelBase
    {
        
        [ObservableProperty]
        PaymentPeriod paymentPeriod;

        [ObservableProperty]
        List<string> moneyOverviewTemplates;

        [ObservableProperty]
        IEnumerable<Payments> lastPaymentsDataSource;

        [ObservableProperty]
        double? outstandingPaymentsForCurrentPeriod;

        [ObservableProperty]
        ObservableCollection<BudgetOverview> budgetInformation;

        [ObservableProperty]
        List<DailySpendAndReceipts> dailySpendAndReceiptsDataSource;

        [ObservableProperty]
        List<Brush> paymentBrushes;

        [ObservableProperty]
        List<Brush> receiptBrushes;

        internal readonly PaymentPeriodService _paymentPeriodService;

        internal readonly FuturePaymentsService _futurePaymentsService;

        internal readonly PaymentCategoryService _paymentCategoryService;

        internal readonly BudgetsService _budgetsService;


        public MainPageViewModel(IConfiguration configuration, ISemanticScreenReader screenReader, INavigationService navigationService, PaymentPeriodService paymentPeriodService, RealmService realmService, PaymentsService paymentsService, FuturePaymentsService futurePaymentsService, PaymentCategoryService paymentCategoryService, BudgetsService budgetsService, PaymentTypeService paymentTypeService, UsersService usersService) : base(configuration, screenReader, navigationService, paymentTypeService, usersService, realmService, paymentsService)
        {
            Title = "Overview";
            _paymentPeriodService = paymentPeriodService;
            _futurePaymentsService = futurePaymentsService;
            _paymentCategoryService = paymentCategoryService;
            _budgetsService = budgetsService;

            BudgetInformation = new();
        }

        [RelayCommand]
        void PaymentPeriodTapped()
        {
            Console.WriteLine("Ok");
        }

        [RelayCommand]
        async Task BudgetInformationTapped(Object selectedItem)
        {
            var overviewType = (Mobile.LocalModels.BudgetOverview.OverviewType) selectedItem;

            switch (overviewType)
            {
                case BudgetOverview.OverviewType.FUTURE_PAYMENTS:
                    await NavigateCommand.ExecuteAsync("FuturePaymentsPage");
                    break;
                case BudgetOverview.OverviewType.HOUSEHOLD_BILLS:
                    await NavigateCommand.ExecuteAsync("HouseholdBillsForCurrentPeriodOverviewPage");
                    break;
            }

        }

        [RelayCommand]
        async Task RefreshView()
        {
            PaymentPeriod = _paymentPeriodService.CurrentPaymentPeriod();
            LastPaymentsDataSource = await _paymentsService.GetLast3DaysPaymentsForCurrentPeriod(PaymentPeriod);

            DailySpendAndReceiptsDataSource = _paymentsService.DailySpendAndReceiptsForLast7Days();

            var futurePayments = _futurePaymentsService.GetFuturePaymentsTotalForPeriod(PaymentPeriod);
            var outstandingHouseholdBills = await _paymentCategoryService.OutstandingHouseholdBillsForPeriod(PaymentPeriod) ?? 0;
            var outstandingBudgets = _budgetsService.OutstandingBudgetsForPeriod(PaymentPeriod) ?? 0;

            OutstandingPaymentsForCurrentPeriod = futurePayments + Math.Round(outstandingHouseholdBills, 2) + Math.Round(outstandingBudgets, 2);

            MoneyOverviewTemplates = new()
            {
                { "BudgetOverview" },
                { "Chris" }
            };
            
            BudgetInformation.Add(new() { Information = $"Total Outstanding Payments: {OutstandingPaymentsForCurrentPeriod:c2}", Type = BudgetOverview.OverviewType.TOTAL_OUTSTANDING_PAYMENTS });
            BudgetInformation.Add(new() { Information = $"Future Payments: {futurePayments:c2}", Type = BudgetOverview.OverviewType.FUTURE_PAYMENTS });
            BudgetInformation.Add(new() { Information = $"Household Bills: {outstandingHouseholdBills:c2}", Type = BudgetOverview.OverviewType.HOUSEHOLD_BILLS });
            BudgetInformation.Add(new() { Information = $"Outstanding budgets: {outstandingBudgets:c2}", Type = BudgetOverview.OverviewType.OUTSTANDING_BUDGETS });

            IsRefreshing = false;
        }

        public override void OnNavigatedFrom(INavigationParameters parameters)
        {
            
        }

        public override async void OnNavigatedTo(INavigationParameters parameters)
        {
            if(_realmService.Realm is null)
            {
                await _realmService.InitializeAsync();
            }
            
            RefreshViewCommand.Execute(null);
        }
    }
}
