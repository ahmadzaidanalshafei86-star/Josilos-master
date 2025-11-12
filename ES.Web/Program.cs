using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.Extensions.Options;
using ES.Web.Areas.EsAdmin.Helpers;
using ES.Web.Areas.EsAdmin.Models;
using ES.Web.Areas.EsAdmin.Repositories;
using ES.Web.Areas.EsAdmin.Services;
using ES.Web.Filters;
using ES.Web.Seeds;
using ES.Web.Services;
using System.Globalization;
using UoN.ExpressiveAnnotations.NetCore.DependencyInjection;
using Microsoft.AspNetCore.DataProtection;
using ES.Web.Helpers;
using Microsoft.AspNetCore.Authentication;
using NuGet.Configuration;
using ES.Web.Models;
using Microsoft.AspNetCore.HttpOverrides;


var builder = WebApplication.CreateBuilder(args);

var configuredPathBase = builder.Configuration["PathBase"];
// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString!));

builder.Services.AddDatabaseDeveloperPageExceptionFilter();


builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultUI()
    .AddDefaultTokenProviders()
    .AddClaimsPrincipalFactory<CustomUserClaimsPrincipalFactory>();

builder.Services.AddDataProtection()
    .PersistKeysToFileSystem(new DirectoryInfo(Path.Combine(Directory.GetCurrentDirectory(), "KeyStorage")))
    .SetApplicationName("ESApp");

builder.Services.ConfigureApplicationCookie(options =>
{
    options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
    options.SlidingExpiration = true;
    options.Cookie.HttpOnly = true;
    options.Cookie.SameSite = SameSiteMode.Lax;
    if (!string.IsNullOrEmpty(configuredPathBase))
    {
        options.Cookie.Path = configuredPathBase;
    }
    options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
});

// Prmissions Policy { Authorization }
builder.Services.AddSingleton<IAuthorizationPolicyProvider, PermissionPolicyProvider>();
builder.Services.AddScoped<IAuthorizationHandler, PermissionAuthorizationHandler>();
builder.Services.AddScoped<IUserClaimsService, UserClaimsService>();
// Add per-request claims transformation to attach permissions at runtime (not persisted in cookie)
builder.Services.AddScoped<IClaimsTransformation, RequestPermissionClaimsTransformation>();
// }



//website services
builder.Services.AddScoped<CategoryService>();
builder.Services.AddScoped<CareerService>();
builder.Services.AddScoped<HomePageService>();
builder.Services.AddScoped<PageService>();
builder.Services.AddScoped<SearchService>();
builder.Services.AddScoped<MenuItemsService>();
builder.Services.AddScoped<FooterService>();

builder.Services.AddScoped<TopService>();

builder.Services.AddScoped<SilosDeclerationsService>();
builder.Services.AddScoped<DeclerationsService>();


builder.Services.AddScoped<CategoriesRepository>();
builder.Services.AddScoped<CareerTranslatesRepository>();
builder.Services.AddScoped<CategoriesTranslatesRepository>();
builder.Services.AddScoped<CareersRepository>();
builder.Services.AddScoped<DocumentsRepository>();
builder.Services.AddScoped<GalleryImagesRepository>();
builder.Services.AddScoped<PagesRepository>();
builder.Services.AddScoped<PageTranslatesRepository>();
builder.Services.AddScoped<PageFilesRepository>();
builder.Services.AddScoped<LanguagesRepository>();
builder.Services.AddScoped<MenuItemsRepository>();
builder.Services.AddScoped<MenuItemTranslatesRepository>();
builder.Services.AddScoped<FormRepository>();
builder.Services.AddScoped<FormTranslatesRepository>();
builder.Services.AddScoped<EcomCategoriesRepository>();
builder.Services.AddScoped<EcomCategoryTranslatesRepository>();
builder.Services.AddScoped<ProductsRepository>();



builder.Services.AddScoped<TendersRepository>();
builder.Services.AddScoped<TenderTranslatesRepository>();

builder.Services.AddScoped<MaterialsRepository>();
builder.Services.AddScoped<MaterialsTranslatesRepository>();

builder.Services.AddScoped<ProductTranslatesRepository>();
builder.Services.AddScoped<BrandsRepository>();
builder.Services.AddScoped<BrandTranslatesRepository>();
builder.Services.AddScoped<ProductAttributesRepository>();
builder.Services.AddScoped<ProductAttributesTranslatesRepository>();
builder.Services.AddScoped<OrdersRepository>();
builder.Services.AddScoped<RowPermission>();

