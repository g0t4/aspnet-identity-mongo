namespace Tests
{
	using AspNet.Identity.MongoDB;
	using ReflectionMagic;

	public static class UserHelpers
	{
		public static void SetUserId(this IdentityUser user, object value)
		{
			// note: nice to keep reflection code isolated in one place
			user.AsDynamic().Id = value;
		}
	}
}