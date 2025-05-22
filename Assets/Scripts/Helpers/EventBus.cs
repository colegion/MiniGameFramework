using System;
using System.Collections.Generic;
using PistiGame.Helpers;
using UnityEngine;

namespace Helpers
{
    public static class EventBus
    {
        private static readonly Dictionary<Type, List<Delegate>> _eventListeners = new Dictionary<Type, List<Delegate>>();

        public static void Register<T>(Action<T> listener) where T : class
        {
            Type eventType = typeof(T);
            if (!_eventListeners.ContainsKey(eventType))
            {
                _eventListeners[eventType] = new List<Delegate>();
            }
            _eventListeners[eventType].Add(listener);
        }

        public static void Unregister<T>(Action<T> listener) where T : class
        {
            Type eventType = typeof(T);
            if (_eventListeners.TryGetValue(eventType, out var eventListeners))
            {
                eventListeners.Remove(listener);
            }
        }

        public static void Trigger<T>(T eventData) where T : class
        {
            Type eventType = typeof(T);
            if (_eventListeners.TryGetValue(eventType, out var eventListeners))
            {
                foreach (var listener in new List<Delegate>(eventListeners))
                {
                    ((Action<T>)listener)(eventData);
                }
            }
        }
        
        public static void ClearEvents()
        {
            _eventListeners.Clear();
        }
    }

    public class OnDifficultySelected
    {
        public BotType BotType;

        public OnDifficultySelected(BotType botType)
        {
            BotType = botType;
        }
    }

    public class OnCardGameRequested { }
}