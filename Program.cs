using System;
using System.Collections.Generic;

namespace Pingular
{
    class Program
    {
        static void Main(string[] args)
        {
            var config = Configuration.Settings.GetSettings();
   
            Console.Clear();
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
                Console.SetCursorPosition(0, 0);

                foreach (var host in pingers.Keys)
                {
                    var pingerInfo = pingers[host];
                    var lastPingEventArgs = pingerInfo.LastPingEventArgs;
                    var lastPingTime = pingerInfo.LastPingTime;
                    if (lastPingEventArgs == null)
                    {
                        ConsoleExtensions.WriteFullLine($"{host,-30} waiting...", ConsoleColor.Gray);
                    }
                    else if (lastPingEventArgs.Success)
                    {
                        ConsoleExtensions.WriteFullLine($"{host,-30} [{lastPingTime.TimeOfDay}] {lastPingEventArgs.Duration} ms", ConsoleColor.Green);
                    }
                    else
                    {
                        ConsoleExtensions.WriteFullLine($"{host,-30} [{lastPingTime.TimeOfDay}] {lastPingEventArgs.Message}", ConsoleColor.Red);
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

    public static class ConsoleExtensions
    {
        public static void WriteFullLine(string s, ConsoleColor c)
        {
            using (new ColorScope(c))
            {
                var sb = new System.Text.StringBuilder(s, Console.BufferWidth);
                sb.Append(' ', Console.BufferWidth - s.Length - 1);
                Console.WriteLine(sb);
            }
        }

        private class ColorScope : IDisposable
        {
            public ColorScope(ConsoleColor color)
            {
                previousColor = Console.ForegroundColor;
                Console.ForegroundColor = color;
            }

            void IDisposable.Dispose()
            {
                Dispose(true);
            }

            ~ColorScope()
            {
                Dispose(false);
            }

            private void Dispose(bool disposing)
            {
                Console.ForegroundColor = previousColor;

                if (disposing)
                {
                    System.GC.SuppressFinalize(this);
                }
            }

            private ConsoleColor previousColor;
        }        
    }
}
