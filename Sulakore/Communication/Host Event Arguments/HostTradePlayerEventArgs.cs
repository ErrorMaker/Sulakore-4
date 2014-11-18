using System;
using Sulakore.Protocol;

namespace Sulakore.Communication
{
    public class HostTradePlayerEventArgs : EventArgs, IHabboEvent
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

        public HostTradePlayerEventArgs(HMessage packet)
        {
            _packet = packet;
            Header = _packet.Header;
        }
    }
}