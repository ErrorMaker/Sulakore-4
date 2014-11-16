using System;
using Sulakore.Protocol;
using System.Collections.Generic;
using Sulakore.Habbo;

namespace Sulakore.Communication
{
    public class PlayerEnterEventArgs : EventArgs, IHabboEvent
    {
        #region Properties
        public static object[] Params
        {
            get { return new object[] { "Header", "PlayerName", "PlayerID", "Index", "X", "Y", "Z", "Clothes", "Motto", "Gender" }; }
        }
        public Dictionary<string, object> Data
        {
            get
            {
                return new Dictionary<string, object>
                {
                    { "Header", Header },
                    { "PlayerName", PlayerName },
                    { "PlayerID", PlayerId },
                    { "Index", Index },
                    { "X", Tile.X },
                    { "Y", Tile.Y },
                    { "Z", Tile.Z },
                    { "Clothes", Clothes },
                    { "Motto", Motto },
                    { "Gender", Gender }
                };
            }
        }
        public HMessage Packet { get; private set; }

        public ushort Header { get; private set; }
        public string PlayerName { get; private set; }
        public int PlayerId { get; private set; }
        public int Index { get; private set; }
        public HPoint Tile { get; private set; }
        public string Clothes { get; private set; }
        public string Motto { get; private set; }
        public HGenders Gender { get; private set; }
        #endregion

        public PlayerEnterEventArgs(ushort header, int playerId, string playerName, string motto, string clothes, int index, int x, int y, string z, HGenders gender)
        {
            Header = header;
            PlayerId = playerId;
            PlayerName = playerName;
            Motto = motto;
            Clothes = clothes;
            Index = index;
            Tile = new HPoint(x, y, z);
            Gender = gender;
        }
        public static PlayerEnterEventArgs CreateArguments(HMessage packet)
        {
            int position = 4;
            return new PlayerEnterEventArgs(packet.Header, packet.ReadInt(ref position), packet.ReadString(ref position), packet.ReadString(ref position), packet.ReadString(ref position), packet.ReadInt(ref position), packet.ReadInt(ref position), packet.ReadInt(ref position), packet.ReadString(ref position), (HGenders)packet.ReadString(position + 8).ToUpper()[0]) { Packet = new HMessage(packet.ToBytes()) };
        }
        public static PlayerEnterEventArgs[] GetPlayers(HMessage packet)
        {
            var playerEnterEventArgses = new List<PlayerEnterEventArgs>();

            return playerEnterEventArgses.ToArray();
        }

        public override string ToString()
        {
            return string.Format("Header: {0} | PlayerID: {1} | PlayerName: {2} | Motto: {3} | Clothes: {4} | Index: {5} | Tile: {6} | Gender: {7}", Header, PlayerId, PlayerName, Motto, Clothes, Index, Tile.ToString(), Gender);
        }
    }
}