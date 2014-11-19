using System;
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
        public HPoint Tile { get; set; }
        public HDirections Direction { get; set; }
        public int State { get; set; }

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

        public static IEnumerable<IHFurnitureData> Extract(HMessage packet)
        {
            string ownerName = string.Empty;
            int totalOwners, ownerId, position = 0;
            var furnitureDataList = new List<HFurnitureData>();
            var furniOwners = new Dictionary<int, string>(totalOwners = packet.ReadInt(ref position));

            try
            {
                do
                {
                    ownerId = packet.ReadInt(ref position);
                    ownerName = packet.ReadString(ref position);
                    furniOwners[ownerId] = ownerName;
                }
                while (furniOwners.Count < totalOwners);

                int furniCount = packet.ReadInt(ref position);
                while (furniCount > 0)
                {
                    int furnitureId = packet.ReadInt(ref position);
                    int furnitueTypeId = packet.ReadInt(ref position);

                    int x = packet.ReadInt(ref position);
                    int y = packet.ReadInt(ref position);
                    HDirections direction = (HDirections)packet.ReadInt(ref position);
                    string z = packet.ReadString(ref position);

                    string a1 = packet.ReadString(ref position);
                    int a2 = packet.ReadInt(ref position);

                    //ODC
                    int a3 = packet.ReadInt(ref position) & 0xFF;
                    string a4 = packet.ReadString(ref position);
                    int state = !string.IsNullOrEmpty(a4) ? int.Parse(a4) : 0;

                    int a5 = packet.ReadInt(ref position);
                    int a6 = packet.ReadInt(ref position);
                    ownerId = packet.ReadInt(ref position);

                    string a8 = furnitueTypeId < 0 ? packet.ReadString(ref position) : string.Empty;

                    furnitureDataList.Add(new HFurnitureData(ownerId, furniOwners[ownerId], furnitureId, furnitueTypeId, new HPoint(x, y, z), direction, state));
                    furniCount--;
                }
            }
            catch (Exception ex) { System.Windows.Forms.MessageBox.Show(ex.ToString()); }

            return furnitureDataList;
        }

        public override string ToString()
        {
            return string.Format("FurnitureOwnerId: {0}, FurnitureOwnerName: {1}, FurnitureId: {2}, FurnitureTypeId: {3}, Tile: {4}, Direction: {5}, State: {6}",
                FurnitureOwnerId, FurnitureOwnerName, FurnitureId, FurnitureTypeId, Tile, Direction, State);
        }
    }
}