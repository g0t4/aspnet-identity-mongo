namespace IntegrationTests
{
	using System.Linq;
	using AspNet.Identity.MongoDB;
	using Microsoft.AspNet.Identity;
	using MongoDB.Bson;
	using NUnit.Framework;
	using Tests;

	[TestFixture]
	public class UserStoreTests : UserIntegrationTestsBase
	{
		[Test]
		public void Create_NewUser_Saves()
		{
			var userName = "name";
			var user = new IdentityUser {UserName = userName};
			var manager = GetUserManager();

			manager.Create(user);

			var savedUser = Users.FindAll().Single();
			Expect(savedUser.UserName, Is.EqualTo(user.UserName));
		}

		[Test]
		public void FindByName_SavedUser_ReturnsUser()
		{
			var userName = "name";
			var user = new IdentityUser {UserName = userName};
			var manager = GetUserManager();
			manager.Create(user);

			var foundUser = manager.FindByName(userName);

			Expect(foundUser, Is.Not.Null);
			Expect(foundUser.UserName, Is.EqualTo(userName));
		}

		[Test]
		public void FindByName_NoUser_ReturnsNull()
		{
			var manager = GetUserManager();

			var foundUser = manager.FindByName("nouserbyname");

			Expect(foundUser, Is.Null);
		}

		[Test]
		public void FindById_SavedUser_ReturnsUser()
		{
			var userId = ObjectId.GenerateNewId().ToString();
			var user = new IdentityUser {UserName = "name"};
			user.SetId(userId);
			var manager = GetUserManager();
			manager.Create(user);

			var foundUser = manager.FindById(userId);

			Expect(foundUser, Is.Not.Null);
			Expect(foundUser.Id, Is.EqualTo(userId));
		}

		[Test]
		public void FindById_NoUser_ReturnsNull()
		{
			var manager = GetUserManager();

			var foundUser = manager.FindById(ObjectId.GenerateNewId().ToString());

			Expect(foundUser, Is.Null);
		}

		[Test]
		public void Delete_ExistingUser_Removes()
		{
			var user = new IdentityUser {UserName = "name"};
			var manager = GetUserManager();
			manager.Create(user);
			Expect(Users.FindAll(), Is.Not.Empty);

			manager.Delete(user);

			Expect(Users.FindAll(), Is.Empty);
		}

		[Test]
		public void Update_ExistingUser_Updates()
		{
			var user = new IdentityUser {UserName = "name"};
			var manager = GetUserManager();
			manager.Create(user);
			var savedUser = manager.FindById(user.Id);
			savedUser.UserName = "newname";

			manager.Update(savedUser);

			var changedUser = Users.FindAll().Single();
			Expect(changedUser, Is.Not.Null);
			Expect(changedUser.UserName, Is.EqualTo("newname"));
		}
	}
}