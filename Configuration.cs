using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Binder;

namespace Pingular.Configuration
{
    class Host
    {
        public string Name { get; set; }
    }

    class Settings
    {
        public string[] Hosts { get; set; }

        public static Settings GetSettings()
        {
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("config.json", optional:true, reloadOnChange:false)
                .Build();

            var settings = new Settings();
            configuration.Bind(settings);

            return settings;
        }
    }
}