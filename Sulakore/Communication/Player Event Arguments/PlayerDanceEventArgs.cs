using System;
using Sulakore.Protocol;
using System.Collections.Generic;
using Sulakore.Habbo;

namespace Sulakore.Communication
{
    public class PlayerDanceEventArgs : EventArgs, IHabboEvent
    {
        #region Properties
        public static object[] Params
        {
            get { return new object[] { "Header", "Index", "Dance" }; }
        }
        public Dictionary<string, object> Data
        {
            get
            {
                return new Dictionary<string, object>
                {
                    { "Header", Header },
                    { "Index", Index },
                    { "Dance", Dance }
                };
            }
        }
        public HMessage Packet { get; private set; }

        public ushort Header { get; private set; }
        public int Index { get; private set; }
        public HDances Dance { get; private set; }
        #endregion

        public PlayerDanceEventArgs(ushort header, int index, HDances dance)
        {
            Header = header;
            Index = index;
            Dance = dance;
        }
        public static PlayerDanceEventArgs CreateArguments(HMessage packet)
        {
            return new PlayerDanceEventArgs(HHeaders.PlayerDance = packet.Header, packet.ReadInt(0), (HDances)packet.ReadInt(4)) { Packet = new HMessage(packet.ToBytes()) };
        }

        public override string ToString()
        {
            return string.Format("Header: {0} | Index: {1} | Dance: {2}", Header, Index, Dance);
        }
    }
}