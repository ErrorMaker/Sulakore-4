using System;
using Sulakore.Habbo;
using Sulakore.Protocol;
using System.Collections.Generic;

namespace Sulakore.Communication
{
    public class FurnitureDataLoadedEventArgs : EventArgs, IHabboEvent
    {
        private readonly HMessage _packet;

        public ushort Header { get; private set; }

        public IEnumerable<IHFurnitureData> FurnitureLoaded { get; private set; }

        public FurnitureDataLoadedEventArgs(HMessage packet)
        {
            _packet = packet;
            Header = _packet.Header;

            FurnitureLoaded = HFurnitureData.Extract(packet);
        }
    }
}