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
        /// <summary>
        /// Gets or sets a value indicating whether the objects on either side are the same type. 
        /// For example ObjectFoo may have a property called 'Length' that is the size of the collection and ObjectBar ay have a property called 'Length' that is the human readable distance that the car has traveled and may be emitted as '53miles 32Feet'.
        /// </summary>
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
