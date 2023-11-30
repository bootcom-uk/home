using CommunityToolkit.Mvvm.ComponentModel;

namespace Services.Config
{
    public partial class RealmDetails : ObservableObject 
    {

        [ObservableProperty]
        string appId;

        [ObservableProperty]
        string emailAddress;

        [ObservableProperty]
        string password;

    }
}
