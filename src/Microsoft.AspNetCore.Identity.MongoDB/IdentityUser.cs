namespace AspNet.Identity.MongoDB
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Security.Claims;
	using global::MongoDB.Bson;
	using global::MongoDB.Bson.Serialization.Attributes;
	using Microsoft.AspNet.Identity;

	public class IdentityUser : IUser<string>
	{
		public IdentityUser()
		{
			Id = ObjectId.GenerateNewId().ToString();
			Roles = new List<string>();
			Logins = new List<UserLoginInfo>();
			Claims = new List<IdentityUserClaim>();
		}

		[BsonRepresentation(BsonType.ObjectId)]
		public string Id { get; private set; }

		public string UserName { get; set; }

		public virtual string SecurityStamp { get; set; }

		public virtual string Email { get; set; }

		public virtual bool EmailConfirmed { get; set; }

		public virtual string PhoneNumber { get; set; }

		public virtual bool PhoneNumberConfirmed { get; set; }

		public virtual bool TwoFactorEnabled { get; set; }

		public virtual DateTime? LockoutEndDateUtc { get; set; }

		public virtual bool LockoutEnabled { get; set; }

		public virtual int AccessFailedCount { get; set; }

		[BsonIgnoreIfNull]
		public List<string> Roles { get; set; }

		public virtual void AddRole(string role)
		{
			Roles.Add(role);
		}

		public virtual void RemoveRole(string role)
		{
			Roles.Remove(role);
		}

		[BsonIgnoreIfNull]
		public virtual string PasswordHash { get; set; }

		[BsonIgnoreIfNull]
		public List<UserLoginInfo> Logins { get; set; }

		public virtual void AddLogin(UserLoginInfo login)
		{
			Logins.Add(login);
		}

		public virtual void RemoveLogin(UserLoginInfo login)
		{
			var loginsToRemove = Logins
				.Where(l => l.LoginProvider == login.LoginProvider)
				.Where(l => l.ProviderKey == login.ProviderKey);

			Logins = Logins.Except(loginsToRemove).ToList();
		}

		public virtual bool HasPassword()
		{
			return false;
		}

		[BsonIgnoreIfNull]
		public List<IdentityUserClaim> Claims { get; set; }

		public virtual void AddClaim(Claim claim)
		{
			Claims.Add(new IdentityUserClaim(claim));
		}

		public virtual void RemoveClaim(Claim claim)
		{
			var claimsToRemove = Claims
				.Where(c => c.Type == claim.Type)
				.Where(c => c.Value == claim.Value);

			Claims = Claims.Except(claimsToRemove).ToList();
		}
	}
}