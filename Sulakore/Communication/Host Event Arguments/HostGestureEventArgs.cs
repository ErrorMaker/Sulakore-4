using Sulakore.Habbo;
using Sulakore.Protocol;
using System;
using System.Collections.Generic;

namespace Sulakore.Communication
{
    public class HostGestureEventArgs : EventArgs, IHabboEvent
    {
        #region Properties
        public static object[] Params
        {
            get { return new object[] { "Header", "Gesture" }; }
        }
        public Dictionary<string, object> Data
        {
            get
            {
                return new Dictionary<string, object> 
                {
                    { "Header", Header },
                    { "Gesture", Gesture }
                };
            }
        }
        public HMessage Packet { get; private set; }

        public ushort Header { get; private set; }
        public HGestures Gesture { get; private set; }
        #endregion

        public HostGestureEventArgs(ushort header, HGestures gesture)
        {
            Header = header;
            Gesture = gesture;
        }
        public static HostGestureEventArgs CreateArguments(HMessage packet)
        {
            return new HostGestureEventArgs(HHeaders.Gesture = packet.Header, (HGestures)packet.ReadInt(0)) { Packet = new HMessage(packet.ToBytes(), HDestinations.Server) };
        }

        public override string ToString()
        {
            return string.Format("Header: {0} | Gesture: {1}", Header, Gesture);
        }
    }
}