using System;
using Sulakore.Protocol;

namespace Sulakore.Communication
{
    public class HostMutePlayerEventArgs : EventArgs, IHabboEvent
    {
        private readonly HMessage _packet;

        public ushort Header { get; private set; }

        private int? _playerId;
        public int PlayerId
        {
            get
            {
                return (int)(_playerId != null ?
                    _playerId :
                    _playerId = _packet.ReadInt(0));
            }
        }

        private int? _roomId;
        public int RoomId
        {
            get
            {
                return (int)(_roomId != null ?
                    _roomId :
                    _roomId = _packet.ReadInt(4));
            }
        }

        private int? _minutes;
        public int Minutes
        {
            get
            {
                return (int)(_minutes != null ?
                    _minutes :
                    _minutes = _packet.ReadInt(8));
            }
        }

        public HostMutePlayerEventArgs(HMessage packet)
        {
            _packet = packet;
            Header = _packet.Header;
        }
    }
}