using Sulakore.Protocol;
using System.Collections.Generic;

namespace Sulakore.Habbo
{
    public class HFurnitureData : IHFurnitureData
    {
        public int FurnitureOwnerId { get; private set; }
        public string FurnitureOwnerName { get; private set; }

        public int FurnitureId { get; private set; }
        public int FurnitureTypeId { get; private set; }

        public int State { get; set; }
        public HPoint Tile { get; set; }
        public HDirections Direction { get; set; }

        public HFurnitureData(int furnitureOwnerId, string furnitureOwnerName,
            int furnitureId, int furnitureTypeId, HPoint tile, HDirections direction, int state)
        {
            FurnitureOwnerId = furnitureOwnerId;
            FurnitureOwnerName = furnitureOwnerName;
            FurnitureId = furnitureId;
            furnitureTypeId = FurnitureTypeId;
            Tile = tile;
            Direction = direction;
            State = state;
        }

        public static IList<IHFurnitureData> Extract(HMessage packet)
        {
            int furniOwnersCapacity, position = 0;
            var furniOwners = new Dictionary<int, string>(furniOwnersCapacity = packet.ReadInt(ref position));
            do furniOwners.Add(packet.ReadInt(ref position), packet.ReadString(ref position));
            while (furniOwners.Count < furniOwnersCapacity);

            string z, uk_2;
            HDirections direction;
            int ownerId, furnitureId, furnitureTypeId, x, y, state, uk_1;
            var furnitureDataList = new List<IHFurnitureData>(packet.ReadInt(ref position));
            do
            {
                furnitureId = packet.ReadInt(ref position);
                furnitureTypeId = packet.ReadInt(ref position);

                x = packet.ReadInt(ref position);
                y = packet.ReadInt(ref position);
                direction = (HDirections)packet.ReadInt(ref position);
                z = packet.ReadString(ref position);

                packet.ReadString(ref position);
                packet.ReadInt(ref position);

                uk_1 = packet.ReadInt(ref position) & 0xFF;
                int.TryParse(packet.ReadString(ref position), out state);

                packet.ReadInt(ref position);
                packet.ReadInt(ref position);
                ownerId = packet.ReadInt(ref position);
                if (furnitureTypeId < 0) uk_2 = packet.ReadString(ref position);

                furnitureDataList.Add(new HFurnitureData(ownerId, furniOwners[ownerId], furnitureId, furnitureTypeId, new HPoint(x, y, z), direction, state));
            }
            while (furnitureDataList.Count < furnitureDataList.Capacity);
            return furnitureDataList;
        }

        public HDirections RotateLeft()
        {
            int direction = (int)Direction;
            if (direction <= 0) direction = 8;
            return Direction = (HDirections)(direction - (direction % 2 == 0 ? 2 : 3));
        }
        public HDirections RotateRight()
        {
            int direction = (int)Direction;
            if (direction >= 6) direction = -2;
            return Direction = (HDirections)(direction + (direction % 2 == 0 ? 2 : 3));
        }

        public override string ToString()
        {
            return string.Format("FurnitureOwnerId: {0}, FurnitureOwnerName: {1}, FurnitureId: {2}, FurnitureTypeId: {3}, Tile: {4}, Direction: {5}, State: {6}",
                FurnitureOwnerId, FurnitureOwnerName, FurnitureId, FurnitureTypeId, Tile, Direction, State);
        }
    }
}