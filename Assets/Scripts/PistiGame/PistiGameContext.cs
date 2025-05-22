using System;
using System.Collections.Generic;
using Helpers;
using Interfaces;
using PistiGame.GameStates;
using PistiGame.Helpers;
using UnityEngine;

namespace PistiGame
{
    public class PistiGameContext : IGameContext
    {
        private readonly CardInputController _cardInputController;
        private readonly CardAnimator _cardAnimator;
        private readonly Table _table;
        private readonly List<User> _users;
        private BotType _selectedBotType;
        
        private static Dictionary<GameStateTypes, IGameState> _gameStates = new Dictionary<GameStateTypes, IGameState>();

        private IGameState _currentState;

        private Deck _deck;
        private GameStateTypes _lastOutcomeCallerType;
        private bool _isInitialized;
        
        public PistiGameContext(CardInputController cardInputController, CardAnimator cardAnimator, Table table, List<User> users)
        {
            _cardInputController = cardInputController;
            _cardAnimator = cardAnimator;
            _table = table;
            _users = users;
        }
        
        public void Initialize()
        {
            if (!_isInitialized)
            {
                _cardInputController.Initialize();
                _cardInputController.InjectPlayer(GetUser(false) as Player);
        
                _gameStates = new Dictionary<GameStateTypes, IGameState>
                {
                    { GameStateTypes.CardDistribution, new CardDistributionState() },
                    { GameStateTypes.BotTurn, new BotTurnState() },
                    { GameStateTypes.PlayerTurn, new PlayerTurnState() },
                    { GameStateTypes.Outcome, new OutcomeState() }
                };
        
                _deck = new Deck();
                _isInitialized = true;
            }
            else
            {
                _deck.RebuildDeck();
            }

            foreach (var user in _users)
            {
                user.ResetAttributes();
                if (user is Bot)
                {
                    ((Bot)user).SetBotStrategy(_selectedBotType);
                }
            }
        
            _table.ResetAttributes();
    
            foreach (var pair in _gameStates)
            {
                pair.Value.ResetAttributes();
            }

            Debug.Log("All states and users reset.");

            StartGame();
        }
        
        public List<Card> GetCardsOnTable()
        {
            return _table.GetCardsOnTable();
        }
        
        public User GetUser(bool isBot)
        {
            if (isBot)
            {
                return _users.Find(u => u is Bot);
            }
            else
            {
                return _users.Find(u => u is Player);
            }
        }
        
        public void ChangeState(IGameState newState)
        {
            _currentState = newState;
            _currentState.EnterState(this);
        }
        
        public void SetLastCallerType(GameStateTypes type)
        {
            _lastOutcomeCallerType = type;
        }

        public bool CheckIfGameFinished()
        {
            bool gameEnded = true;
            foreach (var user in _users)
            {
                if (!user.IsHandEmpty()) gameEnded = false;
            }

            gameEnded = gameEnded && _deck.IsDeckEmpty;
            if (gameEnded)
            {
                GameController.Instance.TriggerOnGameOver(_users.IsPlayerWinner());
                return true;
            }

            return false;
        }

        public GameStateTypes GetLastOutcomeCallerType()
        {
            return _lastOutcomeCallerType;
        }
    
        public void ClearOnTableCards()
        {
            _table.ClearOnTableCards();
        }
        
        public void CheckRemainingCardAmount()
        {
            if (_deck.IsDeckEmpty)
            {
                _table.ToggleCardVisual(false);
            }
        }

        public IGameState GetStateByType(GameStateTypes type)
        {
            return _gameStates[type];
        }

        public void SetBotType(BotType type)
        {
            _selectedBotType = type;
        }
        
        public Card GetCard()
        {
            return (Card)GameController.Instance.PoolController.GetPooledObject(PoolableTypes.Card);
        }
        
        public CardConfig GetRandomConfig()
        {
            return _deck.GetRandomConfig();
        }
        
        public List<User> GetAllUsers()
        {
            return _users;
        }

        public void AppendCardsOnTable(Card card)
        {
            _table.AppendCardsOnTable(card);
            _cardAnimator.AnimateSelectedCard(card, _table.GetCardTarget(), false, () => { });
        }

        public void RemoveCardFromDeck(CardConfig config)
        {
            _deck.RemoveCardFromDeck(config);
        }

        public void StartGame()
        {
            _currentState = _gameStates[GameStateTypes.CardDistribution];
            _currentState.EnterState(this);
        }

        public void EndGame()
        {
            _cardInputController.ToggleInput(false);
        }

        public void Cleanup()
        {
            Initialize();
        }
    }
}
