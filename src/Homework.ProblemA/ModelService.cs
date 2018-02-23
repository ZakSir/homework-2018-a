using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Homework.ProblemA
{
    public class ModelService : IModelService
    {
        // some assumptions made here about caching
        private static readonly Dictionary<string, HashAlgorithm> hashAlgorithms = new Dictionary<string, HashAlgorithm>();

        private static readonly object haloc = new object();

        public string GetCryptographicHashCode(object obj, string hashAlgorithmName)
        {
            if (!hashAlgorithms.TryGetValue(hashAlgorithmName, out HashAlgorithm ha))
            {
                HashAlgorithm tha = HashAlgorithm.Create(hashAlgorithmName);

                if (tha == null)
                {
                    throw new InvalidOperationException($"The hash algorithm '{hashAlgorithmName}' is invalid. ");
                }

                ha = tha;

                lock (haloc)
                {
                    hashAlgorithms[hashAlgorithmName] = ha;
                }
            }

            byte[] hash;
            using (MemoryStream ms = new MemoryStream())
            {
                BinaryFormatter bf = new BinaryFormatter();

                bf.Serialize(ms, obj);

                ms.Position = 0;

                hash = ha.ComputeHash(ms);
            }

            string stringhash = System.Convert.ToBase64String(hash);

            return stringhash;
        }

        public ObjectDifferential GetDifferential<T1, T2>(T1 a, T2 b)
            where T1 : class
            where T2 : class
        {
            Indexed<T1> it1 = new Indexed<T1>(a);
            Indexed<T2> it2 = new Indexed<T2>(b);

            Type it1t = typeof(T1);
            Type it2t = typeof(T2);

            bool indexMatch = it1t == it2t;

            Dictionary<string, object> it1Flat = it1.ToFlatDictionary();
            Dictionary<string, object> it2Flat = it2.ToFlatDictionary();

            HashSet<string> it2SkipNames = new HashSet<string>();

            List<ObjectProperty> matching = new List<ObjectProperty>();
            List<ObjectProperty> onlyInA = new List<ObjectProperty>();
            List<ObjectProperty> onlyInB = new List<ObjectProperty>();
            List<ObjectPropertyPair> mismatching = new List<ObjectPropertyPair>();

            foreach (KeyValuePair<string, object> pair in it1Flat)
            {
                string aval = null;
                string bval = null;

                if (pair.Value != null)
                {
                    aval = pair.Value.ToString();
                }

                if (it2Flat.TryGetValue(pair.Key, out object it2Value))
                {
                    if (it2Value != null)
                    {
                        bval = it2Value.ToString();
                    }


                    if (!indexMatch)
                    {
                        // the key gets something back 
                        Type o1type = pair.Value.GetType();
                        Type o2type = it2Value.GetType();

                        if (o1type != o2type)
                        {
                            // automatic mismatch - dont check equality
                            ObjectPropertyPair opp = new ObjectPropertyPair()
                            {
                                PropertyName = pair.Key,
                                IsUnderlyingTypeMatch = false,
                                A = aval,
                                B = bval
                            };

                            mismatching.Add(opp);

                            continue;
                        }
                    }

                    if (aval != bval)
                    {
                        ObjectPropertyPair opp = new ObjectPropertyPair()
                        {
                            PropertyName = pair.Key,
                            IsUnderlyingTypeMatch = true,
                            A = aval,
                            B = bval
                        };

                        mismatching.Add(opp);
                    }
                    else
                    {
                        ObjectProperty op = new ObjectProperty()
                        {
                            PropertyName = pair.Key,
                            Value = aval
                        };

                        matching.Add(op);
                    }
                }
                else
                {
                    // property does not exist in b.
                    ObjectProperty op = new ObjectProperty()
                    {
                        PropertyName = pair.Key,
                        Value = aval
                    };

                    onlyInA.Add(op);
                }

                it2SkipNames.Add(pair.Key);
            }

            foreach (KeyValuePair<string, object> pair in it2Flat)
            {
                string bval = null;

                if (it2SkipNames.Contains(pair.Key))
                {
                    continue;
                }

                if (pair.Value != null)
                {
                    bval = pair.Value.ToString();
                }

                // property does not exist in b.
                ObjectProperty op = new ObjectProperty()
                {
                    PropertyName = pair.Key,
                    Value = bval
                };

                onlyInB.Add(op);
            }

            ObjectDifferential od = new ObjectDifferential()
            {
                FullTypeNameA = it1t.FullName,
                FullTypeNameB = it2t.FullName,
                IsTypeMatch = indexMatch,
                MatchingProperties = matching,
                MismatchingProperties = mismatching,
                OrphanPropertiesInA = onlyInA,
                OrphanPropertiesInB = onlyInB
            };

            return od;
        }

        public IEnumerable<string> GetMatchingPropertyNames<T1, T2>(T1 a, T2 b)
            where T1 : class
            where T2 : class
        {
            Indexed<T1> it1 = new Indexed<T1>(a);
            Indexed<T2> it2 = new Indexed<T2>(b);

            Type it1t = typeof(T1);
            Type it2t = typeof(T2);

            bool indexMatch = it1t == it2t;

            Dictionary<string, object> it1Flat = it1.ToFlatDictionary();
            Dictionary<string, object> it2Flat = it2.ToFlatDictionary();

            IEnumerable<string> result = it1Flat
                .Where(_a => it2Flat.ContainsKey(_a.Key)).Select(_ => _.Key)
                .ToArray(); // execute enumeration

            return result;
        }
    }
}
