using System;
namespace Homework.FrontEnd
{
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using Microsoft.ApplicationInsights;
    using Microsoft.ApplicationInsights.DataContracts;

    /// <summary>
    /// App wide telemetry interface.
    /// </summary>
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

        /// <summary>
        /// Logs an Verbose to the Stream.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="tag">A unique tag. Try <c> $x = "`"$([Guid]::NewGuid().ToString())`""; $x | write-host; $x | set-clipboard</c> in powershell.</param>
        /// <param name="properties">Additional Properties to track with the telemetry</param>
        /// <param name="callerMemberName">The name of the caller who called the public method.</param>
        public static void Verbose(
            string message,
            string tag,
            Dictionary<string, string> properties = null,
            [CallerMemberName]string callerMemberName = "")
        {
            Log(SeverityLevel.Verbose, message, tag, properties, callerMemberName);
        }

        /// <summary>
        /// Logs an Informational Event to the Stream.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="tag">A unique tag. Try <c> $x = "`"$([Guid]::NewGuid().ToString())`""; $x | write-host; $x | set-clipboard</c> in powershell.</param>
        /// <param name="properties">Additional Properties to track with the telemetry</param>
        /// <param name="callerMemberName">The name of the caller who called the public method.</param>
        public static void Info(
            string message,
            string tag,
            Dictionary<string, string> properties = null,
            [CallerMemberName]string callerMemberName = "")
        {
            Log(SeverityLevel.Information, message, tag, properties, callerMemberName);
        }

        /// <summary>
        /// Logs an Warning to the Stream.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="tag">A unique tag. Try <c> $x = "`"$([Guid]::NewGuid().ToString())`""; $x | write-host; $x | set-clipboard</c> in powershell.</param>
        /// <param name="properties">Additional Properties to track with the telemetry</param>
        /// <param name="callerMemberName">The name of the caller who called the public method.</param>
        public static void Warn(
            string message,
            string tag,
            Dictionary<string, string> properties = null,
            [CallerMemberName]string callerMemberName = "")
        {
            Log(SeverityLevel.Warning, message, tag, properties, callerMemberName);
        }

        /// <summary>
        /// Logs an Error to the Stream. A non-critical error can be recovered from or may be transient.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="tag">A unique tag. Try <c> $x = "`"$([Guid]::NewGuid().ToString())`""; $x | write-host; $x | set-clipboard</c> in powershell.</param>
        /// <param name="ex">The exception that occurred. </param>
        /// <param name="properties">Additional Properties to track with the telemetry</param>
        /// <param name="callerMemberName">The name of the caller who called the public method.</param>
        public static void Error(
            string message,
            string tag,
            Exception ex,
            Dictionary<string, string> properties = null,
            [CallerMemberName]string callerMemberName = "")
        {
            Log(SeverityLevel.Error, message, tag, ex, properties, callerMemberName);
        }

        /// <summary>
        /// Logs a critical Error to the Stream. A critical error cannot be recovered from and will stop your service. 
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="tag">A unique tag. Try <c> $x = "`"$([Guid]::NewGuid().ToString())`""; $x | write-host; $x | set-clipboard</c> in powershell.</param>
        /// <param name="ex">The exception that occurred. </param>
        /// <param name="properties">Additional Properties to track with the telemetry</param>
        /// <param name="callerMemberName">The name of the caller who called the public method.</param>
        public static void Critical(
            string message,
            string tag,
            Exception ex,
            Dictionary<string, string> properties = null,
            [CallerMemberName]string callerMemberName = "")
        {
            Log(SeverityLevel.Critical, message, tag, ex, properties, callerMemberName);
        }

        /// <summary>
        /// Log a mesage internally using trace telemetry
        /// </summary>
        /// <param name="severity">The severity of the message.</param>
        /// <param name="message">The message.</param>
        /// <param name="tag">A unique tag. Try <c> $x = "`"$([Guid]::NewGuid().ToString())`""; $x | write-host; $x | set-clipboard</c> in powershell.</param>
        /// <param name="properties">Additional Properties to track with the telemetry</param>
        /// <param name="callerMemberName">The name of the caller who called the public method.</param>
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

        /// <summary>
        /// Log a message internally using trace telemetry
        /// </summary>
        /// <param name="severity">The severity of the message.</param>
        /// <param name="message">The message.</param>
        /// <param name="tag">A unique tag. Try <c> $x = "`"$([Guid]::NewGuid().ToString())`""; $x | write-host; $x | set-clipboard</c> in powershell.</param>
        /// <param name="ex">The exception that occurred. </param>
        /// <param name="properties">Additional Properties to track with the telemetry</param>
        /// <param name="callerMemberName">The name of the caller who called the public method.</param>
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