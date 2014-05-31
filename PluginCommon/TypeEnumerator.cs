using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace PluginCommon
{
    public class TypeEnumerator : MarshalByRefObject
    {
        public List<string> Enumerate<T>(string assemblyFile)
        {
            var assembly = Assembly.LoadFrom(assemblyFile);
            return assembly.DefinedTypes
                           .Where(type => typeof(T).IsAssignableFrom(type) && !type.IsAbstract)
                           .Select(type => type.FullName)
                           .ToList();
        }
    }
}
