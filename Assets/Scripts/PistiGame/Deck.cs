using System;
using System.Collections.Generic;
using Helpers;
using PistiGame.Helpers;
using UnityEngine;
using Random = UnityEngine.Random;

namespace PistiGame
{
    public class Deck
    {
        private List<CardConfig> _deck;
        private List<CardConfig> _garbageDeck = new List<CardConfig>();
        public Deck() 
        {
            BuildDeck();
        }

        private void BuildDeck()
        {
            _deck = new List<CardConfig>();
            foreach (CardSuit suit in Enum.GetValues(typeof(CardSuit)))
            {
                if (suit == CardSuit.Null) continue;
                foreach (CardValue value in Enum.GetValues(typeof(CardValue)))
                {
                    if (value == CardValue.Null) continue;
                    var tempConfig = new CardConfig()
                    {
                        cardSuit = suit,
                        cardValue = value
                    };

                    tempConfig.point = PistiUtilities.GetCardPoint(tempConfig);

                    _deck.Add(tempConfig);
                }
            }
        
            _deck.Shuffle();
        }

        public void RebuildDeck()
        {
            _deck = new List<CardConfig>(_garbageDeck);
            _garbageDeck.Clear();
            _deck.Shuffle();
            _removedCards.Clear();
        }

        public CardConfig GetRandomConfig()
        {
            var randomIndex = Random.Range(0, _deck.Count);
            var tempConfig = _deck[randomIndex];
            return tempConfig;
        }

        private List<CardConfig> _removedCards = new List<CardConfig>();
        public void RemoveCardFromDeck(CardConfig config)
        {

            foreach (var card in _removedCards)
            {
                if (card.cardValue == config.cardValue && card.cardSuit == config.cardSuit)
                {
                    Debug.LogWarning("duplicate removal!!!!!!");
                }
            }
            bool removed = _deck.RemoveAll(c => c.cardSuit == config.cardSuit && c.cardValue == config.cardValue) > 0;
    
            if (!removed)
            {
                Debug.LogError($"Attempted to remove {config.cardSuit} {config.cardValue} but it was not in the deck!");
            }
            else
            {
                _removedCards.Add(config);
                _garbageDeck.Add(config);
                Debug.Log($"Removed card from deck: {config.cardSuit} {config.cardValue}");
            }
        }
    
        public bool IsDeckEmpty => _deck.Count == 0;
    }
}