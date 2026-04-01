using InterStyle.ApiShared;
using InterStyle.ApiShared.Auth;
using InterStyle.Curtains.Api;
using InterStyle.Curtains.Application;
using InterStyle.Curtains.Application.Commands;
using InterStyle.Curtains.Application.Queries;
using InterStyle.Curtains.Domain;
using InterStyle.Curtains.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHealthChecksDefaults();

builder.Services.AddInterStyleMediatR<CreateCurtainCommand>(builder.Configuration);

builder.Services.AddDefaultDbContext<CurtainsDbContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("curtainsdb"));
});

var withApiVersioning = builder.Services.AddApiVersioning();

builder.AddDefaultOpenApi(withApiVersioning);

builder.Services.AddApiVersioning();

builder.Services.AddScoped<ICurtainRepository, CurtainRepository>();

builder.Services.Configure<CurtainsCacheOptions>(builder.Configuration.GetSection(CurtainsCacheOptions.SectionName));

builder.Services.AddScoped<CurtainsQueries>();
builder.Services.AddScoped(sp =>
    new CachedCurtainsQueries(
        sp.GetRequiredService<CurtainsQueries>(),
        sp.GetRequiredService<IDistributedCache>(),
        sp.GetRequiredService<IOptionsSnapshot<CurtainsCacheOptions>>()));
builder.Services.AddScoped<ICurtainQueries>(sp => sp.GetRequiredService<CachedCurtainsQueries>());
builder.Services.AddScoped<ICurtainCacheInvalidator>(sp => sp.GetRequiredService<CachedCurtainsQueries>());

builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration.GetConnectionString("cache");
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddInterStyleJwtAuth(builder.Configuration);

builder.Services.AddHttpClient("ImageApi", client =>
{
    client.BaseAddress = new Uri(builder.Configuration.GetRequiredValue("INTERSTYLE_IMAGEAPI_HTTP"), UriKind.Absolute);
}).AddServiceDiscovery()
  .AddStandardResilienceHandler(options =>
  {
      options.AttemptTimeout.Timeout = TimeSpan.FromSeconds(10);

      options.Retry.MaxRetryAttempts = 3;
      options.Retry.BackoffType = Polly.DelayBackoffType.Exponential;
      options.Retry.Delay = TimeSpan.FromMilliseconds(200);

      options.CircuitBreaker.FailureRatio = 0.5;
      options.CircuitBreaker.MinimumThroughput = 10;
      options.CircuitBreaker.BreakDuration = TimeSpan.FromSeconds(15);
  }); ;

builder.Services.AddServiceDiscovery();

builder.ConfigureOpenTelemetry("InterStyle.Curtains.Api");

var app = builder.Build();

app.UseSwaggerDefaults();
app.UseHealthChecksDefaults();

app.UseDefaultOpenApi();

var curtains = app.NewVersionedApi("Curtains");

curtains.MapCurainsApiV1();

app.Run();