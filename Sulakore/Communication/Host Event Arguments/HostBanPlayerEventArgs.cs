using System;
using Sulakore.Habbo;
using Sulakore.Protocol;

namespace Sulakore.Communication
{
    public class HostBanPlayerEventArgs : EventArgs, IHabboEvent
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

        private HBans? _ban;
        public HBans Ban
        {
            get
            {
                return (HBans)(_ban != null ?
                    _ban :
                    _ban = SKore.ConvertToHBan(_packet.ReadString(8)));
            }
        }

        public HostBanPlayerEventArgs(HMessage packet)
        {
            _packet = packet;
            Header = _packet.Header;
        }
    }
}