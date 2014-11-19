using Sulakore.Habbo;

namespace Sulakore.Habbo
{
    public interface IHPlayerData
    {
        string PlayerName { get; }
        int PlayerId { get; }
        int PlayerIndex { get; }
        HPoint Tile { get; }
        string FigureId { get; }
        string Motto { get; }
        HGenders Gender { get; }
        string GroupName { get; }
    }
}