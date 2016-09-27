namespace CoreTests
{
	using Microsoft.AspNetCore.Identity;
	using Microsoft.AspNetCore.Identity.MongoDB;
	using Microsoft.Extensions.DependencyInjection;
	using NUnit.Framework;

	[TestFixture]
	public class MongoIdentityBuilderExtensionsTests : AssertionHelper
	{
		private const string FakeConnectionStringWithDatabase = "mongodb://fakehost:27017/database";

		[Test]
		public void AddMongoStores_ResolvesStoresAndManagers()
		{
			var services = new ServiceCollection();

			services
				.AddIdentity<IdentityUser, IdentityRole>()
				.AddMongoStores<IdentityUser, IdentityRole>(FakeConnectionStringWithDatabase);

			// note: UserManager and RoleManager use logging
			services.AddLogging();

			var provider = services.BuildServiceProvider();
			var resolvedUserStore = provider.GetService<IUserStore<IdentityUser>>();
			Expect(resolvedUserStore, Is.Not.Null, "User store did not resolve");

			var resolvedRoleStore = provider.GetService<IRoleStore<IdentityRole>>();
			Expect(resolvedRoleStore, Is.Not.Null, "Role store did not resolve");

			var resolvedUserManager = provider.GetService<UserManager<IdentityUser>>();
			Expect(resolvedUserManager, Is.Not.Null, "User manager did not resolve");
		}

		[Test]
		public void AddMongoStores_ConnectionStringWithoutDatabase_Throws()
		{
			var connectionStringWithoutDatabase = "mongodb://fakehost";

			TestDelegate addMongoStores = () => new ServiceCollection()
				.AddIdentity<IdentityUser, IdentityRole>()
				.AddMongoStores<IdentityUser, IdentityRole>(connectionStringWithoutDatabase);

			Expect(addMongoStores, Throws.Exception
				.With.Message.Contains("Your connection string must contain a database name"));
		}

		protected class WrongUser : IdentityUser
		{
		}

		protected class WrongRole : IdentityRole
		{
		}

		[Test]
		public void AddMongoStores_MismatchedTypes_ThrowsWarningToHelpUsers()
		{
			Expect(() => new ServiceCollection()
					.AddIdentity<IdentityUser, IdentityRole>()
					.AddMongoStores<WrongUser, IdentityRole>(FakeConnectionStringWithDatabase),
				Throws.Exception.With.Message
					.EqualTo("User type passed to AddMongoStores must match user type passed to AddIdentity. You passed Microsoft.AspNetCore.Identity.MongoDB.IdentityUser to AddIdentity and CoreTests.MongoIdentityBuilderExtensionsTests+WrongUser to AddMongoStores, these do not match.")
			);

			Expect(() => new ServiceCollection()
					.AddIdentity<IdentityUser, IdentityRole>()
					.AddMongoStores<IdentityUser, WrongRole>(FakeConnectionStringWithDatabase),
				Throws.Exception.With.Message
					.EqualTo("Role type passed to AddMongoStores must match role type passed to AddIdentity. You passed Microsoft.AspNetCore.Identity.MongoDB.IdentityRole to AddIdentity and CoreTests.MongoIdentityBuilderExtensionsTests+WrongRole to AddMongoStores, these do not match.")
			);
		}
	}
}