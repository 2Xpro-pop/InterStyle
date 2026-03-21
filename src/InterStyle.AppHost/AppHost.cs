using InterStyle.AppHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

var builder = DistributedApplication.CreateBuilder(args);

if(builder.Environment.IsDevelopment())
{
    builder.Services.AddLogging(logging =>
    {
        logging.SetMinimumLevel(LogLevel.Debug);
    });

}

var compose = builder.AddDockerComposeEnvironment("compose");

if (builder.Environment.IsProduction())
{
    compose.WithDashboard(dashboard =>
    {
        dashboard.WithHostPort(32770)
                 .WithForwardedHeaders(enabled: true);
    });
}

var endpoint = builder.AddParameter("registry-endpoint");
var repository = builder.AddParameter("registry-repository");
#pragma warning disable ASPIRECOMPUTE003
if(ShouldUseContainerRegistry())
{
    builder.AddContainerRegistry("container-registry", endpoint, repository);
}
#pragma warning restore ASPIRECOMPUTE003

var postgres = builder.AddPostgres("postgres");

if (builder.Environment.IsDevelopment())
{
    postgres.WithPgAdmin();
}

if(builder.Environment.IsProduction())
{
    postgres.WithPassword(builder.AddParameter("postgres-password", secret: true));
}

var cache = builder.AddRedis("cache");

const string IdentityApiName = "interstyle-identityapi";
const string IdentityApiUrl = $"http://{IdentityApiName}";

var leadsDb = postgres.AddDatabase("leadsdb");
var reviewsDb = postgres.AddDatabase("reviewsdb");
var curtainsDb = postgres.AddDatabase("curtainsdb");

var adminLogin = builder.AddParameter("admin-login", secret: true);
var adminPassword = builder.AddParameter("admin-password", secret: true);

var jwtPfx = builder.AddParameter("jwt-signing-pfx", secret: true);
var jwtPfxPassword = builder.AddParameter("jwt-signing-password", secret: true);
var jwtActiveKid = builder.AddParameter("jwt-active-kid", secret: false);

var captchaGoogleToken = builder.AddParameter("captcha-google-token", secret: true);
var captchaGoogleSiteKey = builder.AddParameter("captcha-google-site-key", "6LdKtoYsAAAAANvmYl4ew2_nQdQzKsb1v3u6eKCk", secret: false);
var mediatRLicenseKey = builder.AddParameter("mediatr-license-key", secret: true);

var leadsApi = builder.AddProject<Projects.InterStyle_Leads_Api>("interstyle-leads-api")
    .WithPublicJwtKey(jwtPfx, jwtPfxPassword)
    .WithReference(leadsDb).WaitFor(leadsDb)
    .WithMediatrLicense(mediatRLicenseKey)
    .WithHttpHealthCheck("/health")
    .WithJwtAuthority(IdentityApiUrl);

var reviewsApi = builder.AddProject<Projects.InterStyle_Reviews_Api>("interstyle-reviews-api")
    .WithReference(reviewsDb).WaitFor(reviewsDb)
    .WithMediatrLicense(mediatRLicenseKey)
    .WithEnvironment("Captcha__SecretKey", captchaGoogleToken)
    .WithReference(cache).WaitFor(cache)
    .WithHttpHealthCheck("/health")
    .WithJwtAuthority(IdentityApiUrl);

var imageApi = builder.AddProject<Projects.InterStyle_ImageApi>("interstyle-imageapi")
    .WithHttpHealthCheck("/health");

var curtainsApi = builder.AddProject<Projects.InterStyle_Curtains_Api>("interstyle-curtains-api")
    .WithReference(curtainsDb).WaitFor(curtainsDb)
    .WithReference(imageApi)
    .WithPublicJwtKey(jwtPfx, jwtPfxPassword)
    .WithMediatrLicense(mediatRLicenseKey)
    .WithReference(cache).WaitFor(cache)
    .WithHttpHealthCheck("/health")
    .WithJwtAuthority(IdentityApiUrl);

var identityApi = builder.AddProject<Projects.InterStyle_IdentityApi>(IdentityApiName)
    .WithEnvironment("Admin__Username", adminLogin)
    .WithEnvironment("Admin__Password", adminPassword)
    .WithHttpHealthCheck("/health")
    .WithPublicJwtKey(jwtPfx, jwtPfxPassword)
    .WithJwtSigningKey(jwtActiveKid, jwtPfx, jwtPfxPassword);

var adminPanel = builder.AddDockerfile(
    "interstyle-admin-panel",
    "../../",
    "src/AdminPanel/Dockerfile")
    .WithHttpEndpoint(targetPort: 8080);


var gateway = builder.AddYarp("interstyle-gateway");

if(builder.Environment.IsProduction())
{
    gateway.WithHostPort(8321);
}

gateway.WithExternalHttpEndpoints();

var client = builder.AddDockerfile("interstyle-client", "../InterStyle.Client")
    .WithReference(gateway)
    .WaitFor(gateway)
    .WithEnvironment("PUBLIC_API_GATEWAY_URL", gateway.GetEndpoint("http"))
    .WithEnvironment("PUBLIC_RECAPTCHA_SITE_KEY", captchaGoogleSiteKey)
    .WithEnvironment("BROWSER", "none")
    .WithHttpEndpoint(targetPort:3000);

if(builder.Environment.IsDevelopment() || builder.Environment.IsStaging())
{
    client.WithEnvironment("PROTOCOL_HEADER", "x-forwarded-proto")
          .WithEnvironment("HOST_HEADER", "x-forwarded-host");
}

if(builder.Environment.IsProduction())
{
    client.WithEnvironment("ORIGIN", "https://interstyle.kg");
}

gateway.ConfigureInterStyleRoutes(leadsApi, reviewsApi, curtainsApi, imageApi, identityApi, adminPanel.GetEndpoint("http"), client.GetEndpoint("http"));

var app = builder.Build();

app.Run();


bool ShouldUseContainerRegistry()
{
    var useContainerRegistry = builder.Configuration.GetValue<bool?>("use-container-registry") ?? true;

    return builder.Environment.IsProduction() && useContainerRegistry;
}