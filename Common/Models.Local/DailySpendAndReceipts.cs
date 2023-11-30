

using CommunityToolkit.Mvvm.ComponentModel;

namespace Models.Local
{
    public partial class DailySpendAndReceipts : ObservableObject
    {
        [ObservableProperty]
        DateTime date;

        [ObservableProperty]
        double amountSpent;

        [ObservableProperty]
        double amountReceived;

    }
}
