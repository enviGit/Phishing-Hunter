using UnityEngine;
using DG.Tweening;
using UnityEngine.EventSystems;

namespace ph.UI {
    public class ButtonHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler {
        [Header("Settings")]
        [SerializeField] private float animationDuration = 0.3f;
        [SerializeField] private float scaleFactor = 1.1f;
        [SerializeField] private Vector3 originalScale = new Vector3(1f, 1f, 1f);

        [Header("Audio")]
        private AudioSource audioSource;
        private AudioClip audioClip;

        private void Start() {
            transform.localScale = originalScale;
            audioSource = GetComponent<AudioSource>();
            if (audioSource != null) audioClip = audioSource.clip;
        }

        public void OnPointerEnter(PointerEventData eventData) {
            transform.DOKill();
            transform.DOScale(scaleFactor, animationDuration).SetEase(Ease.OutBack);

            if (audioSource && audioClip) audioSource.PlayOneShot(audioClip);
        }

        public void OnPointerExit(PointerEventData eventData) {
            transform.DOKill();
            transform.DOScale(originalScale, animationDuration).SetEase(Ease.OutBack);
        }

        public void OnPointerDown(PointerEventData eventData) {
            transform.DOKill();
            transform.DOScale(originalScale * 0.95f, 0.1f).SetEase(Ease.OutQuad);
        }

        public void OnPointerUp(PointerEventData eventData) {
            transform.DOKill();
            transform.DOScale(scaleFactor, 0.1f).SetEase(Ease.OutBack);
        }

        public void OnPointerClick(PointerEventData eventData) {
            transform.DOKill();
            transform.DOPunchScale(new Vector3(-0.1f, -0.1f, -0.1f), 0.2f, 10, 1).OnComplete(() => {
                transform.localScale = originalScale;
            });
        }
    }
}