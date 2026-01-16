using UnityEngine;
using UnityEngine.EventSystems;

namespace ph.OS {
    public class WebBrowser : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {
        private bool isSelected;
        public GameObject selected;
        public GameObject actualWebBrowser;
        public Animator selectedAnim;
        private bool webBrowserIsRunning = false;
        public GameObject webBrowserInTaskbar;
        public Animator webBrowserAnim;

        void Update() {
            if (isSelected) {
                selected.SetActive(true);
                if (Input.GetKeyDown(KeyCode.Mouse0)) {
                    webBrowserIsRunning = true;
                    actualWebBrowser.SetActive(true);
                    selectedAnim.SetTrigger("Clicked");
                }
            }
            else {
                selected.SetActive(false);
            }
            if (webBrowserIsRunning) {
                webBrowserInTaskbar.SetActive(true);
                if (webBrowserAnim.GetBool("exit") == true) {
                    webBrowserIsRunning = false;
                }
            }
            else {
                webBrowserInTaskbar.SetActive(false);
            }
        }
        public void OnPointerEnter(PointerEventData pointerEventData) {
            isSelected = true;
        }

        public void OnPointerExit(PointerEventData pointerEventData) {
            isSelected = false;
        }
    }
}
