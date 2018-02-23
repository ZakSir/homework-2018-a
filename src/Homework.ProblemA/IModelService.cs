using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Homework.ProblemA
{
    public interface IModelService
    {
        ObjectDifferential GetDifferential<T1, T2>(T1 a, T2 b)
            where T1 : class
            where T2 : class;

        IEnumerable<string> GetMatchingPropertyNames<T1, T2>(T1 a, T2 b)
            where T1 : class
            where T2 : class;

        string GetCryptographicHashCode(object obj, string hashAlgorithmName);
    }
}
