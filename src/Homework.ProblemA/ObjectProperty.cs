using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Homework.ProblemA
{
    /// <summary>
    /// A contract for a property (or set of properties) that have the same value.
    /// </summary>
    public class ObjectProperty : ObjectPropertyBase
    {
        /// <summary>
        /// Gets or sets the value of the object.
        /// </summary>
        public string Value;
    }
}
