namespace Homework
{
    using Common;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using System.Text;
    using System.Collections;


    /// <summary>
    /// An indexer class. Allows the use of *most* complex classes with nested elements. 
    /// Supports All normal properties
    /// Supports Array Properties
    /// Supports IDictionary Properties
    /// Supports DataRow Properties
    /// </summary>
    public partial class Indexed : IReadOnlyDictionary<string, object>
    {
        /// <summary>
        /// Forward slash
        /// </summary>
        private const char FSLASH = '/';

        /// <summary>
        /// Forward Slash, in string form, for the start of the index path.
        /// </summary>
        private const string STARTPATH = "/";

        /// <summary>
        /// Seperator character set for string split operations (perf)
        /// </summary>
        private static readonly char[] PathSeperatorChars = new char[] { FSLASH };

        /// <summary>
        /// The map cache.
        /// </summary>
        private static readonly Dictionary<Type, IndexCache> IndexCache = new Dictionary<Type, IndexCache>();

        /// <summary>
        /// The caching lock objects.
        /// </summary>
        private static readonly Dictionary<Type, object> CachingLockObjects = new Dictionary<Type, object>();

        /// <summary>
        /// The type of the object.
        /// </summary>
        private readonly Type objectType;

        /// <summary>
        /// Unique Id to describe the object. Used mostly for tracing.
        /// </summary>
        private readonly Guid objectId = Guid.NewGuid();

        /// <summary>
        /// The Origin Value
        /// </summary>
        private readonly object value;

        /// <summary>
        /// If null properties should be returned or throw exceptions.
        /// </summary>
        private bool coalesceNulls;

        /// <summary>
        /// Initializes a new instance of the <see cref="Indexed{T}"/> class. 
        /// The first initialization of a type using this class will incur the accessor cache for this type.
        /// This will only occur once per type. 
        /// </summary>
        /// <param name="obj">The object instance that will be represented by this indexer.</param>
        /// <param name="maxStackDepth">The maximum stack depth allowed</param>
        /// <param name="performanceMode">The performance mode of the indexer. </param>
        public Indexed(
            object obj,
            bool coalesceNulls = false,
            int maxStackDepth = 10,
            IndexerPerormanceMode performanceMode = IndexerPerormanceMode.NoEnhancement)
        {
            if (obj == null)
            {
                throw new ArgumentNullException(nameof(obj));
            }

            this.objectType = obj.GetType();

            CheckBuildIndex(this.objectType, coalesceNulls, maxStackDepth, performanceMode);

            this.value = obj;
            this.coalesceNulls = coalesceNulls;
        }

        /// <summary>
        /// Gets the base value of this indexed object.
        /// </summary>
        public object Value
        {
            get
            {
                return this.value;
            }
        }

        /// <summary>
        /// Gets the keys for this index.
        /// </summary>
        /// <value>The keys.</value>
        public IEnumerable<string> Keys => this.Accessors.Keys;

        /// <summary>
        /// Gets the values in flat format for this object. Dangerous to use without a key map. 
        /// </summary>
        /// <value>The values.</value>
        public IEnumerable<object> Values => this.ToFlatDictionary().Values;

        /// <summary>
        /// Gets the count of accessors in this index.
        /// </summary>
        /// <value>The count.</value>
        public int Count => this.Accessors.Count;

        /// <summary>
        /// Internal handle to the Accessor cache for this object type.
        /// </summary>
        /// <value>The accessors.</value>
        protected Dictionary<string, IIndexedProperty> Accessors
        {
            get
            {
                /*
                 * If the object constructed and did not throw exceptions,
                 * that means that the cache for this object is built
                 * and we can assume that the object will be in the
                 * dictionary as we never purge the type cache.
                 */

                return Indexed.IndexCache[this.objectType].Accessors;
            }
        }

        /// <summary>
        /// Retrieves a value at the specified index.
        /// </summary>
        /// <param name="index">The index path.</param>
        /// <returns>The Value of the specified location.</returns>
        public object this[string index]
        {
            get
            {
                if (string.IsNullOrWhiteSpace(index))
                {
                    throw new ArgumentNullException(nameof(index));
                }

                string remainder;
                object result = this.GetAccessor(index, out remainder).Get(this.value, remainder);

                return result == DBNull.Value ? null : result;
            }

            set
            {
                if (string.IsNullOrWhiteSpace(index))
                {
                    throw new ArgumentNullException(nameof(index));
                }

                string remainder;
                IIndexedProperty accessor = this.GetAccessor(index, out remainder);
                accessor.Set(this.value, remainder, value);
            }
        }

