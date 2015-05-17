namespace AspNet.Identity.MongoDB {
    using System.Threading.Tasks;
    using global::MongoDB.Driver;
    using Microsoft.AspNet.Identity;
    using System.Threading;
    using System;



    /// todo when the new LINQ implementation arrives in the 2.1 driver we can add back IQueryableRoleStore and IQueryableUserStore: https://jira.mongodb.org/browse/CSHARP-935
    /// <summary>
    ///     Note: Deleting and updating do not modify the roles stored on a user document. If you desire this dynamic
    ///     capability, override the appropriate operations on RoleStore as desired for your application. For example you could
    ///     perform a document modification on the users collection before a delete or a rename.
    /// </summary>
    /// <typeparam name="TRole"></typeparam>
    public class RoleStore<TRole> : IRoleStore<TRole>/*,
                                    IRoleClaimStore<TRole>,
                                    IQueryableRoleStore<TRole>*/ where TRole : IdentityRole {
        #region Fields
        private readonly IMongoCollection<TRole> _roles;
        private bool _disposed;
        #endregion

        public RoleStore (IMongoCollection<TRole> roles) {
            _roles = roles;
        }

        #region IRoleStore<TRole>
        public async Task<IdentityResult> CreateAsync (TRole role, CancellationToken cancellationToken) {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (role == null) throw new ArgumentNullException(nameof(role));

            try {
                await _roles.InsertOneAsync(role, cancellationToken);

            } catch (Exception e) {
                return IdentityResult.Failed(
                    new IdentityError {
                        Description = e.Message
                    }
                );
            }

            return IdentityResult.Success;
        }

        public async Task<IdentityResult> UpdateAsync (TRole role, CancellationToken cancellationToken) {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (role == null) throw new ArgumentNullException(nameof(role));

            try {
                ReplaceOneResult result = await _roles.ReplaceOneAsync(r => r.Id == role.Id, role, cancellationToken: cancellationToken);

                if (result.ModifiedCount == 1) {
                    return IdentityResult.Success;

                } else {
                    return IdentityResult.Failed(new IdentityError {
                        Description = $"The role {role.Name} (ID: {role.Id}) could not be updated."
                    });
                }

            } catch (Exception e) {
                return IdentityResult.Failed(
                    new IdentityError {
                        Description = e.Message
                    }
                );
            }
        }

        public async Task<IdentityResult> DeleteAsync (TRole role, CancellationToken cancellationToken) {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (role == null) throw new ArgumentNullException(nameof(role));

            try {
                DeleteResult result = await _roles.DeleteOneAsync(r => r.Id == role.Id);

                if (result.DeletedCount == 1) {
                    return IdentityResult.Success;

                } else {
                    return IdentityResult.Failed(
                        new IdentityError {
                            Description = $"The role {role.Name} (ID: {role.Id}) could not be deleted."
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

        public Task<String> GetRoleIdAsync (TRole role, CancellationToken cancellationToken) {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (role == null) throw new ArgumentNullException(nameof(role));

            return Task.FromResult(role.Id);
        }

        public Task<String> GetRoleNameAsync (TRole role, CancellationToken cancellationToken) {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (role == null) throw new ArgumentNullException(nameof(role));

            return Task.FromResult(role.Name);
        }

        public Task SetRoleNameAsync (TRole role, String roleName, CancellationToken cancellationToken) {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (role == null) throw new ArgumentNullException(nameof(role));
            if (String.IsNullOrWhiteSpace(roleName)) throw new ArgumentNullException(nameof(roleName));

            role.Name = roleName;

            return Task.FromResult(0);
        }

        public Task<String> GetNormalizedRoleNameAsync (TRole role, CancellationToken cancellationToken) {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (role == null) throw new ArgumentNullException(nameof(role));

            return Task.FromResult(role.Name);
        }

        public Task SetNormalizedRoleNameAsync (TRole role, String normalizedName, CancellationToken cancellationToken) {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (role == null) throw new ArgumentNullException(nameof(role));
            if (String.IsNullOrWhiteSpace(normalizedName)) throw new ArgumentNullException(nameof(normalizedName));

            role.Name = normalizedName;

            return Task.FromResult(0);
        }

        public Task<TRole> FindByIdAsync (String roleId, CancellationToken cancellationToken) {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (String.IsNullOrWhiteSpace(roleId)) throw new ArgumentNullException(nameof(roleId));

            return _roles.Find(r => r.Id == roleId).FirstOrDefaultAsync(cancellationToken);
        }

        public Task<TRole> FindByNameAsync (String normalizedRoleName, CancellationToken cancellationToken) {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (String.IsNullOrWhiteSpace(normalizedRoleName)) throw new ArgumentNullException(nameof(normalizedRoleName));

            return _roles.Find(r => r.Name == normalizedRoleName).FirstOrDefaultAsync();
        }
        #endregion

        private void ThrowIfDisposed () {
            if (_disposed) {
                throw new ObjectDisposedException(GetType().Name);
            }
        }

        public virtual void Dispose () {
            // no need to dispose of anything, mongodb handles connection pooling automatically
            _disposed = true;
        }
    }
}