﻿using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.Configuration;
using MongoDB.Bson;
using Services;
using Services.DataServices;

namespace Mobile.ViewModels.Authentication
{
    public partial class ValidateLoginPageViewModel : ViewModelBase
    {

        internal readonly HttpService _httpService;

        public ValidateLoginPageViewModel(IConfiguration configuration, ISemanticScreenReader screenReader, INavigationService navigationService, HttpService httpService, PaymentTypeService paymentTypeService, UsersService usersService, RealmService realmService, PaymentsService paymentsService) : base(configuration, screenReader, navigationService, paymentTypeService, usersService, realmService, paymentsService)
        {
            _httpService = httpService;
        }

        [ObservableProperty]
        string validateLoginText;

        public override void OnNavigatedFrom(INavigationParameters parameters)
        {
            
        }

        public override async void OnNavigatedTo(INavigationParameters parameters)
        {
            ValidateLoginText = "Please wait whilst we check that your authentication is valid ...";

            try
            {
                var response = await _httpService.ValidateLogin(InternalSettings.DeviceId!.Value, InternalSettings.UserToken, InternalSettings.RefreshToken);

                InternalSettings.CurrentScreen = "MainPage";

                if (response is null)
                {                    
                    await _navigationService.NavigateAsync("MainPage");
                    return;
                }

                InternalSettings.UserToken = response["JwtToken"];
                InternalSettings.RefreshToken = response["RefreshToken"];
                InternalSettings.UserId = ObjectId.Parse(response["UserId"]);
                await _navigationService.NavigateAsync("MainPage");
            }
            catch (Exception ex)
            {
                InternalSettings.UserToken = null;
                InternalSettings.RefreshToken = null;
                InternalSettings.UserId = null;
                await _navigationService.NavigateAsync("LoginPage");
            }

            

        }

    }
}