        /// <summary>
        /// Checks the equality between two <see cref="Indexed{T}"/> objects
        /// </summary>
        /// <param name="a">The first object</param>
        /// <param name="b">The second object</param>
        /// <returns>True of the the underlying objects are equal</returns>
        public static bool operator ==(Indexed a, Indexed b)
        {
            // If both are null, or both are same instance, return true.
            if (object.ReferenceEquals(a, b))
            {
                return true;
            }

            // If one is null, but not both, return false.
            if (((object)a == null) || ((object)b == null))
            {
                return false;
            }

            // check type equality first as its faster
            if (a.objectType != b.objectType)
            {
                return false;
            }

            return a.value.Equals(b.value);
        }

        /// <summary>
        /// Checks the inequality between two <see cref="Indexed{T}"/> objects
        /// </summary>
        /// <param name="a">The first object</param>
        /// <param name="b">The second object</param>
        /// <returns>True of the the underlying objects are not equal</returns>
        public static bool operator !=(Indexed a, Indexed b)
        {
            // If both are null, or both are same instance, return true.
            if (object.ReferenceEquals(a, b))
            {
                return false;
            }

            // If one is null, but not both, return false.
            if (((object)a == null) || ((object)b == null))
            {
                return true;
            }

            // check type equality first as its faster
            if (a.objectType == b.objectType)
            {
                return false;
            }

            return !a.value.Equals(b.Value);
        }


        /// <summary>
        /// Gets a flat dictionary representing the object. Useful for serialization.
        /// </summary>
        /// <returns>A dictionary representing the index of the object.</returns>
        public Dictionary<string, object> ToFlatDictionary()
        {
            this.coalesceNulls = true;
            Dictionary<string, object> result = new Dictionary<string, object>();

            foreach (KeyValuePair<string, IIndexedProperty> row in this.Accessors)
            {
                row.Value.CoalesceNulls = true;
                DiagnosticTrace.TraceInformation($"Getting value of {row.Key} from outer");
                object internalValue = row.Value.Get(this.value, null, true);

                if (internalValue != DBNull.Value)
                {
                    result[row.Key] = internalValue == DBNull.Value ? null : internalValue;
                }
                else
                {
                    DiagnosticTrace.TraceInformation($"Skipping {row.Key} as it is returned as db null, a parent is probably null");
                }
            }

            return result;
        }

        /// <summary>
        /// Gets the hashcode from the internal value. This is a passthru value.
        /// </summary>
        /// <returns>The hashcode from the internal value;</returns>
        public override int GetHashCode()
        {
            return this.value.GetHashCode();
        }

        /// <summary>
        /// Tests equalaties between this and another indexed object using the contined type.
        /// </summary>
        /// <param name="other">The other type to test against. </param>
        /// <returns>The result of the equality test.</returns>
        public bool Equals(Indexed other)
        {
            return this.value.Equals(other.value);
        }

        /// <summary>
        /// Checks equality 
        /// </summary>
        /// <param name="obj">The object of which to check equality</param>
        /// <returns>A value indicating whether the objects are in fact equal.</returns>
        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                /*
                 * no null exceptions thrown here,
                 * we will return false as it is 
                 * technically not equal to null.
                 */
                return false;
            }

            if (obj is Indexed)
            {
                Indexed inx = obj as Indexed;

                /*
                 * Non recursive call, this will call into 
                 * the strongly type equality check that will 
                 * look at the inner value of the object.
                 */
                return this.Equals(inx); // non recursive call 
            }

