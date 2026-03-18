using Microsoft.AspNetCore.Identity;

namespace InterStyle.IdentityApi;

public sealed class SingleAdminUserStore :
    IUserStore<AdminUser>,
    IUserPasswordStore<AdminUser>,
    IUserSecurityStampStore<AdminUser>,
    IUserRoleStore<AdminUser>
{
    private readonly Lock _lock = new();

    private AdminUser? _user; 
    private readonly HashSet<string> _roles = new(StringComparer.OrdinalIgnoreCase);

    public SingleAdminUserStore(AdminUser seededUser, IEnumerable<string>? seededRoles = null)
    {
        _user = seededUser;
        if (seededRoles != null)
            foreach (var r in seededRoles) _roles.Add(r);
    }


    public Task<IdentityResult> CreateAsync(AdminUser user, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        lock (_lock)
        {
            if (_user != null)
            {
                return Task.FromResult(IdentityResult.Failed(new IdentityError { Description = "User already exists." }));
            }

            _user = user;
            return Task.FromResult(IdentityResult.Success);
        }
    }

    public Task<IdentityResult> UpdateAsync(AdminUser user, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        lock (_lock)
        {
            _user = user;
            return Task.FromResult(IdentityResult.Success);
        }
    }

    public Task<IdentityResult> DeleteAsync(AdminUser user, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        lock (_lock)
        {
            if (_user?.Id == user.Id)
            {
                _user = null;
            }

            _roles.Clear();
            return Task.FromResult(IdentityResult.Success);
        }
    }

    public Task<AdminUser?> FindByIdAsync(string userId, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        lock (_lock)
        {
            return Task.FromResult(_user?.Id == userId ? _user : null);
        }
    }

    public Task<AdminUser?> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        lock (_lock)
        {
            var user = _user;
            return user == null 
                ? Task.FromResult<AdminUser?>(null) 
                : Task.FromResult(user.NormalizedUserName == normalizedUserName ? user : null);
        }
    }

    public Task<string> GetUserIdAsync(AdminUser user, CancellationToken cancellationToken)
        => Task.FromResult(user.Id);

    public Task<string?> GetUserNameAsync(AdminUser user, CancellationToken cancellationToken)
        => Task.FromResult(user.UserName);

    public Task SetUserNameAsync(AdminUser user, string? userName, CancellationToken cancellationToken)
    {
        user.UserName = userName;
        return Task.CompletedTask;
    }

    public Task<string?> GetNormalizedUserNameAsync(AdminUser user, CancellationToken cancellationToken)
        => Task.FromResult(user.NormalizedUserName);

    public Task SetNormalizedUserNameAsync(AdminUser user, string? normalizedName, CancellationToken cancellationToken)
    {
        user.NormalizedUserName = normalizedName;
        return Task.CompletedTask;
    }

    public Task SetPasswordHashAsync(AdminUser user, string? passwordHash, CancellationToken cancellationToken)
    {
        user.PasswordHash = passwordHash;
        return Task.CompletedTask;
    }

    public Task<string?> GetPasswordHashAsync(AdminUser user, CancellationToken cancellationToken)
        => Task.FromResult(user.PasswordHash);

    public Task<bool> HasPasswordAsync(AdminUser user, CancellationToken cancellationToken)
        => Task.FromResult(!string.IsNullOrWhiteSpace(user.PasswordHash));

    public Task SetSecurityStampAsync(AdminUser user, string stamp, CancellationToken cancellationToken)
    {
        user.SecurityStamp = stamp;
        return Task.CompletedTask;
    }

    public Task<string?> GetSecurityStampAsync(AdminUser user, CancellationToken cancellationToken)
        => Task.FromResult(user.SecurityStamp);

    public Task AddToRoleAsync(AdminUser user, string roleName, CancellationToken cancellationToken)
    {
        lock (_lock) _roles.Add(roleName);
        return Task.CompletedTask;
    }

    public Task RemoveFromRoleAsync(AdminUser user, string roleName, CancellationToken cancellationToken)
    {
        lock (_lock) _roles.Remove(roleName);
        return Task.CompletedTask;
    }

    public Task<IList<string>> GetRolesAsync(AdminUser user, CancellationToken cancellationToken)
    {
        lock (_lock) return Task.FromResult<IList<string>>(_roles.ToList());
    }

    public Task<bool> IsInRoleAsync(AdminUser user, string roleName, CancellationToken cancellationToken)
    {
        lock (_lock) return Task.FromResult(_roles.Contains(roleName));
    }

    public Task<IList<AdminUser>> GetUsersInRoleAsync(string roleName, CancellationToken cancellationToken)
    {
        lock (_lock)
        {
            var user = _user;
            return user == null
                ? Task.FromResult<IList<AdminUser>>([])
                : Task.FromResult<IList<AdminUser>>(_roles.Contains(roleName) ? [user] : []);
        }
    }

    public void Dispose() { /* nothing */ }
}