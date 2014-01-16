namespace IntegrationTests
{
	using System.Linq;
	using AspNet.Identity.MongoDB;
	using Microsoft.AspNet.Identity;
	using NUnit.Framework;

	[TestFixture]
	public class UserLoginStoreTests : UserIntegrationTestsBase
	{
		[Test]
		public void AddLogin_NewLogin_Adds()
		{
			var manager = GetUserManager();
			var login = new UserLoginInfo("provider", "key");
			var user = new IdentityUser {UserName = "bob"};
			manager.Create(user);

			manager.AddLogin(user.Id, login);

			var savedLogin = Users.FindAll().Single().Logins.Single();
			Expect(savedLogin.LoginProvider, Is.EqualTo("provider"));
			Expect(savedLogin.ProviderKey, Is.EqualTo("key"));
		}


		[Test]
		public void RemoveLogin_NewLogin_Removes()
		{
			var manager = GetUserManager();
			var login = new UserLoginInfo("provider", "key");
			var user = new IdentityUser {UserName = "bob"};
			manager.Create(user);
			manager.AddLogin(user.Id, login);

			manager.RemoveLogin(user.Id, login);

			var savedUser = Users.FindAll().Single();
			Expect(savedUser.Logins, Is.Empty);
		}

		[Test]
		public void GetLogins_OneLogin_ReturnsLogin()
		{
			var manager = GetUserManager();
			var login = new UserLoginInfo("provider", "key");
			var user = new IdentityUser {UserName = "bob"};
			manager.Create(user);
			manager.AddLogin(user.Id, login);

			var logins = manager.GetLogins(user.Id);

			var savedLogin = logins.Single();
			Expect(savedLogin.LoginProvider, Is.EqualTo("provider"));
			Expect(savedLogin.ProviderKey, Is.EqualTo("key"));
		}

		[Test]
		public void Find_UserWithLogin_FindsUser()
		{
			var manager = GetUserManager();
			var login = new UserLoginInfo("provider", "key");
			var user = new IdentityUser {UserName = "bob"};
			manager.Create(user);
			manager.AddLogin(user.Id, login);

			var findUser = manager.Find(login);

			Expect(findUser, Is.Not.Null);
		}

		[Test]
		public void Find_UserWithDifferentKey_DoesNotFindUser()
		{
			var manager = GetUserManager();
			var login = new UserLoginInfo("provider", "key");
			var user = new IdentityUser {UserName = "bob"};
			manager.Create(user);
			manager.AddLogin(user.Id, login);

			var findUser = manager.Find(new UserLoginInfo("provider", "otherkey"));

			Expect(findUser, Is.Null);
		}

		[Test]
		public void Find_UserWithDifferentProvider_DoesNotFindUser()
		{
			var manager = GetUserManager();
			var login = new UserLoginInfo("provider", "key");
			var user = new IdentityUser {UserName = "bob"};
			manager.Create(user);
			manager.AddLogin(user.Id, login);

			var findUser = manager.Find(new UserLoginInfo("otherprovider", "key"));

			Expect(findUser, Is.Null);
		}
	}
}