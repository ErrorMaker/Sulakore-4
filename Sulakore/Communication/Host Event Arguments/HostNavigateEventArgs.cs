using Sulakore.Habbo;
using Sulakore.Protocol;
using System;
using System.Collections.Generic;

namespace Sulakore.Communication
{
    public class HostNavigateEventArgs : EventArgs, IHabboEvent
    {
        #region Properties
        public static object[] Params
        {
            get { return new object[] { "Header", "RoomID", "Password" }; }
        }
        public Dictionary<string, object> Data
        {
            get
            {
                return new Dictionary<string, object>
                {
                    { "Header", Header },
                    { "RoomID", RoomId },
                    { "Password", Password }
                };
            }
        }
        public HMessage Packet { get; private set; }

        public ushort Header { get; private set; }
        public int RoomId { get; private set; }
        public string Password { get; private set; }
        #endregion

        public HostNavigateEventArgs(ushort header, int roomId, string password)
        {
            Header = header;
            RoomId = roomId;
            Password = password;
        }
        public static HostNavigateEventArgs CreateArguments(HMessage packet)
        {
            return new HostNavigateEventArgs(HHeaders.Navigate = packet.Header, packet.ReadInt(0), packet.ReadString(4)) { Packet = new HMessage(packet.ToBytes(), HDestinations.Server) };
        }

        public override string ToString()
        {
            return string.Format("Header: {0} | RoomID: {1} | Password: {2}", Header, RoomId, Password);
        }
    }
}