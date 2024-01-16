using CommunityToolkit.Mvvm.ComponentModel;
using MongoDB.Bson;

namespace Models.Local
{
    public partial class EditablePaymentPeriod : ObservableObject
    {

        [ObservableProperty]
        ObjectId? id;

        [ObservableProperty]
        DateTime? dateTo;

        [ObservableProperty]
        DateTime? dateFrom;

        [ObservableProperty]
        List<EditablePaymentPeriodBudget>? budgets;

    }
}
