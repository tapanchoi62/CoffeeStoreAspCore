using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.EntityFrameworkCore;
using CoffeeStoreAspCore.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Web.Http.Cors;
using CoffeeStoreAspCore.Data.EF;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using CoffeeStoreAspCore.Service;
using FluentValidation.AspNetCore;
using CoffeeStoreAspCore.ViewModels.System;
using CoffeeStoreAspCore.Data.Entities;

namespace CoffeeStoreAspCore
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
           services.AddDistributedMemoryCache();
            services.AddAuthentication("esvlogin")
                         .AddCookie("esvlogin", act =>
                        {

                            act.LoginPath = "/Login/Index/";
                            act.AccessDeniedPath = "/DatHang/";
                            act.SlidingExpiration = true;
                          
                        });
            services.Configure<CookiePolicyOptions>(options =>
            {

                // This lambda determines whether user consent for non-essential cookies is needed for a given request.  
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });
            services.AddControllersWithViews()
                  .AddFluentValidation(fv => fv.RegisterValidatorsFromAssemblyContaining<LoginRequestValidator>());
         services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromSeconds(10);
       
            });
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            services.AddTransient<IUserApi, UserApi>();
            services.AddHttpClient();
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(
                    Configuration.GetConnectionString("DefaultConnection")));
            services.AddDbContext<StoreDBContext>(opt => opt.UseSqlServer(Configuration["ConnectionStrings:DefaultConnection"]));
            //services.AddIdentity<User, Role>()
            //.AddEntityFrameworkStores<StoreDBContext>()
            //.AddDefaultTokenProviders();

            ////services.AddDefaultIdentity<IdentityUser>()
            ////    .AddEntityFrameworkStores<ApplicationDbContext>();
            //services.AddControllersWithViews();
            //services.AddRazorPages();
            //services.AddMvc();
            IMvcBuilder builder = services.AddRazorPages();
            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

#if DEBUG
            if (environment == Environments.Development)
            {
                builder.AddRazorRuntimeCompilation();
            }
#endif




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
            app.UseSession();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
                endpoints.MapRazorPages();
            });
        }
    }
}
