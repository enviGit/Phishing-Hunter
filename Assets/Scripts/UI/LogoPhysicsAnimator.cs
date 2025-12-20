using DG.Tweening;
using ph.Managers;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

namespace ph.UI {
    public class LogoPhysicsAnimator : MonoBehaviour {
        [System.Serializable]
        public class LogoCube {
            public Transform cube;
            [HideInInspector] public Vector3 targetPosition;
            [HideInInspector] public Quaternion targetRotation;
            [HideInInspector] public Rigidbody rb;
        }

        [Header("Logo")]
        public List<LogoCube> cubes;
        public float launchForce = 6f;
        public float chaosTime = 2f;
        public float settleDuration = 50f;
        public float overshootDistance = 0.5f;
        public Vector2 spawnArea = new Vector2(10f, 6f);
        private AudioSource audioSource;

        [Header("Epilepsy UI")]
        public CanvasGroup epilepsyPanel;
        public Button continueButton;
        public float fadeDuration = 1f;
        public float epilepsyWaitTime = 9f;

        [Header("Input")]
        public InputActionAsset inputActions;
        private InputAction submitAction;

        private void Awake() {
            audioSource = GetComponent<AudioSource>();

            submitAction = inputActions.FindAction("Submit", true);

            foreach (var c in cubes) {
                c.targetPosition = c.cube.localPosition;
                c.targetRotation = c.cube.localRotation;
                c.rb = c.cube.GetComponent<Rigidbody>();
            }
        }

        private void OnEnable() {
            epilepsyPanel.alpha = 0f;
            epilepsyPanel.interactable = false;
            epilepsyPanel.blocksRaycasts = false;

            submitAction.Enable();
            submitAction.performed += OnSubmit;

            SetInitialLocale();

            InitLogoPhysics();

            StopAllCoroutines();
            StartCoroutine(SettleAfterChaos());

            if (Settings.IsFirstLaunch) {
                StartCoroutine(ShowEpilepsyWarningEarly());
            }
            else {
                StartCoroutine(AutoSkipEpilepsyScreen());
            }
        }

        private void OnDisable() {
            submitAction.performed -= OnSubmit;
            submitAction.Disable();
        }

        private void SetInitialLocale() {
            string langCode = Settings.Language;
            StartCoroutine(ApplyLocale(langCode));
        }

        private IEnumerator ApplyLocale(string code) {
            yield return LocalizationSettings.InitializationOperation;
            Locale desiredLocale = LocalizationSettings.AvailableLocales.Locales.Find(l => l.Identifier.Code == code);
            if (desiredLocale != null)
                LocalizationSettings.SelectedLocale = desiredLocale;
        }

        private void InitLogoPhysics() {
            foreach (var c in cubes) {
                c.rb.isKinematic = false;
                c.rb.useGravity = false;
                c.rb.linearVelocity = Vector3.zero;
                c.rb.angularVelocity = Vector3.zero;

                Vector3 direction = (c.targetPosition - Vector3.zero).normalized;
                Vector3 spawnPos = c.targetPosition + direction * overshootDistance;
                spawnPos.x = Mathf.Sign(spawnPos.x) * Mathf.Max(Mathf.Abs(spawnPos.x), spawnArea.x);
                spawnPos.y = Mathf.Clamp(spawnPos.y, -spawnArea.y, spawnArea.y);

                c.cube.localPosition = spawnPos;
                c.cube.localRotation = Random.rotation;

                Vector3 toTarget = (c.targetPosition - spawnPos).normalized;
                c.rb.AddForce(toTarget * launchForce, ForceMode.VelocityChange);
            }
        }

        private IEnumerator SettleAfterChaos() {
            yield return new WaitForSeconds(chaosTime);

            if (!gameObject.activeInHierarchy) yield break;

            audioSource.Play();

            foreach (var c in cubes) {
                c.rb.linearVelocity = Vector3.zero;
                c.rb.angularVelocity = Vector3.zero;
                c.rb.isKinematic = true;
            }

            float t = 0f;
            while (t < settleDuration) {
                t += Time.deltaTime;
                float lerpT = t / settleDuration;

                float smoothT = Mathf.SmoothStep(0, 1, lerpT);

                foreach (var c in cubes) {
                    c.cube.localPosition = Vector3.Lerp(c.cube.localPosition, c.targetPosition, smoothT);
                    c.cube.localRotation = Quaternion.Slerp(c.cube.localRotation, c.targetRotation, smoothT);
                }
                yield return null;
            }
        }

        private IEnumerator ShowEpilepsyWarningEarly() {
            yield return new WaitForSeconds(epilepsyWaitTime);
            ShowEpilepsyWarning();
        }

        private IEnumerator AutoSkipEpilepsyScreen() {
            yield return new WaitForSeconds(epilepsyWaitTime);
            LoadNextScene();
        }

        private void ShowEpilepsyWarning() {
            epilepsyPanel.DOFade(1f, fadeDuration).OnComplete(() => {
                epilepsyPanel.interactable = true;
                epilepsyPanel.blocksRaycasts = true;
            });
        }

        public void LoadNextScene() {
            if (Settings.IsFirstLaunch) {
                AudioSource buttonAudio = continueButton.GetComponent<AudioSource>();
                buttonAudio.Play();
                Settings.IsFirstLaunch = false;
                StartCoroutine(WaitForAudioAndLoad(buttonAudio));
            }
            else {
                GlobalSceneManager.Instance.SwitchToScene("MainMenu");
            }
        }

        private IEnumerator WaitForAudioAndLoad(AudioSource audio) {
            yield return new WaitWhile(() => audio.isPlaying);
            GlobalSceneManager.Instance.SwitchToScene("MainMenu");
        }

        private void OnSubmit(InputAction.CallbackContext ctx) {
            if (epilepsyPanel.alpha >= 0.9f) {
                LoadNextScene();
            }
        }
    }
}