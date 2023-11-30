using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Models;
using Models.Local;
using Services;
using Services.DataServices;

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

        internal PaymentPeriodService _paymentPeriodService { get; }

        public PaymentPeriodsPageViewModel(ISemanticScreenReader screenReader, INavigationService navigationService, PaymentTypeService paymentTypeService, UsersService usersService, RealmService realmService, PaymentsService paymentsService, PaymentPeriodService paymentPeriodService) : base(screenReader, navigationService, paymentTypeService, usersService, realmService, paymentsService)
        {
            _paymentPeriodService = paymentPeriodService;
            IsPaymentPeriodScreenOpen = false;
        }

        [RelayCommand]
        async Task AddNewPaymentPeriod()
        {
            PaymentPeriodTitle = "Add New Payment Period";
            PaymentPeriodPopupDataSource = new()
            {
                DateFrom = (await _paymentPeriodService.LastPeriodEnds())?.AddDays(1).Date,
                DateTo = (await _paymentPeriodService.LastPeriodEnds())?.AddDays(28).Date
            };
            IsPaymentPeriodScreenOpen = true;
        }

        [RelayCommand]
        void EditPaymentPeriod(object period)
        {
            var paymentPeriod = period as PaymentPeriod;   
            PaymentPeriodTitle = "Modify Payment Period";
            PaymentPeriodPopupDataSource = new()
            {
                DateTo = paymentPeriod.DateTo?.DateTime,
                DateFrom = paymentPeriod.DateFrom?.DateTime
            };
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
