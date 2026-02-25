var builder = DistributedApplication.CreateBuilder(args);

var postgres = builder.AddPostgres("postgres");

var leadsDb = postgres.AddDatabase("leadsdb");

builder.AddProject<Projects.InterStyle_Leads_Api>("interstyle-leads-api")
    .WithReference(leadsDb);

builder.Build().Run();
