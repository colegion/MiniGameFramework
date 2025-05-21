using System;
using System.Collections.Generic;
using Helpers;
using Interfaces;
using PistiGame.Helpers;
using Pool;
using UnityEngine;

namespace PistiGame
{
    public class PistiGameController : MonoBehaviour
    {
        [SerializeField] private PoolController poolController;
        [SerializeField] private CardTapHandler cardTapHandler;
        [SerializeField] private CardAnimator cardAnimator;
        [SerializeField] private Table table;
        [SerializeField] private List<User> users;

        private static PistiGameController _instance;
        private static Dictionary<GameStateTypes, IGameState> _gameStates = new Dictionary<GameStateTypes, IGameState>();

        private IGameState _currentState;

        private Deck _deck;
        private GameStateTypes _lastOutcomeCallerType;
        private bool _isInitialized;

        public static event Action<bool> OnGameFinished;

        public static PistiGameController Instance
        {
            get
            {
                if (_instance == null)
                {
                    Debug.LogError("GameController is not initialized! Ensure there is a GameController in the scene.");
                }

                return _instance;
            }
        }

        private void Awake()
        {
            if (_instance == null)
            {
                _instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else if (_instance != this)
            {
                Destroy(gameObject);
            }
        }

        public void StartGame(BotType bot)
        {
            if (!_isInitialized)
            {
                poolController.Initialize();
                cardTapHandler.Initialize();
                cardTapHandler.InjectPlayer(GetUser(false) as Player);
        
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

            foreach (var user in users)
            {
                user.ResetAttributes();
                if (user is Bot)
                {
                    ((Bot)user).SetBotStrategy(bot);
                }
            }
        
            table.ResetAttributes();
    
            foreach (var pair in _gameStates)
            {
                pair.Value.ResetAttributes();
            }

            Debug.Log("All states and users reset.");

            _currentState = _gameStates[GameStateTypes.CardDistribution];
            _currentState.EnterState();
        }


        public Card GetCard()
        {
            return (Card)poolController.GetPooledObject(PoolableTypes.Card);
        }

        public void ReturnObjectToPool(IPoolable objectToReturn)
        {
            poolController.ReturnPooledObject(objectToReturn);
        }

        public CardConfig GetRandomConfig()
        {
            return _deck.GetRandomConfig();
        }

        public void AppendCardsOnTable(Card card)
        {
            table.AppendCardsOnTable(card);
            cardAnimator.AnimateSelectedCard(card, table.GetCardTarget(), false, () => { });
        }

        public List<Card> GetCardsOnTable()
        {
            return table.GetCardsOnTable();
        }

        public void ClearOnTableCards()
        {
            table.ClearOnTableCards();
        }

        public void RemoveCardFromDeck(CardConfig config)
        {
            _deck.RemoveCardFromDeck(config);
        }

        public void ChangeState(IGameState newState)
        {
            _currentState = newState;
            _currentState.EnterState();
        }

        public List<User> GetAllUsers()
        {
            return users;
        }

        public User GetUser(bool isBot)
        {
            if (isBot)
            {
                return users.Find(u => u is Bot);
            }
            else
            {
                return users.Find(u => u is Player);
            }
        }

        public void SetLastCallerType(GameStateTypes type)
        {
            _lastOutcomeCallerType = type;
        }

        public bool CheckIfGameFinished()
        {
            bool gameEnded = true;
            foreach (var user in users)
            {
                if (!user.IsHandEmpty()) gameEnded = false;
            }

            gameEnded = gameEnded && _deck.IsDeckEmpty;
            if (gameEnded)
            {
                OnGameFinished?.Invoke(users.IsPlayerWinner());
                return true;
            }

            return false;
        }

        public GameStateTypes GetLastOutcomeCallerType()
        {
            return _lastOutcomeCallerType;
        }
    
        public void CheckRemainingCardAmount()
        {
            if (_deck.IsDeckEmpty)
            {
                table.ToggleCardVisual(false);
            }
        }

        public IGameState GetStateByType(GameStateTypes type)
        {
            return _gameStates[type];
        }
    }
}