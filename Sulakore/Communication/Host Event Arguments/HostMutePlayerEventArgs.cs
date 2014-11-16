using Sulakore.Habbo;
using Sulakore.Protocol;
using System;
using System.Collections.Generic;

namespace Sulakore.Communication
{
    public class HostMutePlayerEventArgs : EventArgs, IHabboEvent
    {
        #region Properties
        public static object[] Params
        {
            get { return new object[] { "Header", "PlayerID", "RoomID", "Minutes" }; }
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
                    { "Minutes", Minutes }
                };
            }
        }
        public HMessage Packet { get; private set; }

        public ushort Header { get; private set; }
        public int PlayerId { get; private set; }
        public int RoomId { get; private set; }
        public int Minutes { get; private set; }
        #endregion

        public HostMutePlayerEventArgs(ushort header, int playerId, int roomId, int minutes)
        {
            Header = header;
            PlayerId = playerId;
            RoomId = roomId;
            Minutes = minutes;
        }
        public static HostMutePlayerEventArgs CreateArguments(HMessage packet)
        {
            return new HostMutePlayerEventArgs(HHeaders.Mute = packet.Header, packet.ReadInt(0), packet.ReadInt(4), packet.ReadInt(8)) { Packet = new HMessage(packet.ToBytes(), HDestinations.Server) };
        }

        public override string ToString()
        {
            return string.Format("Header: {0} | PlayerID: {1} | RoomID: {2} | Minutes: {3}", Header, PlayerId, RoomId, Minutes);
        }
    }
}