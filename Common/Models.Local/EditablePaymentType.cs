using CommunityToolkit.Mvvm.ComponentModel;
using MongoDB.Bson;

namespace Models.Local
{
    public partial class EditablePaymentType : ObservableObject
    {

        [ObservableProperty]
        ObjectId id;

        [ObservableProperty]
        ObjectId? paymentCategory;

        [ObservableProperty]
        string name;

        [ObservableProperty]
        double paymentAmount;

        [ObservableProperty]
        int color;

        [ObservableProperty]
        bool havePaymentsEnded;

        [ObservableProperty]
        bool isResourceRequired;

    }
}
