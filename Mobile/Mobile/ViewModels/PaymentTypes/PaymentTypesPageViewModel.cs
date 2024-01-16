using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Models;
using Models.Local;
using MongoDB.Bson;
using Services;
using Services.DataServices;
using System.Security.Cryptography;

namespace Mobile.ViewModels.PaymentTypes
{
    public partial class PaymentTypesPageViewModel : ViewModelBase
    {
        [ObservableProperty]
        IQueryable<PaymentType> dataSource;

        [ObservableProperty]
        bool isPaymentTypeScreenOpen;

        [ObservableProperty]
        string paymentTypeTitle;

        internal bool _isAddingPaymentType;

        [ObservableProperty]
        EditablePaymentType paymentTypeDataSource;

        [ObservableProperty]
        IQueryable<PaymentCategory> paymentCategoriesDataSource;

        internal PaymentCategoryService _paymentCategoryService;

        public PaymentTypesPageViewModel(ISemanticScreenReader screenReader, INavigationService navigationService, PaymentTypeService paymentTypeService, UsersService usersService, RealmService realmService, PaymentsService paymentsService, PaymentCategoryService paymentCategoryService) : base(screenReader, navigationService, paymentTypeService, usersService, realmService, paymentsService)
        {
            _paymentCategoryService = paymentCategoryService;
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
            
            if (_isAddingPaymentType)
            {
                paymentType.Id = ObjectId.GenerateNewId();                
                await _paymentTypeService.SaveNewPaymentType(paymentType);
                IsPaymentTypeScreenOpen = false;
                return;
            }

            paymentType.Id = PaymentTypeDataSource.Id;
            await _paymentTypeService.UpdatePaymentType(paymentType);
            IsPaymentTypeScreenOpen = false;
        }

        [RelayCommand]
        void AddNewPaymentType()
        {
            _isAddingPaymentType = true;
            PaymentTypeTitle = "Add New Payment Type";
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
            IsPaymentTypeScreenOpen = true;
        }

        [RelayCommand]
        async Task EditPaymentType(object id)
        {
            _isAddingPaymentType = false;
            var paymentId = id as ObjectId?;
            if (paymentId is null) return;
            var dbPaymentType = await _paymentTypeService.GetPaymentTypeById(paymentId.Value);
            PaymentTypeTitle = "Modify Payment Type";
            PaymentTypeDataSource = new()
            {
                PaymentAmount = dbPaymentType.DefaultPaymentAmount.Value,
                Color = dbPaymentType.Colour,
                HavePaymentsEnded = dbPaymentType.HavePaymentsEnded,
                Id = paymentId.Value,
                IsResourceRequired = dbPaymentType.IsResourceRequired,
                Name = dbPaymentType.Name,
                PaymentCategory = dbPaymentType.PaymentCategoryId?.Id
            };
            IsPaymentTypeScreenOpen = true;
        }

        [RelayCommand]
        async Task DeletePaymentType(object id)
        {
            var paymentTypeId = id as ObjectId?;
            if (paymentTypeId is null) return;
            await _paymentTypeService.DeletePaymentType(paymentTypeId.Value);
        }

        public override void OnNavigatedFrom(INavigationParameters parameters)
        {

        }

        public override async void OnNavigatedTo(INavigationParameters parameters)
        {
            PaymentCategoriesDataSource = await _paymentCategoryService.GetAllPaymentCategories();
           DataSource = await _paymentTypeService.GetAllPaymentTypes();
        }
        
    }
}
