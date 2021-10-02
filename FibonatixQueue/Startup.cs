using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Builder.Extensions;
using Microsoft.AspNetCore.Hosting.StaticWebAssets;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi;
using Microsoft.OpenApi.Models;
using Microsoft.Extensions.Azure;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Azure.Storage.Queues;
using Azure.Storage.Blobs;
using Azure.Core.Extensions;
using StackExchange.Redis;
using FibonatixQueue.Services;
using FibonatixQueue.Settings;
using Newtonsoft;
using Newtonsoft.Json;
using Newtonsoft.Json.Bson;
using Newtonsoft.Json.Converters;
[assembly: FunctionsStartup(typeof(FibonatixQueue.Startup))]

namespace FibonatixQueue
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
            Console.WriteLine("Enter the connection string to the Redis database:");
            string connectionString = Console.ReadLine();
            Console.WriteLine("Enter the password for the database:");
            string password = Console.ReadLine();
            Console.WriteLine("Symmetrically encrypt pushed and decrypt pulled queues value?");
            bool transform = bool.Parse(Console.ReadLine());
            if (transform)
            {
                Console.WriteLine("What algorithm to use for encryption? Leave empty for AES algorithm by default.");

                string algorithm = Console.ReadLine();

                if (algorithm == "")
                    algorithm = "AES";

                services.Configure<SecureDBSettings>(db => { db.connectionString = connectionString; db.password = password; db.algorithm = algorithm; });
                services.AddSingleton<ISecureServiceSettings>(s => s.GetRequiredService<IOptions<SecureDBSettings>>().Value);
            }
            else
            {
                services.Configure<CommonDBSettings>(db => { db.connectionString = connectionString; db.password = password; });
                services.AddSingleton<IServiceSettings>(s => s.GetRequiredService<IOptions<CommonDBSettings>>().Value);
            }

            // Keeps the service alive and the symmetric algorithm properties
            services.AddSingleton<RedisQueueService>();

            services.AddControllers();
            services.AddAzureClients(builder =>
            {
                builder.AddBlobServiceClient(Configuration["ConnectionStrings:LocalDBTesting:blob"], preferMsi: true);
                builder.AddQueueServiceClient(Configuration["ConnectionStrings:LocalDBTesting:queue"], preferMsi: true);
            });

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "FibonatixQueue", Version = "v1" });
            });

            //var multiplexer = ConnectionMultiplexer.Connect("localhost");
            //services.AddSingleton<IConnectionMultiplexer>(multiplexer);

            //services.AddControllers().AddNewtonsoftJson(options => options.UseMemberCasing());
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "FibonatixQueue v1"));
            }

            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "FibonatixQueue v1"));

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
    internal static class StartupExtensions
    {
        public static IAzureClientBuilder<BlobServiceClient, BlobClientOptions> AddBlobServiceClient(this AzureClientFactoryBuilder builder, string serviceUriOrConnectionString, bool preferMsi)
        {
            if (preferMsi && Uri.TryCreate(serviceUriOrConnectionString, UriKind.Absolute, out Uri serviceUri))
            {
                return builder.AddBlobServiceClient(serviceUri);
            }
            else
            {
                return builder.AddBlobServiceClient(serviceUriOrConnectionString);
            }
        }
        public static IAzureClientBuilder<QueueServiceClient, QueueClientOptions> AddQueueServiceClient(this AzureClientFactoryBuilder builder, string serviceUriOrConnectionString, bool preferMsi)
        {
            if (preferMsi && Uri.TryCreate(serviceUriOrConnectionString, UriKind.Absolute, out Uri serviceUri))
            {
                return builder.AddQueueServiceClient(serviceUri);
            }
            else
            {
                return builder.AddQueueServiceClient(serviceUriOrConnectionString);
            }
        }
    }
}
