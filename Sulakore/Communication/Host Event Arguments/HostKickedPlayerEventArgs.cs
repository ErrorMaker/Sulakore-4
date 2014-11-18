using System;
using Sulakore.Protocol;

namespace Sulakore.Communication
{
    public class HostKickedPlayerEventArgs : EventArgs, IHabboEvent
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

        public HostKickedPlayerEventArgs(HMessage packet)
        {
            _packet = packet;
            Header = _packet.Header;
        }
    }
}