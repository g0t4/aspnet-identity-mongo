namespace AspNet.Identity.MongoDB
{
	using System.Collections.Generic;
	using global::MongoDB.Bson;
	using global::MongoDB.Bson.Serialization.Attributes;
	using Microsoft.AspNet.Identity;

	public class IdentityUser : IUser<string>
	{
		public IdentityUser()
		{
			Id = ObjectId.GenerateNewId().ToString();
			Roles = new List<string>();
		}

		[BsonRepresentation(BsonType.ObjectId)]
		public string Id { get; private set; }

		public string UserName { get; set; }

		[BsonIgnoreIfNull]
		public List<string> Roles { get; set; }

		public virtual void AddRole(string role)
		{
			Roles.Add(role);
		}

		public virtual void RemoveRole(string role)
		{
			Roles.Remove(role);
		}

		[BsonIgnoreIfNull]
		public virtual string PasswordHash { get; set; }

		public virtual bool HasPassword()
		{
			return false;
		}
	}
}