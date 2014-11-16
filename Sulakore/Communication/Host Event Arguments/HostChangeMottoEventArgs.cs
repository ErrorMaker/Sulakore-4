using Sulakore.Habbo;
using Sulakore.Protocol;
using System;
using System.Collections.Generic;

namespace Sulakore.Communication
{
    public class HostChangeMottoEventArgs : EventArgs, IHabboEvent
    {
        #region Properties
        public static object[] Params
        {
            get { return new object[] { "Header", "Motto" }; }
        }
        public Dictionary<string, object> Data
        {
            get
            {
                return new Dictionary<string, object>
                {
                    { "Header", Header },
                    { "Motto", Motto }
                };
            }
        }
        public HMessage Packet { get; private set; }

        public ushort Header { get; private set; }
        public string Motto { get; private set; }
        #endregion

        public HostChangeMottoEventArgs(ushort header, string motto)
        {
            Header = header;
            Motto = motto;
        }
        public static HostChangeMottoEventArgs CreateArguments(HMessage packet)
        {
            return new HostChangeMottoEventArgs(HHeaders.Motto = packet.Header, packet.ReadString(0)) { Packet = new HMessage(packet.ToBytes(), HDestinations.Server) };
        }

        public override string ToString()
        {
            return string.Format("Header: {0} | Motto: {1}", Header, Motto);
        }
    }
}