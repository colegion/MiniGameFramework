using System;
using System.Collections.Generic;
using GridSystem;
using Helpers;
using Interfaces;
using LinkGame.Helpers;
using LinkGame.LevelDesign;
using Pool;
using ScriptableObjects.Chip;
using ScriptableObjects.Level;
using UnityEngine;
using UnityEngine.Serialization;
using Grid = GridSystem.Grid;

namespace LinkGame.LevelDesign
{
    public class LevelEditor : MonoBehaviour
    {
        public int boardWidth = 5;
        public int boardHeight = 5;
        public int moveLimit = 20;
        [HideInInspector] public Transform puzzleParent;
        [HideInInspector] public BaseCell editorCell;
        [HideInInspector] public PoolController poolController;
        [HideInInspector] public ChipConfigManager chipConfigManager;
        [HideInInspector] public CameraController cameraController;
        [HideInInspector] public EditorTile selectedTile;

        private Grid _grid;
        private LinkModeLevelManager _levelManager;

        private void Start()
        {
            poolController.Initialize();
            _levelManager = new LinkModeLevelManager();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.P))
            {
                SaveLevel();
                SceneLoader.LoadSceneAsync(SceneType.LinkGame);
            }
        }

        public void GenerateBoard()
        {
            ClearBoard();
            _grid = new Grid(boardWidth, boardHeight);
            ServiceLocator.Register(_grid);
            BoardUtility.CreateCells(boardWidth, boardHeight, puzzleParent, editorCell, editorCell, this);
            Debug.Log($"[Editor] Generated {boardWidth}x{boardHeight} board.");
            cameraController.SetGridSize(boardWidth, boardHeight);
        }

        public void SpawnTileAt(int x, int y, ChipType chipType = ChipType.Circle)
        {
            var config = chipConfigManager.GetItemConfig(chipType);
            if (config == null)
            {
                Debug.LogError($"ChipConfig not found for {chipType}");
                return;
            }

            if (_grid.GetCell(x, y).GetTile(LinkUtilities.DefaultChipLayer) != null) return;

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
            if (_grid != null)
            {
                var board = _grid.GetBoard();

                for (int x = 0; x < _grid.Width; x++)
                {
                    for (int y = 0; y < _grid.Height; y++)
                    {
                        var cell = board[x, y];
                        if (cell != null)
                        {
                            var tile = cell.GetTile(LinkUtilities.DefaultChipLayer);
                            if (tile != null)
                            {
                                poolController.ReturnPooledObject(tile);
                            }
                        }
                    }
                }

                _grid.Clear();
            }

            for (int i = puzzleParent.childCount - 1; i >= 0; i--)
            {
                Transform child = puzzleParent.GetChild(i);
#if UNITY_EDITOR
                DestroyImmediate(child.gameObject);
#else
    Destroy(child.gameObject);
#endif
            }

            Debug.Log("[Editor] Board cleared.");

            ServiceLocator.Unregister<Grid>();
        }

        public void SetSelectedTile(EditorTile tile)
        {
            selectedTile = tile;
        }

        public void SaveLevel()
        {
            var levelData = new LevelData();
            
            var linkLevelConfig = new SerializableLinkLevelConfig
            {
                boardWidth = boardWidth,
                boardHeight = boardHeight,
                moveLimit = moveLimit,
                levelTargets = new List<LevelTargetConfig>()
            };

            levelData.linkLevelConfig = linkLevelConfig;
            levelData.tiles = new List<TileData>();

            foreach (var cell in _grid.GetBoard())
            {
                var tile = cell.GetTile(LinkUtilities.DefaultChipLayer);
                if (tile != null)
                {
                    levelData.tiles.Add(tile.TileData);
                }
            }

            _levelManager.SaveLevelToResources(levelData);
        }

    }
}