using MongoDB.Bson;
using Realms;

namespace Models
{
    public partial class BudgetCategories : IRealmObject
    {
        [MapTo("_id")]
        [PrimaryKey]
        public ObjectId? Id { get; set; }

        public int? OriginalId { get; set; }

        public PaymentCategory? PaymentCategoryId { get; set; }

        public int? OriginalPaymentCategoryId { get; set; }

        public double? DefaultBudget { get; set; }

        public User? AssociatedResource { get; set; }
    }
}
