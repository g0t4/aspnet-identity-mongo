namespace IntegrationTests
{
	using System;
	using Microsoft.AspNetCore.Builder;
	using Microsoft.AspNetCore.Identity;
	using Microsoft.AspNetCore.Identity.MongoDB;
	using Microsoft.Extensions.DependencyInjection;
	using Microsoft.Extensions.Logging;
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
		protected IServiceProvider ServiceProvider;

		[SetUp]
		public void BeforeEachTest()
		{
			var client = new MongoClient("mongodb://localhost:27017");
			var identityTesting = "identity-testing";

			// todo move away from GetServer which could be deprecated at some point
			Database = client.GetServer().GetDatabase(identityTesting);
			Users = Database.GetCollection<IdentityUser>("users");
			Roles = Database.GetCollection<IdentityRole>("roles");

			DatabaseNewApi = client.GetDatabase(identityTesting);
			_UsersNewApi = DatabaseNewApi.GetCollection<IdentityUser>("users");
			_RolesNewApi = DatabaseNewApi.GetCollection<IdentityRole>("roles");

			Database.DropCollection("users");
			Database.DropCollection("roles");

			ServiceProvider = CreateServiceProvider<IdentityUser, IdentityRole>();
		}

		protected UserManager<IdentityUser> GetUserManager()
			=> ServiceProvider.GetService<UserManager<IdentityUser>>();

		protected RoleManager<IdentityRole> GetRoleManager()
			=> ServiceProvider.GetService<RoleManager<IdentityRole>>();

		protected IServiceProvider CreateServiceProvider<TUser, TRole>(Action<IdentityOptions> optionsProvider = null)
			where TUser : IdentityUser
			where TRole : IdentityRole
		{
			var services = new ServiceCollection();
			optionsProvider = optionsProvider ?? (options => { });
			services.AddIdentity<TUser, TRole>(optionsProvider)
				.AddDefaultTokenProviders();

			var roles = DatabaseNewApi.GetCollection<TRole>("roles");
			var roleStore = new RoleStore<TRole>(roles);
			services.AddSingleton<IRoleStore<TRole>>(roleStore);

			var users = DatabaseNewApi.GetCollection<TUser>("users");
			var userStore = new UserStore<TUser>(users);
			services.AddSingleton<IUserStore<TUser>>(userStore);

			services.AddLogging();
			services.AddSingleton<ILogger<UserManager<TUser>>>(new TestLogger<UserManager<TUser>>());
			services.AddSingleton<ILogger<RoleManager<TRole>>>(new TestLogger<RoleManager<TRole>>());

			return services.BuildServiceProvider();
		}

		public class TestLogger<TName> : ILogger<TName>
		{
			public IDisposable BeginScope<TState>(TState state)
			{
				return null;
			}

			public bool IsEnabled(LogLevel logLevel)
			{
				return true;
			}

			public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
			{
			}
		}
	}
}