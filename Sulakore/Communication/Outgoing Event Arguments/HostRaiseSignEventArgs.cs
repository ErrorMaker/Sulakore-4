using System;
using Sulakore.Habbo;
using Sulakore.Protocol;

namespace Sulakore.Communication
{
    public class HostRaiseSignEventArgs : EventArgs, IHabboEvent
    {
        private readonly HMessage _packet;

        public ushort Header { get; private set; }

        private HSigns? _sign;
        public HSigns Sign
        {
            get
            {
                return (HSigns)(_sign != null ?
                    _sign :
                    _sign = (HSigns)_packet.ReadInt(0));
            }
        }

        public HostRaiseSignEventArgs(HMessage packet)
        {
            _packet = packet;
            Header = _packet.Header;
        }

        public override string ToString()
        {
            return string.Format("Header: {0}, Sign: {1}", Header, Sign);
        }
    }
}