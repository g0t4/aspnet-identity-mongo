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
		public virtual string Id { get; set; }

		public virtual string UserName { get; set; }

		// todo what should we do with this in Mongo land
		// https://github.com/aspnet/Identity/issues/351
		// todo migration
		public virtual string NormalizedUserName { get; set; }

		/// <summary>
		/// A random value that must change whenever a users credentials change 
		/// (password changed, login removed)
		/// </summary>
		public virtual string SecurityStamp { get; set; }

		public virtual string Email { get; set; }

		// todo migration
		public virtual string NormalizedEmail { get; set; }

		public virtual bool EmailConfirmed { get; set; }

		public virtual string PhoneNumber { get; set; }

		public virtual bool PhoneNumberConfirmed { get; set; }

		public virtual bool TwoFactorEnabled { get; set; }

		// todo migration
		// ef has LockoutEnd ... what was this before?
		public virtual DateTimeOffset? LockoutEndDateUtc { get; set; }

		public virtual bool LockoutEnabled { get; set; }

		public virtual int AccessFailedCount { get; set; }

		[BsonIgnoreIfNull]
		public virtual List<string> Roles { get; set; }

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


		// todo move to a type I manage - and check for changes to UserLoginInfo for migration purposes
		// todo I know that displayName was added
		[BsonIgnoreIfNull]
		public virtual List<UserLoginInfo> Logins { get; set; }

		public virtual void AddLogin(UserLoginInfo login)
		{
			Logins.Add(login);
		}

		// todo testing?
		public virtual void RemoveLogin(string loginProvider, string providerKey)
		{
			Logins.RemoveAll(l => l.LoginProvider == loginProvider && l.ProviderKey == providerKey);
		}

		public virtual bool HasPassword()
		{
			return false;
		}

		[BsonIgnoreIfNull]
		public virtual List<IdentityUserClaim> Claims { get; set; }

		public virtual void AddClaim(Claim claim)
		{
			Claims.Add(new IdentityUserClaim(claim));
		}

		public virtual void RemoveClaim(Claim claim)
		{
			Claims.RemoveAll(c => c.Type == claim.Type && c.Value == claim.Value);
		}

		// todo testing?
		public virtual void ReplaceClaim(Claim claim, Claim newClaim)
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

		// todo testing?
		[BsonIgnoreIfNull]
		public virtual List<IdentityUserToken> Tokens { get; set; }

		// todo testing?
		private IdentityUserToken GetToken(string loginProider, string name)
			=> Tokens.FirstOrDefault(t => t.LoginProvider == loginProider && t.Name == name);

		// todo testing of tokens, what are these for?
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

		// todo testing?
		public virtual string GetTokenValue(string loginProider, string name)
		{
			return GetToken(loginProider, name)?.Value;
		}

		// todo testing?
		public virtual void RemoveToken(string loginProvider, string name)
		{
			Tokens.RemoveAll(t => t.LoginProvider == loginProvider && t.Name == name);
		}
	}
}