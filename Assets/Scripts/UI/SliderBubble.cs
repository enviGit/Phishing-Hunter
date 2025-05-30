using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

namespace ph.UI {
    public class SliderBubbleFeedback : MonoBehaviour, IPointerDownHandler, IPointerUpHandler {
        private Slider slider;
        [SerializeField] private Image bubble;
        [SerializeField] private Image fillImage;
        private CanvasGroup bubbleCanvasGroup;
        private bool isAnimating = false;
        private TextMeshProUGUI bubbleText;
        [SerializeField] private TextMeshProUGUI resolutionText;
        private readonly Color[] gradientColors = new Color[] {
    new Color(1f, 0.2f, 0.2f),     // Red
    new Color(1f, 0.5f, 0.1f),     // Orange
    new Color(1f, 1f, 0.2f),       // Yellow
    new Color(0.2f, 1f, 0.4f),     // Green
    new Color(0.2f, 0.5f, 1f)      // Blue
};
        private readonly float[] colorKeys = new float[] {
    0.0f,  // Red
    0.15f, // Orange
    0.3f,  // Yellow
    0.7f,  // Green
    1.0f   // Blue
};

        private void Awake() {
            slider = GetComponent<Slider>();

            if (bubble != null) {
                bubbleText = bubble.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
                bubble.gameObject.SetActive(false);
                bubbleCanvasGroup = bubble.GetComponent<CanvasGroup>();

                if (bubbleCanvasGroup == null) {
                    bubbleCanvasGroup = bubble.gameObject.AddComponent<CanvasGroup>();
                }
            }

            slider.onValueChanged.AddListener(OnSliderValueChanged);
        }

        private void Start() {
            OnSliderValueChanged(slider.value);
        }

        private void OnSliderValueChanged(float value) {
            int intValue = Mathf.RoundToInt(value);

            if (bubbleText != null && resolutionText == null)
                bubbleText.text = intValue.ToString();
            else if (bubbleText != null && resolutionText != null)
                bubbleText.text = resolutionText.text;

            float normalized = Mathf.InverseLerp(slider.minValue, slider.maxValue, value);
            Color color = GetSmoothColor(normalized);

            if (fillImage != null)
                fillImage.color = color;

            if (bubble != null)
                bubble.color = color;
        }

        public void OnPointerDown(PointerEventData eventData) {
            if (isAnimating) return;

            if (bubble != null) {
                bubble.gameObject.SetActive(true);

                bubbleCanvasGroup.alpha = 0f;
                isAnimating = true;

                DOTween.To(() => bubbleCanvasGroup.alpha, x => bubbleCanvasGroup.alpha = x, 1f, 0.2f)
                    .SetEase(Ease.OutBack)
                    .OnComplete(() => {
                        isAnimating = false;
                    });

                bubble.transform.DOScale(0.75f, 0.2f).SetEase(Ease.OutBack);

                OnSliderValueChanged(slider.value);
            }
        }

        public void OnPointerUp(PointerEventData eventData) {
            if (isAnimating) return;

            if (bubble != null) {
                isAnimating = true;

                DOTween.To(() => bubbleCanvasGroup.alpha, x => bubbleCanvasGroup.alpha = x, 0f, 0.15f)
                    .SetEase(Ease.InBack)
                    .OnComplete(() => {
                        bubble.gameObject.SetActive(false);
                        isAnimating = false;
                    });

                bubble.transform.DOScale(Vector3.zero, 0.15f).SetEase(Ease.InBack);
            }
        }

        private Color GetSmoothColor(float normalized) {
            if (gradientColors.Length != colorKeys.Length || gradientColors.Length < 2)
                return Color.white;

            for (int i = 0; i < colorKeys.Length - 1; i++) {
                float start = colorKeys[i];
                float end = colorKeys[i + 1];

                if (normalized >= start && normalized <= end) {
                    float t = Mathf.InverseLerp(start, end, normalized);
                    return Color.Lerp(gradientColors[i], gradientColors[i + 1], t);
                }
            }

            return gradientColors[gradientColors.Length - 1]; // fallback
        }
    }
}