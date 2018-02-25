using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Homework.ProblemA
{
    public interface IModelService
    {
        ObjectDifferential GetDifferential(object a, object b);

        IEnumerable<string> GetMatchingPropertyNames(object a, object b);

        string GetCryptographicHashCode(object obj, string hashAlgorithmName);
    }
}
