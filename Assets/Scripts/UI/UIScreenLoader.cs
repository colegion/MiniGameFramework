using System;
using Helpers;
using UnityEngine;

namespace UI
{
    public class UIScreenLoader : MonoBehaviour
    {
        [SerializeField] private Canvas mainCanvas;
        private const string SuccessPopupPath = "Prefabs/UI/SuccessInfoPopup";
        private const string FailPopupPath = "Prefabs/UI/FailInfoPopup";

        private void OnEnable()
        {
            AddListeners();
        }

        private void OnDisable()
        {
            RemoveListeners();
        }

        public void LoadPopup(bool isSuccess, GameMode mode)
        {
            var path = isSuccess ? SuccessPopupPath : FailPopupPath;
            var popupPrefab = Resources.Load<GameObject>(path);
    
            if (popupPrefab != null)
            {
                var popupInstance = Instantiate(popupPrefab, mainCanvas.transform);
                popupInstance.transform.localPosition = Vector3.zero;
                popupInstance.GetComponent<InfoPopup>().Initialize(mode);
                popupInstance.SetActive(true);
            }
            else
            {
                Debug.LogError($"Failed to load prefab at path: {path}");
            }
        }

        private void HandleOnGameOver(bool isSuccess, GameMode mode)
        {
            LoadPopup(isSuccess, mode);
        }

        private void AddListeners()
        {
            GameController.OnGameOver += HandleOnGameOver;
        }
        
        private void RemoveListeners()
        {
            GameController.OnGameOver -= HandleOnGameOver;
        }
    }
}
