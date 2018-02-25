namespace Homework
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Interface for the Indexed Property
    /// </summary>
    public interface IIndexedProperty
    {
        /// <summary>
        /// Returns the value of the object defined by this property by giving the value of the parent object, to access.
        /// </summary>
        /// <param name="objParent">The parent object.</param>
        /// <param name="pathContext">The remaining path after the object is found.</param>
        /// <returns>The value.</returns>
        object GetFromParentType(object objParent, string pathContext);

        /// <summary>
        /// Returns the value of the object represnteded by the indexed path given the contextual input object.
        /// </summary>
        /// <param name="context">The input object that will be used to get the data from.</param>
        /// <param name="pathContext">The string path requested by the index caller.</param>
        /// <returns>The value of the object</returns>
        object Get(object context, string pathContext, bool forceCoalesceNulls = false);

        /// <summary>
        /// Returns the value of the object represnted by the indexed path given the contexual input object, with the stack containing references to the child accessors. 
        /// Please do not use this method, it is used internally.
        /// </summary>
        /// <param name="context">The input object that will be used to get the data from.</param>
        /// <param name="pathContext">The string path requested by the index caller.</param>
        /// <param name="propertyIndexes">Stack of accessor objects that are used to pull values from within the tree.</param>
        /// <returns>The value of the object at PathContext.</returns>
        [Obsolete("Used internally, do not use.", false)]
        object Get(object context, string pathContext, Stack<IIndexedProperty> propertyIndexes, bool forceCoalesceNulls = false);

        /// <summary>
        /// Returns the value of the object defined by this property by giving the value of the parent object, to access.
        /// </summary>
        /// <param name="objParent">The direct parent object.</param>
        /// <param name="pathContext">The remainder of the path, if the accessor is deemed to be a collection.</param>
        /// <param name="value">The value to set in this property.</param>
        void SetFromParentType(object objParent, string pathContext, object value);

        /// <summary>
        /// Sets the value of the object represented by the indexed path given the contextual input object.
        /// </summary>
        /// <param name="context">The input object that will be used to get the data from.</param>
        /// <param name="pathContext">The string path requested by the index caller.</param>
        /// <param name="value">The value to set on this property</param>
        void Set(object context, string pathContext, object value);

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="T:Homework.IIndexedProperty"/> coalesces nulls.
        /// </summary>
        /// <value><c>true</c> if coalesce nulls; otherwise, <c>false</c>.</value>
        bool CoalesceNulls { get; set; }
    }
}
