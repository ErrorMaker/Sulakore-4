using System;
using Sulakore.Protocol;
using System.Collections.Generic;
using Sulakore.Habbo;

namespace Sulakore.Communication
{
    public class PlayerChangeStanceEventArgs : EventArgs, IHabboEvent
    {
        #region Properties
        public static object[] Params
        {
            get { return new object[] { "Header", "Index", "X", "Y", "Z", "Direction", "Stance", "Privileges" }; }
        }
        public Dictionary<string, object> Data
        {
            get
            {
                return new Dictionary<string, object> 
                {
                    { "Header", Header },
                    { "Index", Index },
                    { "X", Tile.X },
                    { "Y", Tile.Y },
                    { "Z", Tile.Z },
                    { "Direction", Direction },
                    { "Stance", Stance },
                    { "Privileges", Privileges }
                };
            }
        }
        public HMessage Packet { get; private set; }

        public ushort Header { get; private set; }
        public int Index { get; private set; }
        public HPoint Tile { get; private set; }
        public HDirections Direction { get; private set; }
        public HStances Stance { get; private set; }
        public bool Privileges { get; private set; }
        #endregion

        public PlayerChangeStanceEventArgs(ushort header, int index, int x, int y, string z, HDirections direction, HStances stance, bool privileges)
        {
            Header = header;
            Index = index;
            Tile = new HPoint(x, y, z);
            Direction = direction;
            Stance = stance;
            Privileges = privileges;
        }
        public static PlayerChangeStanceEventArgs CreateArguments(HMessage packet)
        {
            int position = 16;
            return new PlayerChangeStanceEventArgs(packet.Header, packet.ReadInt(4), packet.ReadInt(8), packet.ReadInt(12), packet.ReadString(ref position), (HDirections)packet.ReadInt(position), packet.ReadString(position + 8).Contains("/sit") ? HStances.Sit : HStances.Stand, packet.ReadString(position + 8).Contains("flatctrl")) { Packet = new HMessage(packet.ToBytes()) };
        }
        public static PlayerChangeStanceEventArgs[] GetPlayerChangeStanceList(HMessage packet)
        {
            var playersStance = new List<PlayerChangeStanceEventArgs>();
            try
            {
                int position = 4;
                int playerCount = packet.ReadInt(0);
                for (int i = 0; i < playerCount; i++)
                {
                    var playerStance = new HMessage(packet.Header);
                    playerStance.Append(1);
                    playerStance.Append(packet.ReadInt(ref position));
                    playerStance.Append(packet.ReadInt(ref position));
                    playerStance.Append(packet.ReadInt(ref position));
                    playerStance.Append(packet.ReadString(ref position));
                    playerStance.Append(packet.ReadInt(ref position));
                    playerStance.Append(packet.ReadInt(ref position));
                    playerStance.Append(packet.ReadString(ref position));

                    if (playerStance.IsPlayerChangeStance)
                        playersStance.Add(CreateArguments(playerStance));
                }
            }
            catch (Exception ex) { SKore.Debugger(ex.ToString()); }
            return playersStance.ToArray();
        }
        public static bool HasMultiplePlayers(HMessage packet)
        { return packet.ReadInt(0) > 1; }

        public override string ToString()
        {
            return string.Format("Header: {0} | Index: {1} | Tile: {2} | Direction: {3} | Stance: {4} | Privileges: {5}", Header, Index, Tile.ToString(), Direction, Stance, Privileges.ToString());
        }
    }
}