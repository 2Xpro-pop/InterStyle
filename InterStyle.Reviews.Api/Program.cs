using InterStyle.ApiShared;
using InterStyle.Reviews.Api;
using InterStyle.Reviews.Application.Commands;
using InterStyle.Reviews.Application.Queries;
using InterStyle.Reviews.Domain;
using InterStyle.Reviews.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHealthChecks();

builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<ApproveReviewCommand>());

builder.Services.AddDefaultDbContext<ReviewsDbContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("reviewsdb"));
});

var withApiVersioning = builder.Services.AddApiVersioning();

builder.AddDefaultOpenApi(withApiVersioning);

builder.Services.AddScoped<IReviewRepository, ReviewRepository>();

builder.Services.AddScoped<IReviewQueries, ReviewQueries>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseSwaggerDefaults();
app.UseHealthChecksDefaults();

app.UseDefaultOpenApi();

var reviews = app.NewVersionedApi("Reviews");

reviews.MapReviewsApiV1();

app.Run();
