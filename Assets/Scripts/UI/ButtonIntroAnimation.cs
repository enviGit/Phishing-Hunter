using ph.Managers.Save;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace ph.UI {
    public class ButtonIntroAnimation : MonoBehaviour {
        [SerializeField] private RectTransform[] buttons;
        private Button newGameButton, loadGameButton;
        [SerializeField] private float animationDuration = 1.5f;
        [SerializeField] private float delayBetweenButtons = 0.25f;
        [SerializeField] private Vector2 startOffset = new Vector2(0, -50f);

        private void Start() {
            AnimateButtons();

            if (DataPersistence.instance == null) {
                Debug.LogError("Brak DataPersistence Managera!");
                return;
            }

            newGameButton = buttons[0].GetComponent<Button>();
            loadGameButton = buttons[1].GetComponent<Button>();
            bool hasSave = DataPersistence.instance.HasSaveFile();
            loadGameButton.interactable = hasSave;

            newGameButton.onClick.AddListener(() => {
                DataPersistence.instance.OnNewGameClicked();
            });

            loadGameButton.onClick.AddListener(() => {
                DataPersistence.instance.OnLoadGameClicked();
            });
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
