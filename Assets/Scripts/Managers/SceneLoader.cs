using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ph.Managers {
    public class SceneLoader : MonoBehaviour {
        public static SceneLoader Instance { get; private set; }

        private AsyncOperation preloadOperation;
        private int nextSceneIndex;
        private AsyncOperation preloadPreviousOperation;
        private int previousSceneIndex;

        private void Awake() {
            if (Instance != null && Instance != this) {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        public void PreloadNextScene() {
            nextSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;

            if (nextSceneIndex < SceneManager.sceneCountInBuildSettings) {
                StartCoroutine(PreloadCoroutine());
            }
        }
        private IEnumerator PreloadCoroutine() {
            preloadOperation = SceneManager.LoadSceneAsync(nextSceneIndex, LoadSceneMode.Additive);
            preloadOperation.allowSceneActivation = false;

            yield return null;
            yield return Resources.UnloadUnusedAssets();
            GC.Collect();
        }
        public void ActivateNextScene() {
            if (preloadOperation != null) {
                preloadOperation.allowSceneActivation = true;
                StartCoroutine(UnloadCurrentScene());
            }
            else {
                SceneManager.LoadScene(nextSceneIndex);
            }
        }
        private IEnumerator UnloadCurrentScene() {
            yield return new WaitUntil(() => preloadOperation.isDone);

            int currentScene = SceneManager.GetActiveScene().buildIndex;
            yield return SceneManager.UnloadSceneAsync(currentScene);
        }
        public void PreloadPreviousScene()
        {
            previousSceneIndex = SceneManager.GetActiveScene().buildIndex - 1;

            if (previousSceneIndex >= 0)
            {
                StartCoroutine(PreloadPreviousCoroutine());
            }
            else
            {
                Debug.LogWarning("No previous scene to preload.");
            }
        }
        private IEnumerator PreloadPreviousCoroutine()
        {
            preloadPreviousOperation = SceneManager.LoadSceneAsync(previousSceneIndex, LoadSceneMode.Additive);
            preloadPreviousOperation.allowSceneActivation = false;

            yield return null;
            yield return Resources.UnloadUnusedAssets();
            GC.Collect();
        }
        public void ActivatePreviousScene()
        {
            if (preloadPreviousOperation != null)
            {
                preloadPreviousOperation.allowSceneActivation = true;
                StartCoroutine(UnloadCurrentScene());
            }
            else
            {
                SceneManager.LoadScene(previousSceneIndex);
            }
        }
    }
}