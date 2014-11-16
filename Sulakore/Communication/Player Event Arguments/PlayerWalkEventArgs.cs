using System;
using Sulakore.Protocol;
using System.Collections.Generic;
using Sulakore.Habbo;

namespace Sulakore.Communication
{
    public class PlayerWalkEventArgs : EventArgs, IHabboEvent
    {
        #region Properties
        public static object[] Params
        {
            get { return new object[] { "Header", "Index", "X", "Y", "Z", "WalkingToX", "WalkingToY", "WalkingToZ", "Direction", "Privileges" }; }
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
                    { "WalkingToX", WalkingTo.X },
                    { "WalkingToY", WalkingTo.Y },
                    { "WalkingToZ", WalkingTo.Z },
                    { "Direction", Direction },
                    { "Privileges", Privileges }
                };
            }
        }
        public HMessage Packet { get; private set; }

        public ushort Header { get; private set; }
        public int Index { get; private set; }
        public HPoint Tile { get; private set; }
        public HPoint WalkingTo { get; private set; }
        public HDirections Direction { get; private set; }
        public bool Privileges { get; private set; }
        #endregion

        public PlayerWalkEventArgs(ushort header, int index, int x, int y, string z, int walkingToX, int walkingToY, string walkingToZ, HDirections direction, bool prvileges)
        {
            Header = header;
            Index = index;
            Tile = new HPoint(x, y, z);
            WalkingTo = new HPoint(walkingToX, walkingToY, walkingToZ);
            Direction = direction;
            Privileges = prvileges;
        }
        public static PlayerWalkEventArgs CreateArguments(HMessage packet)
        {
            int position = 18 + packet.ReadShort(16);
            string[] chunks = packet.ReadString(position + 8).GetChild("/mv ", '/').Split(',');
            return new PlayerWalkEventArgs(packet.Header, packet.ReadInt(4), packet.ReadInt(8), packet.ReadInt(12), packet.ReadString(16), int.Parse(chunks[0]), int.Parse(chunks[1]), chunks[2], (HDirections)packet.ReadInt(position), packet.ReadString(position + 8).Contains("flatctrl")) { Packet = new HMessage(packet.ToBytes()) };
        }
        public static PlayerWalkEventArgs[] GetPlayerWalkList(HMessage packet)
        {
            var playersWalking = new List<PlayerWalkEventArgs>();
            try
            {
                int position = 4;
                int playerCount = packet.ReadInt(0);
                for (int i = 0; i < playerCount; i++)
                {
                    var playerWalking = new HMessage(packet.Header);
                    playerWalking.Append(1);
                    playerWalking.Append(packet.ReadInt(ref position));
                    playerWalking.Append(packet.ReadInt(ref position));
                    playerWalking.Append(packet.ReadInt(ref position));
                    playerWalking.Append(packet.ReadString(ref position));
                    playerWalking.Append(packet.ReadInt(ref position));
                    playerWalking.Append(packet.ReadInt(ref position));
                    playerWalking.Append(packet.ReadString(ref position));

                    if (playerWalking.IsPlayerWalking)
                        playersWalking.Add(CreateArguments(playerWalking));
                }
            }
            catch (Exception ex) { SKore.Debugger(ex.ToString()); }
            return playersWalking.ToArray();
        }
        public static bool HasMultiplePlayers(HMessage packet)
        { return packet.ReadInt(0) > 1; }

        public override string ToString()
        {
            return string.Format("Header: {0} | Index: {1} | Tile: {2} | WalkingTo: {3} | Direction: {4} | Privileges: {5}", Header, Index, Tile.ToString(), WalkingTo.ToString(), Direction, Privileges.ToString());
        }
    }
}