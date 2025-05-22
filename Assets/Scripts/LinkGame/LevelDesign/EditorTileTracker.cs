using GridSystem;
using Interfaces;

namespace LinkGame.LevelDesign
{
    public class EditorTileTracker : ITileTracker
    {
        public void AppendTileData(TileData data) { }
        public void RemoveTileData(TileData data) { }
        public void ReturnTileToPool(BaseTile tile) { }
    }
}