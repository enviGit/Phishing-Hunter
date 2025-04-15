using DG.Tweening;
using ph.Managers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ph.Achievements {
    public class AchievementUI : MonoBehaviour {
        //public static AchievementUI Instance;

        [Header("UI")]
        private CanvasGroup panelGroup;
        [SerializeField] private Image icon;
        [SerializeField] private TextMeshProUGUI titleText;
        [SerializeField] private TextMeshProUGUI descText;

        private void Start() {
            panelGroup = GetComponent<CanvasGroup>();
            panelGroup.alpha = 0f;
        }

        public void Show(Achievement achievement) {
            string lang = Settings.Language;

            icon.sprite = achievement.icon;
            titleText.text = achievement.GetTitle(lang);
            descText.text = achievement.GetDescription(lang);

            //transform.SetAsLastSibling();

            panelGroup.DOFade(1f, 0.5f)
                .OnComplete(() => {
                    DOVirtual.DelayedCall(3f, () => {
                        panelGroup.DOFade(0f, 0.5f);
                    });
                });
        }
    }
}
