using InterStyle.ApiShared;
using InterStyle.Leads.Api;
using InterStyle.Leads.Application.Commands;
using InterStyle.Leads.Application.Queries;
using InterStyle.Leads.Domain;
using InterStyle.Leads.Infrastructure.Persistence;
using MediatR;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Routing.Constraints;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHealthChecks();

builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<CreateLeadCommand>());

builder.Services.AddDefaultDbContext<LeadsDbContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("leadsdb"));
});

var withApiVersioning = builder.Services.AddApiVersioning();

builder.AddDefaultOpenApi(withApiVersioning);

builder.Services.AddScoped<ILeadRepository, LeadRepository>();

builder.Services.AddScoped<ILeadQueries, LeadQueries>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseSwaggerDefaults();
app.UseHealthChecksDefaults();

app.UseDefaultOpenApi();

var leads = app.NewVersionedApi("Leads");

leads.MapLeadsApiV1();

app.Run();