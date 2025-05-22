using System;
using System.Collections.Generic;
using ScriptableObjects.Level;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Helpers
{
    public class CommonUtilities : MonoBehaviour
    {
        
    }

    public enum GameMode
    {
        LinkGame,
        CardGame
    }

    public enum SceneType
    {
        MainMenu,
        LinkGame,
        PistiGame,
    }
    
    public enum PoolableTypes
    {
        BaseTile,
        TargetUIElement,
        TrailObject,
        Card
    }
}
