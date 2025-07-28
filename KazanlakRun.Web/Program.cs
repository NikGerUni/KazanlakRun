using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using KazanlakRun.Data;
using KazanlakRun.GCommon;
using KazanlakRun.Web.Areas.Admin.Services;
using KazanlakRun.Web.Areas.Admin.Services.IServices;
using KazanlakRun.Web.Areas.User.Services;
using KazanlakRun.Web.Filters;
using KazanlakRun.Web.MappingProfiles;
using KazanlakRun.Web.Services;
using KazanlakRun.Web.Services.IServices;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace KazanlakRun.Web;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        var port = Environment.GetEnvironmentVariable("PORT");
        if (!string.IsNullOrEmpty(port))
        {
            builder.WebHost.UseUrls($"http://*:{port}");
        }
        var connectionString = builder.Configuration["SQL_CONNECTION_STRING"]
          ?? builder.Configuration.GetConnectionString("DefaultConnection");
        builder.Services.AddDbContext<ApplicationDbContext>(options =>
          options.UseSqlServer(connectionString));
        builder.Services.AddDatabaseDeveloperPageExceptionFilter();
        builder.Services.AddDefaultIdentity<IdentityUser>(options =>
        {
            options.Password.RequireDigit = false;
            options.Password.RequireLowercase = false;
            options.Password.RequireNonAlphanumeric = false;
            options.Password.RequireUppercase = false;
            options.Password.RequiredLength = 6;
        })
          .AddRoles<IdentityRole>()
          .AddEntityFrameworkStores<ApplicationDbContext>();
        builder.Services.ConfigureApplicationCookie(options =>
        {
            options.LoginPath = "/Identity/Account/Login";
            options.LogoutPath = "/Identity/Account/Logout";
            options.AccessDeniedPath = "/Identity/Account/AccessDenied";

            options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
            options.SlidingExpiration = true;

            options.Cookie.Name = "KazanlakRunAuth";
            options.Cookie.HttpOnly = true;
            options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
            options.Cookie.SameSite = SameSiteMode.Lax;

            options.Events.OnValidatePrincipal = context =>
            {
                return Task.CompletedTask;
            };
        });
        var credential = GoogleCredential
          .FromFile("App_Data/drive-service-account.json")
          .CreateScoped(DriveService.ScopeConstants.DriveFile);
        builder.Services.AddSingleton(_ => new DriveService(new BaseClientService.Initializer
        {
            HttpClientInitializer = credential,
            ApplicationName = builder.Environment.ApplicationName
        }));
        builder.Services.AddAutoMapper(typeof(VolunteerProfile).Assembly);
        builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
        builder.Services.AddScoped<IAidStationService, AidStationService>();
        builder.Services.AddScoped<IDistanceEditDtoService, DistanceEditDtoService>();
        builder.Services.AddScoped<IGpxFileService, GpxFileService>();
        builder.Services.AddScoped<IRoleService, RoleService>();
        builder.Services.AddScoped<IReportService, ReportService>();
        builder.Services.AddScoped<IVolunteerServiceAdmin, VolunteerServiceAdmin>();
        builder.Services.AddScoped<IVolunteerService, VolunteerService>();
        builder.Services.Configure<GpxFileSettings>(
          builder.Configuration.GetSection("GpxFileSettings"));
        builder.Services.AddScoped<ReportExceptionFilter>();
        builder.Services.AddControllersWithViews(options =>
        {
            options.Filters.AddService<ReportExceptionFilter>();
        });
        builder.Services.AddRazorPages();
        builder.Services.AddAuthorization();

        var app = builder.Build();
        app.UseExceptionHandler("/Error/500");
        app.UseStatusCodePagesWithReExecute("/Error/{0}");
        app.UseHttpsRedirection();
        app.UseStaticFiles();

        app.UseRouting();
        app.UseAuthentication();
        app.UseAuthorization();
        app.MapControllerRoute(
          name: "areas",
          pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");
        app.MapControllerRoute(
          name: "default",
          pattern: "{controller=Home}/{action=Index}/{id?}");
        app.MapRazorPages();
        using (var scope = app.Services.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            context.Database.Migrate();
        }

        app.Run();
    }
}
