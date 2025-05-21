using PistiGame.Helpers;

namespace PistiGame.GameStates
{
    public class PlayerTurnState : TurnStateBase
    {
        protected override User GetUser() => Context.GetUser(false);
        protected override GameStateTypes GetGameStateType() => GameStateTypes.PlayerTurn;
    
        public override void ExitState()
        {
            (User as Player)?.OnTurnEnd();
            var outcomeState = Context.GetStateByType(GameStateTypes.Outcome);
            Context.SetLastCallerType(GetGameStateType());
            Context.ChangeState(outcomeState);
        }
    }
}
