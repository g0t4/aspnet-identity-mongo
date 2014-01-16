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
			var collectionName = "userindextest";
			Database.DropCollection(collectionName);
			var collection = Database.GetCollection(collectionName);
			new IdentityContext(collection);

			var index = collection.GetIndexes()
				.Where(i => i.IsUnique)
				.Where(i => i.Key.Count() == 1)
				.First(i => i.Key.Contains("UserName"));
			Expect(index.Key.Count(), Is.EqualTo(1));
		}

		[Test]
		public void CreateEmailUniqueIndex_NoIndexOnEmail_AddsUniqueIndexOnEmail()
		{
			var collectionName = "userindextest";
			Database.DropCollection(collectionName);
			var collection = Database.GetCollection(collectionName);
			new IdentityContext(collection).EnsureUniqueIndexOnEmail();

			var index = collection.GetIndexes()
				.Where(i => i.IsUnique)
				.Where(i => i.Key.Count() == 1)
				.First(i => i.Key.Contains("Email"));
			Expect(index.Key.Count(), Is.EqualTo(1));
		}
	}
}