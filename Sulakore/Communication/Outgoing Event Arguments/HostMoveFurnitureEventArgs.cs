using System;
using Sulakore.Habbo;
using Sulakore.Protocol;

namespace Sulakore.Communication
{
    public class HostMoveFurnitureEventArgs : EventArgs, IHabboEvent
    {
        private readonly HMessage _packet;

        public ushort Header { get; private set; }

        private int? _furnitureId;
        public int FurnitureId
        {
            get
            {
                return (int)(_furnitureId != null ?
                    _furnitureId :
                    _furnitureId = _packet.ReadInt(0));
            }
        }

        private HPoint _tile;
        public HPoint Tile
        {
            get
            {
                return _tile != HPoint.Empty ?
                    _tile :
                    _tile = new HPoint(_packet.ReadInt(4), _packet.ReadInt(8));
            }
        }

        private HDirections? _direction;
        public HDirections Direction
        {
            get
            {
                return (HDirections)(_direction != null ?
                    _direction :
                    _direction = (HDirections)_packet.ReadInt(12));
            }
        }

        public HostMoveFurnitureEventArgs(HMessage packet)
        {
            _packet = packet;
            Header = _packet.Header;
        }
    }
}