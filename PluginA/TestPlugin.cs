using System;
using PluginCommon;

namespace PluginA
{
    public class TestRoom : PluginRoomBase
    {
        public TestRoom(IRoom parent) : base(parent)
        {
            
        }

        public override void Send(string message)
        {
            Console.WriteLine("Plugin Room: " + message);
            //Parent.Send(message);
        }

        public override void Dispose()
        {
            
        }
    }
}
