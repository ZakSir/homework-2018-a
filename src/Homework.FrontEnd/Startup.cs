namespace Homework.FrontEnd
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Runtime.ExceptionServices;
    using Microsoft.AspNetCore.Authentication.Cookies;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Swashbuckle.AspNetCore.Swagger;

    /// <summary>
    /// Startup class for web service. 
    /// </summary>
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            this.Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            try
            {
                services.AddMvc();
                services.AddSingleton<IConfiguration>(this.Configuration);

                Telemetry.Info("added mvc", "872992b5-0d91-4243-80e2-3cce7a16cf00");

                services.AddSwaggerGen(c =>
                {
                    c.SwaggerDoc("v1", new Swashbuckle.AspNetCore.Swagger.Info()
                    {
                        Version = "v1",
                        Title = "Homework Api",
                        Description = "Homework Api for a potential Job.",
                        TermsOfService = "None",
                        Contact = new Swashbuckle.AspNetCore.Swagger.Contact { Name = "Zak Fargo", Email = string.Empty, Url = "https://www.hirezak.com" },
                    });

                    // Set the comments path for the Swagger JSON and UI.
                    var basePath = AppContext.BaseDirectory;
                    var xmlPath = Path.Combine(basePath, "Homework.FrontEnd.xml");
                    c.IncludeXmlComments(xmlPath);

                    // api keys
                    c.AddSecurityDefinition("Bearer", new ApiKeyScheme() { In = "header", Name = "Authorization", Type = "apiKey", Description = "test" });
                });

                Telemetry.Info("added swagger", "00ad2d28-4454-470f-91a3-a78ff03007e8");
            }
            catch (Exception ex)
            {
                Telemetry.Critical("Unexpected exception in configuring services", "2ae757a7-91a9-45c8-9864-c863f96c1413", ex);

                ExceptionDispatchInfo.Capture(ex).Throw();

                throw ex; // compiler
            }
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            try
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
                    string ctx = Guid.NewGuid().ToString();

                    Telemetry.Info($"Starting @ {context.Request.Path}", "dcca2297-6696-4a80-a99d-472b991bf2d8", new Dictionary<string, string>() { [nameof(ctx)] = ctx });

                    try
                    {
                        await theTask();
                    }
                    catch (Exception ex)
                    {
                        Telemetry.Error("Unexpected error on request", "b12f1693-e157-463d-a4cd-2b41d8117f3e", ex);

                        ExceptionDispatchInfo.Capture(ex).Throw();
                        throw ex;
                    }
                    finally
                    {
                        Telemetry.Info($"Starting @ {context.Request.Path}", "dcca2297-6696-4a80-a99d-472b991bf2d8", new Dictionary<string, string>() { [nameof(ctx)] = ctx });

                    }

                });

                Telemetry.Info("using logs ", "7cc426d8-0554-4610-9ec3-2449522e46e9");

                // Enable middleware to serve generated Swagger as a JSON endpoint.
                app.UseSwagger();

                Telemetry.Info("Using swagger", "0fc804b5-8cf0-4118-9b61-4ce87f2cbafb");

                // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.), specifying the Swagger JSON endpoint.
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Trupanion Homework");
                });

                Telemetry.Info("using swagger ui ", "49db7b25-23f6-4565-acaf-4939a31f55a0");

                app.UseMvc(routes =>
                {
                    routes.MapRoute(
                        name: "default",
                        template: "{controller=Home}/{action=Index}/{id?}");
                });

                Telemetry.Info("using mvc ", "d7faf997-ba03-4d1f-b3d1-9ebb0ff10b50");
            }
            catch (Exception ex)
            {
                Telemetry.Critical("An unexpected exception has occurred in the configuration of the app", "bfa680e3-1291-4a51-8869-2f8866d66114", ex);

                ExceptionDispatchInfo.Capture(ex).Throw();

                throw ex; // compiler
            }
        }
    }
}
