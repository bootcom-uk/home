using CommunityToolkit.Mvvm.ComponentModel;
using MongoDB.Bson;

namespace Models.Local
{
    public partial class EditablePaymentPeriodBudget : ObservableObject
    {

        [ObservableProperty]
        double? amountReceived;

        [ObservableProperty]
        double? amountSpent;

        [ObservableProperty]
        double budgetRemaining;

        [ObservableProperty]
        double? budget;

        [ObservableProperty]
        string? budgetCategoryInformation;

        [ObservableProperty]
        ObjectId? budgetCategoryId;

    }
}
