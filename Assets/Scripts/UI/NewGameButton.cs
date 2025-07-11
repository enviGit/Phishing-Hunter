using DG.Tweening;
using ph.Managers;
using System.Collections;
using UnityEngine;
using Unity.Cinemachine;
using UnityEngine.UI;

namespace ph.UI {
    public class NewGameButton : MonoBehaviour {
        [SerializeField] private float preloadDelay = 0.5f;
        [SerializeField] private CanvasGroup menuButtons;
        [Header("Cinemachine Camera")]
        [SerializeField] private CinemachineCamera newGameCam;
        [SerializeField] private float cameraTransitionDuration = 1.0f;

        private void Start() {
            StartCoroutine(DelayedPreload());
            GetComponent<Button>().onClick.AddListener(OnNewGameClicked);
        }
        private void OnNewGameClicked() {
            if (menuButtons != null) {
                menuButtons.interactable = false;
                menuButtons.blocksRaycasts = false;
                menuButtons.DOFade(0f, 0.3f).SetEase(Ease.OutQuad);
            }
            else {
                Debug.LogError("GameObject menuButtons is null!");
            }

            if (newGameCam != null) {
                newGameCam.Priority = 10;
                StartCoroutine(WaitForCameraTransitionAndLoad());
            }
            else {
                Debug.LogError("newGameCam is null!");
            }
        }
        private IEnumerator WaitForCameraTransitionAndLoad() {
            yield return new WaitForSeconds(cameraTransitionDuration);
            if (SceneLoader.Instance != null) {
                SceneLoader.Instance.ActivateNextScene();
            }
            else {
                Debug.LogError("SceneLoader.Instance is null!");
            }
        }
        private IEnumerator DelayedPreload() {
            yield return new WaitForSeconds(preloadDelay);
            if (SceneLoader.Instance != null) {
                SceneLoader.Instance.PreloadNextScene();
            }
            else {
                Debug.LogError("SceneLoader.Instance is null!");
            }
        }
    }
}