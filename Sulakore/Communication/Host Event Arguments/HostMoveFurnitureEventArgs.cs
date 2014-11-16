using Sulakore.Habbo;
using Sulakore.Protocol;
using System;
using System.Collections.Generic;

namespace Sulakore.Communication
{
    public class HostMoveFurnitureEventArgs : EventArgs, IHabboEvent
    {
        #region Properties
        public static object[] Params
        {
            get { return new object[] { "Header", "FurniID", "X", "Y", "Z", "Direction" }; }
        }
        public Dictionary<string, object> Data
        {
            get
            {
                return new Dictionary<string, object>
                {
                    { "Header", Header },
                    { "FurniID", FurniId },
                    { "X", Tile.X },
                    { "Y", Tile.Y },
                    { "Z", Tile.Z },
                    { "Direction", Direction }
                };
            }
        }
        public HMessage Packet { get; private set; }

        public ushort Header { get; private set; }
        public int FurniId { get; private set; }
        public HPoint Tile { get; private set; }
        public HDirections Direction { get; private set; }
        #endregion

        public HostMoveFurnitureEventArgs(ushort header, int furniId, int x, int y, string z, HDirections direction)
        {
            Header = header;
            FurniId = furniId;
            Tile = new HPoint(x, y, z);
            Direction = direction;
        }
        public static HostMoveFurnitureEventArgs CreateArguments(HMessage packet, string z)
        {
            return new HostMoveFurnitureEventArgs(HHeaders.MoveFurniture = packet.Header, packet.ReadInt(0), packet.ReadInt(4), packet.ReadInt(8), z, (HDirections)packet.ReadInt(12)) { Packet = new HMessage(packet.ToBytes(), HDestinations.Server) };
        }

        public override string ToString()
        {
            return string.Format("Header: {0} | FurniID: {1} | Tile: {2} | Direction: {3}", Header, FurniId, Tile.ToString(), Direction);
        }
    }
}