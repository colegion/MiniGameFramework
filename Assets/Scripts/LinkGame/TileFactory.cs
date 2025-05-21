using System.Collections;
using System.Collections.Generic;
using GridSystem;
using Helpers;
using Interfaces;
using Pool;
using ScriptableObjects.Chip;
using UnityEngine;
using PoolController = Pool.PoolController;

public class TileFactory : MonoBehaviour, IInjectable
{
    private PoolController _poolController;
    
    public void InjectDependencies()
    {
        _poolController = ServiceLocator.Get<PoolController>();
    }

    public BaseTile SpawnTileByConfig()
    {
        var pooledTile = _poolController.GetPooledObject(PoolableTypes.BaseTile);
        var tile = pooledTile.GetGameObject().GetComponent<BaseTile>();
        return tile;
    }
}
