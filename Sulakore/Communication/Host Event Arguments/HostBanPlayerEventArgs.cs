using System;
using Sulakore.Protocol;
using System.Collections.Generic;
using Sulakore.Habbo;

namespace Sulakore.Communication
{
    public class HostBanPlayerEventArgs : EventArgs, IHabboEvent
    {
        #region Properties
        public static object[] Params
        {
            get { return new object[] { "Header", "PlayerID", "RoomID", "Ban" }; }
        }
        public Dictionary<string, object> Data
        {
            get
            {
                return new Dictionary<string, object>
                {
                    { "Header", Header },
                    { "PlayerID", PlayerId },
                    { "RoomID", RoomId },
                    { "Ban", Ban }
                };
            }
        }
        public HMessage Packet { get; private set; }

        public ushort Header { get; private set; }
        public int PlayerId { get; private set; }
        public int RoomId { get; private set; }
        public HBans Ban { get; private set; }
        #endregion

        public HostBanPlayerEventArgs(ushort header, int playerId, int roomId, HBans ban)
        {
            Header = header;
            PlayerId = playerId;
            RoomId = roomId;
            Ban = ban;
        }
        public static HostBanPlayerEventArgs CreateArguments(HMessage packet)
        {
            return new HostBanPlayerEventArgs(HHeaders.Ban = packet.Header, packet.ReadInt(0), packet.ReadInt(4), (HBans)Enum.Parse(typeof(HBans), packet.ReadString(8), true)) { Packet = new HMessage(packet.ToBytes(), HDestinations.Server) };
        }

        public override string ToString()
        {
            return string.Format("Header: {0} | PlayerID: {1} | RoomID: {2} | Ban: {3}", Header, PlayerId, RoomId, Ban);
        }
    }
}