using System;
using System.Collections.Generic;
using System.IO;

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
            var settings = new Settings();

            var hosts = new List<string>();
            var sr = new StreamReader(
                Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "hosts.txt"));

            string line;
            while ((line = sr.ReadLine()) != null) 
            {
                hosts.Add(line);
            }

            settings.Hosts = hosts.ToArray();
            return settings;
        }
    }
}