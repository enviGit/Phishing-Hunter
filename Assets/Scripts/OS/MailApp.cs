using ph.Managers;
using UnityEngine;
using UnityEngine.EventSystems;

namespace ph.OS {
    public class MailApp : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {
        private bool isSelected;
        public GameObject selected;
        public GameObject actualMail;
        public Animator selectedAnim;
        private bool mailIsRunning = false;
        public GameObject mailInTaskbar;
        public Animator mailAnim;
        [SerializeField] private MailManager mailManager;

        private void Update() {
            if (isSelected) {
                selected.SetActive(true);
                if (Input.GetKeyDown(KeyCode.Mouse0)) {
                    mailManager.RefreshEmails();
                    mailIsRunning = true;
                    actualMail.SetActive(true);
                    selectedAnim.SetTrigger("Clicked");
                }
            }
            else {
                selected.SetActive(false);
            }
            if (mailIsRunning) {
                mailInTaskbar.SetActive(true);
                if (mailAnim.GetBool("exit") == true) {
                    mailIsRunning = false;
                }
            }
            else {
                mailInTaskbar.SetActive(false);
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
