using System;
using System.Collections.Generic;

namespace PluginTest
{
    class PluginManager : IDisposable
    {
        public delegate void PluginChanged(string fileName);

        public event PluginChanged Loaded;
        public event PluginChanged Unloaded;

        public Action<Plugin> Initializer;

        private Dictionary<string, Plugin> _plugins;
         
        public PluginManager()
        {
            _plugins = new Dictionary<string, Plugin>();
        }

        public void Dispose()
        {
            lock (_plugins)
            {
                foreach (var plugin in _plugins.Values)
                {
                    plugin.Dispose();

                    var unloaded = Unloaded;
                    if (unloaded != null)
                        unloaded(plugin.FileName);
                }

                _plugins.Clear();
            }
        }

        public void Load(string assemblyFileName)
        {
            lock (_plugins)
            {
                if (_plugins.ContainsKey(assemblyFileName))
                    return;

                var plugin = new Plugin(assemblyFileName);
                _plugins.Add(assemblyFileName, plugin);

                if (Initializer != null)
                    Initializer(plugin);

                var loaded = Loaded;
                if (loaded != null)
                    loaded(assemblyFileName);
            }
        }

        public void Unload(string assemblyFileName)
        {
            lock (_plugins)
            {
                Plugin plugin;

                if (!_plugins.TryGetValue(assemblyFileName, out plugin))
                    return;

                plugin.Dispose();
                _plugins.Remove(assemblyFileName);

                var unloaded = Unloaded;
                if (unloaded != null)
                    unloaded(assemblyFileName);
            }
        }

        public void Reload(string assemblyFileName)
        {
            Unload(assemblyFileName);
            Load(assemblyFileName);
        }

        public Plugin Get(string assemblyFileName)
        {
            lock (_plugins)
            {
                Plugin plugin;
                if (!_plugins.TryGetValue(assemblyFileName, out plugin))
                    throw new Exception(string.Format("Plugin '{0}' not loaded", assemblyFileName));

                return plugin;
            }
        }
    }
}
