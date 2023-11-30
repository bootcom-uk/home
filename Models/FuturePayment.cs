using MongoDB.Bson;
using Realms;

namespace Models
{
    public partial class FuturePayment : IRealmObject
    {

        [MapTo("_id")]
        [PrimaryKey]
        public ObjectId? Id { get; set; }

        public DateTimeOffset PaymentRequiredDate { get; set; }

        public double PaymentExpectedAmount { get; set; }

        public string? PaymentInformation { get; set; }

        public User? ResourceId { get; set; }

        public PaymentPeriod? PaymentPeriodId { get; set; }

    }
}
