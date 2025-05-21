using PistiGame.Helpers;

namespace PistiGame.GameStates
{
    public class BotTurnState : TurnStateBase
    {
        protected override User GetUser() => Context.GetUser(true);
        protected override GameStateTypes GetGameStateType() => GameStateTypes.BotTurn;
    }
}
