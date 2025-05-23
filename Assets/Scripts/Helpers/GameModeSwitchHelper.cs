using System;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Helpers
{
    public class GameModeSwitchHelper : MonoBehaviour
    {
        [SerializeField] private GameMode currentGameMode;
        [SerializeField] private Button switchButton;

        private void OnEnable()
        {
            AddListeners();
        }

        private void OnDisable()
        {
            RemoveListeners();
        }

        private void SwitchGame()
        {
            GameController.Instance.SwitchGameMode(currentGameMode);
        }

        private void AddListeners()
        {
            switchButton.onClick.AddListener(SwitchGame);
        }

        private void RemoveListeners()
        {
            switchButton.onClick.RemoveListener(SwitchGame);       
        }
    }
}
