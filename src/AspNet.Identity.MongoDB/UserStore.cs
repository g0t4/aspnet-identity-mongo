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

		public virtual void Dispose()
		{
			// no need to dispose of anything, mongodb handles connection pooling automatically
		}

		public virtual Task CreateAsync(TUser user)
		{
			return Task.Run(() => _Context.Users.Insert(user));
		}

		public virtual Task UpdateAsync(TUser user)
		{
			// todo should add an optimistic concurrency check
			return Task.Run(() => _Context.Users.Save(user));
		}

		public virtual Task DeleteAsync(TUser user)
		{
			var queryById = Query<TUser>.EQ(u => u.Id, user.Id);
			return Task.Run(() => _Context.Users.Remove(queryById));
		}

		public virtual Task<TUser> FindByIdAsync(string userId)
		{
			return Task.Run(() => _Context.Users.FindOneByIdAs<TUser>(ObjectId.Parse(userId)));
		}

		public virtual Task<TUser> FindByNameAsync(string userName)
		{
			// todo exception on duplicates? or better to enforce unique index to ensure this
			var queryByName = Query<TUser>.EQ(u => u.UserName, userName);
			return Task.Run(() => _Context.Users.FindOneAs<TUser>(queryByName));
		}

		public virtual Task SetPasswordHashAsync(TUser user, string passwordHash)
		{
			user.PasswordHash = passwordHash;
			return Task.FromResult(0);
		}

		public virtual Task<string> GetPasswordHashAsync(TUser user)
		{
			return Task.FromResult(user.PasswordHash);
		}

		public virtual Task<bool> HasPasswordAsync(TUser user)
		{
			return Task.FromResult(user.HasPassword());
		}

		public virtual Task AddToRoleAsync(TUser user, string roleName)
		{
			user.AddRole(roleName);
			return Task.FromResult(0);
		}

		public virtual Task RemoveFromRoleAsync(TUser user, string roleName)
		{
			user.RemoveRole(roleName);
			return Task.FromResult(0);
		}

		public virtual Task<IList<string>> GetRolesAsync(TUser user)
		{
			return Task.FromResult((IList<string>) user.Roles);
		}

		public virtual Task<bool> IsInRoleAsync(TUser user, string roleName)
		{
			return Task.FromResult(user.Roles.Contains(roleName));
		}

		public virtual Task AddLoginAsync(TUser user, UserLoginInfo login)
		{
			user.AddLogin(login);
			return Task.FromResult(0);
		}

		public virtual Task RemoveLoginAsync(TUser user, UserLoginInfo login)
		{
			user.RemoveLogin(login);
			return Task.FromResult(0);
		}

		public virtual Task<IList<UserLoginInfo>> GetLoginsAsync(TUser user)
		{
			return Task.FromResult((IList<UserLoginInfo>) user.Logins);
		}

		public virtual Task<TUser> FindAsync(UserLoginInfo login)
		{
			return Task.Factory
				.StartNew(() => _Context.Users.AsQueryable<TUser>()
					.FirstOrDefault(u => u.Logins
						.Any(l => l.LoginProvider == login.LoginProvider && l.ProviderKey == login.ProviderKey)));
		}

		public virtual Task SetSecurityStampAsync(TUser user, string stamp)
		{
			user.SecurityStamp = stamp;
			return Task.FromResult(0);
		}

		public virtual Task<string> GetSecurityStampAsync(TUser user)
		{
			return Task.FromResult(user.SecurityStamp);
		}

		public virtual Task<bool> GetEmailConfirmedAsync(TUser user)
		{
			return Task.FromResult(user.EmailConfirmed);
		}

		public virtual Task SetEmailConfirmedAsync(TUser user, bool confirmed)
		{
			user.EmailConfirmed = confirmed;
			return Task.FromResult(0);
		}

		public virtual Task SetEmailAsync(TUser user, string email)
		{
			user.Email = email;
			return Task.FromResult(0);
		}

		public virtual Task<string> GetEmailAsync(TUser user)
		{
			return Task.FromResult(user.Email);
		}

		public virtual Task<TUser> FindByEmailAsync(string email)
		{
			// todo what if a user can have multiple accounts with the same email?
			return Task.Run(() => _Context.Users.AsQueryable<TUser>().FirstOrDefault(u => u.Email == email));
		}

		public virtual Task<IList<Claim>> GetClaimsAsync(TUser user)
		{
			return Task.FromResult((IList<Claim>) user.Claims.Select(c => c.ToSecurityClaim()).ToList());
		}

		public virtual Task AddClaimAsync(TUser user, Claim claim)
		{
			user.AddClaim(claim);
			return Task.FromResult(0);
		}

		public virtual Task RemoveClaimAsync(TUser user, Claim claim)
		{
			user.RemoveClaim(claim);
			return Task.FromResult(0);
		}

		public virtual IQueryable<TUser> Users
		{
			get { return _Context.Users.AsQueryable<TUser>(); }
		}

		public virtual Task SetPhoneNumberAsync(TUser user, string phoneNumber)
		{
			user.PhoneNumber = phoneNumber;
			return Task.FromResult(0);
		}

		public virtual Task<string> GetPhoneNumberAsync(TUser user)
		{
			return Task.FromResult(user.PhoneNumber);
		}

		public virtual Task<bool> GetPhoneNumberConfirmedAsync(TUser user)
		{
			return Task.FromResult(user.PhoneNumberConfirmed);
		}

		public virtual Task SetPhoneNumberConfirmedAsync(TUser user, bool confirmed)
		{
			user.PhoneNumberConfirmed = confirmed;
			return Task.FromResult(0);
		}

		public virtual Task SetTwoFactorEnabledAsync(TUser user, bool enabled)
		{
			user.TwoFactorEnabled = enabled;
			return Task.FromResult(0);
		}

		public virtual Task<bool> GetTwoFactorEnabledAsync(TUser user)
		{
			return Task.FromResult(user.TwoFactorEnabled);
		}

		public virtual Task<DateTimeOffset> GetLockoutEndDateAsync(TUser user)
		{
			return Task.FromResult(user.LockoutEndDateUtc ?? new DateTimeOffset());
		}

		public virtual Task SetLockoutEndDateAsync(TUser user, DateTimeOffset lockoutEnd)
		{
			user.LockoutEndDateUtc = new DateTime(lockoutEnd.Ticks, DateTimeKind.Utc);
			return Task.FromResult(0);
		}

		public virtual Task<int> IncrementAccessFailedCountAsync(TUser user)
		{
			user.AccessFailedCount++;
			return Task.FromResult(user.AccessFailedCount);
		}

		public virtual Task ResetAccessFailedCountAsync(TUser user)
		{
			user.AccessFailedCount = 0;
			return Task.FromResult(0);
		}

		public virtual Task<int> GetAccessFailedCountAsync(TUser user)
		{
			return Task.FromResult(user.AccessFailedCount);
		}

		public virtual Task<bool> GetLockoutEnabledAsync(TUser user)
		{
			return Task.FromResult(user.LockoutEnabled);
		}

		public virtual Task SetLockoutEnabledAsync(TUser user, bool enabled)
		{
			user.LockoutEnabled = enabled;
			return Task.FromResult(0);
		}
	}
}