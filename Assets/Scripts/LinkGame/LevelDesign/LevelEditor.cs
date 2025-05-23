using System.Collections.Generic;
using GridSystem;
using Helpers;
using LinkGame.Helpers;
using LinkGame.LevelDesign;
using Pool;
using ScriptableObjects.Chip;
using UnityEngine;
using UnityEngine.Serialization;
using Grid = GridSystem.Grid;

namespace LinkGame.LevelDesign
{
    public class LevelEditor : MonoBehaviour
    {
        public int boardWidth = 5;
        public int boardHeight = 5;
        public Transform puzzleParent;
        public BaseCell editorCell;
        public PoolController poolController;
        public ChipConfigManager chipConfigManager;
        
        
        [HideInInspector] public EditorTile selectedTile;

        private Grid _grid;

        private void Start()
        {
            poolController.Initialize();
        }

        public void GenerateBoard()
        {
            ClearBoard();
            _grid = new Grid(boardWidth, boardHeight);
            ServiceLocator.Register(_grid);
            BoardUtility.CreateCells(boardWidth, boardHeight, puzzleParent, editorCell, editorCell, this);
            Debug.Log($"[Editor] Generated {boardWidth}x{boardHeight} board.");
        }

        public void SpawnTileAt(int x, int y, ChipType chipType = ChipType.Circle)
        {
            var config = chipConfigManager.GetItemConfig(chipType);
            if (config == null)
            {
                Debug.LogError($"ChipConfig not found for {chipType}");
                return;
            }

            var tile = (EditorTile)poolController.GetPooledObject(PoolableTypes.EditorTile);
            tile.transform.SetParent(puzzleParent);
            tile.gameObject.SetActive(true);
            tile.ConfigureSelf(config, x, y);
            tile.InjectEditor(this);
        }

        public void RemoveTileAt(int x, int y)
        {
            var tile = _grid.GetCell(x, y).GetTile(LinkUtilities.DefaultChipLayer);
            poolController.ReturnPooledObject(tile);
        }

        public void ClearBoard()
        {
            foreach (Transform child in puzzleParent)
            {
                DestroyImmediate(child.gameObject);
            }
        }
        
        public void SetSelectedTile(EditorTile tile)
        {
            selectedTile = tile;
        }
        
        public LevelData SaveLevel()
        {
            var levelData = new LevelData();
            levelData.linkLevelConfig.boardWidth = boardWidth;
            levelData.linkLevelConfig.boardHeight = boardHeight;
            levelData.tiles = new List<TileData>();

            foreach (var cell in _grid.GetBoard())
            {
                var tile = cell.GetTile(LinkUtilities.DefaultChipLayer); 
                if (tile != null)
                {
                    levelData.tiles.Add(tile.TileData);
                }
            }
            return levelData;
        }
    }
}
