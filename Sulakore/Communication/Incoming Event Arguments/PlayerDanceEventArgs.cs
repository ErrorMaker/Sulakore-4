using System;
using Sulakore.Habbo;
using Sulakore.Protocol;

namespace Sulakore.Communication
{
    public class PlayerDanceEventArgs : EventArgs, IHabboEvent
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
                    _playerIndex = _packet.ReadInt(0));
            }
        }

        private HDances? _dance;
        public HDances Dance
        {
            get
            {
                return (HDances)(_dance != null ?
                    _dance :
                    _dance = (HDances)_packet.ReadInt(4));
            }
        }

        public PlayerDanceEventArgs(HMessage packet)
        {
            _packet = packet;
            Header = _packet.Header;
        }
    }
}