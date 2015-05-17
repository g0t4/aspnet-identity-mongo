namespace AspNet.Identity.MongoDB
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Claims;
    using System.Threading.Tasks;
    using global::MongoDB.Driver;
    using Microsoft.AspNet.Identity;
    using System.Threading;


    /// todo when the new LINQ implementation arrives in the 2.1 driver we can add back IQueryableRoleStore and IQueryableUserStore: https://jira.mongodb.org/browse/CSHARP-935
    public class UserStore<TUser> : IUserStore<TUser>,
                                    //IQueryableUserStore<TUser>,
                                    IUserPasswordStore<TUser>,
                                    IUserRoleStore<TUser>,
                                    IUserLoginStore<TUser>,
                                    IUserSecurityStampStore<TUser>,
                                    IUserEmailStore<TUser>,
                                    IUserClaimStore<TUser>,
                                    IUserPhoneNumberStore<TUser>,
                                    IUserTwoFactorStore<TUser>,
                                    IUserLockoutStore<TUser> where TUser : IdentityUser {
        #region Fields
        private readonly IMongoCollection<TUser> _users;
        private bool _disposed;
        #endregion

        public UserStore (IMongoCollection<TUser> users) {
            _users = users;
        }


        private void ThrowIfDisposed () {
            if (_disposed) {
                throw new ObjectDisposedException(GetType().Name);
            }
        }

        public virtual void Dispose () {
            // no need to dispose of anything, mongodb handles connection pooling automatically
            _disposed = true;
        }
        
        #region IUserStore<TUser>
        public virtual Task<String> GetUserIdAsync (TUser user, CancellationToken cancellationToken) {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (user == null) throw new ArgumentNullException(nameof(user));

            return Task.FromResult(user.Id);
        }

        public virtual Task<String> GetUserNameAsync (TUser user, CancellationToken cancellationToken) {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (user == null) throw new ArgumentNullException(nameof(user));

            return Task.FromResult(user.UserName);
        }

        public virtual Task SetUserNameAsync (TUser user, String userName, CancellationToken cancellationToken) {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (user == null) throw new ArgumentNullException(nameof(user));
            if (String.IsNullOrWhiteSpace(userName)) throw new ArgumentNullException(nameof(userName));

            user.UserName = userName;

            return Task.FromResult(0);
        }

        public virtual Task<String> GetNormalizedUserNameAsync (TUser user, CancellationToken cancellationToken) {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (user == null) throw new ArgumentNullException(nameof(user));

            return Task.FromResult(user.UserName);
        }

        public virtual Task SetNormalizedUserNameAsync (TUser user, String normalizedName, CancellationToken cancellationToken) {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (user == null) throw new ArgumentNullException(nameof(user));
            if (String.IsNullOrWhiteSpace(normalizedName)) throw new ArgumentNullException(nameof(normalizedName));

            user.UserName = normalizedName;

            return Task.FromResult(0);
        }

        public virtual async Task<IdentityResult> CreateAsync (TUser user, CancellationToken cancellationToken) {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (user == null) throw new ArgumentNullException(nameof(user));

            try {
                await _users.InsertOneAsync(user, cancellationToken);

            } catch (Exception e) {
                return IdentityResult.Failed(
                    new IdentityError {
                        Description = e.Message
                    }
                );
            }

            return IdentityResult.Success;
        }

        public virtual async Task<IdentityResult> UpdateAsync (TUser user, CancellationToken cancellationToken) {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (user == null) throw new ArgumentNullException(nameof(user));

            try {
                ReplaceOneResult result = await _users.ReplaceOneAsync(u => u.Id == user.Id, user, cancellationToken: cancellationToken);

                if (result.MatchedCount == 1) {
                    return IdentityResult.Success;

                } else {
                    return IdentityResult.Failed(
                        new IdentityError {
                            Description = $"Der User {user.UserName} (ID: {user.Id}) konnte nicht aktualisiert werden."
                        }
                    );
                }

            } catch (Exception e) {
                return IdentityResult.Failed(
                    new IdentityError {
                        Description = e.Message
                    }
                );
            }
        }

        public virtual async Task<IdentityResult> DeleteAsync (TUser user, CancellationToken cancellationToken) {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (user == null) throw new ArgumentNullException(nameof(user));

            try {
                DeleteResult result = await _users.DeleteOneAsync(u => u.Id == user.Id, cancellationToken);

                if (result.DeletedCount == 1) {
                    return IdentityResult.Success;

                } else {
                    return IdentityResult.Failed(
                        new IdentityError {
                            Description = $"Der User {user.UserName} (ID: {user.Id}) konnte nicht gelöscht werden."
                        }
                    );
                }

            } catch (Exception e) {
                return IdentityResult.Failed(
                    new IdentityError {
                        Description = e.Message
                    }
                );
            }
        }

        public virtual Task<TUser> FindByIdAsync (String userId, CancellationToken cancellationToken) {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (String.IsNullOrWhiteSpace(userId)) throw new ArgumentNullException(nameof(userId));

            return _users.Find(u => u.Id == userId).FirstOrDefaultAsync(cancellationToken);
        }

        public virtual Task<TUser> FindByNameAsync (String normalizedUserName, CancellationToken cancellationToken) {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (String.IsNullOrWhiteSpace(normalizedUserName)) throw new ArgumentNullException(nameof(normalizedUserName));

            return _users.Find(u => u.UserName == normalizedUserName).FirstOrDefaultAsync(cancellationToken);
        }
        #endregion

        #region IQueryableUserStore<TUser>
        //public IQueryable<TUser> Users {
        //    get { return _users.AsQueryable(); }
        //}
        #endregion

        #region IUserPasswordStore<TUser>
        public virtual Task SetPasswordHashAsync (TUser user, String passwordHash, CancellationToken cancellationToken) {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (user == null) throw new ArgumentNullException(nameof(user));
            if (String.IsNullOrWhiteSpace(passwordHash)) throw new ArgumentNullException(nameof(passwordHash));

            user.PasswordHash = passwordHash;
            return Task.FromResult(0);
        }

        public virtual Task<String> GetPasswordHashAsync (TUser user, CancellationToken cancellationToken) {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (user == null) throw new ArgumentNullException(nameof(user));

            return Task.FromResult(user.PasswordHash);
        }

        public virtual Task<bool> HasPasswordAsync (TUser user, CancellationToken cancellationToken) {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (user == null) throw new ArgumentNullException(nameof(user));

            return Task.FromResult(user.HasPassword());
        }
        #endregion

        #region IUserRoleStore<TUser>
        public virtual Task AddToRoleAsync (TUser user, String roleName, CancellationToken cancellationToken) {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (user == null) throw new ArgumentNullException(nameof(user));
            if (String.IsNullOrWhiteSpace(roleName)) throw new ArgumentNullException(nameof(roleName));

            user.AddRole(roleName);
            return Task.FromResult(0);
        }

        public virtual Task RemoveFromRoleAsync (TUser user, String roleName, CancellationToken cancellationToken) {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (user == null) throw new ArgumentNullException(nameof(user));
            if (String.IsNullOrWhiteSpace(roleName)) throw new ArgumentNullException(nameof(roleName));

            user.RemoveRole(roleName);
            return Task.FromResult(0);
        }

        public virtual Task<IList<String>> GetRolesAsync (TUser user, CancellationToken cancellationToken) {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (user == null) throw new ArgumentNullException(nameof(user));

            return Task.FromResult((IList<String>)user.Roles);
        }

        public virtual Task<bool> IsInRoleAsync (TUser user, String roleName, CancellationToken cancellationToken) {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (user == null) throw new ArgumentNullException(nameof(user));
            if (String.IsNullOrWhiteSpace(roleName)) throw new ArgumentNullException(nameof(roleName));

            return Task.FromResult(user.Roles.Contains(roleName));
        }

        public virtual async Task<IList<TUser>> GetUsersInRoleAsync (String roleName, CancellationToken cancellationToken) {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (String.IsNullOrWhiteSpace(roleName)) throw new ArgumentNullException(nameof(roleName));

            List<TUser> users = await _users.Find(u => u.Roles.Contains(roleName))
                                            .ToListAsync(cancellationToken);

            return users;
        }
        #endregion

        #region IUserLoginStore<TUser>
        public virtual Task AddLoginAsync (TUser user, UserLoginInfo login, CancellationToken cancellationToken) {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (user == null) throw new ArgumentNullException(nameof(user));
            if (login == null) throw new ArgumentNullException(nameof(login));

            user.AddLogin(login);
            return Task.FromResult(0);
        }

        public virtual Task RemoveLoginAsync (TUser user, String loginProvider, String providerKey, CancellationToken cancellationToken) {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (user == null) throw new ArgumentNullException(nameof(user));
            if (String.IsNullOrWhiteSpace(loginProvider)) throw new ArgumentNullException(nameof(loginProvider));
            if (String.IsNullOrWhiteSpace(providerKey)) throw new ArgumentNullException(nameof(providerKey));

            user.RemoveLogin(loginProvider, providerKey);
            return Task.FromResult(0);
        }

        public virtual Task<IList<UserLoginInfo>> GetLoginsAsync (TUser user, CancellationToken cancellationToken) {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (user == null) throw new ArgumentNullException(nameof(user));

            return Task.FromResult((IList<UserLoginInfo>)user.Logins);
        }

        public virtual Task<TUser> FindByLoginAsync (String loginProvider, String providerKey, CancellationToken cancellationToken) {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (String.IsNullOrWhiteSpace(loginProvider)) throw new ArgumentNullException(nameof(loginProvider));
            if (String.IsNullOrWhiteSpace(providerKey)) throw new ArgumentNullException(nameof(providerKey));

            return _users.Find(
                u => u.Logins.Any(
                    l => l.LoginProvider == loginProvider &&
                         l.ProviderKey == providerKey
                )
            ).FirstOrDefaultAsync(cancellationToken);
        }
        #endregion

        #region IUserSecurityStampStore<TUser>
        public virtual Task SetSecurityStampAsync (TUser user, String stamp, CancellationToken cancellationToken) {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (user == null) throw new ArgumentNullException(nameof(user));
            if (String.IsNullOrWhiteSpace(stamp)) throw new ArgumentNullException(nameof(stamp));

            user.SecurityStamp = stamp;

            return Task.FromResult(0);
        }

        public virtual Task<String> GetSecurityStampAsync (TUser user, CancellationToken cancellationToken) {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (user == null) throw new ArgumentNullException(nameof(user));

            return Task.FromResult(user.SecurityStamp);
        }
        #endregion

        #region IUserEmailStore<TUser>
        public virtual Task SetEmailAsync (TUser user, String email, CancellationToken cancellationToken) {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (user == null) throw new ArgumentNullException(nameof(user));
            if (String.IsNullOrWhiteSpace(email)) throw new ArgumentNullException(nameof(email));

            user.Email = email;

            return Task.FromResult(0);
        }

        public virtual Task<String> GetEmailAsync (TUser user, CancellationToken cancellationToken) {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (user == null) throw new ArgumentNullException(nameof(user));

            return Task.FromResult(user.Email);
        }

        public virtual Task<bool> GetEmailConfirmedAsync (TUser user, CancellationToken cancellationToken) {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (user == null) throw new ArgumentNullException(nameof(user));

            return Task.FromResult(user.EmailConfirmed);
        }

        public virtual Task SetEmailConfirmedAsync (TUser user, bool confirmed, CancellationToken cancellationToken) {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (user == null) throw new ArgumentNullException(nameof(user));

            user.EmailConfirmed = confirmed;

            return Task.FromResult(0);
        }

        public virtual Task<TUser> FindByEmailAsync (String normalizedEmail, CancellationToken cancellationToken) {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (String.IsNullOrWhiteSpace(normalizedEmail)) throw new ArgumentNullException(nameof(normalizedEmail));

            return _users.Find(u => u.Email == normalizedEmail)
                         .FirstOrDefaultAsync();
        }

        public virtual Task<String> GetNormalizedEmailAsync (TUser user, CancellationToken cancellationToken) {
            return GetEmailAsync(user, cancellationToken);
        }

        public virtual Task SetNormalizedEmailAsync (TUser user, String normalizedEmail, CancellationToken cancellationToken) {
            return SetEmailAsync(user, normalizedEmail, cancellationToken);
        }
        #endregion

        #region IUserClaimStore<TUser>
        public virtual Task<IList<Claim>> GetClaimsAsync (TUser user, CancellationToken cancellationToken) {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (user == null) throw new ArgumentNullException(nameof(user));

            return Task.FromResult(
                (IList<Claim>)user.Claims.Select(c => c.ToSecurityClaim())
                                         .ToList()
            );
        }

        public virtual Task AddClaimsAsync (TUser user, IEnumerable<Claim> claims, CancellationToken cancellationToken) {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (user == null) throw new ArgumentNullException(nameof(user));
            if (claims == null) throw new ArgumentNullException(nameof(claims));

            foreach (Claim claim in claims) {
                user.AddClaim(claim);
            }

            return Task.FromResult(0);
        }

        public virtual Task ReplaceClaimAsync (TUser user, Claim claim, Claim newClaim, CancellationToken cancellationToken) {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (user == null) throw new ArgumentNullException(nameof(user));
            if (claim == null) throw new ArgumentNullException(nameof(claim));
            if (newClaim == null) throw new ArgumentNullException(nameof(newClaim));

            user.RemoveClaim(claim);
            user.AddClaim(newClaim);

            return Task.FromResult(0);
        }

        public virtual Task RemoveClaimsAsync (TUser user, IEnumerable<Claim> claims, CancellationToken cancellationToken) {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (user == null) throw new ArgumentNullException(nameof(user));
            if (claims == null) throw new ArgumentNullException(nameof(claims));

            foreach (Claim claim in claims) {
                user.RemoveClaim(claim);
            }

            return Task.FromResult(0);
        }

        public virtual async Task<IList<TUser>> GetUsersForClaimAsync (Claim claim, CancellationToken cancellationToken) {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (claim == null) throw new ArgumentNullException(nameof(claim));

            List<TUser> users = await _users.Find(u => u.Claims.Any(
                                                c => c.Type == claim.Type &&
                                                     c.Value == claim.Value
                                                )
                                            ).ToListAsync(cancellationToken);

            return users;
        }
        #endregion

        #region IUserPhoneNumberStore<TUser>
        public virtual Task SetPhoneNumberAsync (TUser user, String phoneNumber, CancellationToken cancellationToken) {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (user == null) throw new ArgumentNullException(nameof(user));
            if (String.IsNullOrWhiteSpace(phoneNumber)) throw new ArgumentNullException(nameof(phoneNumber));

            user.PhoneNumber = phoneNumber;

            return Task.FromResult(0);
        }

        public virtual Task<String> GetPhoneNumberAsync (TUser user, CancellationToken cancellationToken) {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (user == null) throw new ArgumentNullException(nameof(user));

            return Task.FromResult(user.PhoneNumber);
        }

        public virtual Task<bool> GetPhoneNumberConfirmedAsync (TUser user, CancellationToken cancellationToken) {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (user == null) throw new ArgumentNullException(nameof(user));

            return Task.FromResult(user.PhoneNumberConfirmed);
        }

        public virtual Task SetPhoneNumberConfirmedAsync (TUser user, bool confirmed, CancellationToken cancellationToken) {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (user == null) throw new ArgumentNullException(nameof(user));

            user.PhoneNumberConfirmed = confirmed;

            return Task.FromResult(0);
        }
        #endregion

        #region IUserTwoFactorStore<TUser>
        public virtual Task SetTwoFactorEnabledAsync (TUser user, bool enabled, CancellationToken cancellationToken) {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (user == null) throw new ArgumentNullException(nameof(user));

            user.TwoFactorEnabled = enabled;

            return Task.FromResult(0);
        }

        public virtual Task<bool> GetTwoFactorEnabledAsync (TUser user, CancellationToken cancellationToken) {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (user == null) throw new ArgumentNullException(nameof(user));

            return Task.FromResult(user.TwoFactorEnabled);
        }
        #endregion

        #region IUserLockoutStore<TUser>
        public virtual Task<DateTimeOffset?> GetLockoutEndDateAsync (TUser user, CancellationToken cancellationToken) {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (user == null) throw new ArgumentNullException(nameof(user));

            return Task.FromResult(user.LockoutEnd);
        }

        public virtual Task SetLockoutEndDateAsync (TUser user, DateTimeOffset? lockoutEnd, CancellationToken cancellationToken) {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (user == null) throw new ArgumentNullException(nameof(user));

            user.LockoutEnd = lockoutEnd;

            return Task.FromResult(0);
        }

        public virtual Task<int> IncrementAccessFailedCountAsync (TUser user, CancellationToken cancellationToken) {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (user == null) throw new ArgumentNullException(nameof(user));

            return Task.FromResult(++user.AccessFailedCount);
        }

        public virtual Task ResetAccessFailedCountAsync (TUser user, CancellationToken cancellationToken) {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (user == null) throw new ArgumentNullException(nameof(user));

            user.AccessFailedCount = 0;

            return Task.FromResult(0);
        }

        public virtual Task<int> GetAccessFailedCountAsync (TUser user, CancellationToken cancellationToken) {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (user == null) throw new ArgumentNullException(nameof(user));

            return Task.FromResult(user.AccessFailedCount);
        }

        public virtual Task<bool> GetLockoutEnabledAsync (TUser user, CancellationToken cancellationToken) {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (user == null) throw new ArgumentNullException(nameof(user));

            return Task.FromResult(user.LockoutEnabled);
        }

        public virtual Task SetLockoutEnabledAsync (TUser user, bool enabled, CancellationToken cancellationToken) {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (user == null) throw new ArgumentNullException(nameof(user));

            user.LockoutEnabled = enabled;

            return Task.FromResult(0);
        }
        #endregion
    }
}