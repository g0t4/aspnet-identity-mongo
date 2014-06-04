namespace IntegrationTests
{
	using System.Linq;
	using AspNet.Identity.MongoDB;
	using Microsoft.AspNet.Identity;
	using MongoDB.Bson;
	using NUnit.Framework;
	using Tests;

	[TestFixture]
	public class RoleStoreTests : UserIntegrationTestsBase
	{
		[Test]
		public void Create_NewRole_Saves()
		{
			var roleName = "admin";
			var role = new IdentityRole(roleName);
			var manager = GetRoleManager();

			manager.Create(role);

			var savedRole = Roles.FindAll().Single();
			Expect(savedRole.Name, Is.EqualTo(roleName));
		}

		[Test]
		public void FindByName_SavedRole_ReturnsRole()
		{
			var roleName = "name";
			var role = new IdentityRole {Name = roleName};
			var manager = GetRoleManager();
			manager.Create(role);

			var foundRole = manager.FindByName(roleName);

			Expect(foundRole, Is.Not.Null);
			Expect(foundRole.Name, Is.EqualTo(roleName));
		}

		[Test]
		public void FindById_SavedRole_ReturnsRole()
		{
			var roleId = ObjectId.GenerateNewId().ToString();
			var role = new IdentityRole {Name = "name"};
			role.SetId(roleId);
			var manager = GetRoleManager();
			manager.Create(role);

			var foundRole = manager.FindById(roleId);

			Expect(foundRole, Is.Not.Null);
			Expect(foundRole.Id, Is.EqualTo(roleId));
		}

		[Test]
		public void Delete_ExistingRole_Removes()
		{
			var role = new IdentityRole {Name = "name"};
			var manager = GetRoleManager();
			manager.Create(role);
			Expect(Roles.FindAll(), Is.Not.Empty);

			manager.Delete(role);

			Expect(Roles.FindAll(), Is.Empty);
		}

		[Test]
		public void Update_ExistingRole_Updates()
		{
			var role = new IdentityRole {Name = "name"};
			var manager = GetRoleManager();
			manager.Create(role);
			var savedRole = manager.FindById(role.Id);
			savedRole.Name = "newname";

			manager.Update(savedRole);

			var changedRole = Roles.FindAll().Single();
			Expect(changedRole, Is.Not.Null);
			Expect(changedRole.Name, Is.EqualTo("newname"));
		}
	}
}