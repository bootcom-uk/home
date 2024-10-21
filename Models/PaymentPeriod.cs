using MongoDB.Bson;
using Realms;

namespace Models
{
    public partial class PaymentPeriod : IRealmObject
    {
        [MapTo("_id")]
        [PrimaryKey]
        public ObjectId? Id { get; set; }

        public DateTimeOffset? DateFrom { get; set; }

        public DateTimeOffset? DateTo { get; set; }

        public int? OriginalId { get; set; }

        public IList<PaymentPeriod_Budgets> Budgets { get; }

        public override string ToString()
        {
            return $"Payment period {DateFrom?.LocalDateTime.ToString("dd/MM/yyyy")} to {DateTo?.LocalDateTime.ToString("dd/MM/yyyy")}";
        }

    }
}
