using System;
namespace Homework.FrontEnd
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;
    using Microsoft.ApplicationInsights;
    using Microsoft.ApplicationInsights.DataContracts;

    /// <summary>
    /// App wide telemetry interface.
    /// </summary>
    public static class DebugTelemetry
    {
        /// <summary>
        /// Logs an Verbose to the Stream.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="tag">A unique tag. Try <c> $x = "`"$([Guid]::NewGuid().ToString())`""; $x | write-host; $x | set-clipboard</c> in powershell.</param>
        /// <param name="properties">Additional Properties to track with the telemetry</param>
        /// <param name="callerMemberName">The name of the caller who called the public method.</param>
        [Conditional("DEBUG")]
        public static void Verbose(
            string message,
            string tag,
            Dictionary<string, string> properties = null,
            [CallerMemberName]string callerMemberName = "") 
                => Telemetry.Verbose(message, tag, properties, callerMemberName);

        /// <summary>
        /// Logs an Informational Event to the Stream.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="tag">A unique tag. Try <c> $x = "`"$([Guid]::NewGuid().ToString())`""; $x | write-host; $x | set-clipboard</c> in powershell.</param>
        /// <param name="properties">Additional Properties to track with the telemetry</param>
        /// <param name="callerMemberName">The name of the caller who called the public method.</param>
        [Conditional("DEBUG")]
        public static void Info(
            string message,
            string tag,
            Dictionary<string, string> properties = null,
            [CallerMemberName]string callerMemberName = "")
                => Telemetry.Info(message, tag, properties, callerMemberName);

        /// <summary>
        /// Logs an Warning to the Stream.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="tag">A unique tag. Try <c> $x = "`"$([Guid]::NewGuid().ToString())`""; $x | write-host; $x | set-clipboard</c> in powershell.</param>
        /// <param name="properties">Additional Properties to track with the telemetry</param>
        /// <param name="callerMemberName">The name of the caller who called the public method.</param>
        [Conditional("DEBUG")]
        public static void Warn(
            string message,
            string tag,
            Dictionary<string, string> properties = null,
            [CallerMemberName]string callerMemberName = "")
            => Telemetry.Warn(message, tag, properties, callerMemberName);

        /// <summary>
        /// Logs an Error to the Stream. A non-critical error can be recovered from or may be transient.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="tag">A unique tag. Try <c> $x = "`"$([Guid]::NewGuid().ToString())`""; $x | write-host; $x | set-clipboard</c> in powershell.</param>
        /// <param name="ex">The exception that occurred. </param>
        /// <param name="properties">Additional Properties to track with the telemetry</param>
        /// <param name="callerMemberName">The name of the caller who called the public method.</param>
        [Conditional("DEBUG")]
        public static void Error(
            string message,
            string tag,
            Exception ex,
            Dictionary<string, string> properties = null,
            [CallerMemberName]string callerMemberName = "")
            => Telemetry.Error(message, tag, ex, properties, callerMemberName);

        /// <summary>
        /// Logs a critical Error to the Stream. A critical error cannot be recovered from and will stop your service. 
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="tag">A unique tag. Try <c> $x = "`"$([Guid]::NewGuid().ToString())`""; $x | write-host; $x | set-clipboard</c> in powershell.</param>
        /// <param name="ex">The exception that occurred. </param>
        /// <param name="properties">Additional Properties to track with the telemetry</param>
        /// <param name="callerMemberName">The name of the caller who called the public method.</param>
        [Conditional("DEBUG")]
        public static void Critical(
            string message,
            string tag,
            Exception ex,
            Dictionary<string, string> properties = null,
            [CallerMemberName]string callerMemberName = "")
            => Telemetry.Critical(message, tag, ex, properties, callerMemberName);
    }
}