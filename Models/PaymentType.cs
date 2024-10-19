using MongoDB.Bson;
using Realms;

namespace Models
{
    public partial class PaymentType : IRealmObject
    {
        [MapTo("_id")]
        [PrimaryKey]
        public ObjectId Id { get; set; }
        public int Colour { get; set; }
        public double? DefaultPaymentAmount { get; set; }
        public bool? DisplayAsFuturePayment { get; set; }
        public bool HavePaymentsEnded { get; set; }
        public bool IsResourceRequired { get; set; }
        public required string Name { get; set; }
        public int? OriginalId { get; set; }
        public int? OriginalPaymentCategoryId { get; set; }
        public PaymentCategory? PaymentCategoryId { get; set; }
        public DateTimeOffset? PaymentEndDate { get; set; }
        public PaymentType_PaymentSchedule? PaymentSchedule { get; set; }
        public DateTimeOffset? PaymentStartDate { get; set; }
        public double? PaymentTypeTotal { get; set; }
    }
}
