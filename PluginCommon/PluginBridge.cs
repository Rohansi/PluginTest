using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace PluginCommon
{
    public class PluginBridge : MarshalByRefObject
    {
        private Assembly _assembly;

        public void LoadAssembly(string assemblyFile)
        {
            if (_assembly != null)
                throw new NotSupportedException("PluginBridge: Loaded multiple times");

            var assemblyBytes = File.ReadAllBytes(assemblyFile);
            _assembly = Assembly.Load(assemblyBytes);
        }

        public List<string> EnumerateTypes<T>()
        {
            return _assembly.DefinedTypes
                            .Where(type => typeof(T).IsAssignableFrom(type) && !type.IsAbstract)
                            .Select(type => type.FullName)
                            .ToList();
        }

        public T Create<T>(string assemblyFile, string typeName, params object[] args) where T : class
        {
            var type = _assembly.GetType(typeName);
            return (T)Activator.CreateInstance(type, args);
        }
    }
}
