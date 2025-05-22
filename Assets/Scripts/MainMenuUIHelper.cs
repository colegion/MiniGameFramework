using System;
using System.Collections;
using System.Collections.Generic;
using Helpers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuUIHelper : MonoBehaviour
{
    [SerializeField] private TMP_Dropdown gameModeDropdown;
    [SerializeField] private Button playGameButton;

    private GameMode _selectedGameMode;

    private void OnEnable()
    {
        AddListeners();
    }

    private void OnDisable()
    {
        RemoveListeners();
    }

    private void Start()
    {
        PopulateDropDown();
    }

    private void PopulateDropDown()
    {
        gameModeDropdown.ClearOptions();
            List<string> options = new List<string>(Enum.GetNames(typeof(GameMode)));
            gameModeDropdown.AddOptions(options);
        
            gameModeDropdown.onValueChanged.AddListener(index =>
            {
                _selectedGameMode = (GameMode)index;
            });
    }

    private void RequestGame()
    {
        var type = _selectedGameMode == GameMode.PistiGame ? SceneType.PistiGame : SceneType.LinkGame;
        SceneLoader.LoadSceneAsync(type);
    }

    private void AddListeners()
    {
        playGameButton.onClick.AddListener(RequestGame);
    }

    private void RemoveListeners()
    {
        playGameButton.onClick.RemoveListener(RequestGame);
    }
}
