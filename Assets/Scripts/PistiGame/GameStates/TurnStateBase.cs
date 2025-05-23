using Helpers;
using Interfaces;
using PistiGame.Helpers;
using PistiGame.PistiInterfaces;
using UnityEngine;

namespace PistiGame.GameStates
{
    public abstract class TurnStateBase : IGameState
    {
        protected User User;
        protected PistiGameContext Context;
        public void EnterState(object context)
        {
            if(Context == null)
                Context = context as PistiGameContext;
            if (User == null)
                User = GetUser();

            if (User.IsHandEmpty())
            {
                ExitImmediately();
                return;
            }

            User.InjectUserState(this);
            User.OnTurnStart();
        }

        public virtual void ExitState()
        {
            var outcomeState = Context.GetStateByType(GameStateTypes.Outcome);
            Context.SetLastCallerType(GetGameStateType());
            Context.ChangeState(outcomeState);
        }

        public void ResetAttributes()
        {
            User = null;
        }

        private void ExitImmediately()
        {
            var distributionState = Context.GetStateByType(GameStateTypes.CardDistribution);
            Context.SetLastCallerType(GetGameStateType());
            Context.ChangeState(distributionState);
        }

        protected abstract User GetUser();
        protected abstract GameStateTypes GetGameStateType();
    }
}



