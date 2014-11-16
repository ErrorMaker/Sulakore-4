using System;
using Sulakore.Protocol;
using System.Collections.Generic;
using Sulakore.Habbo;

namespace Sulakore.Communication
{
    public class PlayerChangeDataEventArgs : EventArgs, IHabboEvent
    {
        #region Properties
        public static object[] Params
        {
            get { return new object[] { "Header", "Index", "Clothes", "Motto", "Gender" }; }
        }
        public Dictionary<string, object> Data
        {
            get
            {
                return new Dictionary<string, object>
                {
                    { "Header", Header },
                    { "Index", Index },
                    { "Clothes", Clothes },
                    { "Motto", Motto },
                    { "Gender", Gender }
                };
            }
        }
        public HMessage Packet { get; private set; }

        public ushort Header { get; private set; }
        public int Index { get; private set; }
        public string Clothes { get; private set; }
        public string Motto { get; private set; }
        public HGenders Gender { get; private set; }
        #endregion

        public PlayerChangeDataEventArgs(ushort header, int index, string clothes, HGenders gender, string motto)
        {
            Header = header;
            Index = index;
            Clothes = clothes;
            Gender = gender;
            Motto = motto;
        }
        public static PlayerChangeDataEventArgs CreateArguments(HMessage packet)
        {
            int position = 0;
            return new PlayerChangeDataEventArgs(packet.Header, packet.ReadInt(ref position), packet.ReadString(ref position), (HGenders)packet.ReadString(ref position).ToUpper()[0], packet.ReadString(position)) { Packet = new HMessage(packet.ToBytes()) };
        }

        public override string ToString()
        {
            return string.Format("Header: {0} | Index: {1} | Clothes: {2} | Motto: {3} | Gender: {4}", Header, Index, Clothes, Motto, Gender);
        }
    }
}