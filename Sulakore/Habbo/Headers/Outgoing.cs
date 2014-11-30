using System;

namespace Sulakore.Habbo.Headers
{
    [Serializable]
    public struct Outgoing
    {
        public static ushort Ban { get; set; }
        public static ushort ChangeClothes { get; set; }
        public static ushort ChangeMotto { get; set; }
        public static ushort ChangeStance { get; set; }
        public static ushort ClickPlayer { get; set; }
        public static ushort Dance { get; set; }
        public static ushort Gesture { get; set; }
        public static ushort KickPlayer { get; set; }
        public static ushort MoveFurniture { get; set; }
        public static ushort MutePlayer { get; set; }
        public static ushort RaiseSign { get; set; }
        public static ushort RoomExit { get; set; }
        public static ushort RoomNavigate { get; set; }
        public static ushort Say { get; set; }
        public static ushort Shout { get; set; }
        public static ushort TradePlayer { get; set; }
        public static ushort Walk { get; set; }
    }
}