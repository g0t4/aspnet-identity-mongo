namespace Microsoft.AspNetCore.Identity.MongoDB
{
	public class IdentityUserToken
	{
		public string LoginProvider { get; set; }

		public string Name { get; set; }

		public string Value { get; set; }
	}
}