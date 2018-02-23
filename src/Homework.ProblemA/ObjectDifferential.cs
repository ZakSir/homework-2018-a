using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Homework.ProblemA
{
    /// <summary>
    /// A contract type to show the differential between two disparate POCO
    /// </summary>
    public class ObjectDifferential
    {
        /// <summary>
        /// Are the two POCO the same CLR type.
        /// </summary>
        public bool IsTypeMatch { get; set; }

        /// <summary>
        /// Gets or sets the full name of the type that Object A represents.
        /// </summary>
        public string FullTypeNameA { get; set; }

        /// <summary>
        /// Gets or sets the full name of the type that Object B represents.
        /// </summary>
        public string FullTypeNameB { get; set; }

        /// <summary>
        /// Gets or sets a list of all of the properties in both Object A and Object B where the property name and the property value are the same.
        /// </summary>
        public IEnumerable<ObjectProperty> MatchingProperties { get; set; }

        /// <summary>
        /// Gets or sets a list of the properties that share identical names, but the values of each property are mismatched.
        /// </summary>
        public IEnumerable<ObjectPropertyPair> MismatchingProperties { get; set; }

        /// <summary>
        /// Gets or sets a list of properties that only exist in object type A. See <see cref="FullTypeNameA"/>
        /// </summary>
        public IEnumerable<ObjectProperty> OrphanPropertiesInA { get; set; }

        /// <summary>
        /// Gets or sets a list of properties that only exist in object type B. See <see cref="FullTypeNameB"/>
        /// </summary>
        public IEnumerable<ObjectProperty> OrphanPropertiesInB { get; set; }
    }
}
