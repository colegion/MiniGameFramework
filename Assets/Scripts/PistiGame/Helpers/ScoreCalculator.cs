using System.Collections.Generic;
using System.Linq;
using PistiGame;

namespace Helpers
{
    public static class ScoreCalculator
    {
        private static readonly int CardCountPoint = 3;
        
        public static bool IsPlayerWinner(this List<User> users)
        {
            if (users == null || users.Count == 0)
                return false;

            Dictionary<User, int> userScores = new Dictionary<User, int>();
            User maxCardUser = null;
            int maxCardCount = 0;
        
            foreach (var user in users)
            {
                int totalScore = user.GetCollectedCards().Sum(card => card.point);
                userScores[user] = totalScore;
            
                if (user.GetCollectedCards().Count > maxCardCount)
                {
                    maxCardCount = user.GetCollectedCards().Count;
                    maxCardUser = user;
                }
            }
        
            if (maxCardUser != null)
            {
                userScores[maxCardUser] += CardCountPoint;
            }
        
            var winner = userScores.OrderByDescending(pair => pair.Value).First().Key;

            return winner is Player;
        }
    }
}