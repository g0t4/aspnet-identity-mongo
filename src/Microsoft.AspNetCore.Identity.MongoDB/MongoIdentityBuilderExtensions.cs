// ReSharper disable once CheckNamespace - Common convention to locate extensions in Microsoft namespaces for simplifying autocompletion as a consumer.

namespace Microsoft.Extensions.DependencyInjection
{
	using System;
	using AspNetCore.Identity;
	using AspNetCore.Identity.MongoDB;
	using MongoDB.Driver;

	public static class MongoIdentityBuilderExtensions
	{
		/// <summary>
		///     If you want the default collections, use this method.
		///     Uses "users" collection and "roles" collections.
		/// </summary>
		/// <param name="builder"></param>
		/// <param name="connectionString">Must contain the database name</param>
		public static IdentityBuilder AddMongoStores<TUser, TRole>(this IdentityBuilder builder, string connectionString)
			where TRole : IdentityRole
			where TUser : IdentityUser
		{
			var url = new MongoUrl(connectionString);
			var client = new MongoClient(url);
			if (url.DatabaseName == null)
			{
				throw new ArgumentException("Your connection string must contain a database name", connectionString);
			}
			var database = client.GetDatabase(url.DatabaseName);
			return builder.AddMongoStores(
				p => database.GetCollection<TUser>("users"),
				p => database.GetCollection<TRole>("roles"));
		}

		/// <summary>
		///     If you want control over creating the users and roles collections, use this overload.
		/// </summary>
		/// <typeparam name="TUser"></typeparam>
		/// <typeparam name="TRole"></typeparam>
		/// <param name="builder"></param>
		/// <param name="usersCollectionFactory"></param>
		/// <param name="rolesCollectionFactory"></param>
		/// <returns></returns>
		private static IdentityBuilder AddMongoStores<TUser, TRole>(this IdentityBuilder builder,
			Func<IServiceProvider, IMongoCollection<TUser>> usersCollectionFactory,
			Func<IServiceProvider, IMongoCollection<TRole>> rolesCollectionFactory)
			where TRole : IdentityRole
			where TUser : IdentityUser
		{
			if (typeof(TUser) != builder.UserType)
			{
				var message = "User type passed to AddMongoStores must match user type passed to AddIdentity. "
				              + $"You passed {builder.UserType} to AddIdentity and {typeof(TUser)} to AddMongoStores, "
				              + "these do not match.";

				throw new ArgumentException(message);
			}
			if (typeof(TRole) != builder.RoleType)
			{
				var message = "Role type passed to AddMongoStores must match role type passed to AddIdentity. "
				              + $"You passed {builder.RoleType} to AddIdentity and {typeof(TRole)} to AddMongoStores, "
				              + "these do not match.";

				throw new ArgumentException(message);
			}
			builder.Services.AddSingleton<IUserStore<TUser>>(p => new UserStore<TUser>(usersCollectionFactory(p)));
			builder.Services.AddSingleton<IRoleStore<TRole>>(p => new RoleStore<TRole>(rolesCollectionFactory(p)));
			return builder;
		}
	}
}