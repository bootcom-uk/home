using CommunityToolkit.Mvvm.ComponentModel;
using Models;
using Services;
using Services.DataServices;

namespace Mobile.ViewModels.FuturePayments
{
    public partial class FuturePaymentsPageViewModel : ViewModelBase
    {

        internal FuturePaymentsService _futurePaymentsService;

        [ObservableProperty]
        IQueryable<FuturePayment> dataSource;

        public FuturePaymentsPageViewModel(ISemanticScreenReader screenReader, INavigationService navigationService, PaymentTypeService paymentTypeService, UsersService usersService, RealmService realmService, PaymentsService paymentsService, FuturePaymentsService futurePaymentsService) : base(screenReader, navigationService, paymentTypeService, usersService, realmService, paymentsService)
        {
            _futurePaymentsService = futurePaymentsService;
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
