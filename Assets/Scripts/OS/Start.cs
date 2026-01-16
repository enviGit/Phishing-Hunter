using DG.Tweening;
using UnityEngine;

namespace ph.OS {
    public class Start : MonoBehaviour {
        public GameObject shutDownCanvas;
        [SerializeField] private CanvasGroup buttonCanvasGroup;

        public void StartMenu() {
            if (buttonCanvasGroup.alpha <= 0.9f) {
                buttonCanvasGroup.DOFade(1, 0.5f);
                buttonCanvasGroup.blocksRaycasts = true;
            }
            else {
                buttonCanvasGroup.blocksRaycasts = false;
                buttonCanvasGroup.DOFade(0, 0.5f);
            }

        }
        public void ShutDown() {
            buttonCanvasGroup.alpha = 0;
            shutDownCanvas.SetActive(true);
        }
    }
}
