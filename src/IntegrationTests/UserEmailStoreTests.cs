namespace IntegrationTests
{
	using AspNet.Identity.MongoDB;
	using Microsoft.AspNet.Identity;
	using NUnit.Framework;

	[TestFixture]
	public class UserEmailStoreTests : UserIntegrationTestsBase
	{
		[Test]
		public void Create_NewUser_HasNoEmail()
		{
			var user = new IdentityUser {UserName = "bob"};
			var manager = GetUserManager();
			manager.Create(user);

			var email = manager.GetEmail(user.Id);

			Expect(email, Is.Null);
		}

		[Test]
		public void SetEmail_SetsEmail()
		{
			var user = new IdentityUser {UserName = "bob"};
			var manager = GetUserManager();
			manager.Create(user);

			manager.SetEmail(user.Id, "email");

			Expect(manager.GetEmail(user.Id), Is.EqualTo("email"));
		}

		[Test]
		public void FindUserByEmail_ReturnsUser()
		{
			var user = new IdentityUser {UserName = "bob"};
			var manager = GetUserManager();
			manager.Create(user);
			Expect(manager.FindByEmail("email"), Is.Null);

			manager.SetEmail(user.Id, "email");

			Expect(manager.FindByEmail("email"), Is.Not.Null);
		}
	}
}