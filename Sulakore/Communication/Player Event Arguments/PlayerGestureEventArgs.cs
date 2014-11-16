using System;
using Sulakore.Protocol;
using System.Collections.Generic;
using Sulakore.Habbo;

namespace Sulakore.Communication
{
    public class PlayerGestureEventArgs : EventArgs, IHabboEvent
    {
        #region Properties
        public static object[] Params
        {
            get { return new object[] { "Header", "Index", "Gesture" }; }
        }
        public Dictionary<string, object> Data
        {
            get
            {
                return new Dictionary<string, object> 
                {
                    { "Header", Header },
                    { "Index", Index },
                    { "Gesture", Gesture }
                };
            }
        }
        public HMessage Packet { get; private set; }

        public ushort Header { get; private set; }
        public int Index { get; private set; }
        public HGestures Gesture { get; private set; }
        #endregion

        public PlayerGestureEventArgs(ushort header, int index, HGestures gesture)
        {
            Header = header;
            Index = index;
            Gesture = gesture;
        }
        public static PlayerGestureEventArgs CreateArguments(HMessage packet)
        {
            return new PlayerGestureEventArgs(HHeaders.PlayerGesture = packet.Header, packet.ReadInt(0), (HGestures)packet.ReadInt(4)) { Packet = new HMessage(packet.ToBytes()) };
        }

        public override string ToString()
        {
            return string.Format("Header: {0} | Index: {1} | Gesture: {2}", Header, Index, Gesture);
        }
    }
}