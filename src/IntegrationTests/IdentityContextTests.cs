namespace IntegrationTests
{
	using System.Linq;
	using AspNet.Identity.MongoDB;
	using NUnit.Framework;

	[TestFixture]
	public class IdentityContextTests : UserIntegrationTestsBase
	{
		[Test]
		public void Create_NoIndexOnUserName_AddsUniqueIndexOnUserName()
		{
			var userCollectionName = "userindextest";
			Database.DropCollection(userCollectionName);
			var users = Database.GetCollection(userCollectionName);
			new IdentityContext(users);

			var index = users.GetIndexes()
				.Where(i => i.IsUnique)
				.Where(i => i.Key.Count() == 1)
				.First(i => i.Key.Contains("UserName"));
			Expect(index.Key.Count(), Is.EqualTo(1));
		}

		[Test]
		public void CreateEmailUniqueIndex_NoIndexOnEmail_AddsUniqueIndexOnEmail()
		{
			var userCollectionName = "userindextest";
			Database.DropCollection(userCollectionName);
			var users = Database.GetCollection(userCollectionName);
			new IdentityContext(users).EnsureUniqueIndexOnEmail();

			var index = users.GetIndexes()
				.Where(i => i.IsUnique)
				.Where(i => i.Key.Count() == 1)
				.First(i => i.Key.Contains("Email"));
			Expect(index.Key.Count(), Is.EqualTo(1));
		}

		[Test]
		public void Create_NoIndexOnRoleName_AddsUniqueIndexOnRoleName()
		{
			var roleCollectionName = "roleindextest";
			Database.DropCollection(roleCollectionName);
			var roles = Database.GetCollection(roleCollectionName);
			var users = Database.GetCollection("users");
			new IdentityContext(users, roles);

			var index = roles.GetIndexes()
				.Where(i => i.IsUnique)
				.Where(i => i.Key.Count() == 1)
				.First(i => i.Key.Contains("Name"));
			Expect(index.Key.Count(), Is.EqualTo(1));
		}
	}
}