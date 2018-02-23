using System;
using System.Diagnostics;

namespace Common
{
    public static class TraceSourceExtensions
    {
        public static void Assert(this TraceSource traceSource, bool condition, string message = null)
        {
            if (traceSource == null)
            {
                System.Diagnostics.Trace.WriteLine("The tracesource Specified is null and will not emit telemetry");

                // do not break on logging functions
                return;
            }

            if (condition)
            {
                return;
            }
            // condition failed incur expense
            string stack = Environment.StackTrace;

            string endMessage;
            if (message == null)
            {
                endMessage = "The Assertion failed. Stack: " + stack;
            }
            else
            {
                endMessage = "The Assertion failed. " + message + Environment.NewLine + "Stack: " + stack;
            }

            traceSource.TraceEvent(TraceEventType.Error, 0, endMessage);
        }
    }
}
