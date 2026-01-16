using UnityEngine;

namespace ph.OS {
    public class AppExit : MonoBehaviour {
        public GameObject app;
        public void ExitApp() {
            app.SetActive(false);
        }
    }
}
