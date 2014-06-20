namespace AspNet.Identity.MongoDB
{
	using System;
	using global::MongoDB.Driver;

	public class IdentityContext
	{
		public MongoCollection Users { get; set; }
		public MongoCollection Roles { get; set; }

		public IdentityContext()
		{
		}

		public IdentityContext(MongoCollection users)
		{
			Users = users;
		}

		public IdentityContext(MongoCollection users, MongoCollection roles) : this(users)
		{
			Roles = roles;
		}

		[Obsolete("Use IndexChecks.EnsureUniqueIndexOnEmail")]
		public void EnsureUniqueIndexOnEmail()
		{
			IndexChecks.EnsureUniqueIndexOnEmail(Users);
		}
	}
}