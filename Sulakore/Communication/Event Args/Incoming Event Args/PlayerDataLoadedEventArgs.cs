using System;
using Sulakore.Habbo;
using Sulakore.Protocol;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Sulakore.Communication
{
    public class PlayerDataLoadedEventArgs : EventArgs, IHabboEvent
    {
        private readonly HMessage _packet;

        public ushort Header { get; private set; }

        public ReadOnlyCollection<IHPlayerData> LoadedPlayers { get; private set; }

        public PlayerDataLoadedEventArgs(HMessage packet)
        {
            _packet = packet;
            Header = _packet.Header;

            LoadedPlayers = new ReadOnlyCollection<IHPlayerData>(HPlayerData.Extract(packet));
        }

        public IList<IHPlayerData> PlayersByGender(HGender gender)
        {
            throw new NotImplementedException();
        }

        public override string ToString()
        {
            return string.Format("Header: {0}, Total Players Loaded: {1}",
                Header, LoadedPlayers.Count);
        }
    }
}