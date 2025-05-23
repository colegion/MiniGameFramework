namespace CommonInterfaces
{
    public interface IGameContext
    {
        void Initialize();
        void StartGame();
        void EndGame();
        void Cleanup(); // optional: for OnDestroy or scene transitions
    }
}

