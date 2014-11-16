using Sulakore.Habbo;
using Sulakore.Protocol;
using System;
using System.Collections.Generic;

namespace Sulakore.Communication
{
    public class HostChangeStanceEventArgs : EventArgs, IHabboEvent
    {
        #region Properties
        public static object[] Params
        {
            get { return new object[] { "Header", "Stance" }; }
        }
        public Dictionary<string, object> Data
        {
            get
            {
                return new Dictionary<string, object>
                {
                    { "Header", Header },
                    { "Stance", Stance }
                };
            }
        }
        public HMessage Packet { get; private set; }

        public ushort Header { get; private set; }
        public HStances Stance { get; private set; }
        #endregion

        public HostChangeStanceEventArgs(ushort header, HStances stance)
        {
            Header = header;
            Stance = stance;
        }
        public static HostChangeStanceEventArgs CreateArguments(HMessage packet)
        {
            return new HostChangeStanceEventArgs(HHeaders.Stance = packet.Header, (HStances)packet.ReadInt(0)) { Packet = new HMessage(packet.ToBytes(), HDestinations.Server) };
        }

        public override string ToString()
        {
            return string.Format("Header: {0} | Stance: {1}", Header, Stance);
        }
    }
}