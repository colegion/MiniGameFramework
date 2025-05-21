using UnityEngine;
using UnityEngine.SceneManagement;

namespace Helpers
{
    public class SceneLoader : MonoBehaviour
    {
        public static void LoadSceneAsync(SceneType type)
        {
            SceneManager.LoadSceneAsync(type.ToString());
        }
    }
}
