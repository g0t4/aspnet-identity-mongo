namespace AspNet.Identity.MongoDB
{
	using System.Threading.Tasks;
	using global::MongoDB.Bson;
	using global::MongoDB.Driver.Builders;
	using Microsoft.AspNet.Identity;

	public class UserStore<TUser> : IUserStore<TUser>
		where TUser : IdentityUser
	{
		private readonly IdentityContext _Context;

		public UserStore(IdentityContext context)
		{
			_Context = context;
		}

		public void Dispose()
		{
			// no need to dispose of anything, mongodb handles connection pooling automatically
		}

		public Task CreateAsync(TUser user)
		{
			return Task.Run(() => _Context.Users.Insert(user));
		}

		public Task UpdateAsync(TUser user)
		{
			return Task.Run(() => _Context.Users.Save(user));
		}

		public Task DeleteAsync(TUser user)
		{
			var remove = Query<TUser>.EQ(u => u.Id, user.Id);
			return Task.Run(() => _Context.Users.Remove(remove));
		}

		public Task<TUser> FindByIdAsync(string userId)
		{
			return Task.Run(() => _Context.Users.FindOneByIdAs<TUser>(ObjectId.Parse(userId)));
		}

		public Task<TUser> FindByNameAsync(string userName)
		{
			// todo exception on duplicates? or better to enforce unique index to ensure this
			var byName = Query<TUser>.EQ(u => u.UserName, userName);
			return Task.Run(() => _Context.Users.FindOneAs<TUser>(byName));
		}
	}
}