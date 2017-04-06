namespace Microsoft.AspNetCore.Identity.MongoDB
{
    using global::MongoDB.Bson;
    using global::MongoDB.Bson.Serialization.Attributes;

    public class IdentityUser : IdentityUserBase
    {
        public IdentityUser()
        {
            Id = ObjectId.GenerateNewId().ToString();
        }

        [BsonRepresentation(BsonType.ObjectId)]
        public override string Id { get; set; }
    }
}
