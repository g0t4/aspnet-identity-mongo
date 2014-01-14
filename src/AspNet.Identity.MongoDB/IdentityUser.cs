namespace AspNet.Identity.MongoDB
{
	using global::MongoDB.Bson;
	using global::MongoDB.Bson.Serialization.Attributes;
	using Microsoft.AspNet.Identity;

	public class IdentityUser : IUser<string>
	{
		public IdentityUser()
		{
			Id = ObjectId.GenerateNewId().ToString();
		}

		[BsonRepresentation(BsonType.ObjectId)]
		public string Id { get; private set; }

		public string UserName { get; set; }
	}
}