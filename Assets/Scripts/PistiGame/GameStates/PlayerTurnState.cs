using System.Collections;
using System.Collections.Generic;
using Helpers;
using Interfaces;
using PistiGame;
using PistiGame.GameStates;
using PistiGame.Helpers;
using UnityEngine;

public class PlayerTurnState : TurnStateBase
{
    protected override User GetUser() => PistiGameController.Instance.GetUser(false);
    protected override GameStateTypes GetGameStateType() => GameStateTypes.PlayerTurn;
    
    public override void ExitState()
    {
        (User as Player)?.OnTurnEnd();
        var outcomeState = PistiGameController.Instance.GetStateByType(GameStateTypes.Outcome);
        PistiGameController.Instance.SetLastCallerType(GetGameStateType());
        PistiGameController.Instance.ChangeState(outcomeState);
    }
}
