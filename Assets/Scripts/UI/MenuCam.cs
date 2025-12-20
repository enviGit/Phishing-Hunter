using DG.Tweening;
using System;
using System.Collections;
using Unity.Cinemachine;
using UnityEngine;

namespace ph.UI {
    public class MenuCam : MonoBehaviour {
        [SerializeField] private CinemachineCamera mainMenuCam;
        [SerializeField] private CinemachineCamera settingsCam;
        [SerializeField] private CinemachineCamera confirmationCam;
        [SerializeField] private CinemachineCamera laptopCam;
        [SerializeField] private CinemachineBrain cinemachineBrain;
        [SerializeField] private float transitionDuration = 1f;
        [SerializeField] private CanvasGroup mainMenuCanvasGroup;
        [SerializeField] private GameObject[] canvases;
        public static bool IsReturningFromDesktop = false;

        private void Start() {
            if (cinemachineBrain != null) {
                cinemachineBrain.DefaultBlend.Time = transitionDuration;
            }
        }

        private void OnEnable() {
            ResetPriorities();
            if (IsReturningFromDesktop) {
                laptopCam.Priority = 20;

                if (mainMenuCanvasGroup != null) {
                    mainMenuCanvasGroup.alpha = 0f;
                    mainMenuCanvasGroup.interactable = false;
                }

                StartCoroutine(AnimateReturnFromDesktop());

                IsReturningFromDesktop = false;
            }
            else {
                mainMenuCam.Priority = 20;

                if (mainMenuCanvasGroup != null) {
                    mainMenuCanvasGroup.alpha = 1f;
                    mainMenuCanvasGroup.interactable = true;
                }
            }

            if (canvases.Length > 0) SetActiveCanvas(canvases[0]);
        }

        private IEnumerator AnimateReturnFromDesktop() {
            yield return null;

            mainMenuCam.Priority = 20;
            laptopCam.Priority = 5;

            yield return new WaitForSeconds(0.5f);

            if (mainMenuCanvasGroup != null) {
                mainMenuCanvasGroup.DOFade(1f, 1f).SetEase(Ease.OutQuad);
                mainMenuCanvasGroup.interactable = true;
                mainMenuCanvasGroup.blocksRaycasts = true;
            }
        }

        public void ZoomInAndStartGame(Action onComplete) {
            StartCoroutine(ZoomInRoutine(onComplete));
        }

        private IEnumerator ZoomInRoutine(Action onComplete) {
            if (mainMenuCanvasGroup != null) {
                mainMenuCanvasGroup.interactable = false;
                mainMenuCanvasGroup.blocksRaycasts = false;
                mainMenuCanvasGroup.DOFade(0f, 0.5f).SetEase(Ease.OutQuad);
            }

            laptopCam.Priority = 20;
            mainMenuCam.Priority = 5;
            settingsCam.Priority = 5;

            yield return new WaitForSeconds(transitionDuration);

            onComplete?.Invoke();
        }

        private void ResetPriorities() {
            mainMenuCam.Priority = 5;
            settingsCam.Priority = 5;
            confirmationCam.Priority = 5;
            laptopCam.Priority = 5;
        }

        private void SetActiveCamera(CinemachineCamera activeCam) {
            ResetPriorities();
            activeCam.Priority = 20;
        }

        public void MoveToMainMenu() => SetActiveCamera(mainMenuCam);
        public void MoveToSettings() => SetActiveCamera(settingsCam);
        public void MoveToConfirmation() => SetActiveCamera(confirmationCam);
        public void SetActiveCanvas(GameObject activeCanvas) {
            foreach (var canvas in canvases) {
                canvas.SetActive(canvas == activeCanvas);
            }
        }
    }
}