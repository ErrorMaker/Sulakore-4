using Sulakore.Habbo;
using Sulakore.Protocol;
using System;

namespace Sulakore.Communication
{
    public class HostClickPlayerEventArgs : EventArgs, IHabboEvent
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

        private HPoint _tile;
        public HPoint Tile
        {
            get
            {
                return _tile != HPoint.Empty ?
                    _tile :
                    _tile = new HPoint(_packet.ReadInt(0), _packet.ReadInt(4));
            }
        }

        public HostClickPlayerEventArgs(HMessage packet)
        {
            _packet = packet;
            Header = _packet.Header;
        }
    }
}