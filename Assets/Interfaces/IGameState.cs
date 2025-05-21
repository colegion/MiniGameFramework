using PistiGame;
using UnityEngine;

namespace Interfaces
{
    public interface IGameState
    {
        public void EnterState(PistiGameContext context);
        public void ExitState();
        public void ResetAttributes();
    }
}
