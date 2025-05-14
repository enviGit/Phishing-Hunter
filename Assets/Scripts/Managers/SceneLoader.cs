using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ph.Managers {
    public class SceneLoader : MonoBehaviour {
        public static SceneLoader Instance { get; private set; }

        private AsyncOperation preloadOperation;
        private int nextSceneIndex;

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
	public void LoadSceneWithIndex(int index) {
            SceneManager.LoadScene(index);
        }
        private IEnumerator UnloadCurrentScene() {
            yield return new WaitUntil(() => preloadOperation.isDone);

            int currentScene = SceneManager.GetActiveScene().buildIndex;
            yield return SceneManager.UnloadSceneAsync(currentScene);
        }
    }
}