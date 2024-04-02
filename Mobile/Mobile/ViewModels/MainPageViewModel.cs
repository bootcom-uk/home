
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Configuration;
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
        ObservableCollection<String> budgetInformation;

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
        }

        [RelayCommand]
        void PaymentPeriodTapped()
        {
            Console.WriteLine("Ok");
        }

        [RelayCommand]
        async Task RefreshView()
        {
            PaymentPeriod = _paymentPeriodService.CurrentPaymentPeriod();
            LastPaymentsDataSource = await _paymentsService.GetLast3DaysPaymentsForCurrentPeriod(PaymentPeriod);

            DailySpendAndReceiptsDataSource = _paymentsService.DailySpendAndReceiptsForLast7Days();

            var futurePayments = _futurePaymentsService.GetFuturePaymentsTotalForPeriod(PaymentPeriod);
            var outstandingHouseholdBills = _paymentCategoryService.OutstandingHouseholdBillsForPeriod(PaymentPeriod) ?? 0;
            var outstandingBudgets = _budgetsService.OutstandingBudgetsForPeriod(PaymentPeriod) ?? 0;

            OutstandingPaymentsForCurrentPeriod = futurePayments + Math.Round(outstandingHouseholdBills, 2) + Math.Round(outstandingBudgets, 2);

            BudgetInformation = new()
            {
                { $"Total Outstanding Payments: {OutstandingPaymentsForCurrentPeriod:c2}" },
                { $"Future Payments: {futurePayments:c2}" },
                { $"Household Bills: {outstandingHouseholdBills:c2}" },
                { $"Outstanding budgets: {outstandingBudgets:c2}" }
            };

            MoneyOverviewTemplates = new()
            {
                { "BudgetOverview" },
                { "Chris" }
            };

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
