namespace IntegrationTests
{
    using AspNet.Identity.MongoDB;
    using Microsoft.AspNet.Identity;
    using MongoDB.Driver;
    using NUnit.Framework;

    public class UserIntegrationTestsBase : AssertionHelper
    {
        protected IMongoDatabase Database;
        protected IMongoCollection<IdentityUser> Users;
        protected IMongoCollection<IdentityRole> Roles;

        [SetUp]
        public void BeforeEachTest()
        {
            var client = new MongoClient("mongodb://localhost:27017");
            Database = client.GetDatabase("identity-testing");
            Users = Database.GetCollection<IdentityUser>("users");
            Roles = Database.GetCollection<IdentityRole>("roles");

            Database.DropCollectionAsync("users");
            Database.DropCollectionAsync("roles");
        }

        protected UserManager<IdentityUser> GetUserManager()
        {
            var store = new UserStore<IdentityUser>(Users);
            return new UserManager<IdentityUser>(store);
        }

        protected RoleManager<IdentityRole> GetRoleManager()
        {
            var store = new RoleStore<IdentityRole>(Roles);
            return new RoleManager<IdentityRole>(store);
        }
    }
}