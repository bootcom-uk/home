using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Models;
using Models.Local;
using MongoDB.Bson;
using Services;
using Services.DataServices;

namespace Mobile.ViewModels.FuturePayments
{
    public partial class FuturePaymentsPageViewModel : ViewModelBase
    {

        internal FuturePaymentsService _futurePaymentsService;

        internal PaymentPeriodService _paymentPeriodService;

        [ObservableProperty]
        IQueryable<FuturePayment> dataSource;

        [ObservableProperty]
        DateTime futurePaymentMinimumDate;

        [ObservableProperty]
        bool isFuturePaymentPopupOpen;

        [ObservableProperty]
        EditableFuturePayment popupDataSource;

        internal bool _addingNewFuturePayment = false;

        public FuturePaymentsPageViewModel(ISemanticScreenReader screenReader, INavigationService navigationService, PaymentTypeService paymentTypeService, UsersService usersService, RealmService realmService, PaymentsService paymentsService, FuturePaymentsService futurePaymentsService, PaymentPeriodService paymentPeriodService) : base(screenReader, navigationService, paymentTypeService, usersService, realmService, paymentsService)
        {
            _futurePaymentsService = futurePaymentsService;
            _paymentPeriodService = paymentPeriodService;
            Title = "Future Payments";
            PopupDataSource = new();
        }

        [RelayCommand]
        async Task SaveFuturePayment()
        {
            IsProcessing = true;

            var futurePayment = new FuturePayment()
            {
                Id = PopupDataSource.Id,
                PaymentRequiredDate = PopupDataSource.FuturePaymentDate,
                PaymentExpectedAmount = PopupDataSource.FuturePaymentAmount,
                PaymentInformation = PopupDataSource.FuturePaymentDetail,
                PaymentPeriodId = _paymentPeriodService.PaymentPeriodForDate(new DateTimeOffset(PopupDataSource.FuturePaymentDate)),
                ResourceId = _usersService.GetUserById(InternalSettings.UserId)
            };

            if (_addingNewFuturePayment)
            {
                await _futurePaymentsService.AddFuturePayment(futurePayment);
            }
            else
            {
                await _futurePaymentsService.UpdateFuturePayment(futurePayment);
            }

            IsFuturePaymentPopupOpen = false;
            IsProcessing = false;

            DataSource = _futurePaymentsService.GetFuturePayments();
        }

        [RelayCommand]
        async Task RefreshView()
        {
            IsRefreshing = true;
            await _futurePaymentsService.ResyncPaymentPeriodsForFuturePayment();
            IsRefreshing = false;
        }
        

        [RelayCommand]
        void CancelFuturePayment()
        {
            IsFuturePaymentPopupOpen = false;
        }

        [RelayCommand]
        async Task DeleteFuturePayments(object payment)
        {
            var futurePayment = (payment as FuturePayment);
            await _futurePaymentsService.DeleteFuturePayment(futurePayment);
            DataSource = _futurePaymentsService.GetFuturePayments();
        }

        [RelayCommand]
        void AddNewFuturePayment()
        {
            _addingNewFuturePayment = true;
            FuturePaymentMinimumDate = DateTime.Now.Date;
            IsFuturePaymentPopupOpen = true;
            PopupDataSource.Id = ObjectId.GenerateNewId();
            PopupDataSource.FuturePaymentDate = FuturePaymentMinimumDate;
            PopupDataSource.FuturePaymentDetail = string.Empty;
            PopupDataSource.FuturePaymentAmount = 0;
        }

        [RelayCommand]
        void ModifyFuturePayment(object payment)
        {
            var futurePayment = (payment as FuturePayment);
            _addingNewFuturePayment = false;
            FuturePaymentMinimumDate = DateTime.Now.Date;
            IsFuturePaymentPopupOpen = true;
            PopupDataSource.FuturePaymentDate = futurePayment.PaymentRequiredDate.LocalDateTime;
            PopupDataSource.FuturePaymentDetail = futurePayment.PaymentInformation;
            PopupDataSource.FuturePaymentAmount = futurePayment.PaymentExpectedAmount;
        }

        public override void OnNavigatedFrom(INavigationParameters parameters)
        {
        }

        public async override void OnNavigatedTo(INavigationParameters parameters)
        {
            if(_realmService.Realm is null)
            {
                await _realmService.InitializeAsync();
            }
            DataSource = _futurePaymentsService.GetFuturePayments();
        }
    }
}
