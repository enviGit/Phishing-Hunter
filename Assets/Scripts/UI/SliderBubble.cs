using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

namespace ph.UI {
    public class SliderBubble : MonoBehaviour, IPointerDownHandler, IPointerUpHandler {
        [Header("References")]
        private Slider slider;
        [SerializeField] private Image bubbleImage;
        [SerializeField] private Image fillImage;
        [SerializeField] private TextMeshProUGUI bubbleText;

        [Tooltip("Opcjonalne: Jeśli chcesz brać tekst z innego miejsca (np. ustawień rozdzielczości)")]
        [SerializeField] private TextMeshProUGUI externalValueText;

        [Header("Settings")]
        [SerializeField] private Gradient colorGradient;
        [SerializeField] private string valueFormat = "{0}";

        [Header("Animation")]
        [SerializeField] private float animDuration = 0.2f;
        [SerializeField] private float bubbleScaleSize = 1.0f;

        private CanvasGroup bubbleCanvasGroup;
        private RectTransform bubbleRect;

        private void Awake() {
            slider = GetComponent<Slider>();

            if (bubbleImage != null) {
                bubbleRect = bubbleImage.GetComponent<RectTransform>();
                bubbleCanvasGroup = bubbleImage.GetComponent<CanvasGroup>();

                if (bubbleCanvasGroup == null)
                    bubbleCanvasGroup = bubbleImage.gameObject.AddComponent<CanvasGroup>();

                bubbleCanvasGroup.alpha = 0f;
                bubbleRect.localScale = Vector3.zero;
                bubbleImage.gameObject.SetActive(false);
            }

            slider.onValueChanged.AddListener(UpdateVisuals);
        }

        private void Start() {
            UpdateVisuals(slider.value);
        }

        private void UpdateVisuals(float value) {
            if (bubbleText != null) {
                if (externalValueText != null) {
                    bubbleText.text = externalValueText.text;
                }
                else {
                    bubbleText.text = string.Format(valueFormat, Mathf.RoundToInt(value));
                }
            }

            float normalizedVal = Mathf.InverseLerp(slider.minValue, slider.maxValue, value);
            Color targetColor = colorGradient.Evaluate(normalizedVal);

            if (fillImage != null) fillImage.color = targetColor;
            if (bubbleImage != null) bubbleImage.color = targetColor;
        }

        public void OnPointerDown(PointerEventData eventData) {
            if (bubbleImage == null) return;
            ShowBubble(true);
        }

        public void OnPointerUp(PointerEventData eventData) {
            if (bubbleImage == null) return;
            ShowBubble(false);
        }

        private void ShowBubble(bool show) {
            bubbleCanvasGroup.DOKill();
            bubbleRect.DOKill();

            if (show) {
                bubbleImage.gameObject.SetActive(true);
                bubbleCanvasGroup.DOFade(1f, animDuration);
                bubbleRect.DOScale(bubbleScaleSize, animDuration).SetEase(Ease.OutBack);
            }
            else {
                bubbleCanvasGroup.DOFade(0f, animDuration);
                bubbleRect.DOScale(0f, animDuration).SetEase(Ease.InBack)
                    .OnComplete(() => bubbleImage.gameObject.SetActive(false));
            }
        }
    }
}