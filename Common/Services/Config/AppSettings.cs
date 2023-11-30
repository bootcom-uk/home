using CommunityToolkit.Mvvm.ComponentModel;

namespace Services.Config
{
    public partial class AppSettings : ObservableObject
    {

        [ObservableProperty]
        RealmDetails realmDetails;

    }
}
