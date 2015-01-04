using System;
using System.Diagnostics;

using Sulakore.Habbo;
using Sulakore.Protocol;

namespace Sulakore.Communication
{
    public class PlayerChangeStanceEventArgs : EventArgs, IHabboEvent
    {
        private readonly HMessage _packet;

        public ushort Header { get; private set; }
        public int PlayerIndex { get; private set; }
        public HPoint Tile { get; private set; }
        public HDirection HeadDirection { get; private set; }
        public HDirection BodyDirection { get; private set; }
        public HStance Stance { get; private set; }
        public bool Empowered { get; private set; }

        public PlayerChangeStanceEventArgs(HMessage packet)
        {
            _packet = packet;
            Header = _packet.Header;

            int position = 0;
            var xx = _packet.ReadInt(ref position);
            PlayerIndex = _packet.ReadInt(ref position);
            Tile = new HPoint(_packet.ReadInt(ref position), _packet.ReadInt(ref position), _packet.ReadString(ref position));
            BodyDirection = (HDirection)_packet.ReadInt(ref position);
            HeadDirection = (HDirection)_packet.ReadInt(ref position);
            string stanceAction = _packet.ReadString(ref position);
            Stance = (stanceAction.Contains("/sit") ? HStance.Sit : HStance.Stand);
            Empowered = stanceAction.Contains("flatctrl");
        }

        public override string ToString()
        {
            return string.Format("Header: {0}, PlayerIndex: {1}, Tile: {2}, HeadDirection: {3}, BodyDirection: {4}, Stance: {5}, Empowered: {6}",
                Header, PlayerIndex, Tile, HeadDirection, BodyDirection, Stance, Empowered);
        }
    }
}