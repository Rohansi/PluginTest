using System;
using System.Collections.Generic;
using PluginCommon;

namespace PluginTest
{
    class Plugin
    {
        public string FileName { get; private set; }

        private AppDomain _appDomain;
        private TypeEnumerator _enumerator;
        private Dictionary<dynamic, List<string>> _registeredTypes; 

        public Plugin(string assemblyFileName)
        {
            FileName = assemblyFileName;

            try
            {
                _appDomain = AppDomain.CreateDomain("plugin_" + FileName);

                // cant handle unhandled exceptions, will end up crashing
                _appDomain.UnhandledException += (sender, args) =>
                {
                    var exception = (Exception)args.ExceptionObject;
                    Console.WriteLine(exception); // TODO: log
                };

                var enumeratorType = typeof(TypeEnumerator);
                _enumerator = (TypeEnumerator)_appDomain.CreateInstanceFromAndUnwrap(enumeratorType.Assembly.Location, enumeratorType.FullName);

                _registeredTypes = new Dictionary<dynamic, List<string>>();
            }
            catch
            {
                Dispose();
                throw;
            }
        }

        public void Dispose()
        {
            _enumerator = null;

            lock (_registeredTypes)
            {
                foreach (var kv in _registeredTypes)
                {
                    foreach (var type in kv.Value)
                    {
                        kv.Key.Remove(type);
                    }
                }

                _registeredTypes.Clear();
            }

            if (_appDomain != null)
            {
                AppDomain.Unload(_appDomain);
                _appDomain = null;
            }
        }

        public void Discover<T>(Factory<T> factory) where T : class
        {
            lock (_registeredTypes)
            {
                var types = new List<string>();

                foreach (var type in _enumerator.Enumerate<T>(FileName))
                {
                    if (string.IsNullOrWhiteSpace(type))
                    {
                        Console.WriteLine("ERROR: Attempt to add invalid type");
                        continue;
                    }

                    var typeCopy = type;
                    factory.Add(type, args => (T)_appDomain.CreateInstanceFromAndUnwrap(FileName, typeCopy, false, 0, null, args, null, null));

                    types.Add(type);
                }

                _registeredTypes.Add(factory, types);
            }
        }
    }
}
