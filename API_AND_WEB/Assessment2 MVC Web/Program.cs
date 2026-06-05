using Assessment2_MVC_Web.Models;

// Followed tutorial here at: https://www.yogihosting.com/aspnet-core-identity-mongodb/


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Load MongoDB connection string from configuration (User Secrets / Environment Variables)
var mongoConnectionString = builder.Configuration.GetConnectionString("DbConnection");

if (string.IsNullOrEmpty(mongoConnectionString))
{
    throw new InvalidOperationException(
        "MongoDB connection string 'ConnectionStrings:DbConnection' is missing.\n" +
        "Please set it using User Secrets (recommended for development) or environment variables.");
}

builder.Services.AddIdentity<ApplicationUser, ApplicationRole>()
    .AddMongoDbStores<ApplicationUser, ApplicationRole, Guid>(
        mongoConnectionString,
        "StoreDB"                    // ← Your database name
    );

// Site / Store branding settings (centralized title, display name, emoji, etc.)
builder.Services.Configure<SiteSettings>(builder.Configuration.GetSection("Site"));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
