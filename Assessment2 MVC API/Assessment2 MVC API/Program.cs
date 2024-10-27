using Assessment2_MVC_API.Data;
using Assessment2_MVC_API.Models;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using System.Data.Common;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using AspNetCore.Identity.MongoDbCore.Infrastructure;
using MongoDB.Driver;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

// https://www.yogihosting.com/aspnet-core-identity-mongodb/

// Dependency Injections variants: //
// 1. Singleton
// Singleton service is created once and reused for the tnire liftime of an application
// Same instance will be injected into every dependent class through the application.
// This makes it suitable for stateless services that hold shared state across the application.

// 2. Transient
// Transient service is created each time it is requested.
// It is not reused and is disposed of after the request is completed
// Transient services are suitable for lightweight, stateless services
// Where a new instance is needed for every request or operation.

// 3. Scoped
// A Scoped service is created once per request.
// It remains the same within a single request but differs across different requests.
// The service is disposed of when the request is completed.
// This makes it suitable for services that need to maintain a state across
// multiple operations within a single request.





#region LC FIXING API DOCUMENTATION WEEK 7
builder.Services.AddApiVersioning(options =>
{
    options.ReportApiVersions = true;
    options.DefaultApiVersion = new Microsoft.AspNetCore.Mvc.ApiVersion(1, 0);
    options.AssumeDefaultVersionWhenUnspecified = true;

    options.ApiVersionReader = new QueryStringApiVersionReader("FlowerStore-API-Version");

   // options.ApiVersionReader = new HeaderApiVersionReader("X-API-Version");
});

builder.Services.AddVersionedApiExplorer(options =>
{ 
    // declare version number
    options.GroupNameFormat = "'v'VVV";
    options.SubstituteApiVersionInUrl = true;
});
#endregion

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerGen();

// FORCE CHANGE API NAME HERE:
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "FlowerSales.API", Version = "v1" });
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseHsts();
}

app.UseHttpsRedirection();

// Midware
app.UseAuthentication(); 
app.UseAuthorization();

app.UseCors(); // cors <--

app.MapControllers();

app.Run();
