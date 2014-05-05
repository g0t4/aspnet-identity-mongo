namespace IntegrationTests
{
	using AspNet.Identity.MongoDB;
	using Microsoft.AspNet.Identity;
	using NUnit.Framework;

	[TestFixture]
	public class UserPhoneNumberStoreTests : UserIntegrationTestsBase
	{
		private const string PhoneNumber = "1234567890";

		[Test]
		public void SetPhoneNumber_StoresPhoneNumber()
		{
			var user = new IdentityUser {UserName = "bob"};
			var manager = GetUserManager();
			manager.Create(user);

			manager.SetPhoneNumber(user.Id, PhoneNumber);

			Expect(manager.GetPhoneNumber(user.Id), Is.EqualTo(PhoneNumber));
		}

		[Test]
		public void ConfirmPhoneNumber_StoresPhoneNumberConfirmed()
		{
			var user = new IdentityUser {UserName = "bob"};
			var manager = GetUserManager();
			manager.Create(user);
			var token = manager.GenerateChangePhoneNumberToken(user.Id, PhoneNumber);

			manager.ChangePhoneNumber(user.Id, PhoneNumber, token);

			Expect(manager.IsPhoneNumberConfirmed(user.Id));
		}

		[Test]
		public void ChangePhoneNumber_OriginalPhoneNumberWasConfirmed_NotPhoneNumberConfirmed()
		{
			var user = new IdentityUser {UserName = "bob"};
			var manager = GetUserManager();
			manager.Create(user);
			var token = manager.GenerateChangePhoneNumberToken(user.Id, PhoneNumber);
			manager.ChangePhoneNumber(user.Id, PhoneNumber, token);

			manager.SetPhoneNumber(user.Id, PhoneNumber);

			Expect(manager.IsPhoneNumberConfirmed(user.Id), Is.False);
		}
	}
}