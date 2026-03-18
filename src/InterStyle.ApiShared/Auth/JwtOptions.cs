using System;
using System.Collections.Generic;
using System.Text;

namespace InterStyle.ApiShared.Auth;

public sealed class JwtOptions
{
    public string Issuer { get; set; } = "http://interstyle-identityapi";
    public string Audience { get; set; } = "interstyle";
    public int Minutes { get; set; } = 60;

    public string? Authority { get; set; } = "http://interstyle-identityapi";

    public string? PublicKey { get; set; }

    public JwtSigningOptions Signing { get; set; } = new();
}

public sealed class JwtSigningOptions
{
    public string ActiveKid { get; set; } = "";

    public List<JwtSigningKey> Keys { get; set; } = [];
}

public sealed class JwtSigningKey
{
    public string Kid { get; set; } = "";

    public string PfxBase64 { get; set; } = "";

    public string? PfxPassword { get; set; }
}