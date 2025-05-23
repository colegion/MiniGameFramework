using UnityEngine;

namespace Interfaces
{
    public interface IGameState
    {
        public void EnterState(object context);
        public void ExitState();
        public void ResetAttributes();
    }
}
