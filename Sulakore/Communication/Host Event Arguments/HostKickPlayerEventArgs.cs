using Sulakore.Habbo;
using Sulakore.Protocol;
using System;
using System.Collections.Generic;

namespace Sulakore.Communication
{
    public class HostKickPlayerEventArgs : EventArgs, IHabboEvent
    {
        #region Properties
        public static object[] Params
        {
            get { return new object[] { "Header", "PlayerID" }; }
        }
        public Dictionary<string, object> Data
        {
            get
            {
                return new Dictionary<string, object>
                {
                    { "Header", Header },
                    { "PlayerID", PlayerId }
                };
            }
        }
        public HMessage Packet { get; private set; }

        public ushort Header { get; private set; }
        public int PlayerId { get; private set; }
        #endregion

        public HostKickPlayerEventArgs(ushort header, int playerId)
        {
            Header = header;
            PlayerId = playerId;
        }
        public static HostKickPlayerEventArgs CreateArguments(HMessage packet)
        {
            return new HostKickPlayerEventArgs(HHeaders.Kick = packet.Header, packet.ReadInt(0)) { Packet = new HMessage(packet.ToBytes(), HDestinations.Server) };
        }

        public override string ToString()
        {
            return string.Format("Header: {0} | PlayerID: {1}", Header, PlayerId);
        }
    }
}