namespace Tests
{
	using System;
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
			user.SetId(ObjectId.GenerateNewId().ToString());

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

		[Test]
		public void Create_NewIdentityUser_RolesNotNull()
		{
			var user = new IdentityUser();

			Expect(user.Roles, Is.Not.Null);
		}

		[Test]
		public void Create_NullRoles_DoesNotSerializeRoles()
		{
			// serialized nulls can cause havoc in deserialization, overwriting the constructor's initial empty list 
			var user = new IdentityUser();
			user.Roles = null;

			var document = user.ToBsonDocument();

			Expect(document.Contains("Roles"), Is.False);
		}

		// todo consider if we want to not serialize the empty Roles array, also empty Logins array

		[Test]
		public void Create_NewIdentityUser_LoginsNotNull()
		{
			var user = new IdentityUser();

			Expect(user.Logins, Is.Not.Null);
		}

		[Test]
		public void Create_NullLogins_DoesNotSerializeLogins()
		{
			// serialized nulls can cause havoc in deserialization, overwriting the constructor's initial empty list 
			var user = new IdentityUser();
			user.Logins = null;

			var document = user.ToBsonDocument();

			Expect(document.Contains("Logins"), Is.False);
		}

		[Test]
		public void Create_NewIdentityUser_ClaimsNotNull()
		{
			var user = new IdentityUser();

			Expect(user.Claims, Is.Not.Null);
		}

		[Test]
		public void Create_NullClaims_DoesNotSerializeClaims()
		{
			// serialized nulls can cause havoc in deserialization, overwriting the constructor's initial empty list 
			var user = new IdentityUser();
			user.Claims = null;

			var document = user.ToBsonDocument();

			Expect(document.Contains("Claims"), Is.False);
		}
	}
}