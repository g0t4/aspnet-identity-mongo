namespace AspNet.Identity.MongoDB
{
	using System.Collections.Generic;
	using System.Linq;
	using global::MongoDB.Bson;
	using global::MongoDB.Bson.Serialization.Attributes;
	using Microsoft.AspNet.Identity;

	public class IdentityUser : IUser<string>
	{
		public IdentityUser()
		{
			Id = ObjectId.GenerateNewId().ToString();
			Roles = new List<string>();
			Logins = new List<UserLoginInfo>();
		}

		[BsonRepresentation(BsonType.ObjectId)]
		public string Id { get; private set; }

		public string UserName { get; set; }

		public string SecurityStamp { get; set; }

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

		[BsonIgnoreIfNull]
		public List<UserLoginInfo> Logins { get; set; }

		public virtual void AddLogin(UserLoginInfo login)
		{
			Logins.Add(login);
		}

		public virtual void RemoveLogin(UserLoginInfo login)
		{
			var loginsToRemove = Logins
				.Where(l => l.LoginProvider == login.LoginProvider)
				.Where(l => l.ProviderKey == login.ProviderKey);

			Logins = Logins.Except(loginsToRemove).ToList();
		}

		public virtual bool HasPassword()
		{
			return false;
		}
	}
}