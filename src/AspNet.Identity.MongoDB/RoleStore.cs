namespace AspNet.Identity.MongoDB
{
    using global::MongoDB.Driver;
    using Microsoft.AspNet.Identity;
    using System.Threading.Tasks;

    /// <summary>
    ///     Note: Deleting and updating do not modify the roles stored on a user document. If you desire this dynamic
    ///     capability, override the appropriate operations on RoleStore as desired for your application. For example you could
    ///     perform a document modification on the users collection before a delete or a rename.
    /// </summary>
    /// <typeparam name="TRole"></typeparam>
    public class RoleStore<TRole> : IRoleStore<TRole>
        where TRole : IdentityRole
    {
        public IMongoCollection<TRole> _Roles { get; set; }

        public RoleStore(IMongoCollection<TRole> roles)
        {
            _Roles = roles;
        }

        public virtual void Dispose()
        {
            // no need to dispose of anything, mongodb handles connection pooling automatically
        }

        public async virtual Task CreateAsync(TRole role)
        {
            await _Roles.InsertOneAsync(role);
        }

        public async virtual Task UpdateAsync(TRole role)
        {
            await _Roles.ReplaceOneAsync(r => r.Id == role.Id, role);
        }

        public async virtual Task DeleteAsync(TRole role)
        {
            await _Roles.DeleteOneAsync(r => r.Id == role.Id);
        }

        public async virtual Task<TRole> FindByIdAsync(string roleId)
        {
            return await _Roles.Find(r => r.Id == roleId).FirstOrDefaultAsync();
        }

        public async virtual Task<TRole> FindByNameAsync(string roleName)
        {
            return await _Roles.Find(r => r.Name == roleName).FirstOrDefaultAsync();
        }
    }
}