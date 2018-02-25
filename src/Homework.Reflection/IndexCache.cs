using System;
using System.Collections.Generic;

namespace Homework
{
    /// <summary>
    /// A cache of index information about a type.
    /// </summary>
    public class IndexCache
    {
        /// <summary>
        /// Cached type object.
        /// </summary>
        private readonly Type indexedType;

        /// <summary>
        /// Accessors by path, this is the core of the indexer.
        /// </summary>
        private readonly Dictionary<string, IIndexedProperty> accessors;

        /// <summary>
        /// The maximum depth that we will index properties
        /// </summary>
        private int maxStackDepth;

        /// <summary>
        /// The performance mode of the indexer
        /// </summary>
        private IndexerPerormanceMode performanceMode;

        public IndexCache(Type type)
        {
            this.indexedType = type;
            this.accessors = new Dictionary<string, IIndexedProperty>();
        }

        /// <summary>
        /// Gets the type of object that is being indexed.
        /// </summary>
        /// <value>The type of the indexed.</value>
        public Type IndexedType => indexedType;

        /// <summary>
        /// Gets the accessors for this index.
        /// </summary>
        /// <value>The accessors.</value>
        public Dictionary<string, IIndexedProperty> Accessors => this.accessors;

        /// <summary>
        /// Gets or sets the max stack depth.
        /// </summary>
        /// <value>The max stack depth.</value>
        public int MaxStackDepth
        {
            get => maxStackDepth; 
            set => maxStackDepth = value;
        }

        /// <summary>
        /// Gets or sets the performance mode. This performance mode changes the number of properties that are indexed.
        /// </summary>
        /// <value>The performance mode.</value>
        public IndexerPerormanceMode PerformanceMode 
        { 
            get => performanceMode; 
            set => performanceMode = value; 
        }
    }
}
