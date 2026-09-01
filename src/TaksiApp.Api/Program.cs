using TaksiApp.Api.Extensions;
using TaksiApp.Application;
using TaksiApp.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApiServices();
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();

app.UseApiEndpoints();

app.Run();