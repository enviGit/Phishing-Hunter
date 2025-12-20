using UnityEngine;
using ph.Managers;

namespace ph.Core.OS {
    public class ShutdownScreen : MonoBehaviour {
        public void OnShutdownFinished() {
            GlobalSceneManager.Instance.SwitchToScene("MainMenu");
        }
    }
}