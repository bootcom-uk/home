using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Extensions;
using Microsoft.Extensions.Configuration;
using Models;
using Models.Local;
using MongoDB.Bson;
using Services;
using Services.DataServices;
using System.ComponentModel;

namespace Mobile.ViewModels.Budgets
{
    public partial class BudgetsPageViewModel : ViewModelBase
    {

        internal BudgetsService _budgetsService { get; }

        [ObservableProperty]
        IQueryable<BudgetCategories> dataSource;

        [ObservableProperty, DefaultValue(false)]
        bool isModifyBudgetScreenOpen;

        [ObservableProperty, DefaultValue("")]
        string modifyBudgetScreenTitle;

        [ObservableProperty]
        EditableBudgetCategory popupDataSource;

        public BudgetsPageViewModel(IConfiguration configuration, ISemanticScreenReader screenReader, INavigationService navigationService, PaymentTypeService paymentTypeService, UsersService usersService, RealmService realmService, PaymentsService paymentsService, BudgetsService budgetsService) : base(configuration, screenReader, navigationService, paymentTypeService, usersService, realmService, paymentsService)
        {
            _budgetsService = budgetsService;
            Title = "Budgets";
        }

        [RelayCommand]
        async Task SaveBudget()
        {
            IsProcessing = true;
            var record = await _budgetsService.GetBudgetById(PopupDataSource.Id);
            var newRecord = record.Clone();
            newRecord.DefaultBudget = PopupDataSource.Budget;
            await _budgetsService.UpdateBudget(newRecord);
            IsModifyBudgetScreenOpen = false;
            IsProcessing = false;
        }

        [RelayCommand]
        async Task ModifyBudget(object id)
        {
            IsProcessing = false;
            var budgetId = id as ObjectId?;
            if (budgetId is null) return;
            var record = await _budgetsService.GetBudgetById(budgetId);
            ModifyBudgetScreenTitle = $"Modify Budget {record.PaymentCategoryId.Name}";

            PopupDataSource = new()
            {
                Id = record.Id.Value,
                Budget = record.DefaultBudget.Value
            };

            IsModifyBudgetScreenOpen = true;
        }

        public override void OnNavigatedFrom(INavigationParameters parameters)
        {
            
        }

        public override async void OnNavigatedTo(INavigationParameters parameters)
        {
            DataSource = await _budgetsService.CollectDefaultBudgets();
        }
    }
}
