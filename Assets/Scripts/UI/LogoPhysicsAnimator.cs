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
            [HideInInspector] public Vector3 startSettlePosition;
            [HideInInspector] public Quaternion startSettleRotation;
        }

        [Header("Logo")]
        public List<LogoCube> cubes;
        public float launchForce = 6f;
        public float chaosTime = 2f;

        [Tooltip("Ile czasu przed końcem muzyki klocki mają być już idealnie ułożone (bufor bezpieczeństwa)")]
        public float finishBuffer = 4f;

        public float overshootDistance = 0.5f;
        public Vector2 spawnArea = new Vector2(10f, 6f);
        private AudioSource audioSource;

        [Header("Epilepsy UI")]
        public CanvasGroup epilepsyPanel;
        public Button continueButton;
        public float fadeDuration = 1f;

        [Tooltip("Czas po jakim pojawi się ostrzeżenie (tylko przy First Launch)")]
        public float epilepsyWaitTime = 9f;

        [Header("Input")]
        public InputActionAsset inputActions;
        private InputAction submitAction;

        private bool hasTriggeredLoad = false;

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
            hasTriggeredLoad = false;

            submitAction.Enable();
            submitAction.performed += OnSubmit;

            SetInitialLocale();
            InitLogoPhysics();

            StopAllCoroutines();

            StartCoroutine(SettleAndFinishSequence());

            if (Settings.IsFirstLaunch) {
                StartCoroutine(ShowEpilepsyWarningEarly());
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

        private IEnumerator SettleAndFinishSequence() {
            yield return new WaitForSeconds(chaosTime);

            if (!gameObject.activeInHierarchy) yield break;

            audioSource.Play();

            float audioLength = audioSource.clip != null ? audioSource.clip.length : 5.0f;

            float animationDuration = Mathf.Max(0.5f, audioLength - finishBuffer);

            foreach (var c in cubes) {
                c.rb.linearVelocity = Vector3.zero;
                c.rb.angularVelocity = Vector3.zero;
                c.rb.isKinematic = true;
                c.startSettlePosition = c.cube.localPosition;
                c.startSettleRotation = c.cube.localRotation;
            }

            float elapsedTime = 0f;
            while (elapsedTime < animationDuration) {
                elapsedTime += Time.deltaTime;
                float t = Mathf.Clamp01(elapsedTime / animationDuration);
                float smoothT = Mathf.SmoothStep(0, 1, t);

                foreach (var c in cubes) {
                    c.cube.localPosition = Vector3.Lerp(c.startSettlePosition, c.targetPosition, smoothT);
                    c.cube.localRotation = Quaternion.Slerp(c.startSettleRotation, c.targetRotation, smoothT);
                }
                yield return null;
            }

            foreach (var c in cubes) {
                c.cube.localPosition = c.targetPosition;
                c.cube.localRotation = c.targetRotation;
            }

            yield return new WaitForSeconds(finishBuffer);

            if (!Settings.IsFirstLaunch) {
                LoadNextScene();
            }
        }

        private IEnumerator ShowEpilepsyWarningEarly() {
            yield return new WaitForSeconds(epilepsyWaitTime);
            ShowEpilepsyWarning();
        }

        private void ShowEpilepsyWarning() {
            epilepsyPanel.DOFade(1f, fadeDuration).OnComplete(() => {
                epilepsyPanel.interactable = true;
                epilepsyPanel.blocksRaycasts = true;
            });
        }

        public void LoadNextScene() {
            if (hasTriggeredLoad) return;
            hasTriggeredLoad = true;

            if (Settings.IsFirstLaunch) {
                AudioSource buttonAudio = continueButton.GetComponent<AudioSource>();
                if (buttonAudio != null) buttonAudio.Play();

                Settings.IsFirstLaunch = false;
                StartCoroutine(WaitForAudioAndLoad(buttonAudio));
            }
            else {
                GlobalSceneManager.Instance.SwitchToScene("MainMenu");
            }
        }

        private IEnumerator WaitForAudioAndLoad(AudioSource audio) {
            if (audio != null) {
                yield return new WaitWhile(() => audio.isPlaying);
            }
            GlobalSceneManager.Instance.SwitchToScene("MainMenu");
        }

        private void OnSubmit(InputAction.CallbackContext ctx) {
            if (Settings.IsFirstLaunch) {
                if (epilepsyPanel.alpha >= 0.9f) {
                    LoadNextScene();
                }
            }
            else {
                LoadNextScene();
            }
        }
    }
}