using UnityEngine;
using DG.Tweening;
using UnityEngine.EventSystems;

namespace ph.UI {
    public class ButtonHoverAnimation : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {
        [SerializeField] private float animationDuration = 0.5f;
        [SerializeField] private float scaleFactor = 1.15f;
        [SerializeField] private Vector3 originalScale = new Vector3(1f, 1f, 1f);

        private void Start() {
            transform.localScale = originalScale;
        }
        public void OnPointerEnter(PointerEventData eventData) {
            transform.DOScale(scaleFactor, animationDuration).SetEase(Ease.OutBack);
        }
        public void OnPointerExit(PointerEventData eventData) {
            transform.DOScale(originalScale, animationDuration).SetEase(Ease.InBack);
        }
    }
}
