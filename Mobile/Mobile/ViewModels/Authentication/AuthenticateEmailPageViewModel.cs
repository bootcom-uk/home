using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Configuration;
using Services;
using Services.DataServices;

namespace Mobile.ViewModels.Authentication
{
    public partial class AuthenticateEmailPageViewModel : ViewModelBase
    {

        internal readonly HttpService _httpService;

        [ObservableProperty]
        bool openPopup;

        [ObservableProperty]
        bool loaderImagePlaying;

        [ObservableProperty]
        string quickAccessCode;

        public AuthenticateEmailPageViewModel(IConfiguration configuration, ISemanticScreenReader screenReader, INavigationService navigationService, HttpService httpService, PaymentTypeService paymentTypeService, UsersService usersService, RealmService realmService, PaymentsService paymentsService) : base(configuration, screenReader, navigationService, paymentTypeService, usersService, realmService, paymentsService)
        {
            _httpService = httpService;
        }

        [RelayCommand]
        async Task CompleteSignIn() {
            var response = await _httpService.CompleteLoginProcess(InternalSettings.DeviceId!.Value, QuickAccessCode);
            if (response is null)
            {
                // Do something here to alert our user
                return;
            }

            InternalSettings.UserToken = response["token"];
            InternalSettings.RefreshToken = response["refreshToken"];
            InternalSettings.UserId = Guid.Parse(response["userId"]);

            InternalSettings.CurrentScreen = "MainPage";
            await _navigationService.NavigateAsync("MainPage");
        }


        public override void OnNavigatedFrom(INavigationParameters parameters)
        {
            
        }

        public override async void OnNavigatedTo(INavigationParameters parameters)
        {
            OpenPopup = true;
            LoaderImagePlaying = true;
            var loginRequest = await _httpService.PerformLoginRequest(InternalSettings.DeviceId!.Value, InternalSettings.EmailAddress!);
            OpenPopup = false;
        }
    }
}
