using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Homework.ProblemA
{
    /// <summary>
    /// A contract to show two different values of the same property.
    /// </summary>
    public class ObjectPropertyPair : ObjectPropertyBase
    { 
        public bool IsUnderlyingTypeMatch { get; set; }
        /// <summary>
        /// The value of <see cref="ObjectPropertyBase.PropertyName"/> in object A.
        /// </summary>
        public string A { get; set; }

        /// <summary>
        /// The value of <see cref="ObjectPropertyBase.PropertyName"/> in object B.
        /// </summary>
        public string B { get; set; }
    }
}
