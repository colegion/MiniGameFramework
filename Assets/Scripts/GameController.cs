using System;
using Helpers;
using Interfaces;
using Pool;
using ScriptableObjects.Level;
using UnityEngine;

public class GameController : MonoBehaviour
{
    private static GameController _instance;
    public static GameController Instance => _instance;
    
    private PoolController _poolController;
    public PoolController PoolController => _poolController;
    
    private IGameContext _currentContext;
    public IGameContext CurrentContext => _currentContext;
    
    public static event Action<bool> OnGameOver;

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

    public void TriggerOnGameOver(bool isSuccess)
    {
        OnGameOver?.Invoke(isSuccess);
    }
}