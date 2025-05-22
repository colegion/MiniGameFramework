using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;

namespace Helpers
{
    public static class SceneLoader
    {
        private static readonly Dictionary<SceneType, AsyncOperationHandle<SceneInstance>> LoadedScenes = new();

        public static void LoadSceneAsync(SceneType type, bool additive = false)
        {
            string sceneAddress = type.ToString();
            Debug.Log("scene " + sceneAddress);

            var handle = Addressables.LoadSceneAsync(sceneAddress, additive ? LoadSceneMode.Additive : LoadSceneMode.Single);
            
            handle.Completed += op =>
            {
                if (op.Status == AsyncOperationStatus.Succeeded)
                {
                    if (additive)
                        LoadedScenes[type] = handle;
                }
                else
                {
                    Debug.LogError($"Failed to load scene: {sceneAddress}");
                }
            };
        }

        public static void UnloadSceneAsync(SceneType type)
        {
            if (LoadedScenes.TryGetValue(type, out var handle))
            {
                Addressables.UnloadSceneAsync(handle);
                LoadedScenes.Remove(type);
            }
            else
            {
                Debug.LogWarning($"Scene {type} is not tracked or not loaded additively.");
            }
        }
    }
}