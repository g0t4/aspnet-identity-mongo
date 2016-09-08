namespace Microsoft.AspNetCore.Identity.MongoDB
{
	using global::MongoDB.Bson;
	using global::MongoDB.Bson.Serialization.Attributes;

	public class IdentityRole
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
		public string Id { get; set; }

		public string Name { get; set; }

		// todo migration from legacy AspNet.Identity.MongoDB type? At least in docs
		// lookup how normalization is provided OOB and use that for default case
		public string NormalizedName { get; set; }
	}
}