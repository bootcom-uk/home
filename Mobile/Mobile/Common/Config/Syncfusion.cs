using CommunityToolkit.Mvvm.ComponentModel;

namespace Mobile.Common.Config
{
    public partial class Syncfusion : ObservableObject
    {

        /// <summary>
        /// Specifies the licence key being used for Syncfusion products
        /// </summary>
        /// <remarks>In order to use Syncfusion products</remarks>
        [ObservableProperty]
        string licenceKey;

    }
}
