namespace IntegrationTests
{
    using System.Linq;
    using AspNet.Identity.MongoDB;
    using Microsoft.AspNet.Identity;
    using NUnit.Framework;
    using MongoDB.Driver;
    using MongoDB.Bson;

    [TestFixture]
    public class EnsureWeCanExtendIdentityUserTests : UserIntegrationTestsBase
    {
        private UserManager<ExtendedIdentityUser> _Manager;
        private ExtendedIdentityUser _User;

        public class ExtendedIdentityUser : IdentityUser
        {
            public string ExtendedField { get; set; }
        }

        [SetUp]
        public void BeforeEachTestAfterBase()
        {
            var userStore = new UserStore<ExtendedIdentityUser>(Database.GetCollection<ExtendedIdentityUser>("users"));
            _Manager = new UserManager<ExtendedIdentityUser>(userStore);    
            _User = new ExtendedIdentityUser
            {
                UserName = "bob"
            };
        }

        [Test]
        public void Create_ExtendedUserType_SavesExtraFields()
        {
            _User.ExtendedField = "extendedField";

            _Manager.Create(_User);

            var savedUser = Database.GetCollection<ExtendedIdentityUser>("users").Find(new BsonDocument()).FirstOrDefaultAsync().Result;
            Expect(savedUser.ExtendedField, Is.EqualTo("extendedField"));
        }

        [Test]
        public void Create_ExtendedUserType_ReadsExtraFields()
        {
            _User.ExtendedField = "extendedField";

            _Manager.Create(_User);

            var savedUser = _Manager.FindById(_User.Id);
            Expect(savedUser.ExtendedField, Is.EqualTo("extendedField"));
        }
    }
}