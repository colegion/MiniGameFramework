using UnityEngine;

namespace Interfaces
{
    public interface IGameState
    {
        public void EnterState();
        public void ExitState();
        public void ResetAttributes();
    }
}
