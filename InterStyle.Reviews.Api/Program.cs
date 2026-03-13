using InterStyle.ApiShared;
using InterStyle.Reviews.Api;
using InterStyle.Reviews.Api.Services;
using InterStyle.Reviews.Application.Commands;
using InterStyle.Reviews.Application.Queries;
using InterStyle.Reviews.Domain;
using InterStyle.Reviews.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHealthChecks();

builder.Services.AddInterStyleMediatR<ApproveReviewCommand>(builder.Configuration);

builder.Services.AddDefaultDbContext<ReviewsDbContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("reviewsdb"));
});

var withApiVersioning = builder.Services.AddApiVersioning();

builder.AddDefaultOpenApi(withApiVersioning);

builder.Services.AddScoped<IReviewRepository, ReviewRepository>();

builder.Services.AddScoped<ReviewQueries>();
builder.Services.AddScoped<IReviewQueries>(sp =>
    new CachedReviewQueries(
        sp.GetRequiredService<ReviewQueries>(),
        sp.GetRequiredService<IDistributedCache>()));

builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration.GetConnectionString("cache");
});

builder.Services.AddHttpClient<ICaptchaValidator, GoogleCaptchaValidator>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseSwaggerDefaults();
app.UseHealthChecksDefaults();

app.UseDefaultOpenApi();

var reviews = app.NewVersionedApi("Reviews");

reviews.MapReviewsApiV1();

app.Run();
