using System;
using System.Globalization;
using System.Linq;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.EntityFrameworkCore;
using Voluntary.App.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using LazZiya.ExpressLocalization;
using Voluntary.App.LocalizationResources;

namespace Voluntary.App
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            
            var cultures = new[]
            {
                new CultureInfo("fr"),
                new CultureInfo("en"),
                new CultureInfo("ar")
                
            };

            services.AddRazorPages()
                .AddExpressLocalization<LocSource>(
                    ops =>
                    {
                        ops.ResourcesPath = "LocalizationResources";
                        ops.RequestLocalizationOptions = o =>
                        {
                            o.SupportedCultures = cultures;
                            o.SupportedUICultures = cultures;
                            o.DefaultRequestCulture = new RequestCulture("fr");
                        };
                    });
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(
                    Configuration.GetConnectionString("DefaultConnection")));
            services.AddIdentity<IdentityUser, IdentityRole>(options => options.SignIn.RequireConfirmedAccount = true)
                .AddEntityFrameworkStores<ApplicationDbContext>();
            services.ConfigureApplicationCookie(options =>
            {
                //options.AccessDeniedPath = "/Account/AccessDenied";
                options.Cookie.Name = "VApp";
                options.Cookie.HttpOnly = true;
                options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
                options.LoginPath = "/fr/Account/Login";

                options.ReturnUrlParameter = CookieAuthenticationDefaults.ReturnUrlParameter;
                options.SlidingExpiration = true;
            });

            services.Configure<IdentityOptions>(options =>
            {
                // Password settings.
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequireUppercase = false;
                options.Password.RequiredLength = 6;
                options.Password.RequiredUniqueChars = 1;

                // Lockout settings.
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.AllowedForNewUsers = true;

                // User settings.
                options.User.AllowedUserNameCharacters =
                    "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
                options.User.RequireUniqueEmail = false;
            });
         //   services.AddControllersWithViews();
            services.AddRazorPages();
            services
                .AddMvc()
                .AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix);
            //.AddDataAnnotationsLocalization();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
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

          
            app.UseRequestLocalization();
           // app.UseMvc();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{culture=fr}/{controller=Home}/{action=Index}/{id?}");
            });
            SeedInitialData(app);
        }
        private static void SeedInitialData(IApplicationBuilder app)
        {
            using (var scope = app.ApplicationServices.CreateScope())
            {
                #region seed identity data

                var identityContext = scope.ServiceProvider.GetService<ApplicationDbContext>();
                var identitySeed = new SeedDummyData(identityContext);

                if (!identityContext.Roles.Any())
                {
                    identitySeed.CreateRolesAsync(scope.ServiceProvider).GetAwaiter().GetResult();
                }
                identitySeed.CreateUsersAsync(scope.ServiceProvider).GetAwaiter().GetResult();
                identitySeed.CreateDistrictsAsync(scope.ServiceProvider).GetAwaiter().GetResult();


                #endregion
            }
        }
    }
}
