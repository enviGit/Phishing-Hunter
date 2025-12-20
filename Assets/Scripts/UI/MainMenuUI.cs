using ph.Managers.Save;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace ph.UI {
    public class MainMenuUI : MonoBehaviour {
        [Header("References")]
        [SerializeField] private MenuCam menuCam;
        [SerializeField] private RectTransform[] buttons;
        [SerializeField] private Button newGameButton, loadGameButton;
        [SerializeField] private float animationDuration = 1.5f;
        [SerializeField] private float delayBetweenButtons = 0.25f;
        [SerializeField] private Vector2 startOffset = new Vector2(0, -50f);

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
            for (int i = 0; i < buttons.Length; i++) {
                RectTransform button = buttons[i];

                button.gameObject.SetActive(true);
                button.anchoredPosition += startOffset;
                button.GetComponent<CanvasGroup>().alpha = 0f;

                button.DOAnchorPosY(button.anchoredPosition.y - startOffset.y, animationDuration)
                    .SetEase(Ease.OutBack)
                    .SetDelay(i * delayBetweenButtons);

                button.GetComponent<CanvasGroup>().DOFade(1f, animationDuration)
                    .SetDelay(i * delayBetweenButtons);
            }
        }
    }
}
