using System;
using System.Linq;
using PluginCommon;

namespace PluginTest
{
    class Program
    {
        public static PluginManager PluginManager;
        public static Factory<IRoom> RoomFactory;
         
        static void Main(string[] args)
        {
            RoomFactory = new Factory<IRoom>();
            RoomFactory.Added += type => Console.WriteLine("Added IRoom '{0}'", type);
            RoomFactory.Removed += type => Console.WriteLine("Removed IRoom '{0}'", type);

            RoomFactory.Discover();

            PluginManager = new PluginManager();
            PluginManager.Initializer = plugin => plugin.Discover(RoomFactory);
            PluginManager.Loaded += name => Console.WriteLine("Loaded '{0}'", name);
            PluginManager.Unloaded += name => Console.WriteLine("Unloaded '{0}'", name);

            PluginManager.Load("PluginA.dll");

            var a = new PluginRoom("PluginA.TestRoom");
            a.Send("11111111");

            PluginManager.Unload("PluginA.dll");

            a.Send("22222222");

            Console.WriteLine("Replace PluginA.dll");
            Console.ReadLine();

            PluginManager.Load("PluginA.dll");

            a.Send("33333333");
            
            a.Dispose();

            PluginManager.Unload("PluginA.dll");

            Console.WriteLine(string.Join("\n", AppDomain.CurrentDomain.GetAssemblies().Select(assembly =>
            {
                if (assembly == null)
                    return "ERROR: assembly is null";
                if (assembly.FullName == null)
                    return "ERROR: Assembly.FullName is null";
                return assembly.FullName.Substring(0, 40);
            })));
        }
    }
}
