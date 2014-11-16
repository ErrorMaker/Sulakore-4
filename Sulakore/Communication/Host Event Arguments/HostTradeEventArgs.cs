using System;
using Sulakore.Protocol;
using System.Collections.Generic;
using Sulakore.Habbo;

namespace Sulakore.Communication
{
    public class HostTradeEventArgs : EventArgs, IHabboEvent
    {
        #region Properties
        public static object[] Params
        {
            get { return new object[] { "Header", "Index" }; }
        }
        public Dictionary<string, object> Data
        {
            get
            {
                return new Dictionary<string, object>
                {
                    { "Header", Header },
                    { "Index", Index }
                };
            }
        }
        public HMessage Packet { get; private set; }

        public ushort Header { get; private set; }
        public int Index { get; private set; }
        #endregion

        public HostTradeEventArgs(ushort header, int index)
        {
            Header = header;
            Index = index;
        }
        public static HostTradeEventArgs CreateArguments(HMessage packet)
        {
            return new HostTradeEventArgs(HHeaders.Trade = packet.Header, packet.ReadInt(0)) { Packet = new HMessage(packet.ToBytes(), HDestinations.Server) };
        }

        public override string ToString()
        {
            return string.Format("Header: {0} | Index: {1}", Header, Index);
        }
    }
}