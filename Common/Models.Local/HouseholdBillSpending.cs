using CommunityToolkit.Mvvm.ComponentModel;

namespace Models.Local
{
    public partial class HouseholdBillSpending : ObservableObject
    {

        [ObservableProperty]
        string? name;

        [ObservableProperty]
        double? expectedAmount;

        [ObservableProperty]
        double? amountSpent;

        [ObservableProperty]
        bool paymentTypeActive;

        [ObservableProperty]
        bool showSingleAmountControl;
    }
}
