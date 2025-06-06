using System;
using System.Collections.Generic;
using System.Linq;
using Controllers;
using LinkGame;
using LinkGame.Controllers;
using ScriptableObjects.Level;
using UnityEngine;

namespace Helpers
{
    public class LevelProgressTracker
    {
        private LinkLevelConfig _config;
        private List<LevelTargetConfig> _targets;
        private int _remainingMoves;

        public LevelProgressTracker(LinkLevelConfig config)
        {
            _config = config;
            _targets = LinkLevelConfig.MergeDuplicateTargets(config.levelTargets)
                .Select(t => new LevelTargetConfig
                {
                    targetType = t.targetType,
                    count = t.count
                }).ToList();

            _remainingMoves = config.moveLimit;
        }

        public void RegisterMove(LevelTargetConfig move)
        {
            _remainingMoves--;

            var target = _targets.FirstOrDefault(t => t.targetType == move.targetType);
            if (target != null)
            {
                target.count = Math.Max(0, target.count - move.count);
            }

            var context = GameController.Instance.CurrentContext as LinkGameContext;
            if (context == null)
            {
                Debug.LogError("Context is null!");
                return;
            }
            if (CheckIfLevelCompleted())
            {
                context.OnLevelFinished(true);
            }
            else if (_remainingMoves <= 0)
            {
                context.OnLevelFinished(false);
            }
        }

        private bool CheckIfLevelCompleted()
        {
            return _targets.All(t => t.count <= 0);
        }
        
        public void Reset()
        {
            _remainingMoves = _config.moveLimit;
            _targets = LinkLevelConfig.MergeDuplicateTargets(_config.levelTargets)
                .Select(t => new LevelTargetConfig
                {
                    targetType = t.targetType,
                    count = t.count
                }).ToList();
        }


        public int GetRemainingMoves() => _remainingMoves;
        public List<LevelTargetConfig> GetRemainingTargets() => _targets;
    }
}