using Sulakore.Protocol;
using System.Collections.Generic;

namespace Sulakore.Habbo
{
    public class HPlayerData : IHPlayerData
    {
        public int PlayerId { get; private set; }
        public int PlayerIndex { get; private set; }
        public string PlayerName { get; private set; }

        public HPoint Tile { get; private set; }
        public HGender Gender { get; private set; }

        public string Motto { get; private set; }
        public string FigureId { get; private set; }
        public string GroupName { get; private set; }

        public HPlayerData(string playerName, int playerId, int playerIndex,
            HPoint tile, string figureId, string motto, HGender gender, string groupName)
        {
            PlayerName = playerName;
            PlayerId = playerId;
            PlayerIndex = playerIndex;
            Tile = tile;
            FigureId = figureId;
            Motto = motto;
            Gender = gender;
            GroupName = groupName;
        }

        public static IList<IHPlayerData> Extract(HMessage packet)
        {
            int playerId, playerIndex, playerType, x, y, position = 0;
            string playerName, figureId, motto, gender, groupName, z;
            var playerDataList = new List<IHPlayerData>(packet.ReadInt(ref position));

            do
            {
                playerId = playerIndex = playerType = x = y = 0;
                playerName = figureId = motto = gender = groupName = z = string.Empty;

                playerId = packet.ReadInt(ref position);
                playerName = packet.ReadString(ref position);
                motto = packet.ReadString(ref position);
                figureId = packet.ReadString(ref position);
                playerIndex = packet.ReadInt(ref position);
                x = packet.ReadInt(ref position);
                y = packet.ReadInt(ref position);
                z = packet.ReadString(ref position);
                packet.ReadInt(ref position);
                playerType = packet.ReadInt(ref position);

                if (playerType != 1)
                    playerDataList.Capacity--;

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
                            new HPoint(x, y, z), figureId, motto, SKore.ToGender(gender), groupName));
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

        public override string ToString()
        {
            return string.Format("PlayerName: {0} | PlayerId: {1} | PlayerIndex: {2} | Tile: {3} | FigureId: {4}... | Motto: {5} | Gender: {6} | GroupName: {7}",
                PlayerName, PlayerId, PlayerIndex, Tile, FigureId.Remove(10, FigureId.Length - 10), Motto, Gender, GroupName);
        }
    }
}