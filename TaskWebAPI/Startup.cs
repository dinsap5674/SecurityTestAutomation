using BlobStorage.BlobService;
using BlobStorage.BlobService.Interfaces;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using System;
using TaskWebAPI.QueueService;
using TaskWebAPI.QueueService.Interfaces;

namespace TaskWebAPI
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
            services.AddControllers();

            services.AddTransient<IMessageQueueService, MessageQueueService>();

            //To register the Blob service Client 
            services.AddSingleton<IBlobService, BlobService>();

            // Register the Swagger generator, defining 1 or more Swagger documents
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Task API EndPoints",
                    Version = "v1",
                    Description = "Task API is under development",
                    Contact = new OpenApiContact
                    {
                        Name = "Dinesh Sapkota",
                        Email = string.Empty,
                        Url = new Uri("https://github.com/dinesh2043"),
                    }
                });
            });

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.),
            // specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                //set the Swagger endpoint to a relative path using the ./ prefix. For example, ./swagger/v1/swagger.json
                c.SwaggerEndpoint("../swagger/v1/swagger.json", "Task API V1");
                //To serve the Swagger UI at the app's root (http://localhost:<port>/), set the RoutePrefix property to an empty string:
                c.RoutePrefix = string.Empty;
            });

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
