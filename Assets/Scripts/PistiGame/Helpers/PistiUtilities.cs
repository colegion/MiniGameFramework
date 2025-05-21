using System;
using System.Collections.Generic;
using UnityEngine;

namespace PistiGame.Helpers
{
    public class PistiUtilities : MonoBehaviour
    {
        [SerializeField] private List<CardSpriteContainer> cardSprites;
        [SerializeField] private List<CardConfig> rewardedCards;

        private static Dictionary<CardSuit, CardSpriteContainer> _cardsDictionary;
        private static List<CardConfig> _rewardedCards;
        public static readonly int PistiPoint = 10;
        
        private void Awake()
        {
            _cardsDictionary = new Dictionary<CardSuit, CardSpriteContainer>();
            _rewardedCards = new List<CardConfig>();
            foreach (var container in cardSprites)
            {
                if (!_cardsDictionary.TryAdd(container.suit, container))
                {
                    Debug.LogWarning($"Duplicate entry for suit: {container.suit}. Only the first entry will be used.");
                }
            }

            foreach (var rewarded in rewardedCards)
            {
                _rewardedCards.Add(rewarded);
            }
        }
        
        public static Sprite GetCardSprite(CardSuit suit, CardValue value)
        {
            if (_cardsDictionary == null || !_cardsDictionary.TryGetValue(suit, out var container))
            {
                Debug.LogError($"Card suit '{suit}' not found in the dictionary.");
                return null;
            }

            foreach (var faceCard in container.faceCards)
            {
                if (faceCard.value == value)
                {
                    return faceCard.sprite;
                }
            }
            
            if (value >= CardValue.One && value <= CardValue.Ten)
            {
                return container.defaultSuitSprite;
            }

            Debug.LogError($"Sprite not found for suit: {suit}, value: {value}");
            return null;
        }

        public static int GetCardPoint(CardConfig config)
        {
            foreach (var rewarded in _rewardedCards)
            {
                if (config.cardSuit == rewarded.cardSuit && config.cardValue == rewarded.cardValue)
                {
                    return rewarded.point;
                }
            }

            return 0;
        }
    }

    [Serializable]
    public struct CardSpriteContainer
    {
        public CardSuit suit;
        public Sprite defaultSuitSprite;
        public List<CardSpriteEntry> faceCards;
    }

    [Serializable]
    public struct CardSpriteEntry
    {
        public CardValue value;
        public Sprite sprite;
    }
    
    [Serializable]
    public struct CardConfig : IEquatable<CardConfig>
    {
        public CardSuit cardSuit;
        public CardValue cardValue;
        public int point;

        public bool Equals(CardConfig other)
        {
            return cardValue == other.cardValue;
        }
    }

    public enum CardSuit
    {
        Null,
        Clubs,
        Diamonds,
        Hearts,
        Spades,
    }

    public enum CardValue
    {
        Null,
        One,
        Two,
        Three,
        Four,
        Five,
        Six,
        Seven,
        Eight,
        Nine,
        Ten,
        Jack,
        Queen,
        King
    }

    public enum GameStateTypes
    {
        CardDistribution,
        PlayerTurn,
        BotTurn,
        Outcome,
        
    }

    public enum BotType
    {
        Easy,
        Medium,
        Hard
    }

    public enum CollectType
    {
        None,
        IdenticalCard,
        Jack,
        Pisti
    }
}
