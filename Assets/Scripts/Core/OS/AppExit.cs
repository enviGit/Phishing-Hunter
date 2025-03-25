using UnityEngine;

namespace ph.Core.OS {
    public class AppExit : MonoBehaviour {
        public GameObject app;
        public void ExitApp() {
            app.SetActive(false);
        }
    }
}