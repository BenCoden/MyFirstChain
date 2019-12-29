using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace MyFirstChain
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var configuration = new ConfigurationBuilder()
          //.AddCommandLine(args)
          .AddJsonFile("appsettings.Development.json")
          .Build();
            var port = configuration.GetSection("Host").Value;
          
            var host = new WebHostBuilder()
                .UseKestrel().
                UseContentRoot(Directory.GetCurrentDirectory())
                  .UseConfiguration(configuration)
              .UseStartup<Startup>()
            .UseUrls($"https://localhost:{port}/", $"http://localhost:{port}1/")
            .Build();
            host.Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
           
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
