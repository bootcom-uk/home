using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Models;
using Models.Local;
using Services;
using Services.DataServices;
using Syncfusion.Maui.DataSource.Extensions;

namespace Mobile.ViewModels.PaymentPeriods
{
    public partial class PaymentPeriodsPageViewModel : ViewModelBase
    {

        [ObservableProperty]
        bool isPaymentPeriodScreenOpen;

        [ObservableProperty]
        string paymentPeriodTitle;

        [ObservableProperty]
        IQueryable<PaymentPeriod> dataSource;

        [ObservableProperty]
        EditablePaymentPeriod paymentPeriodPopupDataSource;

        internal bool _updatingPeriod;

        internal PaymentPeriodService _paymentPeriodService { get; }

        internal BudgetsService _budgetsService { get; }

        public PaymentPeriodsPageViewModel(ISemanticScreenReader screenReader, INavigationService navigationService, PaymentTypeService paymentTypeService, UsersService usersService, RealmService realmService, PaymentsService paymentsService, PaymentPeriodService paymentPeriodService, BudgetsService budgetsService) : base(screenReader, navigationService, paymentTypeService, usersService, realmService, paymentsService)
        {
            _paymentPeriodService = paymentPeriodService;
            _budgetsService = budgetsService;
            IsPaymentPeriodScreenOpen = false;
            Title = "Payment Periods";
        }

        [RelayCommand]
        async Task AddNewPaymentPeriod()
        {
            _updatingPeriod = false;
            PaymentPeriodTitle = "Add New Payment Period";
            PaymentPeriodPopupDataSource = new()
            {
                DateFrom = (await _paymentPeriodService.LastPeriodEnds())?.AddDays(1).Date                
            };

            PaymentPeriodPopupDataSource.DateTo = new DateTime(PaymentPeriodPopupDataSource.DateFrom.Value.AddMonths(1).Year, PaymentPeriodPopupDataSource.DateFrom.Value.AddMonths(1).Month, 26);

            switch (PaymentPeriodPopupDataSource.DateTo?.DayOfWeek)
            {
                case DayOfWeek.Sunday:
                    PaymentPeriodPopupDataSource.DateTo = PaymentPeriodPopupDataSource.DateTo?.AddDays(-2);
                    break;
                case DayOfWeek.Saturday:
                    PaymentPeriodPopupDataSource.DateTo = PaymentPeriodPopupDataSource.DateTo?.AddDays(-1);
                    break;
            }

            IsPaymentPeriodScreenOpen = true;
        }

        [RelayCommand]
        async Task UpdatePaymentPeriod()
        {
            IsProcessing = true;
            
            var periodId = await _paymentPeriodService.SavePaymentPeriod(PaymentPeriodPopupDataSource, await _budgetsService.CollectDefaultBudgets(), _updatingPeriod);
            await _budgetsService.FullPaymentPeriodBudgetResync(periodId);


            DataSource = await _paymentPeriodService.GetPaymentPeriods();

            IsProcessing = false;

            IsPaymentPeriodScreenOpen = false;
        }

        [RelayCommand]
        async Task DeletePaymentPeriod(object period)
        {
            var paymentPeriod = period as PaymentPeriod;
            await _paymentPeriodService.DeletePaymentPeriod(paymentPeriod.Id.Value);
            DataSource = await _paymentPeriodService.GetPaymentPeriods();
        }

        [RelayCommand]
        void EditPaymentPeriod(object period)
        {
            _updatingPeriod = true;
            var paymentPeriod = period as PaymentPeriod;   
            PaymentPeriodTitle = "Modify Payment Period";
            PaymentPeriodPopupDataSource = new()
            {
                Id = paymentPeriod.Id,
                DateTo = paymentPeriod.DateTo?.DateTime,
                DateFrom = paymentPeriod.DateFrom?.DateTime
            };
            PaymentPeriodPopupDataSource.Budgets = new();
            paymentPeriod.Budgets.Where(record => record.Budget > 0).ForEach(record =>
            {                
                PaymentPeriodPopupDataSource.Budgets.Add(new()
                {
                    AmountReceived = record.AmountReceived,
                    AmountSpent = record.AmountSpent,
                    Budget = record.Budget,
                    BudgetCategoryId = record.BudgetCategoryId?.Id,
                    BudgetRemaining = record.BudgetRemaining,
                    BudgetCategoryInformation = (record.BudgetCategoryId?.AssociatedResource is null ? record.BudgetCategoryId?.PaymentCategoryId?.Name : $"{record.BudgetCategoryId?.PaymentCategoryId?.Name} ({record.BudgetCategoryId?.AssociatedResource.Name})")                    
                });
            });
            IsPaymentPeriodScreenOpen = true;    
        }

        public override void OnNavigatedFrom(INavigationParameters parameters)
        {
            
        }

        public async override void OnNavigatedTo(INavigationParameters parameters)
        {
            DataSource = await _paymentPeriodService.GetPaymentPeriods();
        }
    }
}
