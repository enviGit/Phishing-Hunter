using Unity.Cinemachine;
using UnityEngine;

namespace ph.Managers {
    public class MenuCamera : MonoBehaviour {
        [SerializeField] private CinemachineCamera mainMenuCam;
        [SerializeField] private CinemachineCamera settingsCam;
        [SerializeField] private CinemachineCamera confirmationCam;
        [SerializeField] private CinemachineBrain cinemachineBrain;
        [SerializeField] private float transitionDuration = 1f;

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
    }
}