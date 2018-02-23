namespace Homework
{
    using System;
    using Homework.Resources;

    /// <summary>
    /// An exception class for a null value somwhere along the object chain for a indexed property.
    /// </summary>
    /// <example>If the Indexed type A.B.C.D.E was called for type "/a/b/c/d" and c == null: this exception will be thrown</example>
    public class NullIndexedReferenceException : NullReferenceException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NullIndexedReferenceException"/> class.
        /// </summary>
        /// <param name="path">The path requested.</param>
        public NullIndexedReferenceException(string path)
            : base(string.Format(TraceResources.EXC_INDEX_NULLINDEXEDREFERENCE, path))
        {
            // chained constructor
        }
    }
}
