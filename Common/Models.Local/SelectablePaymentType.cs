using MongoDB.Bson;

namespace Models.Local
{
    public class SelectablePaymentType
    {

        public ObjectId Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public double? PaymentAmount {  get; set; }

        public bool Active { get; set; }

        public bool IsResourceRequired { get; set; }

    }
}
