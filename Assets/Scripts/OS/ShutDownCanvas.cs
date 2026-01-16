using UnityEngine;
using ph.Managers;
using ph.UI;

namespace ph.OS {
    public class ShutdownScreen : MonoBehaviour {
        public void OnShutdownFinished() {
            MenuCam.IsReturningFromDesktop = true;
            GlobalSceneManager.Instance.SwitchToScene("MainMenu");
        }
    }
}
