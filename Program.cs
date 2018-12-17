using System;
using System.Collections.Generic;
using System.Threading;

namespace Pingular
{
    class Program
    {
        static void Main(string[] args)
        {
            var config = Configuration.Settings.GetSettings();
            
            foreach(var host in config.Hosts)
            {
                var ping = new HostPing(host, 5000);
                ping.PingEvent += UpdatePingResults;
                pingers[host] = new PingerInfo(ping, null, DateTime.MinValue);

                Refresh();
            }

            Console.ReadLine();
        }

        private static void UpdatePingResults(object sender, PingEventArgs args)
        {
            var pingerInfo = pingers[args.Host];
            pingerInfo.LastPingEventArgs = args;
            pingerInfo.LastPingTime = DateTime.Now;
            Refresh();
        }

        private static object consoleLock = new object();
        private static void Refresh()
        {
            
            lock(consoleLock)
            {
                Console.Clear();
                Console.SetCursorPosition(0, 0);

                foreach (var host in pingers.Keys)
                {
                    var pingerInfo = pingers[host];
                    var lastPingEventArgs = pingerInfo.LastPingEventArgs;
                    var lastPingTime = pingerInfo.LastPingTime;
                    if (lastPingEventArgs == null)
                    {
                        Console.WriteLine($"{host,-30} waiting..."); 
                    }
                    else if (lastPingEventArgs.Success)
                    {
                        var color = Console.ForegroundColor;
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine($"{host,-30} [{lastPingTime.TimeOfDay}] {lastPingEventArgs.Duration} ms");
                        Console.ForegroundColor = color;
                    }
                    else
                    {
                        var color = Console.ForegroundColor;
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine($"{host,-30} [{lastPingTime.TimeOfDay}] {lastPingEventArgs.Message}");
                        Console.ForegroundColor = color;
                    }
                }

                Console.Write("Press Enter to quit...");
            }
        }        

        private class PingerInfo
        {
            public PingerInfo(HostPing pinger, PingEventArgs args, DateTime pingTime)
            {
                Pinger = pinger;
                LastPingEventArgs = args;
                LastPingTime = pingTime;
            }

            public HostPing Pinger { get; }
            public PingEventArgs LastPingEventArgs;
            public DateTime LastPingTime;
        }
        static Dictionary<string, PingerInfo> pingers = new Dictionary<string, PingerInfo>();
    }
}
