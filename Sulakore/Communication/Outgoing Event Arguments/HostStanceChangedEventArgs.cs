using System;
using Sulakore.Habbo;
using Sulakore.Protocol;

namespace Sulakore.Communication
{
    public class HostStanceChangedEventArgs : EventArgs, IHabboEvent
    {
        private readonly HMessage _packet;

        public ushort Header { get; private set; }

        private HStances? _stance;
        public HStances Stance
        {
            get
            {
                return (HStances)(_stance != null ?
                    _stance :
                    _stance = (HStances)_packet.ReadInt(0));
            }
        }

        public HostStanceChangedEventArgs(HMessage packet)
        {
            _packet = packet;
            Header = _packet.Header;
        }

        public override string ToString()
        {
            return string.Format("Header: {0}, Stance: {1}", Header, Stance);
        }
    }
}