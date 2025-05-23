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
        PistiGame
    }

    public enum SceneType
    {
        MainMenu,
        LinkGame,
        PistiGame,
    }
}
