using Unity.Cinemachine;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

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
        public void ExitApplication() {
#if UNITY_EDITOR
            EditorApplication.ExitPlaymode();
#else
            Application.Quit();
#endif
        }
    }
}