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

		public void EnsureUniqueIndexOnEmail()
		{
			// note: I'm not making the index on email required, I'd like to start a conversation around how to ensure indexes
			var email = new IndexKeysBuilder().Ascending("Email");
			var unique = new IndexOptionsBuilder().SetUnique(true);
			Users.EnsureIndex(email, unique);
		}
	}
}