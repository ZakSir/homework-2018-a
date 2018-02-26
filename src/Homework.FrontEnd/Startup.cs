using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Homework.FrontEnd
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
            services.AddMvc();
            
            Telemetry.Client.TrackTrace("added mvc good night ");
            
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Swashbuckle.AspNetCore.Swagger.Info
                {
                    Version = "v1",
                    Title = "Homework Api",
                    Description = "Homework Api for a potential Job.",
                    TermsOfService = "None",
                    Contact = new Swashbuckle.AspNetCore.Swagger.Contact { Name = "Zak Fargo", Email = "", Url = "https://www.hirezak.com" },            
                });

                // Set the comments path for the Swagger JSON and UI.
                var basePath = AppContext.BaseDirectory;
                var xmlPath = Path.Combine(basePath, "Homework.FrontEnd.xml");
                c.IncludeXmlComments(xmlPath);
            });
            
             Telemetry.Client.TrackTrace("added swagger good night ");
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

            // request logging
            app.Use(async (context, theTask) =>
            {
                try
                {
                    await theTask();
                }
                catch (Exception ex)
                {
                    Telemetry.Client.TrackException(ex);

                    ExceptionDispatchInfo.Capture(ex).Throw();
                    throw ex;
                }
            });
            
           Telemetry.Client.TrackTrace("using logs "); 

            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();
            
            Telemetry.Client.TrackTrace("Using swagger");

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.), specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Trupanion Homework");
            });
            
            Telemetry.Client.TrackTrace("using swagger ui ");

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
            
            Telemetry.Client.TrackTrace("using mvc ");


        }
    }
}
