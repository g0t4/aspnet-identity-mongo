namespace AspNet.Identity.MongoDB
{
	using global::MongoDB.Driver;
	using global::MongoDB.Driver.Builders;

	public class IndexChecks
	{
		public static void EnsureUniqueIndexOnUserName(MongoCollection users)
		{
			var userName = new IndexKeysBuilder<IdentityUser>().Ascending(t => t.UserName);
			var unique = new IndexOptionsBuilder().SetUnique(true);
			users.CreateIndex(userName, unique);
		}

		public static void EnsureUniqueIndexOnRoleName(MongoCollection roles)
		{
			var roleName = new IndexKeysBuilder<IdentityRole>().Ascending(t => t.Name);
			var unique = new IndexOptionsBuilder().SetUnique(true);
			roles.CreateIndex(roleName, unique);
		}

		public static void EnsureUniqueIndexOnEmail(MongoCollection users)
		{
			var email = new IndexKeysBuilder<IdentityUser>().Ascending(t => t.Email);
			var unique = new IndexOptionsBuilder().SetUnique(true);
			users.CreateIndex(email, unique);
		}
	}
}