            // obj is not indexed.
            return false;
        }

        /// <summary>
        /// Begins to build the index.
        /// </summary>
        /// <param name="maxStackDepth">The maximum property depth to index.</param>
        /// <param name="performanceMode">The performance mode of the indexer. Controls the types of properties indexed.</param>
        private static void CheckBuildIndex(Type objectType, bool coalesceNulls, int maxStackDepth, IndexerPerormanceMode performanceMode)
        {

            if (!Indexed.IndexCache.ContainsKey(objectType))
            {
                lock (GetOrCreateLockObject(objectType))
                {
                    if (!Indexed.IndexCache.ContainsKey(objectType))
                    {
                        IndexCache ic = new IndexCache(objectType);
                        ic.MaxStackDepth = maxStackDepth;
                        ic.PerformanceMode = performanceMode;

                        // if the index has not been built
                        // or the stack depth requested does not match the previous
                        // value, we will rebuild the index
                        BuildIndex(
                            objectType,
                            ic,
                            coalesceNulls,
                            new NullPropertyIndexer(),
                            STARTPATH,
                            new LinkedList<PropertyInfo>());

                        Indexed.IndexCache.Add(objectType, ic);
                    }
                }
            }
        }

        /// <summary>
        /// Gets the accessor object for the specified index. 
        /// </summary>
        /// <param name="index">The index path to get an accessor for.</param>
        /// <param name="remainder">The remainder of the path left over after the accessor is found.</param>
        /// <returns>The located accessor</returns>
        private IIndexedProperty GetAccessor(string index, out string remainder)
        {
            /* 
             * This method will attempt first to locate the 
             * accessor object first by using the Full path 
             * directly specified by the caller.
             * 
             * If this fails, we will sever one section of the path
             * off of the end to attempt to find a subaccessor. 
             * 
             * A subaccessor is used to build an indexed accessor 
             * For things such as arrays and Dictionaries.
             * 
             * For example
             * /yolo/swag/baller
             * 
             * Represents
             * 
             * v-- Indexed object
             * v      v-- Object
             * v      v    v-- Object
             * v      v    v     v-- The dictionary index 
             * Object.yolo.swag["baller"]
             * 
             * The internally indexed path of this would be
             * /yolo/swag that will return the Dictionary that we can index into.
             * 
             * If no subindex path is found, we will throw an exception to the caller.
             */

            IIndexedProperty accessor;
            if (!this.Accessors.TryGetValue(index, out accessor))
            {
                // the property was not found try and see if the property is indexible
                string[] parts = index.Split(PathSeperatorChars, StringSplitOptions.RemoveEmptyEntries);

                if (parts.Length < 2)
                {
                    // there is nothing to subindex.
                    throw new InvalidIndexException(index);
                }

                // the subindex path
                string subPath = BuildPath(parts.Take(parts.Length - 1));

                // try to get the subindex accessor
                if (!Accessors.TryGetValue(subPath, out accessor))
                {
                    // cannot find anything by this path.
                    throw new InvalidIndexException(index);
                }
                else
                {
                    // we found a subaccessor, so 
                    // we will return the remainder
                    // of the path to the caller
                    // using the out variable.
                    remainder = parts.Last();
                }
            }
            else
            {
                // we found an accessor, but it
                // is at the direct path, so
                // we do not have any remainder to work with.
                remainder = null;
            }

            return accessor;
        }

        /// <summary>
        /// Builds a path based on string parts.
        /// </summary>
        /// <param name="parts">The path parts in array form.</param>
        /// <returns>The assembled string path.</returns>
        private static string BuildPath(IEnumerable<string> parts)
        {
            if (parts == null)
            {
                throw new ArgumentNullException(nameof(parts));
            }

            if (!parts.Any())
            {
                throw new ArgumentException("Path Collection Empty", nameof(parts));
            }

            StringBuilder b = new StringBuilder();
            foreach (string part in parts)
            {
                b.Append(FSLASH);
                b.Append(part ?? string.Empty);
            }

            return b.ToString();
        }

        /// <summary>
        /// Builds a level of the Accessor Index for the specified type
        /// </summary>
        /// <param name="currentType">The type to be indexed.</param>
        /// <param name="parentPropertyIndexer">THe parent index</param>
        /// <param name="currentPath">The current path of the index builder.</param>
        /// <param name="typeStack">The type stack, used for redundancy checking.</param>
        private static void BuildIndex(
            Type currentType,
            IndexCache cacheObject,
            bool coalesceNulls,
            IIndexedProperty parentPropertyIndexer,
            string currentPath,
            LinkedList<PropertyInfo> typeStack)
        {
            Indexed.DiagnosticTrace.TraceInformation($"Begin index build for {currentType.FullName} @ {currentPath}");
            Indexed.DiagnosticTrace.Assert(currentType != null, $"The current type is null ({nameof(currentType)})");
            Indexed.DiagnosticTrace.Assert(parentPropertyIndexer != null, $"The parent property indexer is null ({nameof(parentPropertyIndexer)})");
            Indexed.DiagnosticTrace.Assert(currentPath != null, $"The current path is null ({nameof(currentPath)})");

            // get a list of the properties on this type.
            PropertyInfo[] currentTypeProperties = currentType.GetProperties();

            Indexed.DiagnosticTrace.TraceInformation($"{currentType.FullName} has {currentTypeProperties.Length} properties.");

            foreach (PropertyInfo currentTypeProperty in currentTypeProperties)
            {
                // Create a generic type definition of the indexed property class.
                Type indexPropGenericTypeDef = typeof(IndexedProperty<,>);

                // Create a generic type definition that includes the type parameters 
                Type indexPropType = indexPropGenericTypeDef.MakeGenericType(new Type[] { currentType, currentTypeProperty.PropertyType });

                // create the object using the activator.
                object activatedAccessor = Activator.CreateInstance(indexPropType, parentPropertyIndexer, currentTypeProperty, currentPath, coalesceNulls);

                // cast it as the interface (This should not fail)
                IIndexedProperty accessor = (IIndexedProperty)activatedAccessor;

                Indexed.DiagnosticTrace.TraceInformation($"property {currentPath}{currentTypeProperty.Name} has completed accessor building.");

                // add this accessor to the cache at the correct path
                cacheObject.Accessors[$"{currentPath}{currentTypeProperty.Name}"] = accessor;

                if (cacheObject.PerformanceMode == IndexerPerormanceMode.CommonProperties)
                {
                    if (currentTypeProperty.PropertyType == typeof(System.Data.DataRow))
                    {
                        // we only will need the value index
                        continue;
                    }
                }

                // dont subindex things we dont have to
                if (IsSkippableType(currentTypeProperty.PropertyType))
                {
                    Indexed.DiagnosticTrace.TraceInformation($"Skipping subindexing on {currentPath}{currentTypeProperty.Name} as it is a skippable type");
                    continue;
                }

                // if the type of the property is primitive, do not get its subtypes. 
                // also need to not get strings subtypes.
                if (IsSkippableReflectionType(currentTypeProperty.PropertyType))
                {
                    // skip subprocessing of base types
                    Indexed.DiagnosticTrace.TraceInformation($"Skipping subindexing on {currentPath}{currentTypeProperty.Name} as it is a reflection type");
                    continue;
                }

                // if we have an infinite loop property
                if (typeStack.Count > 0)
                {
                    bool found = false;
                    foreach (PropertyInfo previousInfo in typeStack)
                    {
                        if (previousInfo.PropertyType == currentTypeProperty.PropertyType
                                && previousInfo.Name == currentTypeProperty.Name)
                        {
                            found = true;
                            break;
                        }
                    }

                    if (found)
                    {
                        Indexed.DiagnosticTrace.TraceInformation($"Skipping subindexing on {currentPath}{currentTypeProperty.Name} as is already referenced directly above in this property stack (infinite loop property)");

                        continue;
                    }
                }

                if (typeStack.Count > cacheObject.MaxStackDepth)
                {
                    Indexed.DiagnosticTrace.TraceInformation($"Skipping subindexing on {currentPath}{currentTypeProperty.Name} as we have reached the maximum depth allowed to index.");

                    // it doesnt matter if there are more properties, skip them.
                    continue;
                }

                // add current type to stack
                typeStack.AddFirst(currentTypeProperty);

                Indexed.DiagnosticTrace.TraceInformation($"starting subindexing on {currentPath}{currentTypeProperty.Name}");

                BuildIndex(currentTypeProperty.PropertyType, cacheObject, coalesceNulls, accessor, $"{currentPath}{currentTypeProperty.Name}{FSLASH}", typeStack);

                typeStack.RemoveFirst();
            }
        }

        /// <summary>
        /// Checks to see if the type being indexed is skippable.
        /// </summary>
        /// <param name="t">The type to check.</param>
        /// <returns>A boolean value indicating if the type is skippable.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool IsSkippableType(Type t)
        {
            if (t.IsPrimitive)
            {
                return true;
            }

            if (t == typeof(string))
            {
                return true;
            }

            if (IsSkippableReflectionType(t))
            {
                return true;
            }

            if (t.IsArray)
            {
                Type st = t.GetElementType();

                if (IsSkippableReflectionType(st))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Checks to see if the type is skippable as a reflection type.
        /// </summary>
        /// <param name="t">The type to check.</param>
        /// <returns>A boolean value indicating if the type is skippable.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool IsSkippableReflectionType(Type t)
        {
            if (t == typeof(Type))
            {
                return true;
            }

            if (t == typeof(System.Globalization.CultureInfo))
            {
                return true;
            }

            if (typeof(MemberInfo).IsAssignableFrom(t))
            {
                return true;
            }

            return false;
        }

        public bool ContainsKey(string key)
        {
            return Accessors.ContainsKey(key);
        }

        public bool TryGetValue(string key, out object value)
        {
            bool result = Accessors.ContainsKey(key);

            if (result)
            {
                value = this[key];
            }
            else
            {
                value = null;
            }

            return result;
        }

        public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
        {
            return this.ToFlatDictionary().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        private static object GetOrCreateLockObject(Type t)
        {
            if (!CachingLockObjects.TryGetValue(t, out object lockObject))
            {
                lockObject = new object();
                CachingLockObjects.Add(t, lockObject);
            }

            return lockObject;
        }
    }
}
