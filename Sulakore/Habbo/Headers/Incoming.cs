using System;

namespace Sulakore.Habbo.Headers
{
    [Serializable]
    public struct Incoming
    {
        public static ushort PlayersLoaded { get; set; }
        public static ushort Kicked { get; set; }
    }
}