namespace Homework
{
    /// <summary>
    /// The handling mode of the index.
    /// </summary>
    public enum IndexSpecialHandlingMode
    {
        /// <summary>
        /// No special handling, this property will be pulled by path only.
        /// </summary>
        None = 0,

        /// <summary>
        /// The object is an array and can be accessed as such
        /// </summary>
        /// <example>
        /// Can be accessed like "/a/b/c/32" where C is an array property.
        /// <code>
        /// class Foo
        /// {
        ///     public string[] Bar = new [] {"Hello", "World"};
        /// }
        /// 
        /// Class FooFoo
        /// {
        ///     public Foo = new Foo();
        /// }
        /// 
        /// Class FooFooFoo
        /// {
        ///     public FooFoo = new FooFoo();
        /// }
        /// 
        /// Indexed&lt;FooFooFoo&gt; inx = new Indexed&lt;FooFooFoo&gt;();
        /// Console.WriteLine(inx["/FooFoo/Foo/Bar/1"]);
        /// 
        /// // Prints: World
        /// </code>
        /// </example>
        Array = 1,

        /// <summary>
        /// The object is a Dictionary / Hashtable type and can be accessed as such
        /// </summary>
        /// <example>Can be accessed like "/a/b/c/ContosoCompanyLLC" where C is a Dictionary Property</example>
        Dictionary = 2  
    }
}
