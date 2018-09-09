using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AcosTodo.Web;
using AcosTodo.Web.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace AcosTodo.Web
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
            services.Configure<UserTokenServiceOptions>(Configuration.GetSection("UserToken"));
            services.AddTransient<UserService>();
            services.AddTransient<TodoService>();
            services.AddTransient<UserTokenService>();
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            services.AddDbContext<TodoDbContext>(options => options.UseSqlServer(Configuration["ConnectionString"]));

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options => {
                    var config = services.BuildServiceProvider().GetService<IOptions<UserTokenServiceOptions>>().Value;
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        IssuerSigningKey = config.SecurityKey,
                        RequireSignedTokens = true,
                        ValidAudience = config.Audience,
                        ValidIssuer = config.Issuer,
                    };
                });

            services.AddAuthorization(options =>
            {
                options.AddPolicy("admin", policy => policy.RequireClaim("admin"));
            });
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
                app.UseHsts();
            }

            app.UseHttpsRedirection();

            app.UseAuthentication();
            app.UseStaticFiles();
            app.UseMvc();
            app.UseSpa(spa => { });
        }
    }
}
