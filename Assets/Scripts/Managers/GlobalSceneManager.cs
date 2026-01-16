using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace ph.Managers {
    public class GlobalSceneManager : MonoBehaviour {
        public static GlobalSceneManager Instance { get; private set; }

        public List<string> gameScenes = new List<string>();

        public List<string> startupOnlyScenes = new List<string>();

        public Camera bootstrapCamera;
        public GameObject loadingScreenCanvas;
        public Slider progressBar;

        public float minLoadingTime = 2.0f;
        public float minTimePerScene = 0.5f;
        public float visualFillSpeed = 2.0f;

        private Dictionary<string, HashSet<int>> sceneActiveStates = new Dictionary<string, HashSet<int>>();
        private List<string> loadedScenes = new List<string>();

        private float targetProgress = 0f;
        private bool isGameLoaded = false;

        private bool hasFreedStartupMemory = false;

        private void Awake() {
            if (Instance != null && Instance != this) {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        private void Start() {
            var myListener = GetComponent<AudioListener>();
            if (myListener != null) Destroy(myListener);
            if (bootstrapCamera != null) {
                var camListener = bootstrapCamera.GetComponent<AudioListener>();
                if (camListener != null) Destroy(camListener);
            }

            StartCoroutine(BootSequence());
        }

        private void Update() {
            if (loadingScreenCanvas == null || progressBar == null) return;

            if (loadingScreenCanvas.activeSelf && progressBar != null) {
                progressBar.value = Mathf.Lerp(progressBar.value, targetProgress, Time.deltaTime * visualFillSpeed);
            }
        }

        private IEnumerator BootSequence() {
            Application.backgroundLoadingPriority = ThreadPriority.Low;
            loadingScreenCanvas.SetActive(true);
            progressBar.value = 0f;
            targetProgress = 0f;

            float totalStartTime = Time.time;

            List<string> allScenesToLoad = new List<string>();
            allScenesToLoad.AddRange(startupOnlyScenes);
            allScenesToLoad.AddRange(gameScenes);

            int totalScenes = allScenesToLoad.Count;
            int scenesLoadedCount = 0;

            foreach (string sceneName in allScenesToLoad) {
                float sceneStartTime = Time.time;
                AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);

                while (!operation.isDone) {
                    float currentRaw = Mathf.Clamp01(operation.progress / 0.9f);
                    float globalProgress = (scenesLoadedCount + currentRaw) / totalScenes;
                    targetProgress = Mathf.Max(targetProgress, globalProgress);
                    yield return null;
                }

                while (Time.time - sceneStartTime < minTimePerScene) {
                    float fakeP = (Time.time - sceneStartTime) / minTimePerScene;
                    float globalFake = (scenesLoadedCount + fakeP) / totalScenes;
                    targetProgress = Mathf.Max(targetProgress, globalFake);
                    yield return null;
                }

                Scene loadedScene = SceneManager.GetSceneByName(sceneName);
                if (loadedScene.IsValid()) {
                    HideSceneContent(loadedScene);
                    if (!loadedScenes.Contains(sceneName)) {
                        loadedScenes.Add(sceneName);
                    }
                }
                scenesLoadedCount++;
            }

            targetProgress = 1.0f;
            while (Time.time - totalStartTime < minLoadingTime || progressBar.value < 0.99f) {
                yield return null;
            }

            Application.backgroundLoadingPriority = ThreadPriority.Normal;
            isGameLoaded = true;
            loadingScreenCanvas.SetActive(false);

            if (bootstrapCamera != null) bootstrapCamera.gameObject.SetActive(false);

            string firstScene = startupOnlyScenes.Count > 0 ? startupOnlyScenes[0] : gameScenes[0];
            SwitchToScene(firstScene);
        }

        public void SwitchToScene(string targetSceneName) {
            if (!isGameLoaded) return;

            if (targetSceneName == "MainMenu" && !hasFreedStartupMemory) {
                StartCoroutine(FreeUpStartupMemory());
            }

            if (!loadedScenes.Contains(targetSceneName)) {
                Debug.LogError($"Scena {targetSceneName} nie jest załadowana/dostępna!");
                return;
            }

            foreach (string sceneName in loadedScenes) {
                Scene s = SceneManager.GetSceneByName(sceneName);
                if (!s.IsValid()) continue;

                if (sceneName == targetSceneName) {
                    ShowSceneContent(s);
                    SceneManager.SetActiveScene(s);
                    ManageAudioListeners(s, true);
                }
                else {
                    HideSceneContent(s);
                    ManageAudioListeners(s, false);
                }
            }
        }

        private IEnumerator FreeUpStartupMemory() {
            hasFreedStartupMemory = true;

            foreach (string sceneToRemove in startupOnlyScenes) {
                if (loadedScenes.Contains(sceneToRemove)) {
                    yield return SceneManager.UnloadSceneAsync(sceneToRemove);
                    loadedScenes.Remove(sceneToRemove);

                    if (sceneActiveStates.ContainsKey(sceneToRemove))
                        sceneActiveStates.Remove(sceneToRemove);
                }
            }

            Scene bootstrapScene = SceneManager.GetSceneByBuildIndex(0);
            if (bootstrapScene.IsValid() && bootstrapScene.isLoaded) {
                yield return SceneManager.UnloadSceneAsync(bootstrapScene);
            }

            bootstrapCamera = null;

            yield return Resources.UnloadUnusedAssets();
            System.GC.Collect();
        }

        private void ManageAudioListeners(Scene s, bool state) {
            GameObject[] roots = s.GetRootGameObjects();
            foreach (var root in roots) {
                var listeners = root.GetComponentsInChildren<AudioListener>(true);
                foreach (var listener in listeners) {
                    listener.enabled = state;
                }
            }
        }

        private void HideSceneContent(Scene scene) {
            bool isFirstSnapshot = !sceneActiveStates.ContainsKey(scene.name);
            HashSet<int> activeObjectIds = new HashSet<int>();
            GameObject[] roots = scene.GetRootGameObjects();

            foreach (GameObject obj in roots) {
                if (isFirstSnapshot) {
                    if (obj.activeSelf) activeObjectIds.Add(obj.GetInstanceID());
                }
                obj.SetActive(false);
            }

            if (isFirstSnapshot) {
                sceneActiveStates.Add(scene.name, activeObjectIds);
            }
        }

        private void ShowSceneContent(Scene scene) {
            GameObject[] roots = scene.GetRootGameObjects();
            HashSet<int> objectsToActivate = null;
            if (sceneActiveStates.ContainsKey(scene.name)) {
                objectsToActivate = sceneActiveStates[scene.name];
            }

            foreach (GameObject obj in roots) {
                if (objectsToActivate != null && objectsToActivate.Contains(obj.GetInstanceID())) {
                    obj.SetActive(true);
                }
            }
        }
    }
}
