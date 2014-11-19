using System;
using Sulakore.Habbo;
using Sulakore.Protocol;

namespace Sulakore.Communication
{
    public class PlayerDataChangedEventArgs : EventArgs, IHabboEvent
    {
        private readonly HMessage _packet;

        public ushort Header { get; private set; }

        private int? _playerIndex;
        public int PlayerIndex
        {
            get
            {
                return (int)(_playerIndex != null ?
                    _playerIndex :
                    _playerIndex = _packet.ReadInt(0));
            }
        }

        private string _figureId;
        public string FigureId
        {
            get
            {
                return _figureId != null ?
                    _figureId :
                    _figureId = _packet.ReadString(4);
            }
        }

        private HGenders? _gender;
        public HGenders Gender
        {
            get
            {
                return (HGenders)(_gender != null ?
                    _gender :
                    _gender = SKore.ConvertToHGender(_packet.ReadString(6 + FigureId.Length)));
            }
        }

        private string _motto;
        public string Motto
        {
            get
            {
                return _motto != null ?
                    _motto :
                    _motto = _packet.ReadString(9 + FigureId.Length);
            }
        }

        public PlayerDataChangedEventArgs(HMessage packet)
        {
            _packet = packet;
            Header = _packet.Header;
        }
    }
}