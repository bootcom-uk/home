using CommunityToolkit.Mvvm.ComponentModel;

namespace Models.Local
{
    public partial class EditablePaymentPeriod : ObservableObject
    {

        [ObservableProperty]
        DateTime? dateTo;

        [ObservableProperty]
        DateTime? dateFrom;


    }
}
