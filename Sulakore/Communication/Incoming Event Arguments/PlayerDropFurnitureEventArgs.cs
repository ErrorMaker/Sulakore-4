using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using Sulakore.Protocol;
using Sulakore.Habbo;

namespace Sulakore.Communication
{
    public class PlayerDropFurnitureEventArgs : EventArgs, IHabboEvent
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

        private int? _furnitureTypeId;
        public int FurnitureTypeId
        {
            get
            {
                return (int)(_furnitureTypeId != null ?
                    _furnitureTypeId :
                    _furnitureTypeId = _packet.ReadInt(4));
            }
        }

        private HPoint _tile;
        public HPoint Tile
        {
            get
            {
                return _tile != HPoint.Empty ?
                    _tile :
                    _tile = new HPoint(_packet.ReadInt(8), _packet.ReadInt(12), _packet.ReadString(20));
            }
        }

        private HDirections? _direction;
        public HDirections Direction
        {
            get
            {
                return (HDirections)(_direction != null ?
                    _direction :
                    _direction = (HDirections)_packet.ReadInt(16));
            }
        }

        private bool? _isRented;
        public bool IsRented
        {
            get
            {
                if (_isRented != null) return (bool)_isRented;
                return false;
            }
        }

        private int? _furnitureOwnerId;
        public int FurnitureOwnerId;

        private string _furnitureOwnerName;
        public string FurnitureOwnerName;

        public PlayerDropFurnitureEventArgs(HMessage packet)
        {
            _packet = packet;
            Header = _packet.Header;
        }
    }
}