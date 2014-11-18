using System;
using Sulakore.Protocol;

namespace Sulakore.Communication
{
    public class PlayerKickedHostEventArgs : EventArgs, IHabboEvent
    {
        private readonly HMessage _packet;

        public ushort Header { get; private set; }

        public PlayerKickedHostEventArgs(HMessage packet)
        {
            _packet = packet;
            Header = _packet.Header;
        }
    }
}