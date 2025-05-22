using System;
using System.Collections;
using System.Collections.Generic;
using Helpers;
using PistiGame.Helpers;
using Pool;
using UnityEngine;
using UnityEngine.Serialization;

namespace PistiGame
{
    public class PistiBootstrapper : BaseBootstrapper
    {
        [SerializeField] private CardInputController cardInputController;
        [SerializeField] private CardAnimator cardAnimator;
        [SerializeField] private Table table;
        [SerializeField] private List<User> users;

        private BotType _botType;

        private void Awake()
        {
            RegisterEvents();
        }

        private void HandleOnGameRequested()
        {
            StartCoroutine(InitializeDependencies());
        }

        public override IEnumerator InitializeDependencies()
        {
            var poolController = FindObjectOfType<PoolController>();
            ServiceLocator.Register(poolController);
            
            var context = new PistiGameContext(cardInputController, cardAnimator, table, users);
            context.SetBotType(_botType);
            GameController.Instance.SetGameContext(context);
            yield return null;
        }

        private void SetBotType(BotType type)
        {
            _botType = type;
        }
        
        private void OnDifficultySelectedHandler(OnDifficultySelected evt)
        {
            SetBotType(evt.BotType);
        }

        private void OnGameRequestHandler(OnCardGameRequested obj)
        {
            HandleOnGameRequested();
        }
        
        private void RegisterEvents()
        {
            EventBus.Register<OnDifficultySelected>(OnDifficultySelectedHandler);
            EventBus.Register<OnCardGameRequested>(OnGameRequestHandler);
        }

        private void UnRegisterEvents()
        {
            EventBus.Unregister<OnDifficultySelected>(OnDifficultySelectedHandler);
            EventBus.Unregister<OnCardGameRequested>(OnGameRequestHandler);
        }

        private void OnDestroy()
        {
            UnRegisterEvents();
        }
    }
}
