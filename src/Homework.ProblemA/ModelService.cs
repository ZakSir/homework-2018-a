using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

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
                HashAlgorithm tha;
                switch (hashAlgorithmName.ToUpperInvariant())
                {
                    case nameof(MD5):
                        tha = MD5.Create();
                        break;
                    case nameof(KeyedHashAlgorithm):
                        tha = KeyedHashAlgorithm.Create();
                        break;
                    case nameof(SHA1):
                        tha = SHA1.Create();
                        break;
                    case nameof(SHA256):
                        tha = SHA256.Create();
                        break;
                    case nameof(SHA384):
                        tha = SHA384.Create();
                        break;
                    case nameof(SHA512):
                        tha = SHA512.Create();
                        break;
                    default:
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
                // Warning: Ambiguity - This will serialize the public face of the object. And can be controlled if a JsonObject attributed object is passed.
                string serialized = JsonConvert.SerializeObject(obj);

                using (StreamWriter sw = new StreamWriter(ms))
                {
                    sw.Write(serialized);

                    ms.Position = 0;

                    hash = ha.ComputeHash(ms);
                }
            }

            string stringhash = System.Convert.ToBase64String(hash);

            return stringhash;
        }

        public ObjectDifferential GetDifferential(object a, object b)
        {
            Indexed it1 = new Indexed(a);
            Indexed it2 = new Indexed(b);

            Type it1t = a.GetType();
            Type it2t = b.GetType();

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
                object aval = null;
                object bval = null;

                aval = pair.Value;

                if (it2Flat.TryGetValue(pair.Key, out object it2Value))
                {
                    bval = it2Value;

                    if(aval == null && bval == null){
                        ObjectProperty op = new ObjectProperty()
                        {
                            PropertyName = pair.Key,
                            Value = $"BOTH OBJECTS NULL"
                        };

                        matching.Add(op);

                        it2SkipNames.Add(pair.Key);

                        continue;
                    }

                    // one of the objects, but not both are null
                    // last condition would have caught both
                    if (aval == null || bval == null)
                    {
                        // automatic mismatch - dont check equality
                        ObjectPropertyPair opp = new ObjectPropertyPair()
                        {
                            PropertyName = pair.Key,
                            IsUnderlyingTypeMatch = false,
                            A = aval == null ? $"{nameof(aval)} == null" : aval.ToString(),
                            B = bval == null ? $"{nameof(bval)} == null" : bval.ToString()
                        };

                        mismatching.Add(opp);
                        it2SkipNames.Add(pair.Key);

                        continue;
                    }

                    if(object.ReferenceEquals(aval, bval))
                    {
                        ObjectProperty op = new ObjectProperty()
                        {
                            PropertyName = pair.Key,
                            Value = $"Same Object ('{aval.ToString()}')"
                        };

                        matching.Add(op);

                        it2SkipNames.Add(pair.Key);

                        continue;
                    }

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
                            A = aval.ToString(),
                            B = bval.ToString()
                        };

                        mismatching.Add(opp);
                        it2SkipNames.Add(pair.Key);

                        continue;
                    }



                    // we know the type is identical, so we can use just o1type
                    // we have to check equality
                    if(o1type != typeof(string) && // special case as a System.String is actually an Array of char (woo c)
                       typeof(IEnumerable).IsAssignableFrom(o1type))
                    {
                        bool isEqual = true;

                        // we know we are working with a collection

                        Array arrayOfA = (Array)aval;
                        Array arrayOfB = (Array)bval;


                        if(arrayOfA.Length != arrayOfB.Length)
                        {
                            // the arrays are different lengths, 
                            // which by default means the value of the collection will be different.

                            isEqual = false;
                        }
                        else
                        {
                            bool foundFault = false;

                            // arrays are the same length
                            int length = arrayOfA.Length;

                            for (int i = 0; i < length; i++)
                            {
                                object internalAValue = arrayOfA.GetValue(i);
                                object internalBValue = arrayOfB.GetValue(i);

                                if(internalAValue != internalBValue)
                                {
                                    foundFault = true;
                                    break;
                                }
                            }

                            if(foundFault){
                                isEqual = false;
                            }
                            else 
                            {
                                isEqual = true;
                            }
                        }

                        if(isEqual)
                        {
                            ObjectProperty op = new ObjectProperty()
                            {
                                PropertyName = pair.Key,
                                Value = JsonConvert.SerializeObject(aval)
                            };

                            matching.Add(op);
                        }
                        else
                        {
                            ObjectPropertyPair opp = new ObjectPropertyPair()
                            {
                                PropertyName = pair.Key,
                                IsUnderlyingTypeMatch = true,
                                A = JsonConvert.SerializeObject(aval),
                                B = JsonConvert.SerializeObject(bval)
                            };

                            mismatching.Add(opp);
                        }

                        continue;
                    }

                    // the objects are the same type, try to use the default equality operator to invoke equality operations
                    if (!aval.Equals(bval))
                    {
                        ObjectPropertyPair opp = new ObjectPropertyPair()
                        {
                            PropertyName = pair.Key,
                            IsUnderlyingTypeMatch = true,
                            A = aval.ToString(),
                            B = bval.ToString()
                        };

                        mismatching.Add(opp);
                    }
                    else
                    {
                        ObjectProperty op = new ObjectProperty()
                        {
                            PropertyName = pair.Key,
                            Value = aval.ToString()
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
                        Value = aval == null ? "Value in A is NULL" : aval.ToString()
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
                else
                {
                    bval = "Value in B is Null";
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

        public IEnumerable<ObjectProperty> GetMatchingPropertyNames(object a, object b)
        {
            return this.GetDifferential(a, b).MatchingProperties;
        }
    }
}
