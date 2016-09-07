namespace Tests
{
	using AspNet.Identity.MongoDB;
	using MongoDB.Bson;
	using NUnit.Framework;

	[TestFixture]
	public class IdentityRoleTests : AssertionHelper
	{
		[Test]
		public void ToBsonDocument_IdAssigned_MapsToBsonObjectId()
		{
			var role = new IdentityRole();
			role.SetId(ObjectId.GenerateNewId().ToString());

			var document = role.ToBsonDocument();

			Expect(document["_id"], Is.TypeOf<BsonObjectId>());
		}

		[Test]
		public void Create_WithoutRoleName_HasIdAssigned()
		{
			var role = new IdentityRole();

			var parsed = role.Id.SafeParseObjectId();
			Expect(parsed, Is.Not.Null);
			Expect(parsed, Is.Not.EqualTo(ObjectId.Empty));
		}

		[Test]
		public void Create_WithRoleName_SetsName()
		{
			var name = "admin";

			var role = new IdentityRole(name);

			Expect(role.Name, Is.EqualTo(name));
		}

		[Test]
		public void Create_WithRoleName_SetsId()
		{
			var role = new IdentityRole("admin");

			var parsed = role.Id.SafeParseObjectId();
			Expect(parsed, Is.Not.Null);
			Expect(parsed, Is.Not.EqualTo(ObjectId.Empty));
		}
	}
}