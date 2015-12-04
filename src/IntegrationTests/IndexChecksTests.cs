namespace IntegrationTests
{
	using System.Linq;
	using AspNet.Identity.MongoDB;
	using MongoDB.Driver;
	using NUnit.Framework;

	[TestFixture]
	public class IndexChecksTests : UserIntegrationTestsBase
	{
		[Test]
		public void EnsureUniqueIndexOnUserName_NoIndexOnUserName_AddsUniqueIndexOnUserName()
		{
			var userCollectionName = "userindextest";
			Database.DropCollectionAsync(userCollectionName).Wait();
			var usersNewApi = DatabaseNewApi.GetCollection<IdentityUser>(userCollectionName);

			IndexChecks.EnsureUniqueIndexOnUserName(usersNewApi);

            var indexes = usersNewApi.Indexes.ListAsync().Result.ToListAsync().Result;
            var index = indexes.FirstOrDefault(i => i["key"].AsBsonDocument.Contains("UserName"));
            Expect(index, Is.Not.Null);
            Expect(index["unique"].AsBoolean, Is.True);
            Expect(index["key"].AsBsonDocument.ElementCount, Is.EqualTo(1));
		}

		[Test]
		public void EnsureEmailUniqueIndex_NoIndexOnEmail_AddsUniqueIndexOnEmail()
		{
			var userCollectionName = "userindextest";
			Database.DropCollectionAsync(userCollectionName).Wait();
			var usersNewApi = DatabaseNewApi.GetCollection<IdentityUser>(userCollectionName);

			IndexChecks.EnsureUniqueIndexOnEmail(usersNewApi);

            var indexes = usersNewApi.Indexes.ListAsync().Result.ToListAsync().Result;
            var index = indexes.FirstOrDefault(i => i["key"].AsBsonDocument.Contains("Email"));
            Expect(index, Is.Not.Null);
            Expect(index["unique"].AsBoolean, Is.True);
            Expect(index["key"].AsBsonDocument.ElementCount, Is.EqualTo(1));
		}

		[Test]
		public void EnsureUniqueIndexOnRoleName_NoIndexOnRoleName_AddsUniqueIndexOnRoleName()
		{
			var roleCollectionName = "roleindextest";
			Database.DropCollectionAsync(roleCollectionName).Wait();
			var rolesNewApi = DatabaseNewApi.GetCollection<IdentityRole>(roleCollectionName);

			IndexChecks.EnsureUniqueIndexOnRoleName(rolesNewApi);

            var indexes = rolesNewApi.Indexes.ListAsync().Result.ToListAsync().Result;
            var index = indexes.FirstOrDefault(i => i["key"].AsBsonDocument.Contains("Name"));
            Expect(index, Is.Not.Null);
            Expect(index["unique"].AsBoolean, Is.True);
            Expect(index["key"].AsBsonDocument.ElementCount, Is.EqualTo(1));
		}
	}
}