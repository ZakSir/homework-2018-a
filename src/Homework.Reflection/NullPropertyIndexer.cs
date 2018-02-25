namespace Homework
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Property indexer used as the very root of the chain. 
    /// </summary>
    public class NullPropertyIndexer : IIndexedProperty
    {
        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="T:Homework.NullPropertyIndexer"/> coalesces nulls. Does nothing in this context.
        /// </summary>
        /// <value><c>true</c> if coalesce nulls; otherwise, <c>false</c>.</value>
        public bool CoalesceNulls { get; set; }

        /// <summary>
        /// Gets the value at this location in the property tree
        /// </summary>
        /// <param name="context">Root object to traverse</param>
        /// <param name="pathContext">Used to determine where we are in the path tree.</param>
        /// <returns>The value of the object.</returns>
        object IIndexedProperty.Get(object context, string pathContext, bool forceCoalesceNulls = false)
        {
            return null;
        }

        /// <summary>
        /// Gets a value using the current stack of property indexers. 
        /// </summary>
        /// <param name="context">The origin object to pull the value from.</param>
        /// <param name="pathContext">Used to determine where we are in the path tree.</param>
        /// <param name="propertyIndexes">The stack of property indexes</param>
        /// <returns>The boxed object contained at this location.</returns>
        object IIndexedProperty.Get(object context, string pathContext, Stack<IIndexedProperty> propertyIndexes, bool forceCoalesceNulls = false)
        {
            return null;
        }

        /// <summary>
        /// Gets the actual value requested, from the value of the parent.
        /// </summary>
        /// <param name="objParent">The parent value-object</param>
        /// <param name="pathContext">The additional context provided to extract subobjects</param>
        /// <returns>The value-object at this level.</returns>
        object IIndexedProperty.GetFromParentType(object objParent, string pathContext)
        {
            return null;
        }

        /// <summary>
        /// Set the property via the reflection info.
        /// </summary>
        /// <param name="context">The root context object that will.</param>
        /// <param name="pathContext">Used to determine where we are in the path tree.</param>
        /// <param name="value">The value to apply to the specified property.</param>
        void IIndexedProperty.Set(object context, string pathContext, object value)
        {
            // throw new NotImplementedException();
        }

        /// <summary>
        /// Set the value of an object using a path context and its parent.
        /// </summary>
        /// <param name="objParent">The parent of this property.</param>
        /// <param name="pathContext">The additional PathContext</param>
        /// <param name="value">The value to set</param>
        void IIndexedProperty.SetFromParentType(object objParent, string pathContext, object value)
        {
            // throw new NotImplementedException();
        }
    }
}
