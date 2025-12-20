using Unity.Cinemachine;
using UnityEngine;

namespace ph.UI {
    public class MenuCam : MonoBehaviour {
        [SerializeField] private CinemachineCamera mainMenuCam;
        [SerializeField] private CinemachineCamera settingsCam;
        [SerializeField] private CinemachineCamera confirmationCam;
        [SerializeField] private CinemachineBrain cinemachineBrain;
        [SerializeField] private float transitionDuration = 1f;
        [SerializeField] private GameObject[] canvases;

        private void Start() {
            if (cinemachineBrain != null) {
                cinemachineBrain.DefaultBlend.Time = transitionDuration;
            }
        }

        private void OnEnable() {
            MoveToMainMenu();

            if (canvases.Length > 0) {
                SetActiveCanvas(canvases[0]);
                CanvasGroup group = canvases[0].GetComponent<CanvasGroup>();
                group.alpha = 1f;
                group.interactable = true;
                group.blocksRaycasts = true;
            }
        }

        private void SetActiveCamera(CinemachineCamera activeCam) {
            mainMenuCam.Priority = (activeCam == mainMenuCam) ? 10 : 5;
            settingsCam.Priority = (activeCam == settingsCam) ? 10 : 5;
            confirmationCam.Priority = (activeCam == confirmationCam) ? 10 : 5;
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