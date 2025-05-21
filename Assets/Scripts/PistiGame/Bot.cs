using System;
using System.Collections.Generic;
using BotStrategy;
using Helpers;
using Interfaces;
using PistiGame;
using PistiGame.Helpers;
using UnityEngine;

public class Bot : User
{
    private IBotStrategy _currentStrategy;

    private static readonly Dictionary<BotType, Func<IBotStrategy>> StrategyFactory = new()
    {
        { BotType.Easy, () => new EasyBot() },
        { BotType.Medium, () => new MediumBot() },
        { BotType.Hard, () => new HardBot() }
    };

    public void SetBotStrategy(BotType type)
    {
        if (StrategyFactory.TryGetValue(type, out var strategyCreator))
        {
            _currentStrategy = strategyCreator();
        }
        else
        {
            Debug.LogError($"No strategy found for {type}. Defaulting to EasyBot.");
            _currentStrategy = new EasyBot();
        }
    }

    public override void OnTurnStart()
    {
        _currentStrategy?.InjectBot(this);
        _currentStrategy?.Play();
    }

    public List<Card> GetHand()
    {
        return Cards;
    }
}