builder.Services.AddTransient<IImageService, ImageService>();
builder.Services.AddTransient<IFilesService, FilesService>();
builder.Services.AddTransient<IDocumentsService, DocumentsService>();
builder.Services.AddTransient<SlugService>();
builder.Services.AddTransient<ILanguageService, LanguageService>();
builder.Services.Configure<ExternalServicesOptions>(builder.Configuration.GetSection("ExternalServices"));

builder.Services.Configure<ApiSettings>(builder.Configuration.GetSection("ApiSettings"));

//Add Localization Services
builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");
builder.Services.AddMvc()
    .AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix)
    .AddDataAnnotationsLocalization();

//Add Localization Options
builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    var enJoCulture = new CultureInfo("en-JO")
    {
        NumberFormat =
        {
            CurrencySymbol = "JD",
            CurrencyDecimalSeparator = ".",
            NumberDecimalSeparator = ".",
            CurrencyDecimalDigits = 2,
            CurrencyPositivePattern = 3,
            CurrencyNegativePattern = 8
        }
    };
    var arJoCulture = new CultureInfo("ar-JO")
    {
        NumberFormat =
        {
            CurrencySymbol = "JD",
            CurrencyDecimalSeparator = ".",
            NumberDecimalSeparator = ".",
            CurrencyDecimalDigits = 2,
            CurrencyPositivePattern = 3,
            CurrencyNegativePattern = 8
        }
    };
    var supportedCultures = new[] { enJoCulture, arJoCulture };
    options.DefaultRequestCulture = new RequestCulture("ar-JO");
    options.SupportedCultures = supportedCultures;
    options.SupportedUICultures = supportedCultures;

    // Add custom request culture providers
    options.RequestCultureProviders.Insert(0, new CustomCookieRequestCultureProvider());
});



builder.Services.AddControllersWithViews();
builder.Services.AddHttpContextAccessor();
builder.Services.AddSession();

builder.Services.AddExpressiveAnnotations();

builder.Services.AddAuthentication("ApiKeyScheme")
    .AddScheme<AuthenticationSchemeOptions, ApiKeyAuthenticationHandler>("ApiKeyScheme", null);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseStatusCodePagesWithReExecute("/Home/Error", "?statusCode={0}");
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseSession();

// Optionally set the base path (only when configured). When developing locally leave the app at root.
if (!string.IsNullOrEmpty(configuredPathBase))
{
    app.UsePathBase(configuredPathBase);
}

app.UseStaticFiles();
// Add cache control for all images served from the "wwwroot" folder
app.Use(async (context, next) =>
{
    var filePath = context.Request.Path.Value ?? string.Empty;

    // Handle image files with any case (capital and small letters)
    if (filePath.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase) ||
        filePath.EndsWith(".jpeg", StringComparison.OrdinalIgnoreCase) ||
        filePath.EndsWith(".png", StringComparison.OrdinalIgnoreCase) ||
        filePath.EndsWith(".gif", StringComparison.OrdinalIgnoreCase) ||
        filePath.EndsWith(".bmp", StringComparison.OrdinalIgnoreCase) ||
        filePath.EndsWith(".webp", StringComparison.OrdinalIgnoreCase) ||
        filePath.EndsWith(".svg", StringComparison.OrdinalIgnoreCase) ||
        filePath.EndsWith(".tiff", StringComparison.OrdinalIgnoreCase))
    {
        // Prevent caching of images
        context.Response.Headers["Cache-Control"] = "no-store, no-cache, must-revalidate";
        context.Response.Headers["Pragma"] = "no-cache";
        context.Response.Headers["Expires"] = "0";
        context.Response.Headers["ETag"] = Guid.NewGuid().ToString();
    }

    await next();
});



app.UseRouting();

// Move RequestLocalization here (before authentication/authorization)
var locOptions = app.Services.GetService<IOptions<RequestLocalizationOptions>>();
if (locOptions?.Value != null)
{
    app.UseRequestLocalization(locOptions.Value);
}

app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
});


app.UseAuthentication();
app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});

// Seed Data { Roles, Users, Languages, Themes }
var scopeFactory = app.Services.GetRequiredService<IServiceScopeFactory>();
using var scope = scopeFactory.CreateScope();

var RoleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
var UserManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

await DefaultRoles.SeedRoles(RoleManager);
await DefaultUsers.SeedSuperAdmin(UserManager, RoleManager);
await DefaultLanguages.SeedLanguages(context);
await DefaultThemes.SeedThemes(context);
await DefaultSocialMediaLinks.SeedSocialMediaLinks(context);

// Map routes
app.MapControllerRoute(
    name: "EsAdmin",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

app.Run();
