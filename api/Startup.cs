using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerUI;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net;
using System.IO;
using chktr.ApiKeyAuthentication;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Authorization;

namespace chktr
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddAuthentication(o =>
                  {
                      o.DefaultAuthenticateScheme = ApiKeyAuthenticationOptions.DefaultScheme;
                      o.DefaultChallengeScheme = ApiKeyAuthenticationOptions.DefaultScheme;
                  })
                  .AddApiKeyAuth(o => {});

            var dataProectionFolder = new DirectoryInfo(Configuration.GetValue<string>("dataProtection:folder"));
            services.AddDataProtection()
                .PersistKeysToFileSystem(dataProectionFolder);

            services.AddDistributedRedisCache(options =>
            {
                options.InstanceName = Configuration.GetValue<string>("redis:name");
                options.Configuration = Configuration.GetValue<string>("redis:hostname");
            });

            services.AddMvc(o =>
            {
                o.Filters.Add(new AuthorizeFilter(new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build()));
            });

            services.AddSwaggerGen(c =>
            {
                c.IgnoreObsoleteActions();
                c.IgnoreObsoleteProperties();
                c.AddSecurityDefinition("apiKey", new ApiKeyScheme
                {
                    Type = "apiKey",
                    In = "header",
                    Name = "Authorization",
                    Description = "Just write 'A-KEY-GNERATED-BY-A-KEY-MANAGEMENT-SYSTEM' in the Value field."
                });
                c.AddSecurityRequirement(new Dictionary<string, IEnumerable<string>>
                {
              { "apiKey", new string[] { } }
                });

                c.SwaggerDoc("v1", new Info
                {
                    Version = "v1",
                    Title = "Checkout Cart API",
                    Description = "A simple Cart API",
                    TermsOfService = "None",
                    Contact = new Contact
                    {
                        Name = "Fernando Callejon",
                        Email = "fcallejon@gmail.com",
                        Url = "https://www.github.com/fcallejon"
                    }
                });
            });
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseExceptionHandler();

            app.UseAuthentication();

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.DocumentTitle = "Checkout Cart API";
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Checkout Cart API v1");
                c.RoutePrefix = string.Empty;
            });

            app.UseMvc();
        }
    }
}
