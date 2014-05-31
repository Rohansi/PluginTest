using System;
using PluginCommon;

namespace PluginTest
{
    class Room : MarshalByRefObject, IRoom
    {
        public virtual void Send(string message)
        {
            Console.WriteLine("Normal Room: " + message);
        }

        public void Dispose()
        {
            
        }
    }
}
