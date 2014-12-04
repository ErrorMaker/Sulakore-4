using System;
using Sulakore.Protocol;

namespace Sulakore.Communication
{
    public class PlayerWhisperEventArgs : EventArgs, IHabboEvent
    {
        private readonly HMessage _packet;

        public ushort Header { get; private set; }

        public PlayerWhisperEventArgs(HMessage packet)
        {
            throw new NotImplementedException();

            _packet = packet;
            Header = _packet.Header;
        }
    }
}