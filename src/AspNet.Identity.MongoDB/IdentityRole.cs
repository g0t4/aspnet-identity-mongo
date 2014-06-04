namespace AspNet.Identity.MongoDB
{
	using global::MongoDB.Bson;
	using global::MongoDB.Bson.Serialization.Attributes;
	using Microsoft.AspNet.Identity;

	public class IdentityRole : IRole<string>
	{
		public IdentityRole()
		{
			Id = ObjectId.GenerateNewId().ToString();
		}

		public IdentityRole(string roleName) : this()
		{
			Name = roleName;
		}

		[BsonRepresentation(BsonType.ObjectId)]
		public string Id { get; private set; }

		public string Name { get; set; }
	}
}