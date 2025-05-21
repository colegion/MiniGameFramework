using System;
using Helpers;
using UnityEngine;
using IPoolable = Interfaces.IPoolable;

namespace PistiGame
{
    public class PoolController : MonoBehaviour
    {
        private const int DefaultPoolAmount = 60;
        private GamePool _gamePool;

        public void Initialize()
        {
            _gamePool = new GamePool();
            InitializePool();   
        }

        private void InitializePool()
        {
            foreach (PoolableTypes type in Enum.GetValues(typeof(PoolableTypes)))
            {
                var prefab = Resources.Load<GameObject>($"Prefabs/{type}");
                if (prefab == null)
                {
                    Debug.LogError($"Prefab for {type} not found in Resources/Prefabs!");
                    continue;
                }

                var poolable = prefab.GetComponent<IPoolable>();
                if (poolable == null)
                {
                    Debug.LogError($"Prefab for {type} does not implement IPoolable!");
                    continue;
                }

                _gamePool.PoolObjects(type, poolable, DefaultPoolAmount, transform);
            }
        }

        public IPoolable GetPooledObject(PoolableTypes type)
        {
            return _gamePool.FetchFromPool(type);
        }

        public void ReturnPooledObject(IPoolable poolObject)
        {
            var poolGo = poolObject.GetGameObject(); 
            poolGo.transform.SetParent(transform);
            poolGo.transform.localPosition = Vector3.zero;
            _gamePool.ReturnToPool(poolObject.GetPoolableType(), poolObject);
        }
    }
}