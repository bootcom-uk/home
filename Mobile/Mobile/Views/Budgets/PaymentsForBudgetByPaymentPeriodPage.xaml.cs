using Models;
using Syncfusion.Maui.DataSource;
using Syncfusion.Maui.DataSource.Extensions;
using Syncfusion.Maui.ListView;
using System.Collections.Specialized;

namespace Mobile.Views.Budgets;

public partial class PaymentsForBudgetByPaymentPeriodPage : ContentPage
{
	public PaymentsForBudgetByPaymentPeriodPage()
	{
		InitializeComponent();

        lvPayments.Loaded += lvPayments_Loaded;        
        lvPayments.GroupExpanding += lvPayments_GroupExpanding;

        lvPayments.DataSource.SourceCollectionChanged += lvPayments_DataSource_SourceCollectionChanged;

        lvPayments.DataSource.GroupDescriptors.Add(new GroupDescriptor()
        {
            PropertyName = "PaymentTypeId.PaymentCategoryId.Name",
            KeySelector = (object obj1) =>
            {
                var item = (obj1 as Payments);
                if(item.AssociatedResource is null)
                {
                    return item.PaymentTypeId.PaymentCategoryId.Name;
                }
                return $"{item.PaymentTypeId.PaymentCategoryId.Name} ({item.AssociatedResource.Name})";
            }
        });

    }


    internal GroupResult _expandedGroup;

    private void lvPayments_GroupExpanding(object sender, GroupExpandCollapseChangingEventArgs e)
    {
        if (e.Groups.Count > 0)
        {
            var group = e.Groups[0];
            if (_expandedGroup == null || group.Key != _expandedGroup.Key)
            {
                foreach (var otherGroup in lvPayments.DataSource.Groups)
                {
                    if (group.Key != otherGroup.Key)
                    {
                        lvPayments.CollapseGroup(otherGroup);
                    }
                }
                _expandedGroup = group;
                lvPayments.ExpandGroup(_expandedGroup);
            }
        }
    }

    private void lvPayments_Loaded(object sender, ListViewLoadedEventArgs e)
    {
        lvPayments.ExpandAll();
        lvPayments.CollapseAll();
    }

    private void lvPayments_DataSource_SourceCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
        lvPayments.CollapseAll();
    }
}
