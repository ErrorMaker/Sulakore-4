using Sulakore.Habbo;
using Sulakore.Protocol;
using System;
using System.Collections.Generic;

namespace Sulakore.Communication
{
    public class HostDanceEventArgs : EventArgs, IHabboEvent
    {
        #region Properties
        public static object[] Params
        {
            get { return new object[] { "Header", "Dance" }; }
        }
        public Dictionary<string, object> Data
        {
            get
            {
                return new Dictionary<string, object>
                {
                    { "Header", Header },
                    { "Dance",  Dance }
                };
            }
        }
        public HMessage Packet { get; private set; }

        public ushort Header { get; private set; }
        public HDances Dance { get; private set; }
        #endregion

        public HostDanceEventArgs(ushort header, HDances dance)
        {
            Header = header;
            Dance = dance;
        }
        public static HostDanceEventArgs CreateArguments(HMessage packet)
        {
            return new HostDanceEventArgs(HHeaders.Dance = packet.Header, (HDances)packet.ReadInt(0)) { Packet = new HMessage(packet.ToBytes(), HDestinations.Server) };
        }

        public override string ToString()
        {
            return string.Format("Header: {0} | Dance: {1}", Header, Dance);
        }
    }
}