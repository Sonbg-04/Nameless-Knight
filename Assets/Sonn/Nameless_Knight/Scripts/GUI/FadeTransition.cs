using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Sonn.Nameless_Knight
{
    public class FadeTransition : MonoBehaviour, ISingleton
    {
        public static FadeTransition Ins;

        public CanvasGroup canvasGroup;
        public float fadeDuration;

        private bool m_isFading = false;

        private void Awake()
        {
            MakeSingleton();
        }
        private void OnEnable()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void OnDisable()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            StartCoroutine(FadeOut());
        }

        public void FadeToScene(int buildIndex)
        {
            if (m_isFading)
            {
                return;
            }
            StartCoroutine(FadeIn(buildIndex));
        }

        IEnumerator FadeIn(int sceneIndex)
        {
            m_isFading = true;
            canvasGroup.blocksRaycasts = true;
            float t = 0;
            while (t < fadeDuration)
            {
                t += Time.unscaledDeltaTime;
                canvasGroup.alpha = Mathf.Lerp(0, 1, t / fadeDuration);
                yield return null;
            }
            canvasGroup.alpha = 1f;
            yield return SceneManager.LoadSceneAsync(sceneIndex);
        }

        IEnumerator FadeOut()
        {
            m_isFading = true;
            canvasGroup.alpha = 1f;
            float t = 0;
            while (t < fadeDuration)
            {
                t += Time.unscaledDeltaTime;
                canvasGroup.alpha = Mathf.Lerp(1, 0, t / fadeDuration);
                yield return null;
            }

            canvasGroup.alpha = 0f;
            canvasGroup.blocksRaycasts = false;
            m_isFading = false;
        }

        public void MakeSingleton()
        {
            if (Ins == null)
            {
                Ins = this;
                DontDestroyOnLoad(this);
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }
}
