using PluginCommon;

namespace PluginTest
{
    class PluginRoom : IRoom
    {
        private readonly string _roomType;
        private Room _parent;
        private IRoom _room;

        public PluginRoom(string type)
        {
            _roomType = type;
            _parent = new Room();
            _room = Program.RoomFactory.Create(_roomType, _parent);

            Program.RoomFactory.Added += TypeAdded;
            Program.RoomFactory.Removed += TypeRemoved;
        }

        public void Dispose()
        {
            if (_room != null)
            {
                _room.Dispose();
                _room = null;
            }

            Program.RoomFactory.Added -= TypeAdded;
            Program.RoomFactory.Removed -= TypeRemoved;
        }

        private void TypeAdded(string type)
        {
            if (type == _roomType)
                _room = Program.RoomFactory.Create(_roomType, _parent);
        }

        private void TypeRemoved(string type)
        {
            if (type == _roomType && _room != null)
            {
                _room.Dispose();
                _room = null;
            }
        }

        public void Send(string message)
        {
            if (_room != null)
                _room.Send(message);
            else
                _parent.Send(message);
        }
    }
}
