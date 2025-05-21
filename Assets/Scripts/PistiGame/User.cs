using System;
using System.Collections.Generic;
using Helpers;
using Interfaces;
using PistiGame.Helpers;
using UnityEngine;

namespace PistiGame
{
    public abstract class User : MonoBehaviour
    {
        [SerializeField] protected CardAnimator cardAnimator;
        [SerializeField] private Hand hand;
        [SerializeField] private Table table;

        protected List<Card> Cards = new List<Card>();
        protected IGameState UserState;

        private List<CardConfig> _collectedCards = new List<CardConfig>();
        private int _pistiCount;

        public event Action<int, int> OnCollectedCardsUpdated;

        public void InjectUserState(IGameState state)
        {
            if (UserState != null) return;
            UserState = state;
        }

        public void AddCardToHand(Card card)
        {
            Cards.Add(card);
        }
    
        public void ReceiveSingleCard(Card card, Action onComplete)
        {
            var slot = hand.GetAvailableSlot();
            slot.SetCardReference(card);
            bool isPlayer = this is not Bot;
            cardAnimator.AnimateSelectedCard(card, slot.GetTarget().position, isPlayer,
                () => onComplete?.Invoke());
        }

        public void CollectCards(List<Card> cards, CollectType type, Action onComplete)
        {
            foreach (var card in cards)
            {
                _collectedCards.Add(card.GetConfig());
            }

            TriggerCollectedCardsAnimation(cards, type, onComplete);
        }

        private void TriggerCollectedCardsAnimation(List<Card> cards, CollectType type, Action onComplete)
        {
            cardAnimator.DecideAnimationToUse(cards, transform, type, () =>
            {
                OnCollectedCardsUpdated?.Invoke(_collectedCards.Count, GetTotalGatheredPoints());
                onComplete?.Invoke();
            });
        }
    
        private int GetTotalGatheredPoints()
        {
            var total = 0;
            foreach (var card in _collectedCards)
            {
                total += card.point;
            }

            total += PistiUtilities.PistiPoint * _pistiCount;

            return total;
        }

        public List<CardConfig> GetCollectedCards()
        {
            return _collectedCards;
        }

        public void IncrementPistiCount()
        {
            _pistiCount++;
        }

        public bool IsHandEmpty()
        {
            return Cards.Count == 0;
        }

        public void ResetAttributes()
        {
            Cards.Clear();
            _pistiCount = 0;
            _collectedCards.Clear();
            OnCollectedCardsUpdated?.Invoke(0, 0);
        }

        public virtual void OnCardPlayed(Card card)
        {
            Debug.Log($"Played card {card.GetConfig().cardValue} {card.GetConfig().cardSuit}");
            card.ToggleInteractable(false);
            Cards.Remove(card);
            hand.EmptySlotByCard(card);
            PistiGameController.Instance.AppendCardsOnTable(card);
            cardAnimator.AnimateSelectedCard(card, table.GetCardTarget(), true, () => { UserState.ExitState(); });
        }

        public abstract void OnTurnStart();
    }
}