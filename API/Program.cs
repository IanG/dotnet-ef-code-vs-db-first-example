using EFExample.Common.Data;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.OpenApi.Models;
using Serilog;
using Serilog.Core;

var builder = WebApplication.CreateBuilder(args);

// Set up Logging with SeriLog
Logger logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .CreateLogger();

builder.Services.AddSerilog(logger);

// Add services to the container.

// Set up the database contexts
builder.Services.AddEFExampleDbContexts(builder.Configuration);

// Force all routes and query strings to be lowercase
builder.Services.AddRouting(options =>
{
    options.LowercaseUrls = true;
    options.LowercaseQueryStrings = true;
});

builder.Services.AddHttpLogging(o => { });
builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });
    c.CustomSchemaIds(type => type.FullName); 
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();