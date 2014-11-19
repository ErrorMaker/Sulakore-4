using System;
using Sulakore.Habbo;
using Sulakore.Protocol;

namespace Sulakore.Communication
{
    public class HostGestureEventArgs : EventArgs, IHabboEvent
    {
        private readonly HMessage _packet;

        public ushort Header { get; private set; }

        private HGestures? _gesture;
        public HGestures Gesture
        {
            get
            {
                return (HGestures)(_gesture != null ?
                    _gesture :
                    _gesture = (HGestures)_packet.ReadInt(0));
            }
        }

        public HostGestureEventArgs(HMessage packet)
        {
            _packet = packet;
            Header = _packet.Header;
        }

        public override string ToString()
        {
            return string.Format("Header: {0}, Gesture: {1}", Header, Gesture);
        }
    }
}
