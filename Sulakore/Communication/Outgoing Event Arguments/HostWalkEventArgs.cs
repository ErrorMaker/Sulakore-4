using System;
using Sulakore.Habbo;
using Sulakore.Protocol;

namespace Sulakore.Communication
{
    public class HostWalkEventArgs : EventArgs, IHabboEvent
    {
        private readonly HMessage _packet;

        public ushort Header { get; private set; }

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

        public HostWalkEventArgs(HMessage packet)
        {
            _packet = packet;
            Header = _packet.Header;
        }
    }
}