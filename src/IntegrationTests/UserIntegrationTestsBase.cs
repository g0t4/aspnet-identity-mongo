namespace IntegrationTests
{
	using AspNet.Identity.MongoDB;
	using Microsoft.AspNet.Identity;
	using MongoDB.Driver;
	using NUnit.Framework;

	public class UserIntegrationTestsBase : AssertionHelper
	{
		protected MongoDatabase Database;
		protected MongoCollection<IdentityUser> Users;
		protected MongoCollection<IdentityRole> Roles;

		// note: for now we'll have interfaces to both the new and old apis for MongoDB, that way we don't have to update all the tests at once and risk introducing bugs
		protected IMongoDatabase DatabaseNewApi;
		private IMongoCollection<IdentityUser> _UsersNewApi;
		private IMongoCollection<IdentityRole> _RolesNewApi;

		[SetUp]
		public void BeforeEachTest()
		{
			var client = new MongoClient("mongodb://localhost:27017");
			var identityTesting = "identity-testing";

			Database = client.GetServer().GetDatabase(identityTesting);
			Users = Database.GetCollection<IdentityUser>("users");
			Roles = Database.GetCollection<IdentityRole>("roles");

			DatabaseNewApi = client.GetDatabase(identityTesting);
			_UsersNewApi = DatabaseNewApi.GetCollection<IdentityUser>("users");
			_RolesNewApi = DatabaseNewApi.GetCollection<IdentityRole>("roles");

			Database.DropCollection("users");
			Database.DropCollection("roles");
		}

		protected UserManager<IdentityUser> GetUserManager()
		{
			var store = new UserStore<IdentityUser>(_UsersNewApi);
			return new UserManager<IdentityUser>(store);
		}

		protected RoleManager<IdentityRole> GetRoleManager()
		{
			var store = new RoleStore<IdentityRole>(_RolesNewApi);
			return new RoleManager<IdentityRole>(store);
		}
	}
}