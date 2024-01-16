using CommunityToolkit.Mvvm.ComponentModel;
using MongoDB.Bson;

namespace Models.Local
{
    public partial class EditableModifyPayment : ObservableObject 
    {

        [ObservableProperty]
        ObjectId? id;

        [ObservableProperty]
        string? paymentType;

        [ObservableProperty]
        DateTime appointmentDateTime;

        [ObservableProperty]
        string? paymentInformation;

        [ObservableProperty]
        double paymentAmount;

    }
}
