using System.Collections.Generic;
using Helpers;
using Interfaces;
using PistiGame.Helpers;

namespace PistiGame.GameStates
{
    public class OutcomeState : IGameState
    {
        private GameRules _gameRules;
        private PistiGameContext _context;
        public void EnterState(object context)
        {
            if (_context == null)
                _context = context as PistiGameContext;
            if (_gameRules == null)
                _gameRules = new GameRules();
            DecideOutcomeIfPossible();
        }

        private void DecideOutcomeIfPossible()
        {
            var cardsOnTable = _context.GetCardsOnTable();
            if (cardsOnTable.Count <= 1)
            {
                ExitState();
                return;
            }

            var lastCallerType = _context.GetLastOutcomeCallerType();
            var user = _context.GetUser(lastCallerType == GameStateTypes.BotTurn);

            if (_gameRules.IsMoveCollectable(cardsOnTable, out CollectType type))
            {
                if(type == CollectType.Pisti) user.IncrementPistiCount();
                CollectCardsAndExit(user, cardsOnTable, type);
            }
            else
            {
                ExitState();
            }
        }
    
        private void CollectCardsAndExit(User user, List<Card> cardsOnTable, CollectType type)
        {
            user.CollectCards(cardsOnTable, type, () =>
            {
                _context.ClearOnTableCards();
                ExitState();
            });
        }

        public void ExitState()
        {
            var lastCaller = _context.GetLastOutcomeCallerType();
            GameStateTypes newCaller = lastCaller;
            if (lastCaller == GameStateTypes.BotTurn)
            {
                newCaller = GameStateTypes.PlayerTurn;
            }
        
            else if (lastCaller == GameStateTypes.PlayerTurn)
            {
                newCaller = GameStateTypes.BotTurn;
            }
        
            _context.ChangeState(_context.GetStateByType(newCaller));
        }

        public void ResetAttributes()
        {
        
        }
    }
}
