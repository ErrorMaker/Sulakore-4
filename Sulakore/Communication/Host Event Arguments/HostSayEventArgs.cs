using System;
using Sulakore.Protocol;
using System.Collections.Generic;
using Sulakore.Habbo;

namespace Sulakore.Communication
{
    public class HostSayEventArgs : EventArgs, IHabboEvent
    {
        #region Properties
        public static object[] Params
        {
            get { return new object[] { "Header", "Message", "Theme" }; }
        }
        public Dictionary<string, object> Data
        {
            get
            {
                return new Dictionary<string, object> 
                {
                    { "Header", Header },
                    { "Message", Message },
                    { "Theme", Theme }
                };
            }
        }
        public HMessage Packet { get; private set; }

        public ushort Header { get; private set; }
        public string Message { get; private set; }
        public HThemes Theme { get; private set; }
        #endregion

        public HostSayEventArgs(ushort header, string message, HThemes theme)
        {
            Header = header;
            Message = message;
            Theme = theme;
        }
        public static HostSayEventArgs CreateArguments(HMessage packet)
        {
            int p = 0;
            return new HostSayEventArgs(HHeaders.Say = packet.Header, packet.ReadString(ref p), (HThemes)packet.ReadInt(p)) { Packet = new HMessage(packet.ToBytes(), HDestinations.Server) };
        }

        public override string ToString()
        {
            return string.Format("Header: {0} | Message: {1} | Theme: {2}", Header, Message, Theme);
        }
    }
}