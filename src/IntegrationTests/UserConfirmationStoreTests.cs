namespace IntegrationTests
{
	using AspNet.Identity.MongoDB;
	using Microsoft.AspNet.Identity;
	using NUnit.Framework;

	[TestFixture]
	public class UserConfirmationStoreTests : UserIntegrationTestsBase
	{
		[Test]
		public void Create_NewUser_IsNotConfirmed()
		{
			var manager = GetUserManager();
			var user = new IdentityUser {UserName = "bob"};
			manager.Create(user);

			var isConfirmed = manager.IsConfirmed(user.Id);

			Expect(isConfirmed, Is.False);
		}

		[Test]
		public void SetConfirmed_IsConfirmed()
		{
			var manager = GetUserManager();
			var user = new IdentityUser {UserName = "bob"};
			manager.Create(user);

			manager.SetConfirmed(user.Id, true);

			var isConfirmed = manager.IsConfirmed(user.Id);
			Expect(isConfirmed);
		}

		[Test]
		public void SetUnConfirmed_AlreadyConfirmed_IsNotConfirmed()
		{
			var manager = GetUserManager();
			var user = new IdentityUser {UserName = "bob"};
			manager.Create(user);
			manager.SetConfirmed(user.Id, true);

			manager.SetConfirmed(user.Id, false);

			var isConfirmed = manager.IsConfirmed(user.Id);
			Expect(isConfirmed, Is.False);
		}
	}
}