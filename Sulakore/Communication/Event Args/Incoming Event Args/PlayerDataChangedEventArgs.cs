using System;
using Sulakore.Habbo;
using Sulakore.Protocol;

namespace Sulakore.Communication
{
    public class PlayerDataChangedEventArgs : EventArgs, IHabboEvent
    {
        private readonly HMessage _packet;

        public ushort Header { get; private set; }

        public int PlayerIndex { get; private set; }
        public string FigureId { get; private set; }
        public HGender Gender { get; private set; }
        public string Motto { get; private set; }

        public PlayerDataChangedEventArgs(HMessage packet)
        {
            _packet = packet;
            Header = _packet.Header;

            int position = 0;
            PlayerIndex = _packet.ReadInt(ref position);
            FigureId = _packet.ReadString(ref position);
            _packet.ReadInt(ref position);
            Gender = SKore.ToGender(_packet.ReadString(ref position));
            Motto = _packet.ReadString(ref position);
        }

        public override string ToString()
        {
            return string.Format("Header: {0}, PlayerIndex: {1}, FigureId: {2}, Gender: {3}, Motto: {4}",
                Header, PlayerIndex, FigureId, Gender, Motto);
        }
    }
}