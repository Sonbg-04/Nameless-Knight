using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Sonn.Nameless_Knight
{
    public class DontDestroyOnLoad : MonoBehaviour
    {
        private static readonly HashSet<string> existingObjects = new();

        private void Awake()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;

            int sceneIndex = SceneManager.GetActiveScene().buildIndex;
            string key = gameObject.name;

            if (existingObjects.Contains(key))
            {
                Destroy(gameObject);
                return;
            }

            existingObjects.Add(key);
            DontDestroyOnLoad(gameObject);
        }

        private void OnDestroy()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (scene.buildIndex == 0)
            {
                existingObjects.Clear();
                Destroy(gameObject);
                return;
            }
        }
    }
}
