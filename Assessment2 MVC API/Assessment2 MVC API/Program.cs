using Assessment2_MVC_API.Data;
using Assessment2_MVC_API.Models;
using Assessment2_MVC_API.Settings;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;


var builder = WebApplication.CreateBuilder(args);

// https://www.yogihosting.com/aspnet-core-identity-mongodb/            <-- Mongo Identity Tutorial 28/10/2024 
// https://github.com/alexandre-spieser/AspNetCore.Identity.MongoDbCore <-- important for this
//
// Process of how i got it to work:
//
// Installed:
// - AspNetCore.Identity.MongoDbCore - Just this
// 
// Removed:
// - MongoDB.Driver 3.0.0 - It came with too much shit and overwrote functions for Identity.MongoDbCore
//

builder.Services.AddIdentity<ApplicationUser, ApplicationRole>()
        .AddMongoDbStores<ApplicationUser, ApplicationRole, Guid>
        (
            "mongodb+srv://p467103:[REDACTED-MONGO-PASS]@[REDACTED-CLUSTER]/StoreDB?retryWrites=true&w=majority", "StoreDB" //make sure the uri is correct... JESUS
        );

// Add services to the container.
builder.Services.AddControllers();

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

// FORCE CHANGE API NAME HERE:
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo 
    { 
        Title = "FlowerSales.API", 
        Version = "v1" 
    });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme 
    { In = ParameterLocation.Header, 
        Description = "Please enter into field the word 'Bearer' followed by a space and the JWT token", 
        Name = "Authorization", 
        Type = SecuritySchemeType.ApiKey, 
        Scheme = "Bearer" 
    }); 
    
    c.AddSecurityRequirement(new OpenApiSecurityRequirement() 
    { 
        { 
            new OpenApiSecurityScheme 
            { 
                Reference = new OpenApiReference 
                { Type = ReferenceType.SecurityScheme, 
                    Id = JwtBearerDefaults.AuthenticationScheme
                }
            }, 
            new List<string>() 
        } 
    });
});

builder.Services.AddSingleton<MongoDbService>();
builder.Services.AddTransient<LocalDataService>();

builder.Services.AddDbContext<StoreContext>(options =>
{
    options.UseInMemoryDatabase("Store");
});

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidIssuer = builder.Configuration["JwtConfig:Issuer"],
        ValidAudience = builder.Configuration["JwtConfig:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JwtConfig:Key"]!)),
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true
    };
});
builder.Services.AddAuthorization(options =>
{ 
    options.AddPolicy("RequireAdminRole", policy => policy.RequireRole("Admin")); 
    options.AddPolicy("RequireUserRole", policy => policy.RequireRole("User")); 
});

// ENABLE CORS <--
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(builder =>
    {
        builder
            .WithOrigins("https://localhost:7165")
            .WithHeaders("FlowerStore-API-Version");
    });
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

app.UseAuthentication(); 
app.UseAuthorization();

app.UseCors(); // cors <--

app.MapControllers();

app.Run();
