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

        public void HandleOnGameRequested()
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

        public void SetBotType(BotType type)
        {
            _botType = type;
        }
    }
}
