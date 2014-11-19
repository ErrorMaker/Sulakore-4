using System;
using Sulakore.Protocol;

namespace Sulakore.Communication
{
    public class HostMottoChangedEventArgs : EventArgs, IHabboEvent
    {
        private readonly HMessage _packet;

        public ushort Header { get; private set; }

        private string _motto;
        public string Motto
        {
            get
            {
                return !string.IsNullOrEmpty(_motto) ?
                    _motto :
                    _motto = _packet.ReadString(0);
            }
        }

        public HostMottoChangedEventArgs(HMessage packet)
        {
            _packet = packet;
            Header = _packet.Header;
        }
    }
}