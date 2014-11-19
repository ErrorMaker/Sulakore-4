using System;
using Sulakore.Habbo;
using Sulakore.Protocol;
using System.Collections.Generic;

namespace Sulakore.Communication
{
    public class PlayerDataLoadedEventArgs : EventArgs, IHabboEvent
    {
        private readonly HMessage _packet;

        public ushort Header { get; private set; }
        public IEnumerable<IHPlayerData> PlayersLoaded { get; private set; }

        public PlayerDataLoadedEventArgs(HMessage packet)
        {
            _packet = packet;
            Header = _packet.Header;

            PlayersLoaded = HPlayerData.Extract(packet);
        }
    }
}