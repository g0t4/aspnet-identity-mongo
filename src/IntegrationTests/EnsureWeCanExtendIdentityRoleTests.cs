namespace IntegrationTests
{
    using AspNet.Identity.MongoDB;
    using Microsoft.AspNet.Identity;
    using MongoDB.Bson;
    using MongoDB.Driver;
    using NUnit.Framework;
    using System.Threading.Tasks;

    [TestFixture]
    public class EnsureWeCanExtendIdentityRoleTests : UserIntegrationTestsBase
    {
        private RoleManager<ExtendedIdentityRole> _Manager;
        private ExtendedIdentityRole _Role;

        public class ExtendedIdentityRole : IdentityRole
        {
            public string ExtendedField { get; set; }
        }

        [SetUp]
        public void BeforeEachTestAfterBase()
        {
            var roleStore = new RoleStore<ExtendedIdentityRole>(Database.GetCollection<ExtendedIdentityRole>("roles"));
            _Manager = new RoleManager<ExtendedIdentityRole>(roleStore);
            _Role = new ExtendedIdentityRole
            {
                Name = "admin"
            };
        }

        [Test]
        public async Task Create_ExtendedRoleType_SavesExtraFields()
        {
            _Role.ExtendedField = "extendedField";

            _Manager.Create(_Role);

            var savedRole = await Database.GetCollection<ExtendedIdentityRole>("roles").Find(new BsonDocument()).FirstOrDefaultAsync();
            Expect(savedRole.ExtendedField, Is.EqualTo("extendedField"));
        }

        [Test]
        public void Create_ExtendedRoleType_ReadsExtraFields()
        {
            _Role.ExtendedField = "extendedField";

            _Manager.Create(_Role);

            var savedRole = _Manager.FindById(_Role.Id);
            Expect(savedRole.ExtendedField, Is.EqualTo("extendedField"));
        }
    }
}