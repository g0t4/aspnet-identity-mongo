namespace AspNet.Identity.MongoDB
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Security.Claims;
	using System.Threading.Tasks;
	using global::MongoDB.Bson;
	using global::MongoDB.Driver.Builders;
	using global::MongoDB.Driver.Linq;
	using Microsoft.AspNet.Identity;

	public class UserStore<TUser> : IUserStore<TUser>,
		IUserPasswordStore<TUser>,
		IUserRoleStore<TUser>,
		IUserLoginStore<TUser>,
		IUserSecurityStampStore<TUser>,
		IUserEmailStore<TUser>,
		IUserClaimStore<TUser>,
		IQueryableUserStore<TUser>,
		IUserPhoneNumberStore<TUser>,
		IUserTwoFactorStore<TUser, string>,
		IUserLockoutStore<TUser, string>
		where TUser : IdentityUser
	{
		private readonly IdentityContext _Context;

		public UserStore(IdentityContext context)
		{
			_Context = context;
		}

		public void Dispose()
		{
			// no need to dispose of anything, mongodb handles connection pooling automatically
		}

		public Task CreateAsync(TUser user)
		{
			return Task.Run(() => _Context.Users.Insert(user));
		}

		public Task UpdateAsync(TUser user)
		{
			// todo should add an optimistic concurrency check
			return Task.Run(() => _Context.Users.Save(user));
		}

		public Task DeleteAsync(TUser user)
		{
			var remove = Query<TUser>.EQ(u => u.Id, user.Id);
			return Task.Run(() => _Context.Users.Remove(remove));
		}

		public Task<TUser> FindByIdAsync(string userId)
		{
			return Task.Run(() => _Context.Users.FindOneByIdAs<TUser>(ObjectId.Parse(userId)));
		}

		public Task<TUser> FindByNameAsync(string userName)
		{
			// todo exception on duplicates? or better to enforce unique index to ensure this
			var byName = Query<TUser>.EQ(u => u.UserName, userName);
			return Task.Run(() => _Context.Users.FindOneAs<TUser>(byName));
		}

		public Task SetPasswordHashAsync(TUser user, string passwordHash)
		{
			user.PasswordHash = passwordHash;
			return Task.FromResult(0);
		}

		public Task<string> GetPasswordHashAsync(TUser user)
		{
			return Task.FromResult(user.PasswordHash);
		}

		public Task<bool> HasPasswordAsync(TUser user)
		{
			return Task.FromResult(user.HasPassword());
		}

		public Task AddToRoleAsync(TUser user, string roleName)
		{
			user.AddRole(roleName);
			return Task.FromResult(0);
		}

		public Task RemoveFromRoleAsync(TUser user, string roleName)
		{
			user.RemoveRole(roleName);
			return Task.FromResult(0);
		}

		public Task<IList<string>> GetRolesAsync(TUser user)
		{
			return Task.FromResult((IList<string>) user.Roles);
		}

		public Task<bool> IsInRoleAsync(TUser user, string roleName)
		{
			return Task.FromResult(user.Roles.Contains(roleName));
		}

		public Task AddLoginAsync(TUser user, UserLoginInfo login)
		{
			user.AddLogin(login);
			return Task.FromResult(0);
		}

		public Task RemoveLoginAsync(TUser user, UserLoginInfo login)
		{
			user.RemoveLogin(login);
			return Task.FromResult(0);
		}

		public Task<IList<UserLoginInfo>> GetLoginsAsync(TUser user)
		{
			return Task.FromResult((IList<UserLoginInfo>) user.Logins);
		}

		public Task<TUser> FindAsync(UserLoginInfo login)
		{
			return Task.Factory
				.StartNew(() => _Context.Users.AsQueryable<TUser>()
					.FirstOrDefault(u => u.Logins
						.Any(l => l.LoginProvider == login.LoginProvider && l.ProviderKey == login.ProviderKey)));
		}

		public Task SetSecurityStampAsync(TUser user, string stamp)
		{
			user.SecurityStamp = stamp;
			return Task.FromResult(0);
		}

		public Task<string> GetSecurityStampAsync(TUser user)
		{
			return Task.FromResult(user.SecurityStamp);
		}

		public Task<bool> GetEmailConfirmedAsync(TUser user)
		{
			return Task.FromResult(user.EmailConfirmed);
		}

		public Task SetEmailConfirmedAsync(TUser user, bool confirmed)
		{
			user.EmailConfirmed = confirmed;
			return Task.FromResult(0);
		}

		public Task SetEmailAsync(TUser user, string email)
		{
			user.Email = email;
			return Task.FromResult(0);
		}

		public Task<string> GetEmailAsync(TUser user)
		{
			return Task.FromResult(user.Email);
		}

		public Task<TUser> FindByEmailAsync(string email)
		{
			// todo what if a user can have multiple accounts with the same email?
			return Task.Run(() => _Context.Users.AsQueryable<TUser>().FirstOrDefault(u => u.Email == email));
		}

		public Task<IList<Claim>> GetClaimsAsync(TUser user)
		{
			return Task.FromResult((IList<Claim>) user.Claims.Select(c => c.ToSecurityClaim()).ToList());
		}

		public Task AddClaimAsync(TUser user, Claim claim)
		{
			user.AddClaim(claim);
			return Task.FromResult(0);
		}

		public Task RemoveClaimAsync(TUser user, Claim claim)
		{
			user.RemoveClaim(claim);
			return Task.FromResult(0);
		}

		public IQueryable<TUser> Users
		{
			get { return _Context.Users.AsQueryable<TUser>(); }
		}

		public Task SetPhoneNumberAsync(TUser user, string phoneNumber)
		{
			user.PhoneNumber = phoneNumber;
			return Task.FromResult(0);
		}

		public Task<string> GetPhoneNumberAsync(TUser user)
		{
			return Task.FromResult(user.PhoneNumber);
		}

		public Task<bool> GetPhoneNumberConfirmedAsync(TUser user)
		{
			return Task.FromResult(user.PhoneNumberConfirmed);
		}

		public Task SetPhoneNumberConfirmedAsync(TUser user, bool confirmed)
		{
			user.PhoneNumberConfirmed = confirmed;
			return Task.FromResult(0);
		}

		public Task SetTwoFactorEnabledAsync(TUser user, bool enabled)
		{
			user.TwoFactorEnabled = enabled;
			return Task.FromResult(0);
		}

		public Task<bool> GetTwoFactorEnabledAsync(TUser user)
		{
			return Task.FromResult(user.TwoFactorEnabled);
		}

		public Task<DateTimeOffset> GetLockoutEndDateAsync(TUser user)
		{
			return Task.FromResult(user.LockoutEndDateUtc ?? new DateTimeOffset());
		}

		public Task SetLockoutEndDateAsync(TUser user, DateTimeOffset lockoutEnd)
		{
			user.LockoutEndDateUtc = new DateTime(lockoutEnd.Ticks, DateTimeKind.Utc);
			return Task.FromResult(0);
		}

		public Task<int> IncrementAccessFailedCountAsync(TUser user)
		{
			user.AccessFailedCount++;
			return Task.FromResult(user.AccessFailedCount);
		}

		public Task ResetAccessFailedCountAsync(TUser user)
		{
			user.AccessFailedCount = 0;
			return Task.FromResult(0);
		}

		public Task<int> GetAccessFailedCountAsync(TUser user)
		{
			return Task.FromResult(user.AccessFailedCount);
		}

		public Task<bool> GetLockoutEnabledAsync(TUser user)
		{
			return Task.FromResult(user.LockoutEnabled);
		}

		public Task SetLockoutEnabledAsync(TUser user, bool enabled)
		{
			user.LockoutEnabled = enabled;
			return Task.FromResult(0);
		}
	}
}