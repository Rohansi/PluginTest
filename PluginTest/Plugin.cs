using System;
using System.Collections.Generic;
using PluginCommon;

namespace PluginTest
{
    class Plugin
    {
        public string FileName { get; private set; }

        private AppDomain _appDomain;
        private PluginBridge _bridge;
        private Dictionary<dynamic, List<string>> _registeredTypes; 

        public Plugin(string assemblyFileName)
        {
            FileName = assemblyFileName;

            _registeredTypes = new Dictionary<dynamic, List<string>>();

            try
            {
                _appDomain = AppDomain.CreateDomain("plugin_" + FileName);

                // cant handle unhandled exceptions, will end up crashing
                _appDomain.UnhandledException += (sender, args) =>
                {
                    var exception = (Exception)args.ExceptionObject;
                    Console.WriteLine(exception); // TODO: log
                };

                var bridgeType = typeof(PluginBridge);
                _bridge = (PluginBridge)_appDomain.CreateInstanceFromAndUnwrap(bridgeType.Assembly.Location, bridgeType.FullName);
                _bridge.LoadAssembly(FileName);
            }
            catch
            {
                Dispose();
                throw;
            }
        }

        public void Dispose()
        {
            _bridge = null;

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

                foreach (var type in _bridge.EnumerateTypes<T>())
                {
                    if (string.IsNullOrWhiteSpace(type))
                    {
                        Console.WriteLine("ERROR: Attempt to add invalid type");
                        continue;
                    }
                    
                    var typeCopy = type;
                    factory.Add(type, args => _bridge.Create<T>(FileName, typeCopy, args));

                    types.Add(type);
                }

                _registeredTypes.Add(factory, types);
            }
        }
    }
}
