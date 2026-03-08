using InterStyle.AppHost;
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

var endpoint = builder.AddParameter("registry-endpoint");
var repository = builder.AddParameter("registry-repository");
#pragma warning disable ASPIRECOMPUTE003
builder.AddContainerRegistry("container-registry", endpoint, repository);
#pragma warning restore ASPIRECOMPUTE003

var postgres = builder.AddPostgres("postgres");

if (builder.Environment.IsDevelopment())
{
    postgres.WithPgAdmin();
}

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

var leadsApi = builder.AddProject<Projects.InterStyle_Leads_Api>("interstyle-leads-api")
    .WithReference(leadsDb).WaitFor(leadsDb)
    .WithJwtAuthority(IdentityApiUrl);

var reviewsApi = builder.AddProject<Projects.InterStyle_Reviews_Api>("interstyle-reviews-api")
    .WithReference(reviewsDb).WaitFor(reviewsDb)
    .WithJwtAuthority(IdentityApiUrl);

var imageApi = builder.AddProject<Projects.InterStyle_ImageApi>("interstyle-imageapi");

var curtainsApi = builder.AddProject<Projects.InterStyle_Curtains_Api>("interstyle-curtains-api")
    .WithReference(curtainsDb).WaitFor(curtainsDb)
    .WithReference(imageApi);

var identityApi = builder.AddProject<Projects.InterStyle_IdentityApi>(IdentityApiName)
    .WithEnvironment("Admin__Username", adminLogin)
    .WithEnvironment("Admin__Password", adminPassword)
    .WithJwtSigningKey(jwtActiveKid, jwtPfx, jwtPfxPassword);

var adminPanel = builder.AddProject<Projects.AdminPanel>("interstyle-admin-panel");

builder.AddYarp("interstyle-gateway")
    .WithExternalHttpEndpoints()
    .ConfigureInterStyleRoutes(leadsApi, reviewsApi, curtainsApi, imageApi, identityApi, adminPanel);

builder.Build().Run();
