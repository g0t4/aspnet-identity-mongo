namespace Microsoft.AspNetCore.Identity.MongoDB
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Security.Claims;
	using global::MongoDB.Bson;
	using global::MongoDB.Bson.Serialization.Attributes;

	public class IdentityUser
	{
		public IdentityUser()
		{
			Id = ObjectId.GenerateNewId().ToString();
			Roles = new List<string>();
			Logins = new List<UserLoginInfo>();
			Claims = new List<IdentityUserClaim>();
			// todo testing token init
			Tokens = new List<IdentityUserToken>();
		}

		[BsonRepresentation(BsonType.ObjectId)]
		public string Id { get; set; }

		public string UserName { get; set; }

		// todo what should we do with this in Mongo land
		// https://github.com/aspnet/Identity/issues/351
		public virtual string NormalizedUserName { get; set; }

		public virtual string SecurityStamp { get; set; }

		public virtual string Email { get; set; }
		public virtual string NormalizedEmail { get; set; }

		public virtual bool EmailConfirmed { get; set; }

		public virtual string PhoneNumber { get; set; }

		public virtual bool PhoneNumberConfirmed { get; set; }

		public virtual bool TwoFactorEnabled { get; set; }

		public virtual DateTimeOffset? LockoutEndDateUtc { get; set; }

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

		public virtual void RemoveLogin(string loginProvider, string providerKey)
		{
			var loginsToRemove = Logins
				.Where(l => l.LoginProvider == loginProvider)
				.Where(l => l.ProviderKey == providerKey);

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

		public void ReplaceClaim(Claim claim, Claim newClaim)
		{
			var current = Claims
				.FirstOrDefault(c => c.Type == claim.Type && c.Value == claim.Value);
			if (current == null)
			{
				//todo?
				return;
			}
			RemoveClaim(claim);
			AddClaim(newClaim);
		}

		[BsonIgnoreIfNull]
		public List<IdentityUserToken> Tokens { get; set; }

		private IdentityUserToken GetToken(string loginProider, string name)
			=> Tokens.FirstOrDefault(t => t.LoginProvider == loginProider && t.Name == name);

		// todo testing of tokens
		public virtual void SetToken(string loginProider, string name, string value)
		{
			var existingToken = GetToken(loginProider, name);
			if (existingToken != null)
			{
				existingToken.Value = value;
				return;
			}

			Tokens.Add(new IdentityUserToken
			{
				LoginProvider = loginProider,
				Name = name,
				Value = value
			});
		}

		public virtual string GetTokenValue(string loginProider, string name)
		{
			return GetToken(loginProider, name)?.Value;
		}

		public virtual void RemoveToken(string loginProvider, string name)
		{
			Tokens.RemoveAll(t => t.LoginProvider == loginProvider && t.Name == name);
		}
	}
}