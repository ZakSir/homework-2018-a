namespace Homework
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Serialization;
    using System.Text;
    using System.Threading.Tasks;
    using Homework.Resources;

    /// <summary>
    /// An exception representing a request to get an accessor that does not exist.
    /// </summary>
    public class AccessorNotDefinedException : Exception
    {
        private const string MISSING = "<<missing>>";

        /// <summary>
        /// Initializes a new instance of the <see cref="AccessorNotDefinedException"/> class.
        /// </summary>
        /// <param name="rootType">The type</param>
        /// <param name="direction">The direction requested</param>
        /// <param name="path">The path of the request</param>
        public AccessorNotDefinedException(Type rootType, string direction, string path)
            : base(string.Format(
                "The accessor for type {0} cannot be found for path {1} direction {2}",
                rootType.FullName ?? MISSING,
                path ?? MISSING,
                direction ?? MISSING))
        {
            // Chained
        }
    }
}
