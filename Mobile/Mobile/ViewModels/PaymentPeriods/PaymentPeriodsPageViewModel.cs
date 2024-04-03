using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Configuration;
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

        internal FuturePaymentsService _futurePaymentsService {  get; }

        public PaymentPeriodsPageViewModel(IConfiguration configuration, ISemanticScreenReader screenReader, INavigationService navigationService, PaymentTypeService paymentTypeService, UsersService usersService, RealmService realmService, PaymentsService paymentsService, PaymentPeriodService paymentPeriodService, BudgetsService budgetsService, FuturePaymentsService futurePaymentsService) : base(configuration, screenReader, navigationService, paymentTypeService, usersService, realmService, paymentsService)
        {
            _paymentPeriodService = paymentPeriodService;
            _futurePaymentsService = futurePaymentsService;
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
            await _futurePaymentsService.ResyncPaymentPeriodsForFuturePayment();

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
        async Task EditPaymentPeriod(object period)
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
            var allPayments = await _paymentsService.GetPaymentsByDates(paymentPeriod.DateFrom.Value, paymentPeriod.DateTo.Value);
            paymentPeriod.Budgets.Where(record => record.Budget > 0).ForEach(record =>
            {

                var receipts = allPayments.ToList()

                .Where(paymentRecord => paymentRecord.PaymentTypeId.PaymentCategoryId.Id == record.BudgetCategoryId.PaymentCategoryId.Id)
                .Where(paymentRecord => paymentRecord.AmountReceived > 0)
                .Where(paymentRecord => paymentRecord.StartDate >= paymentPeriod.DateFrom && paymentRecord.EndDate <= paymentPeriod.DateTo)
                .Where(paymentRecord => paymentRecord.AssociatedResource != null && record.BudgetCategoryId.AssociatedResource != null && (paymentRecord.AssociatedResource.Id == record.BudgetCategoryId.AssociatedResource.Id));

                
                

                Console.WriteLine(receipts.Count().ToString());


                var receiptTotal = receipts.Select(receiptRecord => receiptRecord.AmountReceived).Sum() ?? 0;

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
