using UnityEngine;
using DG.Tweening;
using UnityEngine.EventSystems;

namespace ph.UI {
    public class ButtonHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler {
        [SerializeField] private float animationDuration = 0.5f;
        [SerializeField] private float scaleFactor = 1.15f;
        [SerializeField] private Vector3 originalScale = new Vector3(1f, 1f, 1f);
        private AudioSource audioSource;
        private AudioClip audioClip;
        private Tween currentTween;

        private void Start() {
            transform.localScale = originalScale;
            audioSource = GetComponent<AudioSource>();
            audioClip = audioSource.clip;
        }
        public void OnPointerEnter(PointerEventData eventData) {
            if (currentTween != null && currentTween.IsActive()) {
                currentTween.Kill();
            }

            transform.DOScale(scaleFactor, animationDuration).SetEase(Ease.OutBack);
            audioSource.PlayOneShot(audioClip);
        }
        public void OnPointerExit(PointerEventData eventData) {
            if (currentTween != null && currentTween.IsActive()) {
                currentTween.Kill();
            }

            transform.DOScale(originalScale, animationDuration).SetEase(Ease.InBack);
        }
        public void OnPointerClick(PointerEventData eventData) {
            if (currentTween != null && currentTween.IsActive()) {
                currentTween.Kill();
            }
            transform.localScale = originalScale;
        }
    }
}
