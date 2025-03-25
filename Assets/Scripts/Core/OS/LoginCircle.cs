using UnityEngine;
using UnityEngine.EventSystems;

namespace ph.Core.OS {
    public class LoginCircle : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {
        private bool isSelected;
        public Animator anim;
        public GameObject loginCanvas;
        public AudioSource beep;
        void Start() {

        }
        void Update() {
            if (isSelected) {
                anim.SetBool("isSelected", true);
                if (Input.GetKeyDown(KeyCode.Mouse0)) {
                    anim.SetTrigger("Clicked");
                    beep.Play(0);
                }
            }
            else {
                anim.SetBool("isSelected", false);
            }
        }
        public void OnPointerEnter(PointerEventData pointerEventData) {
            isSelected = true;
        }

        public void OnPointerExit(PointerEventData pointerEventData) {
            isSelected = false;
        }

        public void newCanvas() {
            loginCanvas.SetActive(true);
        }
    }
}