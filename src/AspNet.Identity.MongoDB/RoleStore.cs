namespace AspNet.Identity.MongoDB
{
	using System.Linq;
	using System.Threading.Tasks;
	using global::MongoDB.Bson;
	using global::MongoDB.Driver.Builders;
	using global::MongoDB.Driver.Linq;
	using Microsoft.AspNet.Identity;

	/// <summary>
	///     Note: Deleting and updating do not modify the roles stored on a user document. If you desire this dynamic
	///     capability, override the appropriate operations on RoleStore as desired for your application. For example you could
	///     perform a document modification on the users collection before a delete or a rename.
	/// </summary>
	/// <typeparam name="TRole"></typeparam>
	public class RoleStore<TRole> : IRoleStore<TRole>, IQueryableRoleStore<TRole>
		where TRole : IdentityRole
	{
		private readonly IdentityContext _Context;

		public RoleStore(IdentityContext context)
		{
			_Context = context;
		}

		public virtual IQueryable<TRole> Roles
		{
			get { return _Context.Roles.AsQueryable<TRole>(); }
		}

		public virtual void Dispose()
		{
			// no need to dispose of anything, mongodb handles connection pooling automatically
		}

		public virtual Task CreateAsync(TRole role)
		{
			return Task.Run(() => _Context.Roles.Insert(role));
		}

		public virtual Task UpdateAsync(TRole role)
		{
			return Task.Run(() => _Context.Roles.Save(role));
		}

		public virtual Task DeleteAsync(TRole role)
		{
			var queryById = Query<TRole>.EQ(r => r.Id, role.Id);
			return Task.Run(() => _Context.Roles.Remove(queryById));
		}

		public virtual Task<TRole> FindByIdAsync(string roleId)
		{
			return Task.Run(() => _Context.Roles.FindOneByIdAs<TRole>(ObjectId.Parse(roleId)));
		}

		public virtual Task<TRole> FindByNameAsync(string roleName)
		{
			var queryByName = Query<TRole>.EQ(r => r.Name, roleName);
			return Task.Run(() => _Context.Roles.FindOneAs<TRole>(queryByName));
		}
	}
}