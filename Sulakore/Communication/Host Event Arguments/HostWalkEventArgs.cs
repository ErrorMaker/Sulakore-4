using Sulakore.Habbo;
using Sulakore.Protocol;
using System;
using System.Collections.Generic;

namespace Sulakore.Communication
{
    public class HostWalkEventArgs : EventArgs, IHabboEvent
    {
        #region Properties
        public static object[] Params
        {
            get { return new object[] { "Header", "X", "Y" }; }
        }
        public Dictionary<string, object> Data
        {
            get
            {
                return new Dictionary<string, object>
                {
                    { "Header", Header },
                    { "X", Tile.X },
                    { "Y", Tile.Y }
                };
            }
        }
        public HMessage Packet { get; private set; }

        public ushort Header { get; private set; }
        public HPoint Tile { get; private set; }
        #endregion

        public HostWalkEventArgs(ushort header, int x, int y)
        {
            Header = header;
            Tile = new HPoint(x, y);
        }
        public static HostWalkEventArgs CreateArguments(HMessage packet)
        {
            return new HostWalkEventArgs(HHeaders.Walk = packet.Header, packet.ReadInt(0), packet.ReadInt(4)) { Packet = new HMessage(packet.ToBytes(), HDestinations.Server) };
        }

        public override string ToString()
        {
            return string.Format("Header: {0} | Tile: {1}", Header, Tile.ToPoint().ToString());
        }
    }
}