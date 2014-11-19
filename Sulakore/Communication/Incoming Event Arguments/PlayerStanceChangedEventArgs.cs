using System;
using Sulakore.Habbo;
using Sulakore.Protocol;

namespace Sulakore.Communication
{
    public class PlayerStanceChangedEventArgs : EventArgs, IHabboEvent
    {
        private readonly HMessage _packet;

        public ushort Header { get; private set; }

        private int? _playerIndex;
        public int PlayerIndex
        {
            get
            {
                return (int)(_playerIndex != null ?
                    _playerIndex :
                    _playerIndex = _packet.ReadInt(4));
            }
        }

        private HPoint _tile;
        public HPoint Tile
        {
            get
            {
                return _tile != HPoint.Empty ?
                    _tile :
                    _tile = new HPoint(_packet.ReadInt(8), _packet.ReadInt(12), _packet.ReadString(16));
            }
        }

        private HDirections? _headDirection;
        public HDirections HeadDirection
        {
            get
            {
                return (HDirections)(_headDirection != null ?
                    _headDirection :
                    _headDirection = (HDirections)_packet.ReadInt(18 + Tile.Z.Length));
            }
        }

        private HDirections? _bodyDirection;
        public HDirections BodyDirection
        {
            get
            {
                return (HDirections)(_bodyDirection != null ?
                    _bodyDirection :
                    _bodyDirection = (HDirections)_packet.ReadInt(22 + Tile.Z.Length));
            }
        }

        private HStances? _stance;
        public HStances Stance
        {
            get
            {
                if (_stance != null) return (HStances)_stance;

                string action = _packet.ReadString(26 + Tile.Z.Length);
                _empowered = action.Contains("flatctrl");

                return (HStances)(_stance = action.Contains("/sit") ? HStances.Sit : HStances.Stand);
            }
        }

        private bool? _empowered;
        public bool Empowered
        {
            get
            {
                if (_empowered != null) return (bool)_empowered;
                string action = _packet.ReadString(26 + Tile.Z.Length);
                _stance = action.Contains("/sit") ? HStances.Sit : HStances.Stand;

                return (bool)(_empowered = action.Contains("flatctrl"));
            }
        }

        public PlayerStanceChangedEventArgs(HMessage packet)
        {
            _packet = packet;
            Header = _packet.Header;
        }
    }
}