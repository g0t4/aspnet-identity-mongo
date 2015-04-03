namespace AspNet.Identity.MongoDB
{
    using global::MongoDB.Driver;
    using System;

    public class IdentityContext<TUsers, TRole>
        where TUsers : IdentityUser
        where TRole : IdentityRole
    {
        public IMongoCollection<TUsers> Users { get; set; }
        public IMongoCollection<TRole> Roles { get; set; }

        public IdentityContext()
        {
        }

        public IdentityContext(IMongoCollection<TUsers> users)
        {
            Users = users;
        }

        public IdentityContext(IMongoCollection<TUsers> users, IMongoCollection<TRole> roles)
            : this(users)
        {
            Roles = roles;
        }

        [Obsolete("Use IndexChecks.EnsureUniqueIndexOnEmail")]
        public void EnsureUniqueIndexOnEmail()
        {
            IndexChecks<TUsers, TRole>.EnsureUniqueIndexOnEmail(Users);
        }
    }
}