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
		protected IdentityContext IdentityContext;

		[SetUp]
		public void BeforeEachTest()
		{
			var client = new MongoClient("mongodb://localhost:27017");
			Database = client.GetServer().GetDatabase("identity-testing");
			Users = Database.GetCollection<IdentityUser>("users");
			Roles = Database.GetCollection<IdentityRole>("roles");
			IdentityContext = new IdentityContext(Users, Roles);

			Database.DropCollection("users");
			Database.DropCollection("roles");
		}

		protected UserManager<IdentityUser> GetUserManager()
		{
			var store = new UserStore<IdentityUser>(IdentityContext);
			return new UserManager<IdentityUser>(store);
		}

		protected RoleManager<IdentityRole> GetRoleManager()
		{
			var store = new RoleStore<IdentityRole>(IdentityContext);
			return new RoleManager<IdentityRole>(store);
		}
	}
}