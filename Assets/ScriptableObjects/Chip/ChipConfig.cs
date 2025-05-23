using Helpers;
using LinkGame;
using LinkGame.Helpers;
using UnityEngine;

namespace ScriptableObjects.Chip
{
    [CreateAssetMenu(fileName = "New Chip Config", menuName = "ScriptableObjects/Chip/ChipConfig")]
    public class ChipConfig : ScriptableObject
    {
        public ChipType chipType;
        public Sprite chipSprite;
    }
}
