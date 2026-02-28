using InterStyle.ApiShared;
using InterStyle.Curtains.Api;
using InterStyle.Curtains.Application.Commands;
using InterStyle.Curtains.Application.Queries;
using InterStyle.Curtains.Domain;
using InterStyle.Curtains.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHealthChecks();

builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<CreateCurtainCommand>());

builder.Services.AddDefaultDbContext<CurtainsDbContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("curtainsdb"));
});

var withApiVersioning = builder.Services.AddApiVersioning();

builder.AddDefaultOpenApi(withApiVersioning);

builder.Services.AddApiVersioning();

builder.Services.AddScoped<ICurtainRepository, CurtainRepository>();

builder.Services.AddScoped<ICurtainQueries, CurtainsQueries>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseSwaggerDefaults();
app.UseHealthChecksDefaults();

app.UseDefaultOpenApi();

var curtains = app.NewVersionedApi("Curtains");

curtains.MapCurainsApiV1();

app.Run();