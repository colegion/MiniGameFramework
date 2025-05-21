using Helpers;
using Interfaces;
using Pool;
using UnityEngine;

public class GameController : MonoBehaviour
{
    private static GameController _instance;
    public static GameController Instance => _instance;
    
    private PoolController _poolController;
    public PoolController PoolController => _poolController;
    
    private IGameContext _currentContext;
    public IGameContext CurrentContext => _currentContext;

    private void Awake()
    {
        if (_instance == null) { _instance = this; DontDestroyOnLoad(gameObject); }
        else { Destroy(gameObject); return; }
    }

    public void SetGameContext(IGameContext context)
    {
        LoadFields();
        _currentContext?.Cleanup();
        _currentContext = context;
        _currentContext.Initialize();
        //StartGame();
    }
    
    private void LoadFields()
    {
        if(_poolController == null)
            _poolController = ServiceLocator.Get<PoolController>();
        _poolController.Initialize();
    }
    
    public void ReturnPooledObject(IPoolable poolable)
    {
        _poolController?.ReturnPooledObject(poolable);
    }

    public void StartGame() => _currentContext?.StartGame();
    public void EndGame() => _currentContext?.EndGame();
}