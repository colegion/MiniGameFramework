using System.Collections.Generic;
using Helpers;
using Interfaces;
using PistiGame;
using PistiGame.Helpers;
using UnityEngine;

namespace BotStrategy
{
    public class HardBot : IBotStrategy
    {
        private Bot _bot;
        private HashSet<CardValue> _playedCards = new HashSet<CardValue>();
        private HashSet<CardValue> _seenCards = new HashSet<CardValue>();

        public void InjectBot(Bot bot)
        {
            if (_bot != null) return;
            _bot = bot;
        }

        public void Play()
        {
            var cardsOnTable = (GameController.Instance.CurrentContext as PistiGameContext)?.GetCardsOnTable();

            if (cardsOnTable != null)
            {
                foreach (var card in cardsOnTable)
                {
                    _seenCards.Add(card.GetConfig().cardValue);
                }

                var hand = _bot.GetHand();
                Card lastCard = cardsOnTable.Count > 0 ? cardsOnTable[^1] : null;

                if (lastCard == null)
                {
                    PlayLeastProbableCard(hand);
                    return;
                }

                foreach (var card in hand)
                {
                    if (card.GetConfig().cardValue == lastCard.GetConfig().cardValue)
                    {
                        PlayCard(card);
                        return;
                    }
                }

                if (TryPlayJack()) return;

                PlayLeastProbableCard(hand);
            }
        }

        private void PlayCard(Card card)
        {
            _playedCards.Add(card.GetConfig().cardValue);
            _bot.OnCardPlayed(card);
        }

        private bool TryPlayJack()
        {
            foreach (var card in _bot.GetHand())
            {
                if (card.IsJackCard())
                {
                    PlayCard(card);
                    return true;
                }
            }
            return false;
        }

        private void PlayLeastProbableCard(List<Card> hand)
        {
            Dictionary<CardValue, int> cardFrequencies = new Dictionary<CardValue, int>();
            foreach (var value in _playedCards) cardFrequencies[value] = cardFrequencies.GetValueOrDefault(value, 0) + 1;
            foreach (var value in _seenCards) cardFrequencies[value] = cardFrequencies.GetValueOrDefault(value, 0) + 1;

            Card leastProbableCard = null;
            int minFrequency = int.MaxValue;

            foreach (var card in hand)
            {
                int frequency = cardFrequencies.GetValueOrDefault(card.GetConfig().cardValue, 0);
                if (frequency < minFrequency)
                {
                    minFrequency = frequency;
                    leastProbableCard = card;
                }
            }

            if (leastProbableCard != null)
            {
                PlayCard(leastProbableCard);
            }
        }
    }
}
