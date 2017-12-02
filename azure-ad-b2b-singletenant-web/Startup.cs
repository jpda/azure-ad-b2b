using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using azure_ad_b2b_services;
using azure_ad_b2b_services.AppTenantRepo;
using azure_ad_b2b_entities;
using Microsoft.Extensions.Options;

namespace azure_ad_b2b_singletenant_web
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
            services.AddAuthentication(sharedOptions =>
            {
                sharedOptions.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                sharedOptions.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
            })
            .AddAzureAd(options => Configuration.Bind("AzureAd", options))
            .AddCookie();

            services.AddOptions();
            services.Configure<StorageOptions>(x => Configuration.Bind("Storage", x));
            services.Configure<GraphConfiguration>(x => Configuration.Bind("Graph", x));

            services.AddTransient<IGraphService, GraphService>();
            services.AddTransient<IUserTableContext, UserTableContext>(x => new UserTableContext("UseDevelopmentStorage=true;"));
            services.AddTransient<ITenantTableContext, TenantTableContext>(x => new TenantTableContext("UseDevelopmentStorage=true;"));
            services.AddTransient<IAuthTableContext, AuthTableContext>(x => new AuthTableContext("UseDevelopmentStorage=true;"));
            services.AddTransient<IAppRepository, AppRepository>();
            services.AddTransient<IAppService, AppService>();

            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            app.UseAuthentication();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}