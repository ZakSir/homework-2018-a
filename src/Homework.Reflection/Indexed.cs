using System;
using System.Collections.Generic;

namespace Homework
{
    using System.Diagnostics;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    using static Homework.Resources.Diagnostics;

    public class Indexed
    {
        /// <summary>
        /// Lock object for the init of the diagnostic trace.
        /// </summary>
        private static readonly object diagnosticTraceInitializationLockObject = new object();


        /// <summary>
        /// Diagnostic trace
        /// </summary>
        private static TraceSource diagnosticTrace;


        /// <summary>
        /// Gets the diagnostic trace source to attach a listener to.
        /// </summary>
        internal static TraceSource DiagnosticTrace => diagnosticTrace;

        /// <summary>
        /// Attaches a TraceListener directly to the internal diagnostic trace of the Indexer
        /// </summary>
        /// <param name="listener"></param>
        public static void AttachTraceListener(TraceListener listener)
        {
            diagnosticTrace.Listeners.Add(listener);
        }

        /// <summary>
        /// Initializes the Diagnostic tracer for tracing this class
        /// </summary>
        private static void InitializeDiagnosticTracing()
        {
            if (diagnosticTrace != null)
            {
                // diagnostic trace is already setup.
                return;
            }

            lock (diagnosticTraceInitializationLockObject)
            {
                if (diagnosticTrace != null)
                {
                    // operation completed during lock acquisition
                    return;
                }

                diagnosticTrace = new TraceSource(DiagnosticTraceName, SourceLevels.All);
            }
        }
    }
}
