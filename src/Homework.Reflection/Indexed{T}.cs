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


    /// <summary>
    /// An indexer class. Allows the use of *most* complex classes with nested elements. 
    /// Supports All normal properties
    /// Supports Array Properties
    /// Supports IDictionary Properties
    /// Supports DataRow Properties
    /// </summary>
    /// <typeparam name="T">The type to Index.</typeparam>
    public class Indexed<T> : IEquatable<Indexed<T>>
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
        /// Cached type object.
        /// </summary>
        private static readonly Type IndexedType;

        /// <summary>
        /// Accessors by path, this is the core of the indexer.
        /// </summary>
        private static readonly Dictionary<string, IIndexedProperty> Accessors;

        /// <summary>
        /// Specifies if the index has been built for this type;
        /// </summary>
        private static bool indexBuilt;

        /// <summary>
        /// The maximum depth that we will index properties
        /// </summary>
        private static int maxStackDepth;

        /// <summary>
        /// The performance mode of the indexer
        /// </summary>
        private static IndexerPerormanceMode performanceMode;

        /// <summary>
        /// Unique Id to describe the object. Used mostly for tracing.
        /// </summary>
        private readonly Guid objectId = Guid.NewGuid();

        /// <summary>
        /// The Origin Value
        /// </summary>
        private readonly T value;

        /// <summary>
        /// Initializes static members of the <see cref="Indexed{T}"/> class. 
        /// </summary>
        static Indexed()
        {
            IndexedType = typeof(T);
            Accessors = new Dictionary<string, IIndexedProperty>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Indexed{T}"/> class. 
        /// The first initialization of a type using this class will incur the accessor cache for this type.
        /// This will only occur once per type. 
        /// </summary>
        /// <param name="obj">The object instance that will be represented by this indexer.</param>
        /// <param name="maxStackDepth">The maximum stack depth allowed</param>
        /// <param name="performanceMode">The performance mode of the indexer. </param>
        public Indexed(
            T obj,
            int maxStackDepth = 10,
            IndexerPerormanceMode performanceMode = IndexerPerormanceMode.NoEnhancement)
        {
            CheckBuildIndex(maxStackDepth, performanceMode);

            if (obj == null)
            {
                throw new ArgumentNullException(nameof(obj));
            }

            this.value = obj;
        }

        /// <summary>
        /// Gets the base value of this indexed object.
        /// </summary>
        public T Value
        {
            get
            {
                return this.value;
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
                return GetAccessor(index, out remainder).Get(this.value, remainder);
            }

            set
            {
                if (string.IsNullOrWhiteSpace(index))
                {
                    throw new ArgumentNullException(nameof(index));
                }

                string remainder;
                IIndexedProperty accessor = GetAccessor(index, out remainder);
                accessor.Set(this.value, remainder, value);
            }
        }

        /// <summary>
        /// Converts this object to its base value. Returns the value stored in this.value
        /// </summary>
        /// <param name="obj">The object to be cast</param>
        public static explicit operator T(Indexed<T> obj)
        {
            return obj.value;
        }

        /// <summary>
        /// Converts this object to an Indexed version of this object by creating a new instance of the class.
        /// </summary>
        /// <param name="obj">The object to be cast</param>
        public static explicit operator Indexed<T>(T obj)
        {
            return new Indexed<T>(obj);
        }

        /// <summary>
        /// Checks the equality between two <see cref="Indexed{T}"/> objects
        /// </summary>
        /// <param name="a">The first object</param>
        /// <param name="b">The second object</param>
        /// <returns>True of the the underlying objects are equal</returns>
        public static bool operator ==(Indexed<T> a, Indexed<T> b)
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

            return a.value.Equals(b.value);
        }

        /// <summary>
        /// Checks the inequality between two <see cref="Indexed{T}"/> objects
        /// </summary>
        /// <param name="a">The first object</param>
        /// <param name="b">The second object</param>
        /// <returns>True of the the underlying objects are not equal</returns>
        public static bool operator !=(Indexed<T> a, Indexed<T> b)
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

            return !a.value.Equals(b.Value);
        }


        /// <summary>
        /// Gets a flat dictionary representing the object. Useful for serialization.
        /// </summary>
        /// <returns>A dictionary representing the index of the object.</returns>
        public Dictionary<string, object> ToFlatDictionary()
        {
            Dictionary<string, object> result = new Dictionary<string, object>();

            foreach (KeyValuePair<string, IIndexedProperty> row in Indexed<T>.Accessors)
            {
                object internalValue = row.Value.Get(this.value, null);

                result[row.Key] = internalValue;
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
        public bool Equals(Indexed<T> other)
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

            if (obj is Indexed<T>)
            {
                Indexed<T> inx = obj as Indexed<T>;

                /*
                 * Non recursive call, this will call into 
                 * the strongly type equality check that will 
                 * look at the inner value of the object.
                 */
                return this.Equals(inx); // non recursive call 
            }

            return base.Equals(obj);
        }

        /// <summary>
        /// Begins to build the index.
        /// </summary>
        /// <param name="maxStackDepth">The maximum property depth to index.</param>
        /// <param name="performanceMode">The performance mode of the indexer. Controls the types of properties indexed.</param>
        private static void CheckBuildIndex(int maxStackDepth, IndexerPerormanceMode performanceMode)
        {
            if (
                !indexBuilt
                || Indexed<T>.maxStackDepth != maxStackDepth
                || Indexed<T>.performanceMode != performanceMode)
            {
                Indexed<T>.maxStackDepth = maxStackDepth;
                Indexed<T>.performanceMode = performanceMode;

                // if the index has not been built
                // or the stack depth requested does not match the previous
                // value, we will rebuild the index
                BuildIndex(
                    IndexedType,
                    new NullPropertyIndexer(),
                    STARTPATH,
                    new LinkedList<PropertyInfo>());

                indexBuilt = true;
            }
        }

        /// <summary>
        /// Gets the accessor object for the specified index. 
        /// </summary>
        /// <param name="index">The index path to get an accessor for.</param>
        /// <param name="remainder">The remainder of the path left over after the accessor is found.</param>
        /// <returns>The located accessor</returns>
        private static IIndexedProperty GetAccessor(string index, out string remainder)
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
            if (!Accessors.TryGetValue(index, out accessor))
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
                object activatedAccessor = Activator.CreateInstance(indexPropType, parentPropertyIndexer, currentTypeProperty, currentPath, false);

                // cast it as the interface (This should not fail)
                IIndexedProperty accessor = (IIndexedProperty)activatedAccessor;

                Indexed.DiagnosticTrace.TraceInformation($"property {currentPath}{currentTypeProperty.Name} has completed accessor building.");

                // add this accessor to the cache at the correct path
                Accessors[$"{currentPath}{currentTypeProperty.Name}"] = accessor;

                if (performanceMode == IndexerPerormanceMode.CommonProperties)
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

                if (typeStack.Count > maxStackDepth)
                {
                    Indexed.DiagnosticTrace.TraceInformation($"Skipping subindexing on {currentPath}{currentTypeProperty.Name} as we have reached the maximum depth allowed to index.");

                    // it doesnt matter if there are more properties, skip them.
                    continue;
                }

                // add current type to stack
                typeStack.AddFirst(currentTypeProperty);

                BuildIndex(currentTypeProperty.PropertyType, accessor, $"{currentPath}{currentTypeProperty.Name}{FSLASH}", typeStack);

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
    }
}
