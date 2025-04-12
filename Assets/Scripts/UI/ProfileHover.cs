using UnityEngine;
using DG.Tweening;
using UnityEngine.EventSystems;

namespace ph.UI {
    public class ProfileHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {
        [SerializeField] private float animationDuration = 1f;
        private CanvasGroup profileCanvas;

        private void Start() {
            profileCanvas = GetComponent<CanvasGroup>();
            profileCanvas.alpha = 0.1f;
        }
        public void OnPointerEnter(PointerEventData eventData) {
            profileCanvas.DOFade(1f, animationDuration).SetEase(Ease.OutBack);
        }
        public void OnPointerExit(PointerEventData eventData) {
            profileCanvas.DOFade(0.1f, animationDuration).SetDelay(2f).SetEase(Ease.InBack);
        }
    }
}
