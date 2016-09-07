namespace Tests
{
	using System.Security.Claims;
	using AspNet.Identity.MongoDB;
	using NUnit.Framework;

	[TestFixture]
	public class IdentityUserClaimTests : AssertionHelper
	{
		[Test]
		public void Create_FromClaim_SetsTypeAndValue()
		{
			var claim = new Claim("type", "value");

			var userClaim = new IdentityUserClaim(claim);

			Expect(userClaim.Type, Is.EqualTo("type"));
			Expect(userClaim.Value, Is.EqualTo("value"));
		}

		[Test]
		public void ToSecurityClaim_SetsTypeAndValue()
		{
			var userClaim = new IdentityUserClaim {Type = "t", Value = "v"};

			var claim = userClaim.ToSecurityClaim();

			Expect(claim.Type, Is.EqualTo("t"));
			Expect(claim.Value, Is.EqualTo("v"));
		}
	}
}