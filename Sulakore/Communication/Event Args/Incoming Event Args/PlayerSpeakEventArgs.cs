using System;
using Sulakore.Habbo;
using Sulakore.Protocol;

namespace Sulakore.Communication
{
    public class PlayerSpeakEventArgs : EventArgs, IHabboEvent
    {
        private readonly HMessage _packet;

        public ushort Header { get; private set; }

        public int PlayerIndex { get; private set; }
        public string Message { get; private set; }
        public HTheme Theme { get; private set; }
        public HSpeeches Speech { get; private set; }

        public PlayerSpeakEventArgs(HMessage packet)
        {
            throw new NotImplementedException();

            _packet = packet;
            Header = _packet.Header;
        }

        public override string ToString()
        {
            return string.Format("Header: {0}, PlayerIndex: {1}, Message: {2}, Theme: {3}, Speech: {4}",
                Header, PlayerIndex, Message, Theme, Speech);
        }
    }
}