namespace Homework
{
    using System;
    using Homework.Resources;

    /// <summary>
    /// Invalid Index. The index requested does not represent a valid object path.
    /// </summary>
    public class InvalidIndexException : NullReferenceException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidIndexException"/> class.
        /// </summary>
        /// <param name="path">The path requested.</param>
        public InvalidIndexException(string path)
            : base(string.Format(TraceResources.ERR_INDEXER_INVALID_INDEX, path))
        {
            // chained constructor
        }
    }
}
