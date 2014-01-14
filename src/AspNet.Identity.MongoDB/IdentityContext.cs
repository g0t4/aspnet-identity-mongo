namespace AspNet.Identity.MongoDB
{
	using global::MongoDB.Driver;

	public class IdentityContext
	{
		public MongoCollection Users { get; private set; }

		public IdentityContext(MongoCollection users)
		{
			Users = users;
		}
	}
}