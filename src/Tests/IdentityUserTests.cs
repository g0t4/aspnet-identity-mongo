namespace Tests
{
	using AspNet.Identity.MongoDB;
	using MongoDB.Bson;
	using NUnit.Framework;

	[TestFixture]
	public class IdentityUserTests : AssertionHelper
	{
		[Test]
		public void ToBsonDocument_IdAssigned_MapsToBsonObjectId()
		{
			var user = new IdentityUser();
			user.SetUserId(ObjectId.GenerateNewId().ToString());

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

		[Test]
		public void Create_NoPassword_DoesNotSerializePasswordField()
		{
			// if a particular consuming application doesn't intend to use passwords, there's no reason to store a null entry except for padding concerns, if that is the case then the consumer may want to create a custom class map to serialize as desired.

			var user = new IdentityUser();

			var document = user.ToBsonDocument();

			Expect(document.Contains("PasswordHash"), Is.False);
		}
	}
}