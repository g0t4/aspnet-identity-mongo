﻿namespace IntegrationTests
{
	using System.Linq;
	using AspNet.Identity.MongoDB;
	using Microsoft.AspNet.Identity;
	using MongoDB.Driver;
	using NUnit.Framework;

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
			var users = DatabaseNewApi.GetCollection<ExtendedIdentityUser>("users");
            UserStore<ExtendedIdentityUser> userStore = new UserStore<ExtendedIdentityUser>(users);
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

			var savedUser = Users.OfType<ExtendedIdentityUser>().AsQueryable().Single();
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