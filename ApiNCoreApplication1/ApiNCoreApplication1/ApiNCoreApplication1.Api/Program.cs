using System;
using System.IO;
using ApiNCoreApplication1.Api.Settings;
using Loggly;
using Loggly.Config;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;

/// <summary>
/// Designed by AnaSoft Inc. 2018
/// http://www.anasoft.net/apincore 
/// 
/// The template version VSIX with test projects included you can get here: http://www.anasoft.net/apincore with added features:
/// -XUnit integration tests project added to the solution (just update the connection string and run tests)
/// -API tests as json file for import to the Postman
/// Another VSIX control can be downloaded to create API .NET Core solution with Dapper ORM implemented instead of Entity Framework and for migration
/// FluentMigrator.Runner is added to created solution.
/// 
/// NOTE:
/// Must update database connection in appsettings.json - "ApiNCoreApplication1.ApiDB"
/// </summary>

namespace ApiNCoreApplication1.Api
{
    public class Program
    {

        private static string _environmentName;

        public static void Main(string[] args)
        {
            var webHost = BuildWebHost(args);

            //read configuration
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                        .AddJsonFile("appsettings.json")
                        .AddJsonFile($"appsettings.{_environmentName}.json", optional: true, reloadOnChange: true)
                        .Build();

            //Must have Loggly account and setup correct info in appsettings
            if (configuration["Serilog:UseLoggly"] == "true")
            {
                var logglySettings = new LogglySettings();
                configuration.GetSection("Serilog:Loggly").Bind(logglySettings);
                SetupLogglyConfiguration(logglySettings);
            }

            Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(configuration)
            .CreateLogger();

            //Start webHost
            try
            {
                Log.Information("Starting web host");
                webHost.Run();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Host terminated unexpectedly");
            }
            finally
            {
                Log.CloseAndFlush();
            }

        }


        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .ConfigureLogging((hostingContext, config) =>
                {
                    config.ClearProviders();  //Disabling default integrated logger
                    _environmentName = hostingContext.HostingEnvironment.EnvironmentName;
                })
                .UseStartup<Startup>()
                .UseSerilog() // <-- Add this line
                .Build();


        /// <summary>
        /// Configure Loggly provider
        /// </summary>
        /// <param name="logglySettings"></param>
        private static void SetupLogglyConfiguration(LogglySettings logglySettings)
        {
            //Configure Loggly
            var config = LogglyConfig.Instance;
            config.CustomerToken = logglySettings.CustomerToken;
            config.ApplicationName = logglySettings.ApplicationName;
            config.Transport = new TransportConfiguration()
            {
                EndpointHostname = logglySettings.EndpointHostname,
                EndpointPort = logglySettings.EndpointPort,
                LogTransport = logglySettings.LogTransport
            };
            config.ThrowExceptions = logglySettings.ThrowExceptions;

            //Define Tags sent to Loggly
            config.TagConfig.Tags.AddRange(new ITag[]{
                new ApplicationNameTag {Formatter = "Application-{0}"},
                new HostnameTag { Formatter = "Host-{0}" }
            });
        }


    }
}


//Configuration in code for serilog in Main
//no appsetting option - in code settings not in configuration file
//Log.Logger = new LoggerConfiguration()
//    .MinimumLevel.Debug()
//    .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
//    .Enrich.FromLogContext()
//    .WriteTo.Console()
//    .WriteTo.File(
//        @"iCodify-API.txt",
//        fileSizeLimitBytes: 1_000_000,
//        rollOnFileSizeLimit: true,
//        shared: true,
//        flushToDiskInterval: TimeSpan.FromSeconds(1))
//    .CreateLogger();