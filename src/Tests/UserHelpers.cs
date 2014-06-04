namespace Tests
{
	using ReflectionMagic;

	public static class UserHelpers
	{
		public static void SetId(this object instance, object value)
		{
			// note: nice to keep reflection code isolated in one place
			instance.AsDynamic().Id = value;
		}
	}
}