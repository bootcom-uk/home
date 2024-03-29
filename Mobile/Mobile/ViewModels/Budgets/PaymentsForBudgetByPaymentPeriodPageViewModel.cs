using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Models;
using Services;
using Services.DataServices;
using Syncfusion.Maui.DataSource.Extensions;

namespace Mobile.ViewModels.Budgets
{
    public partial class PaymentsForBudgetByPaymentPeriodPageViewModel : ViewModelBase
    {

        private PaymentPeriodService _paymentPeriodService;

        public PaymentsForBudgetByPaymentPeriodPageViewModel(ISemanticScreenReader screenReader, INavigationService navigationService, PaymentTypeService paymentTypeService, UsersService usersService, RealmService realmService, PaymentsService paymentsService, PaymentPeriodService paymentPeriodService) : base(screenReader, navigationService, paymentTypeService, usersService, realmService, paymentsService)
        {
            Title = "Payments For Budget By Payment Period";
            _paymentPeriodService = paymentPeriodService;
            this.PropertyChanged += PaymentsForBudgetByPaymentPeriodPageViewModel_PropertyChanged; ;
        }

        private async void PaymentsForBudgetByPaymentPeriodPageViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "SelectedPaymentPeriod":
                    if (SelectedPaymentPeriod is null) return;
                    var payments = (await _paymentsService.GetPaymentsByDates(SelectedPaymentPeriod.DateFrom.Value, SelectedPaymentPeriod.DateTo.Value)).ToList();
                    payments.ForEach(record => record.PaymentPeriod = SelectedPaymentPeriod);
                    DataSource = payments
                        .OrderBy(record => record.PaymentTypeId.PaymentCategoryId.DisplayOrder)
                        .OrderByDescending(record => record.StartDate);                                        
                    break;
            }
        }

        [RelayCommand]
        void RefreshView()
        {
            IsRefreshing = false;
        }


        public override void OnNavigatedFrom(INavigationParameters parameters)
        {
        }

        [ObservableProperty]
        IEnumerable<Payments> dataSource;

        [ObservableProperty]
        IQueryable<PaymentPeriod> paymentPeriodsDataSource;

        [ObservableProperty]
        PaymentPeriod selectedPaymentPeriod;

        public async override void OnNavigatedTo(INavigationParameters parameters)
        {
            PaymentPeriodsDataSource = await _paymentPeriodService.GetPaymentPeriods();
        }
    }
}
