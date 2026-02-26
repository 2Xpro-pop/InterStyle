var builder = DistributedApplication.CreateBuilder(args);

var postgres = builder.AddPostgres("postgres");

var leadsDb = postgres.AddDatabase("leadsdb");
var reviewsDb = postgres.AddDatabase("reviewsdb");
var curtainsDb = postgres.AddDatabase("curtainsdb");

builder.AddProject<Projects.InterStyle_Leads_Api>("interstyle-leads-api")
    .WithReference(leadsDb).WaitFor(leadsDb);

builder.AddProject<Projects.InterStyle_Reviews_Api>("interstyle-reviews-api")
    .WithReference(reviewsDb).WaitFor(reviewsDb);

builder.AddProject<Projects.InterStyle_Curtains_Api>("interstyle-curtains-api")
    .WithReference(curtainsDb).WaitFor(curtainsDb);

builder.Build().Run();
