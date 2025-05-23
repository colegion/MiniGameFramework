using System;
using System.Collections.Generic;
using System.Linq;
using Helpers;
using LinkGame;
using LinkGame.Helpers;
using UnityEngine;

namespace ScriptableObjects.Level
{
    [Serializable]
    [CreateAssetMenu(fileName = "LevelConfig", menuName = "ScriptableObjects/Level/LevelConfig")]
    public class LinkLevelConfig : ScriptableObject
    {
        public int boardWidth;
        public int boardHeight;
        public int moveLimit;
        public List<LevelTargetConfig> levelTargets;
        
        public static List<LevelTargetConfig> MergeDuplicateTargets(List<LevelTargetConfig> targets)
        {
            Dictionary<ChipType, int> merged = new();

            foreach (var target in targets)
            {
                if (merged.ContainsKey(target.targetType))
                    merged[target.targetType] += target.count;
                else
                    merged[target.targetType] = target.count;
            }

            return merged
                .Select(pair => new LevelTargetConfig { targetType = pair.Key, count = pair.Value })
                .ToList();
        }
    }

    [Serializable]
    public class LevelTargetConfig
    {
        public ChipType targetType;
        public int count;
    }

}