using Sulakore.Habbo;
using Sulakore.Protocol;
using System;
using System.Collections.Generic;

namespace Sulakore.Communication
{
    public class HostExitEventArgs : EventArgs, IHabboEvent
    {
        #region Properties
        public static object[] Params
        {
            get { return new object[] { "Header" }; }
        }
        public Dictionary<string, object> Data
        {
            get
            {
                return new Dictionary<string, object>
                {
                    { "Header", Header }
                };
            }
        }
        public HMessage Packet { get; private set; }

        public ushort Header { get; private set; }
        #endregion

        public HostExitEventArgs(ushort header)
        {
            Header = header;
        }
        public static HostExitEventArgs CreateArguments(HMessage packet)
        {
            return new HostExitEventArgs(HHeaders.Exit = packet.Header) { Packet = new HMessage(packet.ToBytes(), HDestinations.Server) };
        }

        public override string ToString()
        {
            return string.Format("Header: {0}", Header);
        }
    }
}