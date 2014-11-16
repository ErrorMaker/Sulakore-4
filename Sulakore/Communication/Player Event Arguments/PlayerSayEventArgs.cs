using System;
using Sulakore.Protocol;
using System.Collections.Generic;
using Sulakore.Habbo;

namespace Sulakore.Communication
{
    public class PlayerSayEventArgs : EventArgs, IHabboEvent
    {
        #region Properties
        public static object[] Params
        {
            get { return new object[] { "Header", "Index", "Message", "Theme" }; }
        }
        public Dictionary<string, object> Data
        {
            get
            {
                return new Dictionary<string, object> 
                {
                    { "Header", Header },
                    { "Index", Index },
                    { "Message", Message },
                    { "Theme", Theme }
                };
            }
        }
        public HMessage Packet { get; private set; }

        public ushort Header { get; private set; }
        public int Index { get; private set; }
        public string Message { get; private set; }
        public HThemes Theme { get; private set; }
        #endregion

        public PlayerSayEventArgs(ushort header, int index, string message, HThemes theme)
        {
            Header = header;
            Index = index;
            Message = message;
            Theme = theme;
        }
        public static PlayerSayEventArgs CreateArguments(HMessage packet)
        {
            int position = 4;
            return new PlayerSayEventArgs(HHeaders.PlayerSay = packet.Header, packet.ReadInt(0), packet.ReadString(ref position), (HThemes)packet.ReadInt(position + 4)) { Packet = new HMessage(packet.ToBytes()) };
        }

        public override string ToString()
        {
            return string.Format("Header: {0} | Index: {1} | Message: {2} | Theme: {3}", Header, Index, Message, Theme);
        }
    }
}