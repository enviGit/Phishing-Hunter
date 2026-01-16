using UnityEngine;
using UnityEngine.EventSystems;

namespace ph.OS {
    public class QuizApp : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {
        private bool isSelected;
        public GameObject selected;
        public GameObject actualQuizApp;
        public Animator selectedAnim;
        private bool quizAppIsRunning = false;
        public GameObject quizAppInTaskbar;
        public Animator quizAppAnim;

        void Update() {
            if (isSelected) {
                selected.SetActive(true);
                if (Input.GetKeyDown(KeyCode.Mouse0)) {
                    quizAppIsRunning = true;
                    actualQuizApp.SetActive(true);
                    selectedAnim.SetTrigger("Clicked");
                }
            }
            else {
                selected.SetActive(false);
            }
            if (quizAppIsRunning) {
                quizAppInTaskbar.SetActive(true);
                if (quizAppAnim.GetBool("exit") == true) {
                    quizAppIsRunning = false;
                }
            }
            else {
                quizAppInTaskbar.SetActive(false);
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
