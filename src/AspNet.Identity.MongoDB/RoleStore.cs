namespace AspNet.Identity.MongoDB
{
    using System.Linq;
    using System.Threading.Tasks;
	using global::MongoDB.Driver;
	using Microsoft.AspNet.Identity;

	/// todo when the new LINQ implementation arrives in the 2.1 driver we can add back IQueryableRoleStore and IQueryableUserStore: https://jira.mongodb.org/browse/CSHARP-935
	/// <summary>
	///     Note: Deleting and updating do not modify the roles stored on a user document. If you desire this dynamic
	///     capability, override the appropriate operations on RoleStore as desired for your application. For example you could
	///     perform a document modification on the users collection before a delete or a rename.
	/// </summary>
	/// <typeparam name="TRole"></typeparam>
    public class RoleStore<TRole> : IRoleStore<TRole>,
        IQueryableRoleStore<TRole>
		where TRole : IdentityRole
	{
		private readonly IMongoCollection<TRole> _Roles;

		public RoleStore(IMongoCollection<TRole> roles)
		{
			_Roles = roles;
		}

		public virtual void Dispose()
		{
			// no need to dispose of anything, mongodb handles connection pooling automatically
		}

        public IQueryable<TRole> Roles { get { return _Roles.AsQueryable(); } }

		public virtual Task CreateAsync(TRole role)
		{
			return _Roles.InsertOneAsync(role);
		}

		public virtual Task UpdateAsync(TRole role)
		{
			return _Roles.ReplaceOneAsync(r => r.Id == role.Id, role);
		}

		public virtual Task DeleteAsync(TRole role)
		{
			return _Roles.DeleteOneAsync(r => r.Id == role.Id);
		}

		public virtual Task<TRole> FindByIdAsync(string roleId)
		{
			return _Roles.Find(r => r.Id == roleId).FirstOrDefaultAsync();
		}

		public virtual Task<TRole> FindByNameAsync(string roleName)
		{
			return _Roles.Find(r => r.Name == roleName).FirstOrDefaultAsync();
		}
	}
}