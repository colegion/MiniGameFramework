using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Helpers;
using Interfaces;
using PistiGame.Helpers;
using PistiGame.PistiInterfaces;
using UnityEngine;

namespace PistiGame.GameStates
{
    public class CardDistributionState : IGameState
    {
        private const float CardAnimationDelay = 0.25f;
        private const int CardAmount = 4;
        private bool _initialDistributionCompleted;
        private int _roundIndex = 0;
        private Sequence _tableCardSequence;
        public static event Action<int, Action> OnRoundDistributed;
    
        private PistiGameContext _context;
        public void EnterState(object context)
        {
            if(_context == null)
                _context = context as PistiGameContext;
            if (_context != null && _context.CheckIfGameFinished())
            {
                return;
            }
            else
            {
                DistributeCards();
            }
        }
    
        private void DistributeCards()
        {
            if (!_initialDistributionCompleted)
            {
                _initialDistributionCompleted = true;
                DistributeTableCards(() =>
                {
                    DistributeUserCards(ExitState);
                });
            }
            else
            {
                DistributeUserCards(ExitState);
            }
        }

        private void DistributeTableCards(Action onComplete)
        {
            _tableCardSequence = DOTween.Sequence();
            var sequence = _tableCardSequence;
            Debug.Log("Distributing table cards...");

            for (int i = 0; i < CardAmount; i++)
            {
                var config = _context.GetRandomConfig();
                if (config.cardValue == CardValue.Null)
                {
                    Debug.LogError("Deck is empty while distributing table cards!");
                    break;
                }

                var card = _context.GetCard();
                card.ConfigureSelf(config, i < CardAmount - 1);
                _context.RemoveCardFromDeck(config);
                Debug.Log($"Added {config.cardValue} to table.");

                sequence.AppendCallback(() =>
                {
                    _context.AppendCardsOnTable(card);
                });

                sequence.AppendInterval(CardAnimationDelay);
            }

            sequence.OnComplete(() =>
            {
                Debug.Log("Table cards distributed.");
                onComplete?.Invoke();
            });
        }


        private void DistributeUserCards(Action onComplete)
        {
            var users = _context.GetAllUsers();
            RoutineHelper.Instance.StartRoutine(DistributeUserCardsCoroutine(users, CardAmount, onComplete));
        }

        private IEnumerator DistributeUserCardsCoroutine(List<User> users, int cardAmount, Action onComplete)
        {
            Debug.Log($"Starting user card distribution. Expected cards per user: {cardAmount}");
    
            for (int i = 0; i < cardAmount; i++)
            {
                for (int j = 0; j < users.Count; j++)
                {
                    var user = users[j];
                    var faceDown = user is not Player;

                    var config = _context.GetRandomConfig();
                    if (config.cardValue == CardValue.Null)
                    {
                        Debug.LogError("Deck is out of cards!");
                        yield break;
                    }

                    var card = _context.GetCard();
                    card.ConfigureSelf(config, faceDown);
                    _context.RemoveCardFromDeck(config);
                    user.AddCardToHand(card);

                    Debug.Log($"Distributed card {config.cardValue} to {user.GetType()}");

                    bool animationCompleted = false;
                    user.ReceiveSingleCard(card, () => animationCompleted = true);

                    yield return new WaitUntil(() => animationCompleted);
                    yield return new WaitForSeconds(0.1f);
                }
            }

            Debug.Log("User card distribution complete.");
            onComplete?.Invoke();
        }
        
        public void ExitState()
        {
            _roundIndex++;
            _context.CheckRemainingCardAmount();
            OnRoundDistributed?.Invoke(_roundIndex, () =>
            {
                var playerTurn = _context.GetStateByType(GameStateTypes.PlayerTurn);
                _context.ChangeState(playerTurn);
            });
        
        }

        public void ResetAttributes()
        {
            if (_tableCardSequence != null && _tableCardSequence.IsActive())
            {
                _tableCardSequence.Kill();
                _tableCardSequence = null;
            }

            _initialDistributionCompleted = false;
            _roundIndex = 0;
        }
    }
}
