using GridSystem;
using Helpers;
using LinkGame;

namespace Interfaces
{
    public interface ITileTracker
    {
        void AppendTileData(TileData data);
        void RemoveTileData(TileData data);
        void ReturnTileToPool(BaseTile tile);
    }
}