using System;

namespace PluginCommon
{
    public interface IRoom : IDisposable
    {
        void Send(string message);
    }
}
