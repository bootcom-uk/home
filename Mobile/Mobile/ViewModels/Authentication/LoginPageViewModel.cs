
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Services;
using Services.DataServices;
using System.ComponentModel;

namespace Mobile.ViewModels.Authentication
{
    public partial class LoginPageViewModel : ViewModelBase
    {
        public LoginPageViewModel(ISemanticScreenReader screenReader, INavigationService navigationService, PaymentTypeService paymentTypeService, UsersService usersService, RealmService realmService, PaymentsService paymentsService) : base(screenReader, navigationService, paymentTypeService, usersService, realmService, paymentsService)
        {
        }


        [ObservableProperty]
        string emailAddress;

        [RelayCommand]
        async Task Login()
        {
            InternalSettings.EmailAddress = EmailAddress;
            await _navigationService.NavigateAsync("AuthenticateEmailPage");
        }

        public override void OnNavigatedFrom(INavigationParameters parameters)
        {
            
        }

        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            EmailAddress = InternalSettings.EmailAddress;
        }
    }
}
