using Sulakore.Habbo;
using Sulakore.Protocol;
using System;
using System.Collections.Generic;

namespace Sulakore.Communication
{
    public class HostShoutEventArgs : EventArgs, IHabboEvent
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

        public HostShoutEventArgs(ushort header, string message, HThemes theme)
        {
            Header = header;
            Message = message;
            Theme = theme;
        }
        public static HostShoutEventArgs CreateArguments(HMessage packet)
        {
            return new HostShoutEventArgs(HHeaders.Shout = packet.Header, packet.ReadString(0), (HThemes)packet.ReadInt(packet.Length - 6)) { Packet = new HMessage(packet.ToBytes(), HDestinations.Server) };
        }

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>
        /// A string that represents the current object.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public override string ToString()
        {
            return string.Format("Header: {0} | Message: {1} | Theme: {2}", Header, Message, Theme);
        }
    }
}