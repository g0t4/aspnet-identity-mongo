namespace IntegrationTests
{
    using AspNet.Identity.MongoDB;
    using Microsoft.AspNet.Identity;
    using MongoDB.Bson;
    using MongoDB.Driver;
    using NUnit.Framework;

    [TestFixture]
    public class UserSecurityStampStoreTests : UserIntegrationTestsBase
    {
        [Test]
        public void Create_NewUser_HasSecurityStamp()
        {
            var manager = GetUserManager();
            var user = new IdentityUser { UserName = "bob" };

            manager.Create(user);

            var savedUser = Users.Find(new BsonDocument()).FirstOrDefaultAsync().Result;
            Expect(savedUser.SecurityStamp, Is.Not.Null);
        }

        [Test]
        public void GetSecurityStamp_NewUser_ReturnsStamp()
        {
            var manager = GetUserManager();
            var user = new IdentityUser { UserName = "bob" };
            manager.Create(user);

            var stamp = manager.GetSecurityStamp(user.Id);

            Expect(stamp, Is.Not.Null);
        }
    }
}