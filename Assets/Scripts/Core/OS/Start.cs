using DG.Tweening;
using ph.Managers;
using System.Collections;
using UnityEngine;

namespace ph.Core.OS {
    public class Start : MonoBehaviour {
        public GameObject shutDownCanvas;
        [SerializeField] private CanvasGroup buttonCanvasGroup;
        [SerializeField] private float shutdownDelay = 3f;

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
            shutDownCanvas.SetActive(true);
            StartCoroutine(WaitAndActivatePreviousScene());
        }
        private IEnumerator WaitAndActivatePreviousScene() {
            yield return new WaitForSeconds(shutdownDelay);

            GlobalSceneManager.Instance.SwitchToScene("MainMenu");
        }
    }
}