using System;
using System.Collections;
using System.Collections.Generic;
using Controllers;
using GridSystem;
using Helpers;
using Interfaces;
using LinkGame.Controllers;
using Pool;
using ScriptableObjects.Level;
using Unity.VisualScripting;
using UnityEngine;
using Grid = GridSystem.Grid;

namespace LinkGame
{
    public class LinkGameContext : IGameContext, ITileTracker
    {
        private readonly LevelConfig _levelConfig;
        private readonly Transform _puzzleParent;
        private readonly CameraController _cameraController;
        private readonly LinkInputController _linkInputController;
        
        private LinkModeLevelManager _linkModeLevelManager;
        private LinkSearcher _linkSearcher;
        private ShuffleController _shuffleController;
        private LevelProgressTracker _tracker;
        private TileLinkController _linkController;
        private TileHighlightController _highlightController;
        private TileFallController _fallController;
        private TileFillController _fillController;

        private List<TileData> _levelTiles = new();
        
        public int GridWidth => _levelConfig.boardWidth;
        public int GridHeight => _levelConfig.boardHeight;
        
        public static event Action<LevelConfig> OnLevelLoaded;
        public static event Action<LevelTargetConfig> OnSuccessfulMove;

        public LinkGameContext(LevelConfig config, Transform parent, CameraController camera, LinkInputController linkInput)
        {
            _levelConfig = config;
            _puzzleParent = parent;
            _cameraController = camera;
            _linkInputController = linkInput;
        }

        public void Initialize()
        {
            _cameraController.SetGridSize(_levelConfig.boardWidth, _levelConfig.boardHeight);
            _linkController = ServiceLocator.Get<TileLinkController>();
            _fallController = ServiceLocator.Get<TileFallController>();
            _fillController = ServiceLocator.Get<TileFillController>();
            _highlightController = ServiceLocator.Get<TileHighlightController>();
            _shuffleController = ServiceLocator.Get<ShuffleController>();

            _linkSearcher = new LinkSearcher();
            ServiceLocator.Register(_linkSearcher);

            _linkModeLevelManager = new LinkModeLevelManager(_puzzleParent, this);
            _tracker = new LevelProgressTracker(_levelConfig);

            OnLevelLoaded?.Invoke(_levelConfig);
            StartGame();
        }

        public void StartGame()
        {
            _linkInputController.ToggleInput(true);
        }

        public void EndGame()
        {
            _linkInputController.ToggleInput(false);
            _linkModeLevelManager.ClearProgress();
        }

        public void Cleanup()
        {
            RestartLevel();
        }

        public void TryAppendToCurrentLink(ITappable tappable)
        {
            _linkController.TryAppendToCurrentLink(tappable);
        }

        public void HighlightAdjacentTiles(BaseTile origin)
        {
            _highlightController.HighlightAdjacentTiles(origin);
        }

        public void HandleOnRelease()
        {
            RoutineHelper.Instance.StartRoutine(HandleOnReleaseRoutine());
        }

        private IEnumerator HandleOnReleaseRoutine()
        {
            if (!_linkController.IsLinkProcessable()) yield break;

            _linkInputController.ToggleInput(false);
            _highlightController.ClearPreviousHighlights();

            _fallController.FillFallConfig(_linkController.GetCurrentLink());

            bool linkProcessed = false;

            yield return _linkController.TriggerLinkProcess((chipType, count) =>
            {
                if (count >= Utilities.LinkThreshold)
                {
                    var config = new LevelTargetConfig
                    {
                        targetType = chipType,
                        count = count
                    };
                    _tracker.RegisterMove(config);
                    OnSuccessfulMove?.Invoke(config);
                    linkProcessed = true;
                }
            });

            if (!linkProcessed) yield break;

            _fallController.TriggerDrop();
            yield return new WaitForSeconds(0.3f);

            _fillController.TriggerFillProcess(_fallController.GetEmptyRowsByColumn());
            yield return new WaitForSeconds(0.3f);

            if (!_linkSearcher.HasPossibleLink())
            {
                _shuffleController.TriggerShuffle();
                yield return new WaitForSeconds(0.5f);
            }

            _linkInputController.ToggleInput(true);
        }
        
        public void RestartLevel()
        {
            _levelTiles.Clear();
            Transform[] children = new Transform[_puzzleParent.childCount];
            for (int i = 0; i < children.Length; i++)
            {
                children[i] = _puzzleParent.GetChild(i);
            }

            foreach (Transform child in children)
            {
                var poolable = child.GetComponent<BaseTile>();
                if (poolable != null)
                {
                    GameController.Instance.ReturnPooledObject(poolable);
                }
            }
            
            ServiceLocator.Get<Grid>().Clear();
            _tracker.Reset();
            _linkModeLevelManager.CreateRandomBoard(GridWidth, GridHeight);
            _linkInputController.ToggleInput(true);
            OnLevelLoaded?.Invoke(_levelConfig);
        }

        public void AppendTileData(TileData data) => _levelTiles.Add(data);
        public void RemoveTileData(TileData data) => _levelTiles.Remove(data);
        public void ReturnTileToPool(BaseTile tile)
        {
            GameController.Instance.PoolController.ReturnPooledObject(tile);
        }
        
        public Transform GetPuzzleParent()
        {
            return _puzzleParent;
        }
        
        public void OnLevelFinished(bool isSuccess)
        {
            _linkInputController.ToggleInput(false);
            _linkModeLevelManager.ClearProgress();
            GameController.Instance.TriggerOnGameOver(isSuccess);
            EndGame();
        }
    }
}
