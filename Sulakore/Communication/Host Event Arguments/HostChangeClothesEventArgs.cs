using Sulakore.Habbo;
using Sulakore.Protocol;
using System;
using System.Collections.Generic;

namespace Sulakore.Communication
{
    public class HostChangeClothesEventArgs : EventArgs, IHabboEvent
    {
        #region Properties
        public static object[] Params
        {
            get { return new object[] { "Header", "Gender", "Clothes" }; }
        }
        public Dictionary<string, object> Data
        {
            get
            {
                return new Dictionary<string, object> 
                {
                    { "Header", Header },
                    { "Gender", Gender },
                    { "Clothes", Clothes }
                };
            }
        }
        public HMessage Packet { get; private set; }

        public ushort Header { get; private set; }
        public HGenders Gender { get; private set; }
        public string Clothes { get; private set; }
        #endregion

        public HostChangeClothesEventArgs(ushort header, HGenders gender, string clothes)
        {
            Header = header;
            Gender = gender;
            Clothes = clothes;
        }
        public static HostChangeClothesEventArgs CreateArguments(HMessage packet)
        {
            return new HostChangeClothesEventArgs(HHeaders.Clothes = packet.Header, (HGenders)packet.ReadString(0).ToUpper()[0], packet.ReadString(3)) { Packet = new HMessage(packet.ToBytes(), HDestinations.Server) };
        }

        public override string ToString()
        {
            return string.Format("Header: {0} | Gender: {1} | Clothes: {2}", Header, Gender, Clothes);
        }
    }
}