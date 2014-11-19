using System;
using Sulakore.Habbo;
using Sulakore.Protocol;

namespace Sulakore.Communication
{
    public class HostClothesChangedEventArgs : EventArgs, IHabboEvent
    {
        private readonly HMessage _packet;

        public ushort Header { get; private set; }

        private HGenders? _gender;
        public HGenders Gender
        {
            get
            {
                return (HGenders)(_gender != null ?
                    _gender :
                    _gender = SKore.ConvertToHGender(_packet.ReadString(0)));
            }
        }

        private string _figureId;
        public string FigureId
        {
            get
            {
                return !string.IsNullOrEmpty(_figureId) ?
                    _figureId :
                    _figureId = _packet.ReadString(3);
            }
        }

        public HostClothesChangedEventArgs(HMessage packet)
        {
            _packet = packet;
            Header = _packet.Header;
        }
    }
}