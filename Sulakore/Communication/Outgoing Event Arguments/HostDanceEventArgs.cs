using Sulakore.Habbo;
using Sulakore.Protocol;
using System;

namespace Sulakore.Communication
{
    public class HostDanceEventArgs : EventArgs, IHabboEvent
    {
        private readonly HMessage _packet;

        public ushort Header { get; private set; }

        private HDances? _dance;
        public HDances Dance
        {
            get
            {
                return (HDances)(_dance != null ?
                    _dance :
                    _dance = (HDances)_packet.ReadInt(0));
            }
        }

        public HostDanceEventArgs(HMessage packet)
        {
            _packet = packet;
            Header = _packet.Header;
        }

        public override string ToString()
        {
            return string.Format("Header: {0}, Dance: {1}", Header, Dance);
        }
    }
}