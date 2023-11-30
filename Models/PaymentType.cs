using MongoDB.Bson;
using Realms;

namespace Models
{
    public partial class PaymentType : IRealmObject
    {
        [MapTo("_id")]
        [PrimaryKey]
        public ObjectId Id { get; set; }

        public int? OriginalId { get; set; }

        public string Name { get; set; } = string.Empty;

        public PaymentCategory? PaymentCategoryId { get; set; }

        public int? OriginalPaymentCategoryId { get; set; }

        public int Colour { get; set; }

        public double? DefaultPaymentAmount { get; set; }

        public bool HavePaymentsEnded { get; set; }

        public bool IsResourceRequired { get; set; }
    }
}
