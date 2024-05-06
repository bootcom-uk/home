using Models.Local;

namespace Mobile.Views.HouseholdBills;

public partial class HouseholdBillsForCurrentPeriodOverviewPage : ContentPage
{
	public HouseholdBillsForCurrentPeriodOverviewPage()
	{
		InitializeComponent();
	}

    private bool _displayOutstandingPayments = false;

    private void swDisplayOutstandingPayments_Toggled(object sender, ToggledEventArgs e)
    {
        _displayOutstandingPayments = e.Value;
        var listView = paymentPeriodListView;
               
        listView.DataSource.Filter = FilterOutstandingPayments;
        listView.DataSource.RefreshFilter();        
    }

    private bool FilterOutstandingPayments(object obj)
    {
        if (!_displayOutstandingPayments) return true;
        return ((HouseholdBillSpending)obj).AmountSpent == 0;
    }

}
