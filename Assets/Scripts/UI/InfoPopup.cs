using System;
using System.Collections.Generic;
using Controllers;
using Helpers;
using LinkGame.Controllers;
using PistiGame.Helpers;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace UI
{
    public class InfoPopup : MonoBehaviour
    {
        [SerializeField] private Button popupButton;
        [SerializeField] private TMP_Dropdown botTypeDropdown;
        private BotType _selectedBotType;

        private void OnEnable()
        {
            popupButton.onClick.AddListener(Restart);
        }

        private void OnDisable()
        {
            popupButton.onClick.RemoveListener(Restart);
            botTypeDropdown.gameObject.SetActive(false);
        }

        public void Initialize(GameMode mode)
        {
            switch (mode)
            {
                case GameMode.PistiGame:
                    InitializeDropDown();
                    break;
            }
        }

        private void InitializeDropDown()
        {
            botTypeDropdown.gameObject.SetActive(true);
            botTypeDropdown.ClearOptions();
            List<string> options = new List<string>(Enum.GetNames(typeof(BotType)));
            botTypeDropdown.AddOptions(options);
        
            botTypeDropdown.onValueChanged.AddListener(index =>
            {
                _selectedBotType = (BotType)index;
                EventBus.Trigger(new OnDifficultySelected(_selectedBotType));
            });

            _selectedBotType = (BotType)botTypeDropdown.value;
        }
        
        private void Restart()
        {
            GameController.Instance.CurrentContext.Cleanup();
            gameObject.SetActive(false);
            Destroy(gameObject);
        }
    }
}
