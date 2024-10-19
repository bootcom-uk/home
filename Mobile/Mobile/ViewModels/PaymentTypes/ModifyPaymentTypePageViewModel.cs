using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.Configuration;
using Models.Local;
using Models;
using Services;
using Services.DataServices;
using MongoDB.Bson;
using CommunityToolkit.Mvvm.Input;

namespace Mobile.ViewModels.PaymentTypes
{
    public partial class ModifyPaymentTypePageViewModel : ViewModelBase
    {

        internal PaymentCategoryService _paymentCategoryService;

        public ModifyPaymentTypePageViewModel(IConfiguration configuration, ISemanticScreenReader screenReader, INavigationService navigationService, PaymentTypeService paymentTypeService, UsersService usersService, RealmService realmService, PaymentsService paymentsService, PaymentCategoryService paymentCategoryService) : base(configuration, screenReader, navigationService, paymentTypeService, usersService, realmService, paymentsService)
        {
            _paymentCategoryService = paymentCategoryService;
        }

        public override void OnNavigatedFrom(INavigationParameters parameters)
        {
            
        }

        [ObservableProperty]
        EditablePaymentType paymentTypeDataSource;

        [ObservableProperty]
        IQueryable<PaymentCategory> paymentCategoriesDataSource;

        private bool _isNewRecord = false;

        public override async void OnNavigatedTo(INavigationParameters parameters)
        {
            PaymentCategoriesDataSource = await _paymentCategoryService.GetAllPaymentCategories();

            if (!parameters.TryGetValue<bool>("IsNewRecord", out _isNewRecord)) await _navigationService.GoBackAsync();

            // New record - set the defaults
            if(_isNewRecord) {
                PaymentTypeDataSource = new()
                {
                    PaymentAmount = 0.0
                };
                var randomNumberGenerator = new Random();
                var red = randomNumberGenerator.Next(0, 255);
                var green = randomNumberGenerator.Next(0, 255);
                var blue = randomNumberGenerator.Next(0, 255);
                PaymentTypeDataSource.Color = new Color(red, green, blue).ToInt();
                PaymentTypeDataSource.PaymentCategory = PaymentCategoriesDataSource.FirstOrDefault(record => record.Name == "Household Bills")?.Id;
                return;
            }

            // Existing record - collect info from the db
            if (!parameters.TryGetValue<ObjectId?>("RecordId", out var recordId)) await _navigationService.GoBackAsync();
            if (recordId is null) return;
            var dbPaymentType = await _paymentTypeService.GetPaymentTypeById(recordId.Value);
            PaymentTypeTitle = "Modify Payment Type";
            PaymentTypeDataSource = new()
            {
                PaymentAmount = dbPaymentType.DefaultPaymentAmount.Value,
                Color = dbPaymentType.Colour,
                HavePaymentsEnded = dbPaymentType.HavePaymentsEnded,
                Id = recordId.Value,
                IsResourceRequired = dbPaymentType.IsResourceRequired,
                Name = dbPaymentType.Name,
                PaymentCategory = dbPaymentType.PaymentCategoryId?.Id
            };
        }

        [RelayCommand]
        async Task ClosePage()
        {
            await _navigationService.GoBackAsync();
        }


        [RelayCommand]
        async Task SavePaymentType()
        {
            var paymentType = new PaymentType();
            paymentType.Name = PaymentTypeDataSource.Name;
            paymentType.Colour = PaymentTypeDataSource.Color;
            paymentType.PaymentCategoryId = await _paymentCategoryService.GetPaymentCategory(PaymentTypeDataSource.PaymentCategory);
            paymentType.DefaultPaymentAmount = PaymentTypeDataSource.PaymentAmount;
            paymentType.IsResourceRequired = PaymentTypeDataSource.IsResourceRequired;
            paymentType.HavePaymentsEnded = PaymentTypeDataSource.HavePaymentsEnded;

            if (_isNewRecord)
            {
                paymentType.Id = ObjectId.GenerateNewId();
                await _paymentTypeService.SaveNewPaymentType(paymentType);
                await _navigationService.GoBackAsync();
                return;
            }

            paymentType.Id = PaymentTypeDataSource.Id;
            await _paymentTypeService.UpdatePaymentType(paymentType);
            await _navigationService.GoBackAsync();
        }

    }
}
