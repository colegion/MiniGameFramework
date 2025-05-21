using System.Collections;
using System.Collections.Generic;
using Helpers;
using Interfaces;
using PistiGame;
using PistiGame.GameStates;
using PistiGame.Helpers;
using UnityEngine;

public class BotTurnState : TurnStateBase
{
    protected override User GetUser() => PistiGameController.Instance.GetUser(true);
    protected override GameStateTypes GetGameStateType() => GameStateTypes.BotTurn;
}
