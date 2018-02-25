using System;
namespace Homework.FrontEnd
{
    using Microsoft.ApplicationInsights;

    public static class Telemetry
    {
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
        public static TelemetryClient Client => telemetryClient.Value;
    }
}