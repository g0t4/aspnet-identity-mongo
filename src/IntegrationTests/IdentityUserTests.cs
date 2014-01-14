namespace IntegrationTests
{
	using AspNet.Identity.MongoDB;
	using MongoDB.Bson;
	using MongoDB.Driver;
	using NUnit.Framework;
	using ReflectionMagic;
	using Tests;

	[TestFixture]
	public class IdentityUserTests : AssertionHelper
	{
		private MongoDatabase _Database;
		private MongoCollection<IdentityUser> _Users;

		[SetUp]
		public void BeforeEachTest()
		{
			var client = new MongoClient("mongodb://localhost:27017");
			_Database = client.GetServer().GetDatabase("identity-testing");
			_Users = _Database.GetCollection<IdentityUser>("users");

			_Database.DropCollection("users");
		}

		[Test]
		public void Insert_NoId_SetsId()
		{
			var user = new IdentityUser();
			user.AsDynamic().Id = null;

			_Users.Insert(user);

			Expect(user.Id, Is.Not.Null);
			var parsed = user.Id.SafeParseObjectId();
			Expect(parsed, Is.Not.Null);
			Expect(parsed, Is.Not.EqualTo(ObjectId.Empty));
		}
	}
}