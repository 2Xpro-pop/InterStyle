var builder = DistributedApplication.CreateBuilder(args);

var postgres = builder.AddPostgres("postgres")
    .WithPgAdmin();

var leadsDb = postgres.AddDatabase("leadsdb");
var reviewsDb = postgres.AddDatabase("reviewsdb");
var curtainsDb = postgres.AddDatabase("curtainsdb");

var username = builder.AddParameter("username", secret: true);
var password = builder.AddParameter("password", secret: true);

var rabbit = builder.AddRabbitMQ("rabbitmq", username, password)
    .WithManagementPlugin();

builder.AddProject<Projects.InterStyle_Leads_Api>("interstyle-leads-api")
    .WithReference(leadsDb).WaitFor(leadsDb);

builder.AddProject<Projects.InterStyle_Reviews_Api>("interstyle-reviews-api")
    .WithReference(reviewsDb).WaitFor(reviewsDb);

var imageApi = builder.AddProject<Projects.InterStyle_ImageApi>("interstyle-imageapi")
    .WithReference(rabbit).WaitFor(rabbit);

builder.AddProject<Projects.InterStyle_Curtains_Api>("interstyle-curtains-api")
    .WithReference(curtainsDb).WaitFor(curtainsDb)
    .WithReference(rabbit).WaitFor(rabbit)
    .WithReference(imageApi);

builder.Build().Run();
