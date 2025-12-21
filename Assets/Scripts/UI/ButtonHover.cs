using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.EventSystems;

namespace ph.UI {
    [RequireComponent(typeof(Image))]
    public class ButtonHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler {
        [Header("Motion Settings")]
        [SerializeField] private float animationDuration = 0.2f;
        [SerializeField] private float scaleFactor = 1.05f;

        [Header("Cyber Shader Settings")]
        [SerializeField] private float hoverGlitchIntensity = 0.02f;
        [SerializeField] private float hoverScanlineIntensity = 0.4f;
        private float defaultGlitch;
        private float defaultScanline;

        [Header("Audio")]
        [SerializeField] private AudioSource audioSource;

        private Vector3 originalScale;
        private Image targetImage;
        private Material uiMaterial;

        private void Start() {
            originalScale = transform.localScale;
            targetImage = GetComponent<Image>();

            if (targetImage.material != null) {
                uiMaterial = new Material(targetImage.material);
                targetImage.material = uiMaterial;

                defaultGlitch = uiMaterial.GetFloat("_GlitchIntensity");
                defaultScanline = uiMaterial.GetFloat("_ScanlineIntensity");
            }

            if (audioSource == null) audioSource = GetComponent<AudioSource>();
        }

        public void OnPointerEnter(PointerEventData eventData) {
            transform.DOKill();
            uiMaterial.DOKill();

            transform.DOScale(originalScale * scaleFactor, animationDuration).SetEase(Ease.OutBack);

            if (uiMaterial != null) {
                uiMaterial.DOFloat(hoverGlitchIntensity, "_GlitchIntensity", animationDuration);
                uiMaterial.DOFloat(hoverScanlineIntensity, "_ScanlineIntensity", animationDuration);
            }

            if (audioSource && audioSource.clip) {
                audioSource.pitch = Random.Range(0.95f, 1.05f);
                audioSource.PlayOneShot(audioSource.clip);
            }
        }

        public void OnPointerExit(PointerEventData eventData) {
            transform.DOKill();
            uiMaterial.DOKill();

            transform.DOScale(originalScale, animationDuration).SetEase(Ease.OutQuad);

            if (uiMaterial != null) {
                uiMaterial.DOFloat(defaultGlitch, "_GlitchIntensity", animationDuration);
                uiMaterial.DOFloat(defaultScanline, "_ScanlineIntensity", animationDuration);
            }
        }

        public void OnPointerDown(PointerEventData eventData) {
            transform.DOKill();
            transform.DOScale(originalScale * 0.95f, 0.05f).SetEase(Ease.OutQuad);
        }

        public void OnPointerUp(PointerEventData eventData) {
            if (eventData.hovered.Contains(gameObject)) {
                transform.DOKill();
                transform.DOScale(originalScale * scaleFactor, 0.1f).SetEase(Ease.OutBack);
            }
        }
    }
}