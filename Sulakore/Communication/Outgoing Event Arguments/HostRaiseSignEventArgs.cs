using System;
using Sulakore.Habbo;
using Sulakore.Protocol;

namespace Sulakore.Communication
{
    public class HostRaiseSignEventArgs : EventArgs, IHabboEvent
    {
        private readonly HMessage _packet;

        public ushort Header { get; private set; }

        public HSigns Sign { get; private set; }

        public HostRaiseSignEventArgs(HMessage packet)
        {
            _packet = packet;
            Header = _packet.Header;

            Sign = (HSigns)_packet.ReadInt(0);
        }

        public override string ToString()
        {
            return string.Format("Header: {0}, Sign: {1}",
                Header, Sign);
        }
    }
}