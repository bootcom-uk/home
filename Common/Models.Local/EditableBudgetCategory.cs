using CommunityToolkit.Mvvm.ComponentModel;
using MongoDB.Bson;

namespace Models.Local
{
    public partial class EditableBudgetCategory : ObservableObject
    {

        [ObservableProperty]
        ObjectId id;

        [ObservableProperty]
        double budget;

    }
}
