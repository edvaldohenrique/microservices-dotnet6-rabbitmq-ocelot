using Duende.IdentityServer.Services;
using GeekShopping.IdentityServer.Configuration;
using GeekShopping.IdentityServer.Initializer;
using GeekShopping.IdentityServer.Model;
using GeekShopping.IdentityServer.Model.Context;
using GeekShopping.IdentityServer.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

//Config EntityFrameWork
var connection = builder.Configuration["MySqlConnection:MySqlConnectionString"];

builder.Services.AddControllers();
builder.Services.AddDbContext<MySQLContext>(options => options.
    UseMySql(connection,
             new MySqlServerVersion(
                 new Version(8, 0, 28))));

//Config Duende identity server
builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<MySQLContext>()
    .AddDefaultTokenProviders();

//Injection de classes do DB
builder.Services.AddScoped<IDbInitializer, DbInitializer>();

var identity = builder.Services.AddIdentityServer(options => 
                                {
                                    options.Events.RaiseErrorEvents = true;
                                    options.Events.RaiseFailureEvents = true;
                                    options.Events.RaiseInformationEvents  = true;
                                    options.Events.RaiseSuccessEvents = true;
                                    options.EmitStaticAudienceClaim = true;
                                }).AddInMemoryIdentityResources(IdentityConfiguration.IdentityResourcers)
                                  .AddInMemoryApiScopes(IdentityConfiguration.ApiScopes)
                                  .AddInMemoryClients(IdentityConfiguration.Clients)
                                  .AddAspNetIdentity<ApplicationUser>();

identity.AddDeveloperSigningCredential();

builder.Services.AddScoped<IProfileService, ProfileService>();

var app = builder.Build();

//Injection de classes do DB
var dbInitializer = app.Services.CreateScope().ServiceProvider.GetService<IDbInitializer>();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();
app.UseIdentityServer(); //Habilita identity server
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

dbInitializer.Initializer();

app.Run();
