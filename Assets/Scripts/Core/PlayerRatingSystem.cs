using DG.Tweening;
using ph.Managers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ph.Core {
    public class PlayerRatingSystem : MonoBehaviour {
        public enum PlayerPosition {
            Intern,
            JuniorAnalyst,
            Analyst,
            SeniorAnalyst,
            Expert
        }
        public static PlayerRatingSystem Instance;

        [Header("UI")]
        [SerializeField] private TextMeshProUGUI levelText;
        [SerializeField] private TextMeshProUGUI positionText;
        [SerializeField] private Image progressSlider;
        [SerializeField] private CanvasGroup progressGroup;

        [Header("Progress")]
        [HideInInspector] public int level = 0;
        [HideInInspector] public PlayerPosition position = PlayerPosition.Intern;
        private float totalProgress;

        private void Awake() {
            Instance = this;
        }
        private void Start() {
            UpdateUI();
            progressSlider.fillAmount = 0f;
        }
        public void UpdateProgress() {
            int correctMails = MailManager.CorrectMailAnswers;
            int correctQuizzes = QuizManager.CorrectQuizAnswers;
            int totalMails = MailManager.TotalMailCount;
            int totalQuizzes = QuizManager.TotalQuizCount;

            progressGroup.transform.SetSiblingIndex(progressGroup.transform.parent.childCount - 3);
            progressGroup.DOFade(1f, 0.5f).SetEase(Ease.OutBack);

            float rawProgress = (correctMails * 2f + correctQuizzes) / (totalMails * 2f + totalQuizzes);
            totalProgress = rawProgress * MaxLevelCount(); // gdzie MaxLevelCount = ile poziomów przewidujesz

            int newLevel = Mathf.FloorToInt(totalProgress);
            float currentLevelProgress = totalProgress - newLevel;

            if (newLevel > level && level < MaxLevelCount()) {
                level = newLevel;
                UpdateDifficulty();
                UpdatePosition();
            }

            if (level >= MaxLevelCount()) {
                currentLevelProgress = 1f;
            }

            progressGroup.DOFade(1f, 0.5f).OnComplete(() => {
                progressSlider.DOFillAmount(currentLevelProgress, 1f).SetEase(Ease.OutSine);
            });

            UpdateUI();

            progressGroup.DOFade(0.1f, 0.5f).SetDelay(2f).SetEase(Ease.InBack).OnComplete(() => {
                progressGroup.transform.SetSiblingIndex(1);
            });
        }
        private int MaxLevelCount() {
            return 50;
        }
        private void UpdatePosition() {
            if (level < 5) position = PlayerPosition.Intern;
            else if (level < 10) position = PlayerPosition.JuniorAnalyst;
            else if (level < 20) position = PlayerPosition.Analyst;
            else if (level < 40) position = PlayerPosition.SeniorAnalyst;
            else position = PlayerPosition.Expert;
        }
        private void UpdateDifficulty() {
            Settings.Difficulty = level < 5 ? 0 : 1;
        }
        private void UpdateUI() {
            levelText.text = $"{level}";
            positionText.text = GetPositionDisplayName(position);
        }
        private string GetPositionDisplayName(PlayerPosition pos) {
            switch (pos) {
                case PlayerPosition.Intern: return "Intern";
                case PlayerPosition.JuniorAnalyst: return "Junior Cybersecurity Analyst";
                case PlayerPosition.Analyst: return "Cybersecurity Analyst";
                case PlayerPosition.SeniorAnalyst: return "Senior Cybersecurity Analyst";
                case PlayerPosition.Expert: return "Cybersecurity Expert";
                default: return "Unknown";
            }
        }
    }
}