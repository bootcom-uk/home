using Services;
using Services.DataServices;

namespace Mobile.ViewModels.PaymentTypes
{
    public class PaymentTypesPageViewModel : ViewModelBase
    {
        public PaymentTypesPageViewModel(ISemanticScreenReader screenReader, INavigationService navigationService, PaymentTypeService paymentTypeService, UsersService usersService, RealmService realmService, PaymentsService paymentsService) : base(screenReader, navigationService, paymentTypeService, usersService, realmService, paymentsService)
        {
        }

        public override void OnNavigatedFrom(INavigationParameters parameters)
        {
        }

        public override void OnNavigatedTo(INavigationParameters parameters)
        {
        }
    }
}
