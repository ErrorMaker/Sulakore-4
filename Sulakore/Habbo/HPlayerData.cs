using Sulakore.Protocol;
using System.Collections.Generic;

namespace Sulakore.Habbo
{
    public class HPlayerData : IHPlayerData
    {
        public string PlayerName { get; private set; }
        public int PlayerId { get; private set; }
        public int PlayerIndex { get; private set; }
        public HPoint Tile { get; private set; }
        public string PlayerFigureId { get; private set; }
        public string Motto { get; private set; }
        public HGenders Gender { get; private set; }
        public string GroupName { get; private set; }

        public HPlayerData(string playerName, int playerId, int playerIndex,
            HPoint tile, string playerFigureId, string motto, HGenders gender,
            string groupName)
        {
            PlayerName = playerName;
            PlayerId = playerId;
            PlayerIndex = playerIndex;
            Tile = tile;
            PlayerFigureId = playerFigureId;
            Motto = motto;
            Gender = gender;
            GroupName = groupName;
        }
        
        public static IEnumerable<IHPlayerData> Extract(HMessage packet)
        {
            var playerDataList = new List<HPlayerData>(packet.ReadInt());
            string playerName, playerFigureId, motto, gender, groupName, z;
            int playerId, playerIndex, playerType, x, y, position;

            do
            {
                playerId = playerIndex = playerType = x = y = position = 0;
                playerName = playerFigureId = motto = gender = groupName = z = string.Empty;

                playerId = packet.ReadInt(ref position);
                playerName = packet.ReadString(ref position);
                motto = packet.ReadString(ref position);
                playerFigureId = packet.ReadString(ref position);
                playerIndex = packet.ReadInt(ref position);
                x = packet.ReadInt(ref position);
                y = packet.ReadInt(ref position);
                z = packet.ReadString(ref position);
                packet.ReadInt(ref position);
                playerType = packet.ReadInt(ref position);

                switch (playerType)
                {
                    case 1:
                    {
                        gender = packet.ReadString(ref position);
                        packet.ReadInt(ref position);
                        packet.ReadInt(ref position);
                        groupName = packet.ReadString(ref position);
                        packet.ReadString(ref position);
                        packet.ReadInt(ref position);
                        packet.ReadBool(ref position);

                        playerDataList.Add(new HPlayerData(playerName, playerId, playerIndex,
                            new HPoint(x, y, z), playerFigureId, motto, SKore.ConvertToHGender(gender), groupName));
                        break;
                    }
                    case 2:
                    {
                        packet.ReadInt(ref position);
                        packet.ReadInt(ref position);
                        packet.ReadString(ref position);
                        packet.ReadInt(ref position);
                        packet.ReadBool(ref position);
                        packet.ReadBool(ref position);
                        packet.ReadBool(ref position);
                        packet.ReadBool(ref position);
                        packet.ReadBool(ref position);
                        packet.ReadBool(ref position);
                        packet.ReadInt(ref position);
                        packet.ReadString(ref position);
                        break;
                    }
                    case 4:
                    {
                        packet.ReadString(ref position);
                        packet.ReadInt(ref position);
                        packet.ReadString(ref position);
                        for (int i = packet.ReadInt(ref position); i > 0; i--)
                            packet.ReadShort(ref position);
                        break;
                    }
                }
            }
            while (playerDataList.Count < playerDataList.Capacity);

            return playerDataList;
        }

        public override string ToString() => "PlayerName: \{PlayerName} | PlayerId: \{PlayerId} | PlayerIndex: \{PlayerIndex} | " +
            "Tile: \{Tile} | FigureId: \{PlayerFigureId.Remove(6, PlayerFigureId.Length - 6)} | Motto: \{Motto} | Gender: \{Gender}";
    }
}