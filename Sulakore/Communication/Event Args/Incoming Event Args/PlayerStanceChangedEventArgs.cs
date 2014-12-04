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

        private HDirection? _headDirection;
        public HDirection HeadDirection
        {
            get
            {
                return (HDirection)(_headDirection != null ?
                    _headDirection :
                    _headDirection = (HDirection)_packet.ReadInt(18 + Tile.Z.Length));
            }
        }

        private HDirection? _bodyDirection;
        public HDirection BodyDirection
        {
            get
            {
                return (HDirection)(_bodyDirection != null ?
                    _bodyDirection :
                    _bodyDirection = (HDirection)_packet.ReadInt(22 + Tile.Z.Length));
            }
        }

        private HStance? _stance;
        public HStance Stance
        {
            get
            {
                if (_stance != null) return (HStance)_stance;

                string action = _packet.ReadString(26 + Tile.Z.Length);
                _empowered = action.Contains("flatctrl");

                return (HStance)(_stance = action.Contains("/sit") ? HStance.Sit : HStance.Stand);
            }
        }

        private bool? _empowered;
        public bool Empowered
        {
            get
            {
                if (_empowered != null) return (bool)_empowered;
                string action = _packet.ReadString(26 + Tile.Z.Length);
                _stance = action.Contains("/sit") ? HStance.Sit : HStance.Stand;

                return (bool)(_empowered = action.Contains("flatctrl"));
            }
        }

        public PlayerStanceChangedEventArgs(HMessage packet)
        {
            throw new NotImplementedException();

            _packet = packet;
            Header = _packet.Header;
        }
    }
}