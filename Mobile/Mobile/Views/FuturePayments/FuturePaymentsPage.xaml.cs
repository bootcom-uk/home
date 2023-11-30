namespace Mobile.Views.FuturePayments;

public partial class FuturePaymentsPage : ContentPage
{
	public FuturePaymentsPage()
	{
		InitializeComponent();
	}

    private void SfListView_SwipeStarting(object sender, Syncfusion.Maui.ListView.SwipeStartingEventArgs e)
    {
        Console.WriteLine("Started");
    }

}
