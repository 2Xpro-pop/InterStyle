using InterStyle.ApiShared.Auth;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace InterStyle.IdentityApi;

public sealed class JwtTokenIssuer(JwtSigningKeyStore store, IOptions<JwtOptions> jwtOptions)
{
    private readonly JwtOptions _jwt = jwtOptions.Value;

    public string IssueAdminToken(string userId, string username)
    {
        var cert = store.GetActiveCert();

        var signingKey = new X509SecurityKey(cert)
        {
            KeyId = store.ActiveKid
        };

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, userId),
            new(JwtRegisteredClaimNames.UniqueName, username),
            new(ClaimTypes.Name, username),
            new(ClaimTypes.Role, InterStyleRoles.Admin),
        };

        var creds = new SigningCredentials(signingKey, SecurityAlgorithms.RsaSha256);

        var token = new JwtSecurityToken(
            issuer: _jwt.Issuer,
            audience: _jwt.Audience,
            claims: claims,
            notBefore: DateTime.UtcNow,
            expires: DateTime.UtcNow.AddMinutes(_jwt.Minutes),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}