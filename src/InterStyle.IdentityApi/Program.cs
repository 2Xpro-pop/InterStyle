using Asp.Versioning;
using InterStyle.ApiShared;
using InterStyle.ApiShared.Auth;
using InterStyle.IdentityApi;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System.Security.Cryptography.X509Certificates;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHealthChecksDefaults();

var withApiVersioning = builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.AssumeDefaultVersionWhenUnspecified = true;
});

builder.AddDefaultOpenApi(withApiVersioning);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleUserIdentity(builder.Configuration);

builder.Services.AddInterStyleJwtAuth(builder.Configuration);
builder.Services.AddSingleton<JwtSigningKeyStore>();
builder.Services.AddSingleton<JwtTokenIssuer>();

builder.ConfigureOpenTelemetry("InterStyle.IdentityApi");

var app = builder.Build();

app.UseSwaggerDefaults();
app.UseHealthChecksDefaults();

app.UseDefaultOpenApi();

app.MapJwks();

var identity = app.NewVersionedApi("identity");

identity.MapIdentityApiV1();

app.Run();

