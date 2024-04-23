using CommunityToolkit.Mvvm.ComponentModel;

namespace Mobile.Common.Config
{
    public partial class Sentry : ObservableObject
    {

        [ObservableProperty]
        string dsn;

    }
}
