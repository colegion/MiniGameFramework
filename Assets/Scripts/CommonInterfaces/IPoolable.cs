using UnityEngine;

namespace Interfaces
{
    public interface IPoolable
    {
        public void OnPooled();
        public void OnFetchFromPool();
        public void OnReturnPool();
        public PoolableTypes GetPoolableType();
        public GameObject GetGameObject();
    }
    
    public enum PoolableTypes
    {
        BaseTile,
        EditorTile,
        TargetUIElement,
        TrailObject,
        Card
    }
}