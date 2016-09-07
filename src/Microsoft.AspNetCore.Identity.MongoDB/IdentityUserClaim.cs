namespace AspNet.Identity.MongoDB
{
	using System.Security.Claims;

	public class IdentityUserClaim
	{
		public IdentityUserClaim()
		{
		}

		public IdentityUserClaim(Claim claim)
		{
			Type = claim.Type;
			Value = claim.Value;
		}

		public string Type { get; set; }
		public string Value { get; set; }

		public Claim ToSecurityClaim()
		{
			return new Claim(Type, Value);
		}
	}
}