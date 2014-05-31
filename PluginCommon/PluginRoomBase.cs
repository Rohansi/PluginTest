using System;

namespace PluginCommon
{
    public abstract class PluginRoomBase : MarshalByRefObject, IRoom
    {
        public IRoom Base { get; private set; }

        protected PluginRoomBase(IRoom @base)
        {
            Base = @base;
        }

        public abstract void Send(string message);

        public abstract void Dispose();
    }
}
