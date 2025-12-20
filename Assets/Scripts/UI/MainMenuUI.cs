using DG.Tweening;
using ph.Managers.Save;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ph.UI {
    public class MainMenuUI : MonoBehaviour {
        [Header("References")]
        [SerializeField] private MenuCam menuCam;
        [SerializeField] private RectTransform[] buttons;
        [SerializeField] private Button newGameButton, loadGameButton;
        private float animationDuration = 0.6f;
        private float delayBetweenButtons = 0.1f;
        private Vector2 startOffset = new Vector2(0, -50f);
        private Ease animationEase = Ease.OutBack;

        private class CachedButton {
            public RectTransform rect;
            public CanvasGroup group;
            public Vector2 originalPosition;
        }

        private List<CachedButton> cachedButtons = new List<CachedButton>();
        private Sequence currentSequence;

        private void Awake() {
            foreach (var btnRect in buttons) {
                if (btnRect == null) continue;

                var cg = btnRect.GetComponent<CanvasGroup>();
                if (cg == null) cg = btnRect.gameObject.AddComponent<CanvasGroup>();

                cachedButtons.Add(new CachedButton {
                    rect = btnRect,
                    group = cg,
                    originalPosition = btnRect.anchoredPosition
                });
            }
        }

        private void Start() {
            if (DataPersistence.instance == null) {
                Debug.LogError("Brak DataPersistence Managera!");
                return;
            }

            newGameButton.onClick.AddListener(() => {
                menuCam.ZoomInAndStartGame(() => {
                    DataPersistence.instance.OnNewGameClicked();
                });
            });

            loadGameButton.onClick.AddListener(() => {
                menuCam.ZoomInAndStartGame(() => {
                    DataPersistence.instance.OnLoadGameClicked();
                });
            });
        }

        private void OnEnable() {
            if (DataPersistence.instance != null) {
                bool hasSave = DataPersistence.instance.HasSaveFile();
                loadGameButton.interactable = hasSave;
            }

            AnimateButtons();
        }

        private void AnimateButtons() {
            if (currentSequence != null) currentSequence.Kill();

            currentSequence = DOTween.Sequence();

            foreach (var btn in cachedButtons) {
                btn.rect.gameObject.SetActive(true);
                btn.group.alpha = 0f;
                btn.rect.anchoredPosition = btn.originalPosition + startOffset;

                float delay = cachedButtons.IndexOf(btn) * delayBetweenButtons;

                currentSequence.Insert(delay,
                    btn.rect.DOAnchorPos(btn.originalPosition, animationDuration).SetEase(animationEase)
                );

                currentSequence.Insert(delay,
                    btn.group.DOFade(1f, animationDuration)
                );
            }
        }
        private void OnDisable() {
            if (currentSequence != null) currentSequence.Kill();
        }
    }
}
