using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using MAINTENANCE_MOLD.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace MAINTENANCE_MOLD
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
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });


           

            services.AddMvc().AddControllersAsServices(); //call other controller
            services.AddDbContext<IT>(options => options.UseSqlServer(Configuration.GetConnectionString("IT")));
            services.AddDbContext<HRMS>(options => options.UseSqlServer(Configuration.GetConnectionString("HRMS")));
            services.AddDbContext<ThsReport>(options => options.UseSqlServer(Configuration.GetConnectionString("ThsReport")));
            services.AddDbContext<MOLD>(options => options.UseSqlServer(Configuration.GetConnectionString("MOLD")));

            // Atherize
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
              .AddCookie(x =>
              {
                  x.ExpireTimeSpan = TimeSpan.FromDays(7);
                  x.LoginPath = "/Home/Login"; //path login
                  x.LogoutPath = "/Home/Logout"; //path loout
                  x.AccessDeniedPath = "/Home/Login"; //logined but no auth
                  x.ReturnUrlParameter = "/Home/Login";
                  x.Cookie.IsEssential = true;
              });

            services.AddAuthorization(x =>
            {
                x.AddPolicy("Checked", y => { y.RequireClaim(ClaimTypes.Country, "MAINTENANCE_MOLD"); });
                x.AddPolicy("Authorize", y => { y.RequireClaim(ClaimTypes.Role, "admin"); });
            });

            services.AddMemoryCache();
            services.AddSession(x => x.Cookie.IsEssential = true);
            services.AddDistributedMemoryCache();
            services.AddHttpContextAccessor();
            services.TryAddSingleton<IActionContextAccessor, ActionContextAccessor>();
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            app.UseAuthentication();
            app.UseSession();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Login}/{id?}");
            });

            app.UseCookiePolicy();  //add 
        }

    }
}
