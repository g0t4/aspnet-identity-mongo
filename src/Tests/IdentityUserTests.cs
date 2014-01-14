namespace Tests
{
	using AspNet.Identity.MongoDB;
	using MongoDB.Bson;
	using NUnit.Framework;
	using ReflectionMagic;

	[TestFixture]
	public class IdentityUserTests : AssertionHelper
	{
		[Test]
		public void ToBsonDocument_IdAssigned_MapsToBsonObjectId()
		{
			var user = new IdentityUser();
			user.AsDynamic().Id = ObjectId.GenerateNewId().ToString();

			var document = user.ToBsonDocument();

			Expect(document["_id"], Is.TypeOf<BsonObjectId>());
		}

		[Test]
		public void Create_NewIdentityUser_HasIdAssigned()
		{
			var user = new IdentityUser();

			var parsed = user.Id.SafeParseObjectId();
			Expect(parsed, Is.Not.Null);
			Expect(parsed, Is.Not.EqualTo(ObjectId.Empty));
		}
	}
}