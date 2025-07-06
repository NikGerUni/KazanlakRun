
using Microsoft.EntityFrameworkCore;

namespace KazanlakRun.Web
{
    
    using KazanlakRun.Web.Areas.Admin.Services;
    using KazanlakRun.Web.Areas.Admin.Services.IServices;
    using KazanlakRun.Web.Areas.User.Services;
    
    
    using Microsoft.AspNetCore.Identity;
    using Microsoft.EntityFrameworkCore;
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(connectionString));
            builder.Services.AddDatabaseDeveloperPageExceptionFilter();

            builder.Services.AddScoped<IVolunteerService, VolunteerService>();
            builder.Services.AddScoped<IDistanceService, DistanceService>();
            builder.Services.AddScoped<IAidStationService, AidStationService>();
            builder.Services.AddScoped<IVolunteerServiceAdmin, VolunteerServiceAdmin>();

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
                    // ѕринудителна валидаци€ на вс€ка за€вка
                    return Task.CompletedTask;
                };
            });

            builder.Services.AddControllers();            // [ApiController]
            builder.Services.AddControllersWithViews();   // MVC + Views + Areas
            builder.Services.AddRazorPages();             // Razor Pages (Identity UI)
            builder.Services.AddAuthorization();

            var app = builder.Build();

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseExceptionHandler("/Error/500");
            app.UseStatusCodePagesWithReExecute("/Error/{0}");

            // 1. API (attribute routes)
            app.MapControllers();

            // 2. Identity UI
            app.MapRazorPages();

            // 3. MVC Areas + conventional routes
            app.MapControllerRoute(
                name: "areas",
                pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
        }
}
