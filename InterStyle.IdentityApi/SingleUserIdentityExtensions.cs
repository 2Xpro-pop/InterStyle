using InterStyle.ApiShared.Auth;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace InterStyle.IdentityApi;

public static class SingleUserIdentityExtensions
{
    public static IServiceCollection AddSingleUserIdentity(this IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddIdentityCore<AdminUser>(options =>
            {
                options.User.RequireUniqueEmail = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireDigit = false;
                options.Password.RequiredLength = 6;
            })
            .AddSignInManager()
            .AddDefaultTokenProviders();

        services.AddScoped<IPasswordHasher<AdminUser>, PasswordHasher<AdminUser>>();

        services.AddAuthorization();

        services.AddScoped(sp =>
        {
            var hasher = sp.GetRequiredService<IPasswordHasher<AdminUser>>();

            var username = configuration["Admin:Username"] ?? "admin";
            var password = configuration["Admin:Password"] ?? "admin";

            var user = new AdminUser
            {
                Id = "admin-1",
                UserName = username,
                NormalizedUserName = username.ToUpperInvariant(),
                SecurityStamp = Guid.NewGuid().ToString("N")
            };

            user.PasswordHash = hasher.HashPassword(user, password);

            return new SingleAdminUserStore(user, seededRoles: [InterStyleRoles.Admin]);
        });

        services.AddScoped<IUserStore<AdminUser>>(sp => sp.GetRequiredService<SingleAdminUserStore>());

        services.AddScoped<UserManager<AdminUser>>();

        return services;
    }
}
