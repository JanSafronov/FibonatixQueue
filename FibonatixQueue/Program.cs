using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using System.Threading.Tasks.Sources;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.ApplicationInsights.AspNetCore.Logging;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Builder.Extensions;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;

namespace FibonatixQueue
{
    public class Program
    {
        public static void Main(string[] args)
        {
            IHost host = CreateHostBuilder(args)
                .ConfigureAppConfiguration((hostContext, builder) =>
                {
                    Layout.Main(hostContext.Configuration);
                    if (hostContext.HostingEnvironment.IsDevelopment())
                        builder.AddUserSecrets<Program>();

                    builder.AddJsonFile("appsettings." + hostContext.HostingEnvironment.EnvironmentName + ".json");
                }).Build();

            Console.WriteLine("Visit https://localhost:5001/swagger for interactive UI with the microservice.\n");

            host.Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });

        public static void WaitFor()
        {
            while(true)
            {
                string input = Console.ReadLine();
                if (input == "exit")
                    break;
            }
        }
    }
}
