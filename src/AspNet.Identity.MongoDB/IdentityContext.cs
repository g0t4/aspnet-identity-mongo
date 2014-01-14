namespace AspNet.Identity.MongoDB
{
	using global::MongoDB.Driver;
	using global::MongoDB.Driver.Builders;

	public class IdentityContext
	{
		public MongoCollection Users { get; private set; }

		public IdentityContext(MongoCollection users)
		{
			Users = users;
			EnsureUniqueIndexOnUserName(users);
		}

		private void EnsureUniqueIndexOnUserName(MongoCollection users)
		{
			var userName = new IndexKeysBuilder().Ascending("UserName");
			var unique = new IndexOptionsBuilder().SetUnique(true);
			users.EnsureIndex(userName, unique);
		}
	}
}