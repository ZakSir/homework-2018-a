namespace Homework
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
#if NET45 || NET451 || NET452 || NET46 || NET461 || NET462 || NET47 || NET471
    using System.Data;
#endif
    using System.Diagnostics;
    using System.Reflection;
    using Common;
    using Homework.Resources;

    /// <summary>
    /// Indexed property. This is a component of Indexed.
    /// </summary>
    /// <typeparam name="TParent">The type of the parent indexed property.</typeparam>
    /// <typeparam name="T">The type of this property.</typeparam>
    public class IndexedProperty<TParent, T> : IIndexedProperty
    {
        // v--- Index<ComplexTypes>
        // vv--- IndexedProperty<A>
        // vv v--- IndexedProperty<B>
        // vv v v--- IndexedProperty<C>
        // vv v v v--- IndexedProperty<E>
        // vv v v v v--- IndexedProperty<String>
        // /a/b/c/e/Foo

        /// <summary>
        /// The parent object, used for walking up the stack
        /// </summary>
        private readonly IIndexedProperty parent;

        /// <summary>
        /// boolean value to signify if the class has a parent
        /// </summary>
        private readonly bool hasParent;

        /// <summary>
        /// boolean value to signify if the class has a Getter accessor
        /// </summary>
        private readonly bool hasGetter;

        /// <summary>
        /// boolean value to signify if the class has a Setter accessor
        /// </summary>
        private readonly bool hasSetter;

        /// <summary>
        /// The getter reflection info that is the Getter Method for the property
        /// </summary>
        private readonly MethodInfo getter;

        /// <summary>
        /// The Setter reflection info that is the Setter Method for the propertys
        /// </summary>
        private readonly MethodInfo setter;

        /// <summary>
        /// The fully qualified path that this Indexed property exists at.
        /// </summary>
        private readonly string path;

        /// <summary>
        /// if this is set to true, null parent properties will return nulls when requested from this level.
        /// </summary>
        private bool coalesceNulls;

        /// <summary>
        /// The handling mode for special object types, like dictionaries and other indexes.
        /// </summary>
        private readonly IndexSpecialHandlingMode handlingMode;

        /// <summary>
        /// Initializes a new instance of the <see cref="IndexedProperty{TParent, T}"/> class.
        /// </summary>
        /// <param name="parent">The parent indexer</param>
        /// <param name="info">The property info gathered from the parent.</param>
        /// <param name="path">The fully qualified path of the property. Determined by object Name.</param>
        /// <param name="coalesceNulls">Not Implemented</param>
        public IndexedProperty(
            IIndexedProperty parent,
            PropertyInfo info,
            string path,
            bool coalesceNulls = false)
        {
            if (info == null)
            {
                throw new ArgumentNullException(nameof(info));
            }

            if (path == null)
            {
                throw new ArgumentNullException(nameof(path));
            }

            this.parent = parent;

            // the first level nodes wont have a parent.
            this.hasParent = parent.GetType() != typeof(NullPropertyIndexer);

            // see if this is a collection
            Type thisType = info.PropertyType;

            if (thisType.IsArray)
            {
                // this is an array type
                this.handlingMode = IndexSpecialHandlingMode.Array;
            }
            else if (typeof(IDictionary).IsAssignableFrom(thisType))
            {
                this.handlingMode = IndexSpecialHandlingMode.Dictionary;
            }
#if NET45 || NET451 || NET452 || NET46 || NET461 || NET462 || NET47 || NET471
            else if (thisType == typeof(DataRow))
            {
                this.handlingMode = IndexSpecialHandlingMode.DataRow;
            }
#endif
            else
            {
                this.handlingMode = IndexSpecialHandlingMode.None;
            }

            this.getter = info.GetGetMethod();
            this.setter = info.GetSetMethod();

            this.hasGetter = this.getter != null;
            this.hasSetter = this.setter != null;

            this.coalesceNulls = coalesceNulls;
            this.path = path;
        }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="T:Homework.IndexedProperty`2"/> coalesces nulls.
        /// </summary>
        /// <value><c>true</c> if coalesce nulls; otherwise, <c>false</c>.</value>
        public bool CoalesceNulls
        {
            get => this.coalesceNulls;
            set => this.coalesceNulls = value;
        }

        /// <summary>
        /// Gets the path that this accessor represents.
        /// </summary>
        public string Path => this.path;

        /// <summary>
        /// Gets a value using the current stack of property indexers. 
        /// </summary>
        /// <param name="context">The origin object to pull the value from.</param>
        /// <param name="pathContext">Used to determine where we are in the path tree.</param>
        /// <param name="propertyIndexes">The stack of property indexes</param>
        /// <returns>The boxed object contained at this location.</returns>
        public object Get(object context, string pathContext, Stack<IIndexedProperty> propertyIndexes, bool forceCoalesceNulls = false)
        {
            Indexed.DiagnosticTrace.TraceInformation($"Start Get");

            propertyIndexes.Push(this);

            if (this.hasParent)
            {
#pragma warning disable CS0618 // Type or member is obsolete
                object o = this.parent.Get(context, pathContext, propertyIndexes, forceCoalesceNulls);
#pragma warning restore CS0618 // Type or member is obsolete

                return o;
            }
            else
            {
                // we are the last accessor
                object result = context;
                IIndexedProperty accessor = null;
                while (propertyIndexes.Count > 0)
                {
                    Indexed.DiagnosticTrace.TraceInformation($"Property Stack Depth: {propertyIndexes.Count }, result == {result}");

                    accessor = propertyIndexes.Pop();

                    Indexed.DiagnosticTrace.TraceInformation($"Accessor for: {accessor.Path} this is {this.path}");

                    result = accessor.GetFromParentType(result, pathContext);

                    if (result == null)
                    {
                        Indexed.DiagnosticTrace.TraceInformation($"result is null @ Stack depth {propertyIndexes.Count }");

                        if (forceCoalesceNulls || this.coalesceNulls)
                        {
                            if (propertyIndexes.Count > 1)
                            {
                                return DBNull.Value;
                            }

                            // we cant go any father, but we can give a null and no exceptions
                            return null;
                        }
                        else
                        {
                            throw new NullIndexedReferenceException(this.path);
                        }
                    }

                    Indexed.DiagnosticTrace.TraceInformation($"result is {result.GetType().FullName}");
                }

                return result;
            }
        }

        /// <summary>
        /// Gets the value at this location in the property tree
        /// </summary>
        /// <param name="context">Root object to traverse</param>
        /// <param name="pathContext">Used to determine where we are in the path tree.</param>
        /// <param name="forceCoalesceNulls">If set to true will overwrite the local coalesce nulls.</param>
        /// <returns>The value of the object.</returns>
        object IIndexedProperty.Get(object context, string pathContext, bool forceCoalesceNulls)
        {
            object o = this.Get(context, pathContext, new Stack<IIndexedProperty>(), forceCoalesceNulls);

            return o;
        }

        /// <summary>
        /// Gets the actual value requested, from the value of the parent.
        /// </summary>
        /// <param name="objParent">The parent value-object</param>
        /// <param name="pathContext">The additional context provided to extract subobjects</param>
        /// <returns>The value-object at this level.</returns>
        public object GetFromParentType(object objParent, string pathContext)
        {
            if (objParent == null)
            {
                throw new ArgumentNullException(nameof(objParent), "Cannot get parent object and this indexer is not set to coalesce nulls.");
            }

            switch (this.handlingMode)
            {
                case IndexSpecialHandlingMode.None:
                    return this.TryGetFromParentObject(objParent);
                case IndexSpecialHandlingMode.Array:
                    return this.GetFromParentArrayType(objParent, pathContext);
                case IndexSpecialHandlingMode.Dictionary:
                    return this.GetFromParentIDictionaryType(objParent, pathContext);
                default:
                    // should be impossible to reach;
                    throw new NotSupportedException("This handling mode is not supported");
            }
        }

        /// <summary>
        /// Set the property via the reflection info.
        /// </summary>
        /// <param name="context">The root context object that will.</param>
        /// <param name="pathContext">Used to determine where we are in the path tree.</param>
        /// <param name="value">The value to apply to the specified property.</param>
        public void Set(object context, string pathContext, object value)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context), "The context object is null");
            }

            // value can be null (what if they are setting a null value :O)
            if (!this.hasSetter)
            {
                throw new AccessorNotDefinedException(context.GetType(), "Set", this.path);
            }

            object parentValue;

            if (this.hasParent)
            {
#pragma warning disable CS0618 // Type or member is obsolete <- Internal call used to discourage external calls
                parentValue = this.parent.Get(context, string.Empty, new Stack<IIndexedProperty>(), false);
#pragma warning restore CS0618 // Type or member is obsolete
            }
            else
            {
                parentValue = context;
            }

            if (parentValue == null)
            {
                throw new NullIndexedReferenceException(this.path);
            }

            this.SetFromParentType(parentValue, pathContext, value);
        }

        /// <summary>
        /// Set the value of an object using a path context and its parent and a typed value.
        /// </summary>
        /// <param name="context">the object to set a value against</param>
        /// <param name="pathContext">Used to determine where we are in the path tree.</param>
        /// <param name="value">the value to set.</param>
        public void Set(object context, string pathContext, T value)
        {
            this.Set(context, pathContext, value);
        }

        /// <summary>
        /// Set the value of an object using a path context and its parent.
        /// </summary>
        /// <param name="objParent">The parent of this property.</param>
        /// <param name="pathContext">The additional PathContext</param>
        /// <param name="value">The value to set</param>
        public void SetFromParentType(object objParent, string pathContext, object value)
        {
            if (objParent == null)
            {
                throw new NullIndexedReferenceException(this.path);
            }

            // we know that the value is not null, lets try and set it
            switch (this.handlingMode)
            {
                case IndexSpecialHandlingMode.Array:
                    this.SetFromParentArrayType(objParent, pathContext, value);
                    break;
                case IndexSpecialHandlingMode.Dictionary:
                    this.SetFromParentDictionaryType(objParent, pathContext, value);
                    break;
                default:
                    this.setter.Invoke(objParent, new object[1] { value });
                    break;
            }
        }

        /// <summary>
        /// Set a value in a parent array type
        /// </summary>
        /// <param name="objParent">The parent object</param>
        /// <param name="pathContext">The context of the Path.</param>
        /// <param name="value">The value to set.</param>
        public void SetFromParentArrayType(object objParent, string pathContext, object value)
        {
            if (string.IsNullOrEmpty(pathContext))
            {
                throw new ArgumentNullException(nameof(pathContext), "The path used to seek the object via context is null");
            }

            Indexed.DiagnosticTrace.Assert(objParent != null, "Cannot coninue getting accessor tree as the parent object is null");

            object parentObject = this.TryGetFromParentObject(objParent);
            if (parentObject == null)
            {
                // need to throw an exception as we cannot process an array
                // index on a null object
                throw new NullIndexedReferenceException(pathContext);
            }

            // use parse instead of tryParse to throw exceptions
            int index = int.Parse(pathContext);

            object[] array = (object[])parentObject;

            array[index] = value;
        }

        /// <summary>
        /// Sets the value of an object in a Dictionary Type.
        /// </summary>
        /// <param name="objParent">The parent object</param>
        /// <param name="pathContext">The context of the Path.</param>
        /// <param name="value">The value to set.</param>
        public void SetFromParentDictionaryType(object objParent, string pathContext, object value)
        {
            if (string.IsNullOrEmpty(pathContext))
            {
                throw new ArgumentNullException(nameof(pathContext));
            }

            #region Assertions
            Indexed.DiagnosticTrace.Assert(objParent != null, "Parent object is null");
            #endregion

            object parentObject = this.TryGetFromParentObject(objParent);
            if (parentObject == null)
            {
                // need to throw an exception as we cannot process an array
                // index on a null object
                throw new NullIndexedReferenceException(pathContext);
            }

            IDictionary dictionary = (IDictionary)parentObject;

            dictionary[pathContext] = value;
        }

        /// <summary>
        /// Get the value of this property from its parent that is an array type
        /// </summary>
        /// <param name="objParent">the object that is this properties parent.</param>
        /// <param name="pathContext">Used to determine where we are in the path tree.</param>
        /// <returns>The object requested by path.</returns>
        private object GetFromParentArrayType(object objParent, string pathContext)
        {
            if (string.IsNullOrEmpty(pathContext))
            {
                throw new ArgumentNullException(nameof(pathContext), "The incoming path context is null");
            }

            #region Assertions
            Indexed.DiagnosticTrace.Assert(objParent != null, "The parent Object is null");
            #endregion

            object parentObject = this.TryGetFromParentObject(objParent);
            if (parentObject == null)
            {
                // we can assert that we are coalescing nulls because otherwise an exception
                // would have been thrown
                return null;
            }

            // use parse instead of tryParse to throw exceptions
            int index = int.Parse(pathContext);

            object[] array = (object[])parentObject;

            // this will bubble index outofrange and such
            return array[index];
        }

        /// <summary>
        /// Gets an object from the parent dictionary type 
        /// </summary>
        /// <param name="objParent">The parent object</param>
        /// <param name="pathContext">Used to determine where we are in the path tree.</param>
        /// <returns>the object by path.</returns>
        private object GetFromParentIDictionaryType(object objParent, string pathContext)
        {
            if (string.IsNullOrEmpty(pathContext))
            {
                throw new ArgumentNullException(nameof(pathContext));
            }

            #region Assertions
            Indexed.DiagnosticTrace.Assert(objParent != null, "The parent object is null");
            #endregion

            object parentObject = this.TryGetFromParentObject(objParent);
            if (parentObject == null)
            {
                // we can assert that we are coalescing nulls because otherwise an exception
                // would have been thrown
                return null;
            }

            IDictionary dictionary = (IDictionary)parentObject;

            object result;
            if (dictionary.Contains(pathContext))
            {
                // the dictionary contains the key!
                result = dictionary[pathContext];
            }
            else
            {
                // the dictionary does not contain the key!
                result = null;
            }

            return result;

            // return dictionary?[]
        }

        private object TryGetFromParentObject(object objParent)
        {
            Indexed.DiagnosticTrace.Assert(objParent != null, $"{nameof(objParent)} is null");

            if (objParent == null)
            {
                return DBNull.Value;
            }
            else
            {
                object result = this.getter.Invoke(objParent, null);

                return result;
            }
        }
    }
}