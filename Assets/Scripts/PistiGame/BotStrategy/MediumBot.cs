using System.Collections.Generic;
using Interfaces;
using PistiGame;
using UnityEngine;

namespace BotStrategy
{
    public class MediumBot : IBotStrategy
    {
        private Bot _bot;
        public void InjectBot(Bot bot)
        {
            if (_bot != null) return;
            _bot = bot;
        }

        public void Play()
        {
            var cardsOnTable = (GameController.Instance.CurrentContext as PistiGameContext)?.GetCardsOnTable();
            var hand = _bot.GetHand();
            Card lastCard = cardsOnTable is { Count: > 0 } ? cardsOnTable[^1] : null;
            
            if (lastCard == null)
            {
                PlayRandomCardExceptJack(hand);
                return;
            }
            
            foreach (var card in hand)
            {
                if (card.GetConfig().cardValue == lastCard.GetConfig().cardValue)
                {
                    _bot.OnCardPlayed(card);
                    return;
                }
            }
            
            if (TryPlayJack()) return;
            
            PlayRandomCardExceptJack(hand);
        }

        private bool TryPlayJack()
        {
            foreach (var card in _bot.GetHand())
            {
                if (card.IsJackCard())
                {
                    _bot.OnCardPlayed(card);
                    return true;
                }
            }
            return false;
        }

        private void PlayRandomCardExceptJack(List<Card> hand)
        {
            var cardToPlay = GetRandomCard(hand);
            if (cardToPlay != null)
            {
                while (cardToPlay.IsJackCard())
                {
                    cardToPlay = GetRandomCard(hand);
                }
                
                _bot.OnCardPlayed(cardToPlay);
            }
        }
        
        private Card GetRandomCard(List<Card> cards)
        {
            return cards.Count > 0 ? cards[Random.Range(0, cards.Count)] : null;
        }
    }
}