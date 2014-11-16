using System;
using Sulakore.Protocol;
using System.Collections.Generic;
using Sulakore.Habbo;

namespace Sulakore.Communication
{
    public class PlayerMoveFurnitureEventArgs : EventArgs, IHabboEvent
    {
        #region Properties
        public static object[] Params
        {
            get { return new object[] { "Header", "FurniID", "FurniTypeID", "PlayerID", "X", "Y", "Z", "Direction" }; }
        }
        public Dictionary<string, object> Data
        {
            get
            {
                return new Dictionary<string, object>
                {
                    { "Header", Header },
                    { "FurniID", FurniId },
                    { "FurniTypeID", FurniTypeId },
                    { "PlayerID", PlayerId },
                    { "X", Tile.X },
                    { "Y", Tile.Y },
                    { "Z", Tile.Z },
                    { "Direction", Direction },
                };
            }
        }
        public HMessage Packet { get; private set; }

        public ushort Header { get; private set; }
        public int FurniId { get; private set; }
        public int FurniTypeId { get; private set; }
        public HPoint Tile { get; private set; }
        public HDirections Direction { get; private set; }
        public int PlayerId { get; private set; }
        #endregion

        public PlayerMoveFurnitureEventArgs(ushort header, int furniId, int furniTypeId, int x, int y, string z, HDirections direction, int playerId)
        {
            Header = header;
            FurniId = furniId;
            FurniTypeId = furniTypeId;
            Tile = new HPoint(x, y, z);
            Direction = direction;
            PlayerId = playerId;
        }
        public static PlayerMoveFurnitureEventArgs CreateArguments(HMessage packet)
        {
            return new PlayerMoveFurnitureEventArgs(packet.Header, packet.ReadInt(0), packet.ReadInt(4), packet.ReadInt(8), packet.ReadInt(12), packet.ReadString(20), (HDirections)packet.ReadInt(16), packet.ReadInt(packet.Length - 6)) { Packet = new HMessage(packet.ToBytes()) };
        }

        public override string ToString()
        {
            return string.Format("Header: {0} | FurniID: {1} | FurniTypeID: {2} | PlayerID: {3} | Tile: {4} | Direction: {5}", Header, FurniId, FurniTypeId, PlayerId, Tile.ToString(), Direction);
        }
    }
}