using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Configuration;
using Models;
using Models.Local;
using MongoDB.Bson;
using Services;
using Services.DataServices;
using System.ComponentModel;

namespace Mobile.ViewModels
{
    public abstract partial class ViewModelBase : ObservableObject, INavigationAware
    {

        [ObservableProperty]
        string title;

        [ObservableProperty]
        bool isRefreshing;

        [ObservableProperty]
        bool isProcessing;

        internal IConfiguration _configuration { get; }

        internal ISemanticScreenReader _screenReader { get; }

        internal INavigationService _navigationService { get; }

        internal PaymentTypeService _paymentTypeService { get; }

        internal PaymentsService _paymentsService {  get; }

        internal UsersService _usersService { get; }

        internal RealmService _realmService { get; }

        public ViewModelBase(IConfiguration configuration, ISemanticScreenReader screenReader, INavigationService navigationService, PaymentTypeService paymentTypeService, UsersService usersService, RealmService realmService, PaymentsService paymentsService)
        {
            SelectedPaymentTypeId = null;

            _configuration = configuration;
            _screenReader = screenReader;
            _navigationService = navigationService;
            _paymentTypeService = paymentTypeService;            
            _usersService = usersService;
            _realmService = realmService;
            _paymentsService = paymentsService;

            CurrentScreen = InternalSettings.CurrentScreen;

        }

        public abstract void OnNavigatedFrom(INavigationParameters parameters);

        public abstract void OnNavigatedTo(INavigationParameters parameters);

        #region "Navigation"

        [ObservableProperty]
        string navigationScreenName;

        [ObservableProperty]
        bool isMenuOpen;

        [ObservableProperty]
        string currentScreen;

        [RelayCommand]
        void ShowNavigationMenu()
        {
            IsMenuOpen = true;
        }

        [RelayCommand]
        async Task Navigate(string pageName)
        {
            IsMenuOpen = false;
            InternalSettings.CurrentScreen = pageName;
            await _navigationService.NavigateAsync($"{pageName}");
        }

        #endregion

        #region "Payments"

        [ObservableProperty]
        List<SelectablePaymentType> paymentTypeDataSource;

        [ObservableProperty]
        List<User> usersDataSource;

        [ObservableProperty]
        bool isPaymentScreenOpen;

        [ObservableProperty]
        string paymentTypeTitle;

        [ObservableProperty]
        bool isOutgoingPayment;

        [ObservableProperty]
        double paymentAmount;

        [ObservableProperty]
        bool displayArchivedRecords;

        [ObservableProperty]
        ObjectId? selectedPaymentId;

        [ObservableProperty]
        ObjectId? selectedPaymentTypeId;

        [ObservableProperty]
        DateTime selectedPaymentDate;

        [ObservableProperty]
        bool selectedPaymentTypeResourceRequired;

        [ObservableProperty]
        ObjectId? selectedPaymentTypeResource;

        [ObservableProperty]
        string paymentInfo;

        [ObservableProperty]
        string savingPaymentMessage;

        [RelayCommand]
        void PaymentDateChanged(DateTime newDateTime)
        {
            SelectedPaymentDate = newDateTime;
        }

        [RelayCommand]
        async Task SavePayment()
        {
            SavingPaymentMessage = "This record is being saved, please wait";

            NavigationScreenName = "SavingPayment";

            var associatedResource = _realmService.Realm.All<User>()
                .FirstOrDefault(record => record.Id == SelectedPaymentTypeResource);

            var selectedPaymentType = _realmService.Realm.All<PaymentType>()
                .FirstOrDefault(record => record.Id == SelectedPaymentTypeId);

            var addedByUser = _realmService.Realm.All<User>()
                .FirstOrDefault(record => record.OriginalId == InternalSettings.UserId);

            var amountPaid = 0d;

            var amountReceived = 0d;

            amountReceived = PaymentAmount;

            if (IsOutgoingPayment)
            {
                amountPaid = PaymentAmount;
                amountReceived = 0d;
            }

            await _paymentsService.AddPayment(new Payments()
            {
                DateAdded = new(DateTime.Now),
                StartDate = new(SelectedPaymentDate),
                EndDate = new(SelectedPaymentDate.AddDays(1).AddMicroseconds(-1)),
                IsDirectDebit = false,
                PaymentTypeDescription = PaymentInfo,
                AssociatedResource = associatedResource,
                Id = SelectedPaymentId,
                PaymentTypeId = selectedPaymentType,
                PaymentTypeName = selectedPaymentType.Name,
                AmountPaid = amountPaid,
                AmountReceived = amountReceived,
                AddedBy = addedByUser
            });

            SavingPaymentMessage = "This payment has been successfully saved. This screen will close shortly";
            await Task.Delay(TimeSpan.FromSeconds(3));

            IsPaymentScreenOpen = false;
        }

        [RelayCommand]
        void PaymentAmountSelected()
        {
            NavigationScreenName = "SelectPaymentDate";
        }

        [RelayCommand]
        void PaymentResourceSelected(ObjectId? resourceId)
        {
            SelectedPaymentTypeResource = resourceId;
        }

        [RelayCommand]
        void PaymentTypeResourceSelected()
        {
            NavigationScreenName = "SelectPaymentAmount";
        }

        [RelayCommand]
        void PaymentTypeSelected()
        {
            var paymentType = PaymentTypeDataSource.
                First(record => record.Id == SelectedPaymentTypeId);
            PaymentInfo = paymentType.Name;
            if (paymentType.PaymentAmount != null)
            {
                PaymentAmount = paymentType.PaymentAmount.Value;
            }
            SelectedPaymentTypeResourceRequired = paymentType.IsResourceRequired;
            if (SelectedPaymentTypeResourceRequired)
            {
                UsersDataSource = _usersService.ListUsers();
                NavigationScreenName = "SelectPaymentResource";
                return;
            }
            NavigationScreenName = "SelectPaymentAmount"; 
        }

        [RelayCommand]
        void ChoosePaymentType(ObjectId? id)
        {
            if(SelectedPaymentTypeId == id)
            {
                SelectedPaymentTypeId = null;
                return;
            }
            SelectedPaymentTypeId = id;            
        }

        [RelayCommand]
        void SelectPaymentType(string paymentType)
        {
            PaymentTypeDataSource = _paymentTypeService.GetSelectablePaymentTypes(DisplayArchivedRecords);
            IsOutgoingPayment = (paymentType == "Outgoing");
            NavigationScreenName = "SelectPaymentType";
        }

        [RelayCommand]
        void DisplayNewPayment()
        {
            IsMenuOpen = false;
            PaymentTypeTitle = "Add New Payment";
            NavigationScreenName = "SelectIncomingOutgoingType";
            SelectedPaymentId = ObjectId.GenerateNewId();
            SelectedPaymentTypeId = null;
            PaymentAmount = 0;
            DisplayArchivedRecords = false;
            IsPaymentScreenOpen = true;
            SelectedPaymentTypeResourceRequired = false;
            SelectedPaymentTypeResource = null;
            SelectedPaymentDate = DateTime.Today;
        }

        #endregion

        #region "Modify Payments"

        [ObservableProperty]
        bool isModifyTransactionScreenOpen;

        [ObservableProperty]
        EditableModifyPayment modifyPaymentDataSource;

        #endregion

        protected override void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);

            switch (e.PropertyName)
            {
                case nameof(DisplayArchivedRecords):
                    PaymentTypeDataSource = _paymentTypeService.GetSelectablePaymentTypes(DisplayArchivedRecords);
                    break;
            }

            
        }
    }
}
