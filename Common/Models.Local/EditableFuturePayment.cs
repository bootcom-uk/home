using CommunityToolkit.Mvvm.ComponentModel;
using MongoDB.Bson;

namespace Models.Local
{
    public partial class EditableFuturePayment : ObservableObject
    {

        [ObservableProperty]
        ObjectId id;

        [ObservableProperty]
        DateTime futurePaymentDate;

        [ObservableProperty]
        string? futurePaymentDetail;

        [ObservableProperty]
        double futurePaymentAmount;

    }
}
