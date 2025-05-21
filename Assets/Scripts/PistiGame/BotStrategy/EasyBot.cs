using System.Collections.Generic;
using Interfaces;
using PistiGame;

namespace BotStrategy
{
    public class EasyBot : IBotStrategy
    {
        private Bot _bot;
        
        public void InjectBot(Bot bot)
        {
            if (_bot != null) return;
            _bot = bot;
        }

        public void Play()
        {
            var cardsInHand = _bot.GetHand();
            var cardToPlay = GetRandomCard(cardsInHand);
            _bot.OnCardPlayed(cardToPlay);
        }

        private Card GetRandomCard(List<Card> cards)
        {
            return cards[UnityEngine.Random.Range(0, cards.Count)];
        }
    }
}
