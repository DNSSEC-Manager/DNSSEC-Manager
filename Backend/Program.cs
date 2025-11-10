using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Authorization;
using System.IO;
using System.Threading.Tasks;
using Backend.Business;
using Backend.Data;
using Backend.Scheduler;
using Backend.Services;
using Backend.Infrastructure;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;

namespace Backend
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            var env = builder.Environment;
            var configuration = builder.Configuration;
            
            // Configure clean console logging via MinimalConsoleFormatter
            builder.Logging.AddConsoleFormatter<MinimalConsoleFormatter, ConsoleFormatterOptions>();
            
            // Add Health Checks
            builder.Services.AddHealthChecks();

            // Create data folder
            var dataFolder = Path.Combine(env.ContentRootPath, "storage");
            Directory.CreateDirectory(dataFolder);

            // Register Services
            var services = builder.Services;

            services.AddDataProtection()
                    .PersistKeysToFileSystem(new DirectoryInfo(dataFolder));

            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlite(configuration.GetConnectionString("DefaultConnection")));

            services.AddDatabaseDeveloperPageExceptionFilter();

            services.AddDefaultIdentity<IdentityUser>()
                .AddDefaultUI()
                .AddEntityFrameworkStores<ApplicationDbContext>();

            services.AddAuthorization(options =>
            {
                options.FallbackPolicy = new AuthorizationPolicyBuilder()
                    .RequireAuthenticatedUser()
                    .Build();
            });

            services.AddControllersWithViews();
                //options => options.ModelBinderProviders.RemoveType<DateTimeModelBinderProvider>());

            services.AddRazorPages();

            // Hosted background scheduler
            services.AddHostedService<JobSchedulerService>();

            // Custom services
            services.AddScoped<IProviderDecider, ProviderDecider>();
            services.AddScoped<IUtilities, Utilities>();
            services.AddScoped<IGlobals, Globals>();
            services.AddScoped<IDnsServerService, DnsServerService>();

            var app = builder.Build();

            app.UseForwardedHeaders();
            
            // Health Checks
            app.MapHealthChecks("/health").AllowAnonymous();

            // Middleware pipeline
            app.UseDeveloperExceptionPage();
            app.UseMigrationsEndPoint();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Dashboard}/{action=Index}/{id?}");
            app.MapRazorPages();

            // Database initialization
            using (var scope = app.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                await db.Database.MigrateAsync();

                var dnsServerService = scope.ServiceProvider.GetRequiredService<IDnsServerService>();
                await dnsServerService.CreateDnsServerFromEnvironmentAsync();
            }

            await app.RunAsync();
        }
    }
}
