using System;
using Sulakore.Protocol;
using System.Collections.Generic;
using Sulakore.Habbo;

namespace Sulakore.Communication
{
    public class HostSignEventArgs : EventArgs, IHabboEvent
    {
        #region Properties
        public static object[] Params
        {
            get { return new object[] { "Header", "Sign" }; }
        }
        public Dictionary<string, object> Data
        {
            get
            {
                return new Dictionary<string, object>
                {
                    { "Header", Header },
                    { "Sign", Sign }
                };
            }
        }
        public HMessage Packet { get; private set; }

        public ushort Header { get; private set; }
        public HSigns Sign { get; private set; }
        #endregion

        public HostSignEventArgs(ushort header, HSigns sign)
        {
            Header = header;
            Sign = sign;
        }
        public static HostSignEventArgs CreateArguments(HMessage packet)
        {
            return new HostSignEventArgs(HHeaders.Sign = packet.Header, (HSigns)packet.ReadInt(0)) { Packet = new HMessage(packet.ToBytes(), HDestinations.Server) };
        }

        public override string ToString()
        {
            return string.Format("Header: {0} | Sign: {1}", Header, Sign);
        }
    }
}