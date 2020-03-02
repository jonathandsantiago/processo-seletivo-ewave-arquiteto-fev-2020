using Microsoft.Extensions.Configuration;
using System;

namespace FavoDeMel.Tests
{
    public class Startup
    {
        private static IConfigurationRoot _configuration;

        public static IConfigurationRoot Configuration
        {
            get
            {
                if (_configuration == null)
                {
                    _configuration = new Startup().GetConfigurationRoot();
                }

                return _configuration;
            }
        }

        public IConfigurationRoot GetConfigurationRoot()
        {
            string environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            string appsettings = string.IsNullOrEmpty(environment) ?
                $"appsettings.json" :
                $"appsettings.{environment}.json";

            return new ConfigurationBuilder()
                .AddJsonFile(appsettings)
                .Build();
        }
    }
}
