using System;
using Sulakore.Protocol;

namespace Sulakore.Communication
{
    public class HostRoomNavigateEventArgs : EventArgs, IHabboEvent
    {
        private readonly HMessage _packet;

        public ushort Header { get; private set; }

        private int? _roomId;
        public int RoomId
        {
            get
            {
                return (int)(_roomId != null ?
                    _roomId :
                    _roomId = _packet.ReadInt(0));
            }
        }

        private string _password;
        public string Password
        {
            get
            {
                return _password != null ?
                    _password :
                    _password = _packet.ReadString(4);
            }
        }

        public HostRoomNavigateEventArgs(HMessage packet)
        {
            _packet = packet;
            Header = _packet.Header;
        }
    }
}