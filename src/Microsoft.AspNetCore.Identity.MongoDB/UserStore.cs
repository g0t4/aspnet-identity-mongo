namespace Microsoft.AspNetCore.Identity.MongoDB
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Security.Claims;
	using System.Threading;
	using System.Threading.Tasks;
	using global::MongoDB.Driver;

	/// <summary>
	///     FYI as for methods with CancellationToken, unless a database op is involved the token is ignored as any in memory
	///     ops to manipulate the object graph of a user, or query, are so fast that canellation is a waste of time.
	/// </summary>
	/// <typeparam name="TUser"></typeparam>
	public class UserStore<TUser> : IUserPasswordStore<TUser>,
			IUserRoleStore<TUser>,
			IUserLoginStore<TUser>,
			IUserSecurityStampStore<TUser>,
			IUserEmailStore<TUser>,
			IUserClaimStore<TUser>,
			IUserPhoneNumberStore<TUser>,
			IUserTwoFactorStore<TUser>,
			IUserLockoutStore<TUser>,
			IQueryableUserStore<TUser>,
			IUserAuthenticationTokenStore<TUser>
		where TUser : IdentityUser
	{
		private readonly IMongoCollection<TUser> _Users;

		public UserStore(IMongoCollection<TUser> users)
		{
			_Users = users;
		}

		public virtual void Dispose()
		{
			// no need to dispose of anything, mongodb handles connection pooling automatically
		}

		public virtual async Task<IdentityResult> CreateAsync(TUser user, CancellationToken token)
		{
			await _Users.InsertOneAsync(user, cancellationToken: token);
			return IdentityResult.Success;
		}

		public virtual async Task<IdentityResult> UpdateAsync(TUser user, CancellationToken token)
		{
			// todo should add an optimistic concurrency check
			await _Users.ReplaceOneAsync(u => u.Id == user.Id, user, cancellationToken: token);
			return IdentityResult.Success;
		}

		public virtual async Task<IdentityResult> DeleteAsync(TUser user, CancellationToken token)
		{
			await _Users.DeleteOneAsync(u => u.Id == user.Id, token);
			return IdentityResult.Success;
		}

		// todo testing
		public virtual async Task<string> GetUserIdAsync(TUser user, CancellationToken cancellationToken)
			=> user.Id;

		// todo testing
		public virtual async Task<string> GetUserNameAsync(TUser user, CancellationToken cancellationToken)
			=> user.UserName;

		// todo testing
		public virtual async Task SetUserNameAsync(TUser user, string userName, CancellationToken cancellationToken)
			=> user.UserName = userName;

		// todo testing
		public virtual async Task<string> GetNormalizedUserNameAsync(TUser user, CancellationToken cancellationToken)
			=> user.NormalizedUserName;

		// todo testing
		public virtual async Task SetNormalizedUserNameAsync(TUser user, string normalizedName, CancellationToken cancellationToken)
			=> user.NormalizedUserName = normalizedName;

		// todo testing
		public virtual Task<TUser> FindByIdAsync(string userId, CancellationToken token)
			=> _Users.Find(u => u.Id == userId).FirstOrDefaultAsync(token);

		public virtual Task<TUser> FindByNameAsync(string userName, CancellationToken token)
			// todo exception on duplicates? or better to enforce unique index to ensure this
			=> _Users.Find(u => u.UserName == userName).FirstOrDefaultAsync(token);

		public virtual async Task SetPasswordHashAsync(TUser user, string passwordHash, CancellationToken token)
			=> user.PasswordHash = passwordHash;

		public virtual async Task<string> GetPasswordHashAsync(TUser user, CancellationToken token)
			=> user.PasswordHash;

		public virtual async Task<bool> HasPasswordAsync(TUser user, CancellationToken token)
			=> user.HasPassword();

		public virtual async Task AddToRoleAsync(TUser user, string roleName, CancellationToken token)
			=> user.AddRole(roleName);

		public virtual async Task RemoveFromRoleAsync(TUser user, string roleName, CancellationToken token)
			=> user.RemoveRole(roleName);

		public virtual Task<IList<string>> GetRolesAsync(TUser user, CancellationToken token)
			=> Task.FromResult((IList<string>) user.Roles);

		public virtual Task<bool> IsInRoleAsync(TUser user, string roleName, CancellationToken token)
			=> Task.FromResult(user.Roles.Contains(roleName));

		// todo testing
		public virtual async Task<IList<TUser>> GetUsersInRoleAsync(string roleName, CancellationToken token)
			=> await _Users.Find(u => u.Roles.Contains(roleName))
				.ToListAsync(token);

		public virtual async Task AddLoginAsync(TUser user, UserLoginInfo login, CancellationToken token)
			=> user.AddLogin(login);

		public virtual async Task RemoveLoginAsync(TUser user, string loginProvider, string providerKey, CancellationToken cancellationToken = default(CancellationToken))
			=> user.RemoveLogin(loginProvider, providerKey);

		public virtual async Task<IList<UserLoginInfo>> GetLoginsAsync(TUser user, CancellationToken token)
			=> user.Logins;

		public virtual Task<TUser> FindByLoginAsync(string loginProvider, string providerKey, CancellationToken cancellationToken = default(CancellationToken))
			=> _Users
				.Find(u => u.Logins.Any(l => l.LoginProvider == loginProvider && l.ProviderKey == providerKey))
				.FirstOrDefaultAsync(cancellationToken);

		public virtual async Task SetSecurityStampAsync(TUser user, string stamp, CancellationToken token)
			=> user.SecurityStamp = stamp;

		public virtual async Task<string> GetSecurityStampAsync(TUser user, CancellationToken token)
			=> user.SecurityStamp;

		public virtual Task<bool> GetEmailConfirmedAsync(TUser user, CancellationToken token)
		{
			return Task.FromResult(user.EmailConfirmed);
		}

		public virtual Task SetEmailConfirmedAsync(TUser user, bool confirmed, CancellationToken token)
		{
			user.EmailConfirmed = confirmed;
			return Task.FromResult(0);
		}

		public virtual Task SetEmailAsync(TUser user, string email, CancellationToken token)
		{
			user.Email = email;
			return Task.FromResult(0);
		}

		public virtual Task<string> GetEmailAsync(TUser user, CancellationToken token)
		{
			return Task.FromResult(user.Email);
		}

		// todo testing
		public virtual Task<string> GetNormalizedEmailAsync(TUser user, CancellationToken cancellationToken = default(CancellationToken))
		{
			// todo return Task.FromResult(user.NormalizedEmail);
			return null;
		}

		// todo testing
		public virtual Task SetNormalizedEmailAsync(TUser user, string normalizedEmail, CancellationToken cancellationToken = default(CancellationToken))
		{
			// todo user.NormalizedEmail = normalizedEmail;
			return Task.FromResult(0);
		}

		public virtual Task<TUser> FindByEmailAsync(string email, CancellationToken token)
		{
			// todo what if a user can have multiple accounts with the same email?
			return _Users.Find(u => u.Email == email).FirstOrDefaultAsync();
		}

		public virtual async Task<IList<Claim>> GetClaimsAsync(TUser user, CancellationToken token)
			=> user.Claims.Select(c => c.ToSecurityClaim()).ToList();

		public virtual Task AddClaimsAsync(TUser user, IEnumerable<Claim> claims, CancellationToken token)
		{
			foreach (var claim in claims)
			{
				user.AddClaim(claim);
			}
			return Task.FromResult(0);
		}

		public virtual Task RemoveClaimsAsync(TUser user, IEnumerable<Claim> claims, CancellationToken token)
		{
			foreach (var claim in claims)
			{
				user.RemoveClaim(claim);
			}
			return Task.FromResult(0);
		}

		// todo testing
		public virtual async Task ReplaceClaimAsync(TUser user, Claim claim, Claim newClaim, CancellationToken cancellationToken = default(CancellationToken))
		{
			user.ReplaceClaim(claim, newClaim);
		}

		public virtual Task SetPhoneNumberAsync(TUser user, string phoneNumber, CancellationToken token)
		{
			user.PhoneNumber = phoneNumber;
			return Task.FromResult(0);
		}

		public virtual Task<string> GetPhoneNumberAsync(TUser user, CancellationToken token)
		{
			return Task.FromResult(user.PhoneNumber);
		}

		public virtual Task<bool> GetPhoneNumberConfirmedAsync(TUser user, CancellationToken token)
		{
			return Task.FromResult(user.PhoneNumberConfirmed);
		}

		public virtual Task SetPhoneNumberConfirmedAsync(TUser user, bool confirmed, CancellationToken token)
		{
			user.PhoneNumberConfirmed = confirmed;
			return Task.FromResult(0);
		}

		public virtual Task SetTwoFactorEnabledAsync(TUser user, bool enabled, CancellationToken token)
		{
			user.TwoFactorEnabled = enabled;
			return Task.FromResult(0);
		}

		public virtual Task<bool> GetTwoFactorEnabledAsync(TUser user, CancellationToken token)
		{
			return Task.FromResult(user.TwoFactorEnabled);
		}

		public virtual async Task<IList<TUser>> GetUsersForClaimAsync(Claim claim, CancellationToken cancellationToken = default(CancellationToken))
		{
			return await _Users
				// todo integration test
				.Find(u => u.Claims.Any(c => c.Type == claim.Type && c.Value == claim.Value))
				.ToListAsync(cancellationToken);
		}

		public virtual Task<DateTimeOffset?> GetLockoutEndDateAsync(TUser user, CancellationToken token)
		{
			// todo migration?
			return Task.FromResult(user.LockoutEndDateUtc);
		}

		public virtual Task SetLockoutEndDateAsync(TUser user, DateTimeOffset? lockoutEnd, CancellationToken token)
		{
			user.LockoutEndDateUtc = lockoutEnd;
			return Task.FromResult(0);
		}

		public virtual Task<int> IncrementAccessFailedCountAsync(TUser user, CancellationToken token)
		{
			user.AccessFailedCount++;
			return Task.FromResult(user.AccessFailedCount);
		}

		public virtual Task ResetAccessFailedCountAsync(TUser user, CancellationToken token)
		{
			user.AccessFailedCount = 0;
			return Task.FromResult(0);
		}

		public virtual async Task<int> GetAccessFailedCountAsync(TUser user, CancellationToken token)
			=> user.AccessFailedCount;

		public virtual async Task<bool> GetLockoutEnabledAsync(TUser user, CancellationToken token)
			=> user.LockoutEnabled;

		public virtual async Task SetLockoutEnabledAsync(TUser user, bool enabled, CancellationToken token)
			=> user.LockoutEnabled = enabled;

		public virtual IQueryable<TUser> Users => _Users.AsQueryable();

		// todo testing
		public virtual async Task SetTokenAsync(TUser user, string loginProvider, string name, string value, CancellationToken cancellationToken)
			=> user.SetToken(loginProvider, name, value);

		// todo testing
		public virtual async Task RemoveTokenAsync(TUser user, string loginProvider, string name, CancellationToken cancellationToken)
			=> user.RemoveToken(loginProvider, name);

		// todo testing
		public virtual async Task<string> GetTokenAsync(TUser user, string loginProvider, string name, CancellationToken cancellationToken)
			=> user.GetTokenValue(loginProvider, name);
	}
}