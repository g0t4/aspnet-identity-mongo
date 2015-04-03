namespace IntegrationTests
{
	using System.Linq;
	using AspNet.Identity.MongoDB;
	using NUnit.Framework;
    using MongoDB.Bson;
    using MongoDB.Driver;
    using System.Threading.Tasks;

	[TestFixture]
	public class IndexChecksTests : UserIntegrationTestsBase
	{
		[Test]
		public async Task EnsureUniqueIndexOnUserName_NoIndexOnUserName_AddsUniqueIndexOnUserName()
		{
			var userCollectionName = "userindextest";
			await Database.DropCollectionAsync(userCollectionName);
			var users = Database.GetCollection<IdentityUser>(userCollectionName);

			IndexChecks<IdentityUser, IdentityRole>.EnsureUniqueIndexOnUserName(users);

            using (var cursor = await users.Indexes.ListAsync())
            {
                var indexes = await cursor.ToListAsync();
                var index = indexes.Where(i => i.Contains("unique") && i["unique"] == true).Where(i => i["name"].AsString.Contains("UserName"));
                Expect(index.Count(), Is.EqualTo(1));
            }
		}

		[Test]
        public async Task EnsureEmailUniqueIndex_NoIndexOnEmail_AddsUniqueIndexOnEmail()
		{
			var userCollectionName = "userindextest";
			await Database.DropCollectionAsync(userCollectionName);
			var users = Database.GetCollection<IdentityUser>(userCollectionName);

			IndexChecks<IdentityUser, IdentityRole>.EnsureUniqueIndexOnEmail(users);

            using (var cursor = await users.Indexes.ListAsync())
            {
                var indexes = await cursor.ToListAsync();
                var index = indexes.Where(i => i.Contains("unique") && i["unique"] == true).Where(i => i["name"].AsString.Contains("Email"));
                Expect(index.Count(), Is.EqualTo(1));
            }
		}

		[Test]
		public async Task EnsureUniqueIndexOnRoleName_NoIndexOnRoleName_AddsUniqueIndexOnRoleName()
		{
			var roleCollectionName = "roleindextest";
			await Database.DropCollectionAsync(roleCollectionName);
			var roles = Database.GetCollection<IdentityRole>(roleCollectionName);

            IndexChecks<IdentityUser, IdentityRole>.EnsureUniqueIndexOnRoleName(roles);

            using (var cursor = await roles.Indexes.ListAsync())
            {
                var indexes = await cursor.ToListAsync();
                var index = indexes.Where(i => i.Contains("unique") && i["unique"] == true).Where(i => i["name"].AsString.Contains("Name"));
                Expect(index.Count(), Is.EqualTo(1));
            }
		}
	}
}