using System;
using Sulakore.Protocol;
using System.Collections.Generic;
using Sulakore.Habbo;

namespace Sulakore.Communication
{
    public class PlayerSignEventArgs : EventArgs, IHabboEvent
    {
        #region Properties
        public static object[] Params
        {
            get { return new object[] { "Header", "Index", "X", "Y", "Z", "Sign", "Direction", "Privileges" }; }
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
                    { "Sign", Sign },
                    { "Direction", Direction },
                    { "Privileges", Privileges }
                };
            }
        }
        public HMessage Packet { get; private set; }

        public ushort Header { get; private set; }
        public int Index { get; private set; }
        public HPoint Tile { get; private set; }
        public HSigns Sign { get; private set; }
        public HDirections Direction { get; private set; }
        public bool Privileges { get; private set; }
        #endregion

        public PlayerSignEventArgs(ushort header, int index, int x, int y, string z, HDirections direction, HSigns sign, bool privileges)
        {
            Header = header;
            Index = index;
            Tile = new HPoint(x, y, z);
            Direction = direction;
            Sign = sign;
            Privileges = privileges;
        }
        public static PlayerSignEventArgs CreateArguments(HMessage packet)
        {
            int position = 18 + packet.ReadShort(16);
            return new PlayerSignEventArgs(packet.Header, packet.ReadInt(4), packet.ReadInt(8), packet.ReadInt(12), packet.ReadString(16), (HDirections)packet.ReadInt(position), (HSigns)int.Parse(packet.ReadString(position + 8).GetChild("/sign ", '/')), packet.ReadString(position + 8).Contains("flatctrl")) { Packet = new HMessage(packet.ToBytes()) };
        }

        public override string ToString()
        {
            return string.Format("Header: {0} | Index: {1} | Tile: {2} | Sign: {3} | Direction: {4} | Privileges: {5}", Header, Index, Tile.ToString(), Sign, Direction, Privileges.ToString());
        }
    }
}