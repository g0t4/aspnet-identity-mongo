namespace AspNet.Identity.MongoDB
{
    using global::MongoDB.Driver;

    public class IndexChecks<TUsers, TRole>
        where TUsers : IdentityUser
        where TRole : IdentityRole
    {
        public static void EnsureUniqueIndexOnUserName(IMongoCollection<TUsers> users)
        {
            var index = new IndexKeysDefinitionBuilder<TUsers>().Ascending(u => u.UserName);
            var options = new CreateIndexOptions();
            options.Unique = true;
            users.Indexes.CreateOneAsync(index, options);
        }

        public static void EnsureUniqueIndexOnRoleName(IMongoCollection<TRole> roles)
        {
            var index = new IndexKeysDefinitionBuilder<TRole>().Ascending(r => r.Name);
            var options = new CreateIndexOptions();
            options.Unique = true;
            roles.Indexes.CreateOneAsync(index, options);
        }

        public static void EnsureUniqueIndexOnEmail(IMongoCollection<TUsers> users)
        {
            var index = new IndexKeysDefinitionBuilder<TUsers>().Ascending(u => u.Email);
            var options = new CreateIndexOptions();
            options.Unique = true;
            users.Indexes.CreateOneAsync(index, options);
        }
    }
}