namespace Sulakore.Habbo
{
    public interface IHFurnitureData
    {
        int FurnitureOwnerId { get; }
        string FurnitureOwnerName { get; }

        int FurnitureId { get; }
        int FurnitureTypeId { get; }

        int State { get; set; }
        HPoint Tile { get; set; }
        HDirection Direction { get; set; }
    }
}