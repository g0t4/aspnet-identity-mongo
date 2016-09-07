namespace IntegrationTests
{
	using System.Linq;
	using System.Security.Claims;
	using System.Threading.Tasks;
	using Microsoft.AspNetCore.Identity.MongoDB;
	using NUnit.Framework;

	[TestFixture]
	public class UserClaimStoreTests : UserIntegrationTestsBase
	{
		[Test]
		public async Task Create_NewUser_HasNoClaims()
		{
			var user = new IdentityUser {UserName = "bob"};
			var manager = GetUserManager();
			await manager.CreateAsync(user);

			var claims = await manager.GetClaimsAsync(user);

			Expect(claims, Is.Empty);
		}

		[Test]
		public async Task AddClaim_ReturnsClaim()
		{
			var user = new IdentityUser {UserName = "bob"};
			var manager = GetUserManager();
			await manager.CreateAsync(user);

			await manager.AddClaimAsync(user, new Claim("type", "value"));

			var claim = (await manager.GetClaimsAsync(user)).Single();
			Expect(claim.Type, Is.EqualTo("type"));
			Expect(claim.Value, Is.EqualTo("value"));
		}

		[Test]
		public async Task RemoveClaim_RemovesExistingClaim()
		{
			var user = new IdentityUser {UserName = "bob"};
			var manager = GetUserManager();
			await manager.CreateAsync(user);
			await manager.AddClaimAsync(user, new Claim("type", "value"));

			await manager.RemoveClaimAsync(user, new Claim("type", "value"));

			Expect(await manager.GetClaimsAsync(user), Is.Empty);
		}

		[Test]
		public async Task RemoveClaim_DifferentType_DoesNotRemoveClaim()
		{
			var user = new IdentityUser {UserName = "bob"};
			var manager = GetUserManager();
			await manager.CreateAsync(user);
			await manager.AddClaimAsync(user, new Claim("type", "value"));

			await manager.RemoveClaimAsync(user, new Claim("otherType", "value"));

			Expect(await manager.GetClaimsAsync(user), Is.Not.Empty);
		}

		[Test]
		public async Task RemoveClaim_DifferentValue_DoesNotRemoveClaim()
		{
			var user = new IdentityUser {UserName = "bob"};
			var manager = GetUserManager();
			await manager.CreateAsync(user);
			await manager.AddClaimAsync(user, new Claim("type", "value"));

			await manager.RemoveClaimAsync(user, new Claim("type", "otherValue"));

			Expect(await manager.GetClaimsAsync(user), Is.Not.Empty);
		}
	}
}