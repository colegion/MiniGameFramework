using System;
using System.Collections;
using System.IO;
using System.Linq;
using Controllers;
using Helpers;
using Interfaces;
using LinkGame.Controllers;
using LinkGame.Helpers;
using Pool;
using ScriptableObjects.Level;
using UnityEngine;
using UnityEngine.Serialization;
using Grid = GridSystem.Grid;

namespace LinkGame
{
    public class LinkBootstrapper : BaseBootstrapper
    {
        [SerializeField] private LinkLevelConfig linkLevelConfig;
        [SerializeField] private Transform puzzleParent;
        [SerializeField] private CameraController cameraController;
        [SerializeField] private LinkInputController inputController;

        private const string PersistentLevelFolder = "Levels";
        private const string PersistentLevelFileName = "CurrentLevel.json";

        private void Awake()
        {
            LoadSavedLevelIfExists();
            StartCoroutine(InitializeDependencies());
        }

        private void LoadSavedLevelIfExists()
        {
            string path = Path.Combine(Application.persistentDataPath, PersistentLevelFolder, PersistentLevelFileName);

            if (!File.Exists(path))
            {
                Debug.Log("[LinkBootstrapper] No saved level found. Using default config.");
                return;
            }

            try
            {
                string json = File.ReadAllText(path);
                var savedData = JsonUtility.FromJson<LevelData>(json);

                if (savedData != null)
                {
                    Debug.Log("[LinkBootstrapper] Loaded saved level. Overriding default config.");
                    linkLevelConfig.OverrideWith(savedData);
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"[LinkBootstrapper] Failed to load saved level: {ex.Message}");
            }
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
