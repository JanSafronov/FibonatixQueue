using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using System.IO.IsolatedStorage;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Design;
using System.Drawing.Configuration;
using Microsoft.ApplicationInsights.AspNetCore.Logging;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Builder.Extensions;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace FibonatixQueue
{
    sealed class Layout : LayoutView
    {

        private static readonly string[] _services = new string[] { "Redis", "MongoDB", "Azure" };

        private static readonly string[] _ciphers = new string[] { "AES", "DES", "RC2", "TripleDES" };

        public static void Main(IConfiguration configuration) {
            Console.WriteLine("Please enter the NoSQL service to use, enter 'list' to view the available services.");
            string input = "";

            while(true)
            {
                input = Task.Run(() => Console.ReadLine()).Result;
                if (input == "list")
                    Array.ForEach<string>(_services, s => Console.Write(s + "; "));
                if (_services.Contains(input)) {
                    configuration["Service"] = input;
                    break;
                }
                else
                    Console.WriteLine("\nPlease enter the NoSQL service to use, enter 'list' to view the available services.");

            }

            Console.WriteLine("Enter the connection string to the {0} database:", input);
            input = Task.Run(() => Console.ReadLine()).Result;

            configuration["ConnectionString"] = input;

            Console.WriteLine("Enter the password for the database:");
            input = Task.Run(() => Console.ReadLine()).Result;

            configuration["Password"] = input;

            Console.WriteLine("Symmetrically encrypt pushed and decrypt pulled queues value?");
            input = Task.Run(() => Console.ReadLine()).Result;

            configuration["Transform"] = input;

            if (bool.Parse(configuration["Transform"])) {
                Console.WriteLine("What algorithm to use for the cipher? Leave empty for AES algorithm by default or enter 'list' to view some available algorithms.");

                while (true) {
                    input = Task.Run(() => Console.ReadLine()).Result;
                    if (input == "list")
                        Array.ForEach<string>(_ciphers, s => Console.Write(s + "; "));
                    Console.WriteLine();

                    if (input == "" || _ciphers.Contains(input))
                        break;
                }

                if (input == "")
                    input = "AES";

                configuration["Algorithm"] = input;
            }
        }
    }
}
