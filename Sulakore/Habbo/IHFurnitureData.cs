using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sulakore.Habbo
{
    public interface IHFurnitureData
    {
        int FurnitureOwnerId { get; }
        string FurnitureOwnerName { get; }

        int FurnitureId { get; }
        int FurnitureTypeId { get; }

        HPoint Tile { get; set; }
        HDirections Direction { get; set; }

        int State { get; set; }
    }
}