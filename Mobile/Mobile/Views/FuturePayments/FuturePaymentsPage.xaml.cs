using Models;
using Syncfusion.Maui.DataSource;

namespace Mobile.Views.FuturePayments;

public partial class FuturePaymentsPage : ContentPage
{
	public FuturePaymentsPage()
	{
		InitializeComponent();

        lvFuturePayments.DataSource.GroupDescriptors.Add(new GroupDescriptor()
        {
            PropertyName = "PaymentPeriodId",
            KeySelector = (object obj1) =>
            {
                var item = (obj1 as FuturePayment);
                if(item.PaymentPeriodId is null) return "No Payment Period Defined";
                return item.PaymentPeriodId;
            }
        });

    }

    private void SfListView_SwipeStarting(object sender, Syncfusion.Maui.ListView.SwipeStartingEventArgs e)
    {
        Console.WriteLine("Started");
    }

}
