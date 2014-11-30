﻿using System;
using Sulakore.Habbo;
using Sulakore.Protocol;

namespace Sulakore.Communication
{
    public class HostStanceChangedEventArgs : EventArgs, IHabboEvent
    {
        private readonly HMessage _packet;

        public ushort Header { get; private set; }

        public HStances Stance { get; private set; }

        public HostStanceChangedEventArgs(HMessage packet)
        {
            _packet = packet;
            Header = _packet.Header;

            Stance = (HStances)_packet.ReadInt(0);
        }

        public override string ToString()
        {
            return string.Format("Header: {0}, Stance: {1}",
                Header, Stance);
        }
    }
}