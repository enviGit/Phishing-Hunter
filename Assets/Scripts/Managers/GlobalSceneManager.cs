using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace ph.Managers {
    public class GlobalSceneManager : MonoBehaviour {
        public static GlobalSceneManager Instance { get; private set; }

        [Header("Konfiguracja Scen")]
        public List<string> gameScenes = new List<string>();

        [Header("Referencje Bootstrap")]
        public Camera bootstrapCamera;

        [Header("UI Loading Screenu")]
        public GameObject loadingScreenCanvas;
        public Slider progressBar;

        [Header("Styling")]
        [Tooltip("Czy kolorować pasek dynamicznie?")]
        public bool useDynamicColor = true;
        public Gradient progressGradient;
        private Image fillImage;

        [Header("Ustawienia Płynności")]
        public float minLoadingTime = 2.0f;
        [Tooltip("Minimalny czas poświęcony na wizualizację ładowania JEDNEJ sceny (zapobiega skokom przy lekkich scenach)")]
        public float minTimePerScene = 0.5f;
        public float visualFillSpeed = 2.0f;

        private Dictionary<string, HashSet<int>> sceneActiveStates = new Dictionary<string, HashSet<int>>();
        private List<string> loadedScenes = new List<string>();

        private float targetProgress = 0f;
        private bool isGameLoaded = false;

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

            if (progressBar != null) {
                if (progressBar.fillRect != null)
                    fillImage = progressBar.fillRect.GetComponent<Image>();
            }

            StartCoroutine(BootSequence());
        }

        private void Update() {
            if (loadingScreenCanvas.activeSelf && progressBar != null) {
                progressBar.value = Mathf.Lerp(progressBar.value, targetProgress, Time.deltaTime * visualFillSpeed);

                if (useDynamicColor && fillImage != null) {
                    fillImage.color = progressGradient.Evaluate(progressBar.value);
                }
            }
        }

        private IEnumerator BootSequence() {
            Application.backgroundLoadingPriority = ThreadPriority.Low;
            loadingScreenCanvas.SetActive(true);

            progressBar.value = 0f;
            targetProgress = 0f;

            float totalStartTime = Time.time;
            List<string> allScenesToLoad = new List<string>(gameScenes);

            int totalScenes = allScenesToLoad.Count;
            int scenesLoadedCount = 0;

            foreach (string sceneName in allScenesToLoad) {
                float sceneStartTime = Time.time;

                AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);

                while (!operation.isDone) {
                    float currentSceneRawProgress = Mathf.Clamp01(operation.progress / 0.9f);

                    float globalMathProgress = (scenesLoadedCount + currentSceneRawProgress) / totalScenes;

                    targetProgress = Mathf.Max(targetProgress, globalMathProgress);

                    yield return null;
                }

                while (Time.time - sceneStartTime < minTimePerScene) {
                    float fakeProgress = (Time.time - sceneStartTime) / minTimePerScene;
                    float globalFakeProgress = (scenesLoadedCount + fakeProgress) / totalScenes;

                    targetProgress = Mathf.Max(targetProgress, globalFakeProgress);

                    yield return null;
                }

                Scene loadedScene = SceneManager.GetSceneByName(sceneName);
                if (loadedScene.IsValid()) {
                    HideSceneContent(loadedScene);
                    loadedScenes.Add(sceneName);
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

            if (allScenesToLoad.Count > 0) SwitchToScene(allScenesToLoad[0]);
        }

        public void SwitchToScene(string targetSceneName) {
            if (!isGameLoaded) return;

            if (!loadedScenes.Contains(targetSceneName)) {
                Debug.LogError($"Scena {targetSceneName} nie została załadowana!");
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