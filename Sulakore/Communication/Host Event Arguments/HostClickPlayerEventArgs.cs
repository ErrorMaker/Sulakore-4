using Sulakore.Habbo;
using Sulakore.Protocol;
using System;
using System.Collections.Generic;

namespace Sulakore.Communication
{
    public class HostClickPlayerEventArgs : EventArgs, IHabboEvent
    {
        #region Properties
        public static object[] Params
        {
            get { return new object[] { "Header", "PlayerID", "X", "Y" }; }
        }
        public Dictionary<string, object> Data
        {
            get
            {
                return new Dictionary<string, object>
                {
                    { "Header", Header },
                    { "PlayerID", PlayerId },
                    { "X", Tile.X },
                    { "Y", Tile.Y }
                };
            }
        }
        public HMessage Packet { get; private set; }

        public ushort Header { get; private set; }
        public int PlayerId { get; private set; }
        public HPoint Tile { get; private set; }
        #endregion

        public HostClickPlayerEventArgs(ushort header, int playerId, int x, int y)
        {
            Header = header;
            PlayerId = playerId;
            Tile = new HPoint(x, y);
        }
        public static HostClickPlayerEventArgs CreateArguments(HMessage packet, int playerId)
        {
            return new HostClickPlayerEventArgs(HHeaders.Rotate = packet.Header, playerId, packet.ReadInt(0), packet.ReadInt(4)) { Packet = new HMessage(packet.ToBytes(), HDestinations.Server) };
        }

        public override string ToString()
        {
            return string.Format("Header: {0} | PlayerID: {1} | Tile: {2}", Header, PlayerId, Tile.ToPoint().ToString());
        }
    }
}