using System.Collections;
using UnityEngine;

namespace Helpers
{
    public class RoutineHelper : MonoBehaviour
    {
        private static RoutineHelper _instance;

        public static RoutineHelper Instance
        {
            get
            {
                if (_instance == null)
                {
                    GameObject obj = new GameObject("RoutineHelper");
                    _instance = obj.AddComponent<RoutineHelper>();
                    DontDestroyOnLoad(obj);
                }
                return _instance;
            }
        }

        public void StartRoutine(IEnumerator routine)
        {
            StartCoroutine(routine);
        }
    }
}
