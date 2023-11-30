using MongoDB.Bson;
using Realms;

namespace Models
{
    public partial class PaymentCategory : IRealmObject
    {
        [MapTo("_id")]
        [PrimaryKey]
        public ObjectId? Id { get; set; }

        public string? Name { get; set; }

        public int? OriginalId { get; set; }
    }
}
