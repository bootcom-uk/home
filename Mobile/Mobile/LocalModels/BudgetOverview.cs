using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mobile.LocalModels
{
    public partial class BudgetOverview : ObservableObject 
    {

        public enum OverviewType
        {
            TOTAL_OUTSTANDING_PAYMENTS = 0,
            FUTURE_PAYMENTS = 1,
            HOUSEHOLD_BILLS = 2,
            OUTSTANDING_BUDGETS = 3
        }

        [ObservableProperty]
        string information;

        [ObservableProperty]
        OverviewType type;

    }
}
