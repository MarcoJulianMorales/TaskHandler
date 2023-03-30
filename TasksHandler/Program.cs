using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using TasksHandler;
using Microsoft.AspNetCore.Mvc.Razor;
using TasksHandler.Services;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

var AuthUsersPolicy = new AuthorizationPolicyBuilder()
    .RequireAuthenticatedUser().Build();

// Add services to the container.
builder.Services.AddControllersWithViews(options =>
{
    options.Filters.Add(new AuthorizeFilter(AuthUsersPolicy));
}).AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix)
.AddDataAnnotationsLocalization(options =>
{
    options.DataAnnotationLocalizerProvider = (_, factory) => factory.Create(typeof(SharedResource));
}).AddJsonOptions(options =>
{
    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
});

//Configurando ApplicationDbContext como un servicio
builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer("name=DefaultConnection") );

builder.Services.AddAuthentication();
builder.Services.AddIdentity<IdentityUser, IdentityRole>( options =>{
    options.SignIn.RequireConfirmedAccount= false;
}).AddEntityFrameworkStores<ApplicationDbContext>().AddDefaultTokenProviders();

builder.Services.PostConfigure<CookieAuthenticationOptions>(IdentityConstants.ApplicationScheme,
    options =>
    {
        options.LoginPath = "/Users/Login";
        options.AccessDeniedPath= "/Users/Login";
    });

builder.Services.AddLocalization(options =>
{
    options.ResourcesPath = ("Resources");
});

builder.Services.AddTransient<IUsersService, UsersService>();
builder.Services.AddAutoMapper(typeof(Program));
builder.Services.AddTransient<IFilesRepository, LocalFilesStore>();

var app = builder.Build();

//var SuportedUIcultures = new[] { "es", "en" };

app.UseRequestLocalization(options =>
{
    options.DefaultRequestCulture = new RequestCulture("en");
    options.SupportedUICultures = Constants.supportedUICultures.Select(culture => new CultureInfo(culture.Value)).ToList();
});

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
