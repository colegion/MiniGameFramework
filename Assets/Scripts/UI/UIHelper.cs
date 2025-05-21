using System;
using Controllers;
using LinkGame;
using LinkGame.Controllers;
using ScriptableObjects;
using ScriptableObjects.Level;
using TMPro;
using UnityEngine;

namespace UI
{
    public class UIHelper : MonoBehaviour
    {
        [SerializeField] private Canvas mainCanvas;
        [SerializeField] private TextMeshProUGUI moveLimitField;
        [SerializeField] private TargetUIManager targetUIManager;

        private int _moveCount;
        private void OnEnable()
        {
            AddListeners();
        }

        private void OnDisable()
        {
            RemoveListeners();
        }

        private void HandleOnLevelLoaded(LevelConfig config)
        {
            targetUIManager.Reset();
            _moveCount = config.moveLimit;
            moveLimitField.text = $"{_moveCount}";
            LevelConfig.MergeDuplicateTargets(config.levelTargets);
            targetUIManager.Initialize(config.levelTargets);
        }

        private void HandleOnMove(LevelTargetConfig moveConfig)
        {
            _moveCount--;
            moveLimitField.text = $"{_moveCount}";
            targetUIManager.OnMove(moveConfig);
        }
        private void AddListeners()
        {
            LinkGameContext.OnLevelLoaded += HandleOnLevelLoaded;
            LinkGameContext.OnSuccessfulMove += HandleOnMove;
        }
        
        private void RemoveListeners()
        {
            LinkGameContext.OnLevelLoaded -= HandleOnLevelLoaded;
            LinkGameContext.OnSuccessfulMove -= HandleOnMove;
        }
    }
}
