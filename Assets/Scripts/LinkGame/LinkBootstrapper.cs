using System;
using System.Collections;
using System.Linq;
using Controllers;
using Helpers;
using Interfaces;
using LinkGame.Controllers;
using Pool;
using ScriptableObjects.Level;
using UnityEngine;
using UnityEngine.Serialization;
using Grid = GridSystem.Grid;

namespace LinkGame
{
    public class LinkBootstrapper : BaseBootstrapper
    {
        [FormerlySerializedAs("levelConfig")] [SerializeField] private LinkLevelConfig linkLevelConfig;
        [SerializeField] private Transform puzzleParent;
        [SerializeField] private CameraController cameraController;
        [SerializeField] private LinkInputController inputController;

        private void Awake()
        {
            StartCoroutine(InitializeDependencies());
        }

        public override IEnumerator InitializeDependencies()
        {
            var configManager = FindObjectOfType<ChipConfigManager>();
            while (!configManager.IsReady)
                yield return null;

            ServiceLocator.Register(configManager);
            Debug.Log("Bootstrapper: Registering dependencies...");

            // Register runtime dependencies
            var poolController = FindObjectOfType<PoolController>();
            var tileFactory = FindObjectOfType<TileFactory>();
            var highlightController = FindObjectOfType<TileHighlightController>();
            var linkController = FindObjectOfType<TileLinkController>();
            var fallController = FindObjectOfType<TileFallController>();
            var fillController = FindObjectOfType<TileFillController>();
            var shuffleManager = FindObjectOfType<ShuffleController>();

            ServiceLocator.Register(poolController);
            ServiceLocator.Register(tileFactory);
            ServiceLocator.Register(highlightController);
            ServiceLocator.Register(linkController);
            ServiceLocator.Register(fallController);
            ServiceLocator.Register(fillController);
            ServiceLocator.Register(shuffleManager);

            // Create and register grid before context
            var tempContext = new LinkGameContext(linkLevelConfig, puzzleParent, cameraController, inputController);
            var grid = new Grid(tempContext.GridWidth, tempContext.GridHeight);
            ServiceLocator.Register(grid);

            // Inject other dependencies
            foreach (var injectable in FindObjectsOfType<MonoBehaviour>().OfType<IInjectable>())
                injectable.InjectDependencies();

            // Set context
            GameController.Instance.SetGameContext(tempContext);
            Debug.Log("Bootstrapper: Dependencies injected, loading level...");
        }
    }
}
