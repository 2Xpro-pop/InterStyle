using InterStyle.AppHost;
using Microsoft.Extensions.Hosting;

var builder = DistributedApplication.CreateBuilder(args);

var compose = builder.AddDockerComposeEnvironment("compose");

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

builder.AddYarp("interstyle-apigateway")
    .WithExternalHttpEndpoints()
    .ConfigureInterStyleRoutes(leadsApi, reviewsApi, curtainsApi, imageApi, identityApi);

builder.Build().Run();
