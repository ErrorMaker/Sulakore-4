using System;
using Sulakore.Protocol;
using System.Collections.Generic;
using Sulakore.Habbo;

namespace Sulakore.Communication
{
    public class PlayerDropFurnitureEventArgs : EventArgs, IHabboEvent
    {
        #region Properties
        public static object[] Params
        {
            get { return new object[] { "Header", "FurniID", "FurniTypeID", "PlayerID", "PlayerName", "IsRented", "X", "Y", "Z", "Direction" }; }
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
                    { "PlayerName", PlayerName },
                    { "IsRented", IsRented },
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
        public string PlayerName { get; private set; }
        public bool IsRented { get; private set; }
        #endregion

        public PlayerDropFurnitureEventArgs(ushort header, int furniId, int furniTypeId, int x, int y, string z, HDirections direction, int playerId, string playerName, bool isRented)
        {
            Header = header;
            FurniId = furniId;
            FurniTypeId = furniTypeId;
            Tile = new HPoint(x, y, z);
            Direction = direction;
            PlayerId = playerId;
            PlayerName = playerName;
            IsRented = isRented;
        }
        public static PlayerDropFurnitureEventArgs CreateArguments(HMessage packet)
        {
            int position = 0;
            int furniId = packet.ReadInt(ref position);
            int furniTypeId = packet.ReadInt(ref position);
            int fx = packet.ReadInt(ref position);
            int fy = packet.ReadInt(ref position);
            int ff = packet.ReadInt(ref position);
            string fz = packet.ReadString(ref position);
            packet.ReadString(ref position);
            packet.ReadInt(ref position);
            packet.ReadInt(ref position);
            packet.ReadString(ref position);
            bool isRented = packet.ReadInt(ref position) != -1;
            packet.ReadInt(ref position);
            int playerId = packet.ReadInt(ref position);
            string playerName = packet.ReadString(position);
            return new PlayerDropFurnitureEventArgs(packet.Header, furniId, furniTypeId, fx, fy, fz, (HDirections)ff, playerId, playerName, isRented) { Packet = new HMessage(packet.ToBytes()) };
        }

        public override string ToString()
        {
            return string.Format("Header: {0} | FurniID: {1} | FurniTypeID: {2} | PlayerID: {3} | PlayerName: {4} | IsRented: {5} | Tile: {6} | Direction: {7}", Header, FurniId, FurniTypeId, PlayerId, PlayerName, IsRented, Tile.ToString(), Direction);
        }
    }
}