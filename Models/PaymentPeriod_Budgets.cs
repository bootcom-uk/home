using MongoDB.Bson;
using Realms;

namespace Models
{
    public partial class PaymentPeriod_Budgets : IEmbeddedObject
    {
        public double? AmountReceived { get; set; }

        public double? AmountSpent { get; set; }

        public double BudgetRemaining { get; set; }

        public double? Budget { get; set; }

        public BudgetCategories? BudgetCategoryId { get; set; }
    }
}
