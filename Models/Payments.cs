using MongoDB.Bson;
using Realms;

namespace Models
{
    public partial class Payments : IRealmObject
    {
        [MapTo("_id")]
        [PrimaryKey]
        public ObjectId? Id { get; set; }

        public double? AmountPaid { get; set; }

        public double? AmountReceived { get; set; }

        public User? AssociatedResource { get; set; }

        public User? AddedBy { get; set; }  

        public DateTimeOffset? EndDate { get; set; }

        public DateTimeOffset? DateAdded { get; set; }

        public bool? IsDirectDebit { get; set; }

        public string? Notes { get; set; }

        public int? OriginalId { get; set; }

        public string? PaymentTypeDescription { get; set; }

        public PaymentType? PaymentTypeId { get; set; }

        public string? PaymentTypeName { get; set; }

        public DateTimeOffset? StartDate { get; set; }

        public double? TotalHours { get; set; }

        [Ignored]
        public DateTime LocalStartDate
        {
            get
            {
                return StartDate!.Value.LocalDateTime;
            }
        }

        [Ignored]
        public DateTime LocalEndDate
        {
            get
            {
                return EndDate!.Value.LocalDateTime;
            }
        }


    }
}
