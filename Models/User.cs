using MongoDB.Bson;
using Realms;

namespace Models
{
    public partial class User : IRealmObject
    {
        [MapTo("_id")]
        [PrimaryKey]
        public ObjectId? Id { get; set; }

        public string? Name { get; set; }    

        public Guid? OriginalId { get; set; }

        public string ImagePath { get; set; }

    }
}
