using System;
using Sulakore.Habbo;
using Sulakore.Protocol;
using System.Collections.Generic;

namespace Sulakore.Communication
{
    public class PlayerDataLoadedEventArgs : EventArgs, IHabboEvent
    {
        private readonly HMessage _packet;
        private readonly IEnumerable<IHPlayerData> _playerData;

        public ushort Header { get; private set; }

        public PlayerDataLoadedEventArgs(HMessage packet)
        {
            _packet = packet;
            Header = _packet.Header;

            _playerData = HPlayerData.Extract(packet);
        }
    }
}