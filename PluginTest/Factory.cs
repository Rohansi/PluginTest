using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Reflection;

namespace PluginTest
{
    public class Factory<T> where T : class
    {
        public delegate void FactoryChanged(string type);

        public event FactoryChanged Added;
        public event FactoryChanged Removed;

        private ConcurrentDictionary<string, Func<object[], T>> _factories;

        public Factory()
        {
            _factories = new ConcurrentDictionary<string, Func<object[], T>>();
        }

        public void Add(string type, Func<object[], T> factory)
        {
            if (_factories.TryAdd(type, factory))
            {
                var added = Added;
                if (added != null)
                    Added(type);
            }
        }

        public void Remove(string type)
        {
            Func<object[], T> value;
            if (_factories.TryRemove(type, out value))
            {
                var removed = Removed;
                if (removed != null)
                    removed(type);
            }
        }

        public T Create(string type, params object[] args)
        {
            Func<object[], T> value;

            if (!_factories.TryGetValue(type, out value))
                return null;

            try
            {
                return value(args);
            }
            catch (Exception e)
            {
                Console.WriteLine(e); // TODO log
                return null;
            }
        }

        public void Discover()
        {
            var assembly = Assembly.GetCallingAssembly();
            var types = assembly.DefinedTypes.Where(type => typeof(T).IsAssignableFrom(type) && !type.IsAbstract);

            foreach (var type in types)
            {
                var typeCopy = type;
                Add(type.FullName, args => (T)Activator.CreateInstance(typeCopy, args));
            }
        }
    }
}
