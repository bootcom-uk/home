using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Configuration;
using Models.Local;
using Services;
using Services.DataServices;

namespace Mobile.ViewModels
{
    public partial class HouseholdBillsForCurrentPeriodOverviewPageViewModel : ViewModelBase
    {
        public HouseholdBillsForCurrentPeriodOverviewPageViewModel(IConfiguration configuration, ISemanticScreenReader screenReader, INavigationService navigationService, PaymentTypeService paymentTypeService, UsersService usersService, RealmService realmService, PaymentsService paymentsService) : base(configuration, screenReader, navigationService, paymentTypeService, usersService, realmService, paymentsService)
        {
        }

        public override void OnNavigatedFrom(INavigationParameters parameters)
        {
            
        }

        [ObservableProperty]
        List<HouseholdBillSpending> dataSource;

        [RelayCommand]
        async Task RefreshView()
        {
            IsRefreshing = true;
            DataSource = await _paymentTypeService.GetAllHouseholdBillsForCurrentPaymentPeriod();
            IsRefreshing = false;
        }

        public async override void OnNavigatedTo(INavigationParameters parameters)
        {
            await RefreshViewCommand.ExecuteAsync(null);
        }
    }
}
