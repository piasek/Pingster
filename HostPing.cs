using System;
using System.Net.NetworkInformation;
using System.Timers;

namespace Pingular
{
    public class PingEventArgs
    {
        public PingEventArgs(string host, bool success, long duration, string message) 
        { 
            Host = host;
            Success = success;
            Duration = duration;
            Message = message;
        }

        public string Host { get; }
        public bool Success { get; }
        public long Duration { get; }
        public string Message { get; }
    }

    class HostPing
    {
        public HostPing(string host, int interval)
        {
            this.host = host;
            this.interval = interval;

            this.timer = new Timer(interval);
            this.timer.Elapsed += async ( sender, e ) =>  
            {
                using(var ping = new Ping())
                {
                    try
                    {
                        var reply = await ping.SendPingAsync(this.host);
                        RaisePingEvent(this.host, reply.Status == IPStatus.Success, reply.RoundtripTime, reply.Status == IPStatus.Success ? "" : reply.Status.ToString());
                    }
                    catch(Exception ex)
                    {
                        RaisePingEvent(this.host, false, 0, FormatException(ex));
                    }

                    this.timer.Start();
                }
            };
            this.timer.AutoReset = false;
            this.timer.Start();
        }

#region Events
        public delegate void PingEventHandler(object sender, PingEventArgs args);
        public event PingEventHandler PingEvent;

        protected virtual void RaisePingEvent(string host, bool success, long duration, string message)
        {        
            if (PingEvent != null)
            {
                PingEvent(this, new PingEventArgs(host, success, duration, message));
            }
        }
#endregion
        
        private static string FormatException(Exception e)
        {
            return 
                e.InnerException != null ? 
                e.Message + " >> " + FormatException(e.InnerException) : 
                e.Message;
        }

        private int interval;
        private string host;
        private Timer timer;
    }

}