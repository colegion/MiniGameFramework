using Helpers;
using Interfaces;
using PistiGame.Helpers;
using UnityEngine;

namespace PistiGame.GameStates
{
    public abstract class TurnStateBase : IGameState
    {
        protected User User;

        public void EnterState()
        {
            Debug.Log("entered turn state");
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
            var outcomeState = PistiGameController.Instance.GetStateByType(GameStateTypes.Outcome);
            PistiGameController.Instance.SetLastCallerType(GetGameStateType());
            PistiGameController.Instance.ChangeState(outcomeState);
        }

        public void ResetAttributes()
        {
            User = null;
        }

        private void ExitImmediately()
        {
            var distributionState = PistiGameController.Instance.GetStateByType(GameStateTypes.CardDistribution);
            PistiGameController.Instance.SetLastCallerType(GetGameStateType());
            PistiGameController.Instance.ChangeState(distributionState);
        }

        protected abstract User GetUser();
        protected abstract GameStateTypes GetGameStateType();
    }
}



