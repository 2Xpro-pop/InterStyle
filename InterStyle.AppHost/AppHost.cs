var builder = DistributedApplication.CreateBuilder(args);

var postgres = builder.AddPostgres("postgres")
    .WithPgAdmin();

var leadsDb = postgres.AddDatabase("leadsdb");
var reviewsDb = postgres.AddDatabase("reviewsdb");
var curtainsDb = postgres.AddDatabase("curtainsdb");

var rabbit = builder.AddRabbitMQ("rabbitmq");

builder.AddProject<Projects.InterStyle_Leads_Api>("interstyle-leads-api")
    .WithReference(leadsDb).WaitFor(leadsDb);

builder.AddProject<Projects.InterStyle_Reviews_Api>("interstyle-reviews-api")
    .WithReference(reviewsDb).WaitFor(reviewsDb);

builder.AddProject<Projects.InterStyle_Curtains_Api>("interstyle-curtains-api")
    .WithReference(curtainsDb).WaitFor(curtainsDb)
    .WithReference(rabbit).WaitFor(rabbit);

builder.AddProject<Projects.InterStyle_ImageApi>("interstyle-imageapi")
    .WithReference(rabbit).WaitFor(rabbit);

builder.Build().Run();
