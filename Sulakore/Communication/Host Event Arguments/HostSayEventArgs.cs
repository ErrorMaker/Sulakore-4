using System;
using Sulakore.Habbo;
using Sulakore.Protocol;

namespace Sulakore.Communication
{
    public class HostSayEventArgs : EventArgs, IHabboEvent
    {
        private readonly HMessage _packet;

        public ushort Header { get; private set; }

        private string _message;
        public string Message
        {
            get
            {
                return !string.IsNullOrEmpty(_message) ?
                    _message :
                    _message = _packet.ReadString(0);
            }
        }

        private HThemes? _theme;
        public HThemes Theme
        {
            get
            {
                return (HThemes)(_theme != null ?
                    _theme :
                    _theme = (HThemes)_packet.ReadInt(_packet.Length - 10));
            }
        }

        public HostSayEventArgs(HMessage packet)
        {
            _packet = packet;
            Header = _packet.Header;
        }
    }
}