using InterStyle.ApiShared;
using InterStyle.Reviews.Api;
using InterStyle.Reviews.Api.Services;
using InterStyle.Reviews.Application;
using InterStyle.Reviews.Application.Commands;
using InterStyle.Reviews.Application.Queries;
using InterStyle.Reviews.Domain;
using InterStyle.Reviews.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;

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

builder.Services.Configure<ReviewsCacheOptions>(builder.Configuration.GetSection(ReviewsCacheOptions.SectionName));

builder.Services.AddScoped<ReviewQueries>();
builder.Services.AddScoped(sp =>
    new CachedReviewQueries(
        sp.GetRequiredService<ReviewQueries>(),
        sp.GetRequiredService<IDistributedCache>(),
        sp.GetRequiredService<IOptionsSnapshot<ReviewsCacheOptions>>()));
builder.Services.AddScoped<IReviewQueries>(sp => sp.GetRequiredService<CachedReviewQueries>());
builder.Services.AddScoped<IReviewCacheInvalidator>(sp => sp.GetRequiredService<CachedReviewQueries>());

builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration.GetConnectionString("cache");
});

builder.Services.AddHttpClient<ICaptchaValidator, GoogleCaptchaValidator>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.ConfigureOpenTelemetry("InterStyle.Reviews.Api");

var app = builder.Build();

app.UseSwaggerDefaults();
app.UseHealthChecksDefaults();

app.UseDefaultOpenApi();

var reviews = app.NewVersionedApi("Reviews");

reviews.MapReviewsApiV1();

app.Run();
