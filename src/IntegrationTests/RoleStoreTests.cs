namespace IntegrationTests
{
	using System.Linq;
	using System.Runtime.CompilerServices;
	using AspNet.Identity.MongoDB;
	using Microsoft.AspNet.Identity;
	using MongoDB.Bson;
    using MongoDB.Driver;
	using NUnit.Framework;
	using Tests;
    using System.Threading.Tasks;

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

            var savedRole = Roles.Find(new BsonDocument()).FirstOrDefaultAsync().Result;
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
		public async Task Delete_ExistingRole_Removes()
		{
			var role = new IdentityRole {Name = "name"};
			var manager = GetRoleManager();
			manager.Create(role);
			Expect(await Roles.Find(new BsonDocument()).CountAsync(), Is.GreaterThan(0));

			manager.Delete(role);

            Expect(await Roles.Find(new BsonDocument()).CountAsync(), Is.EqualTo(0));
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

			var changedRole = Roles.Find(new BsonDocument()).FirstOrDefaultAsync().Result;
			Expect(changedRole, Is.Not.Null);
			Expect(changedRole.Name, Is.EqualTo("newname"));
		}
	}
}