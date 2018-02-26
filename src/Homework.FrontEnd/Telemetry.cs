using System;
namespace Homework.FrontEnd
{
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using Microsoft.ApplicationInsights;
    using Microsoft.ApplicationInsights.DataContracts;

    public static class Telemetry
    {
        private const string NOTAG = "<NOTAG>";

        /// <summary>
        /// The telemetry client lazy initializer.
        /// </summary>
        private static Lazy<TelemetryClient> telemetryClient;

        /// <summary>
        /// Initializes the <see cref="T:Homework.FrontEnd.Telemetry"/> class.
        /// </summary>
        static Telemetry()
        {
            telemetryClient = new Lazy<TelemetryClient>(() =>
            {
                return new TelemetryClient();
            });
        }

        /// <summary>
        /// Gets the Telemetry Client
        /// </summary>
        /// <value>The client.</value>
        private static TelemetryClient Client => telemetryClient.Value;

        public static void Verbose(
            string message,
            string tag,
            Dictionary<string, string> properties = null,
            [CallerMemberName]string callerMemberName = "")
        {
            Log(SeverityLevel.Verbose, message, tag, properties, callerMemberName);
        }

        public static void Info(
            string message,
            string tag,
            Dictionary<string, string> properties = null,
            [CallerMemberName]string callerMemberName = "")
        {
            Log(SeverityLevel.Information, message, tag, properties, callerMemberName);
        }

        public static void Warn(
            string message,
            string tag,
            Dictionary<string, string> properties = null,
            [CallerMemberName]string callerMemberName = "")
        {
            Log(SeverityLevel.Warning, message, tag, properties, callerMemberName);
        }

        public static void Error(
            string message,
            string tag,
            Exception ex,
            Dictionary<string, string> properties = null,
            [CallerMemberName]string callerMemberName = "")
        {
            Log(SeverityLevel.Error, message, tag, ex, properties, callerMemberName);
        }

        public static void Critical(
            string message,
            string tag,
            Exception ex,
            Dictionary<string, string> properties = null,
            [CallerMemberName]string callerMemberName = "")
        {
            Log(SeverityLevel.Critical, message, tag, ex, properties, callerMemberName);
        }

        private static void Log(SeverityLevel severity, string message, string tag, Dictionary<string, string> properties, string callerMemberName)
        {
            if(string.IsNullOrWhiteSpace(tag))
            {
                tag = NOTAG;  
            }

            TraceTelemetry tt = new TraceTelemetry()
            {
                Message = message,
                SeverityLevel = severity
            };

            if (properties != null && properties.Count > 0)
            {
                foreach (KeyValuePair<string, string> pair in properties)
                {
                    tt.Properties.Add(pair.Key, pair.Value);
                }
            }

            tt.Properties.Add(nameof(tag), tag);
            tt.Properties.Add(nameof(callerMemberName), callerMemberName);

            Client.TrackTrace(tt);
        }

        private static void Log(SeverityLevel severity, string message, string tag, Exception ex, Dictionary<string, string> properties, string callerMemberName)
        {
            if (string.IsNullOrWhiteSpace(tag))
            {
                tag = NOTAG;
            }
            ExceptionTelemetry et = new ExceptionTelemetry(ex){
                Message = message,
                SeverityLevel = severity
            };

            if (properties != null && properties.Count > 0)
            {
                foreach (KeyValuePair<string, string> pair in properties)
                {
                    et.Properties.Add(pair.Key, pair.Value);
                }
            }

            et.Properties.Add(nameof(tag), tag);
            et.Properties.Add(nameof(callerMemberName), callerMemberName);

            Client.TrackException(et);
        }
    }
}