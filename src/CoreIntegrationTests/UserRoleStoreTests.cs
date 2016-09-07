namespace IntegrationTests
{
	using System.Linq;
	using System.Threading.Tasks;
	using Microsoft.AspNetCore.Identity.MongoDB;
	using NUnit.Framework;

	[TestFixture]
	public class UserRoleStoreTests : UserIntegrationTestsBase
	{
		[Test]
		public async Task GetRoles_UserHasNoRoles_ReturnsNoRoles()
		{
			var manager = GetUserManager();
			var user = new IdentityUser {UserName = "bob"};
			await manager.CreateAsync(user);

			var roles = await manager.GetRolesAsync(user);

			Expect(roles, Is.Empty);
		}

		[Test]
		public async Task AddRole_Adds()
		{
			var manager = GetUserManager();
			var user = new IdentityUser {UserName = "bob"};
			await manager.CreateAsync(user);

			// todo it would be nice if the API for UserManager asked for a normalized name and not just a name
			await manager.AddToRoleAsync(user, "role");

			var savedUser = Users.FindAll().Single();
			// note: addToRole now passes a normalized role name
			Expect(savedUser.Roles, Is.EquivalentTo(new[] {"ROLE"}));
			Expect(await manager.IsInRoleAsync(user, "role"), Is.True);
		}

		[Test]
		public async Task RemoveRole_Removes()
		{
			var manager = GetUserManager();
			var user = new IdentityUser {UserName = "bob"};
			await manager.CreateAsync(user);
			await manager.AddToRoleAsync(user, "role");

			await manager.RemoveFromRoleAsync(user, "role");

			var savedUser = Users.FindAll().Single();
			Expect(savedUser.Roles, Is.Empty);
			Expect(await manager.IsInRoleAsync(user, "role"), Is.False);
		}
	}
}