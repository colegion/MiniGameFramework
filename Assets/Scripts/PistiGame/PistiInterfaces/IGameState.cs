namespace PistiGame.PistiInterfaces
{
    public interface IGameState
    {
        public void EnterState(object context);
        public void ExitState();
        public void ResetAttributes();
    }
}
