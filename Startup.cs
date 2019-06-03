using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DonorTestWithIndividual.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DonorTestWithIndividual
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

            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(
                    Configuration.GetConnectionString("DefaultConnection")));
            services.AddIdentity<RegisteredPerson,IdentityRole>()
                //.AddDefaultUI(UIFramework.Bootstrap4)
                .AddEntityFrameworkStores<ApplicationDbContext>();

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, UserManager<RegisteredPerson> userManager, RoleManager<IdentityRole> roleManager,ApplicationDbContext context)
        {
            context.Database.Migrate();


            var user = userManager.FindByEmailAsync("anna.grig@gmail.com").Result;

            if (user == null)
                {
                user = new RegisteredPerson
                    {
                    FirstName = "Anna",
                    LastName = "Grigorian",
                    UserName = "anna.grig@gmail.com",
                    Email = "anna.grig@gmail.com",
                    EmailConfirmed = true,
                    Role = Roles.Admin
                    };

                var result = userManager.CreateAsync(user, "Mic1234!").Result;

                if (result.Succeeded)
                    {
                    var roleResult = roleManager.CreateAsync(new IdentityRole { Name = "Admin" }).Result;

                    if (roleResult.Succeeded)
                        {
                        userManager.AddToRoleAsync(user, "Admin");
                        }
                    else
                        {
                        throw new Exception();
                        }
                    }
                else
                    {
                    throw new Exception();
                    }
                }

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
            app.UseCookiePolicy();

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
